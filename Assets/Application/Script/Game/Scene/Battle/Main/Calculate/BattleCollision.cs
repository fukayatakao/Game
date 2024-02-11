using UnityEngine;

namespace Project.Game {
    /// <summary>
    /// バトルのコリジョン計算を行う
    /// </summary>
    public static class BattleCollision {
        private static bool enable_ = false;
        public static void Enable(){ enable_ = true; }
        public static void Disable() { enable_ = false; }

        //移動のコリジョン計算はちょっとコリジョンを小さく見積もる
        const float ToleranceRatio = 0.9f;

		const float BulletHitDistanceSq = 0.001f;
		/// <summary>
		/// 初期化処理
		/// </summary>
		public static void Initialize() {
			enable_ = false;
		}

		/// <summary>
		/// キャラ移動前計算
		/// </summary>
		public static void PreCalculate(CharacterEntity[] characters, int count, FieldEntity field) {
			if (!enable_)
				return;
			//フラグをリセット
			ResetLossFlag(characters, count);
			//展開座標計算
			CorrectDeplayCharacter(characters, count, field);
			//隣人リストの作成
			PreCalculateNearEnemy(characters, count);
			//移動を行う前の計算
			PreCalculateMove(characters, count);
		}

		/// <summary>
		/// キャラ移動後計算
		/// </summary>
		public static void PostCalculate(CharacterEntity[] characters, int count, FieldEntity field) {
			if (!enable_)
				return;

			PostCalculatePush(characters, count, field);
			PostCalculateLoss(characters, count);
		}
		/// <summary>
		/// 弾オブジェクトとの衝突計算
		/// </summary>
		public static void BulletCalculate(CharacterEntity[] characters, int count, BulletEntity[] bullets, int bulletCount) {
			if (!enable_)
				return;
			for(int i = 0; i < bulletCount; i++) {
				BulletEntity bullet = bullets[i];
				//弾の当たり判定が残ってない場合は無視
				if (!bullet.HaveCollision.IsAvailable)
					continue;
				for (int j = 0; j < count; j++) {
					CharacterEntity character = characters[j];
					//非アクティブのキャラは無視
					if (!character.IsActive())
						continue;
					//キャラが死んでいたら無視
					if (!character.IsAlive)
						continue;
					//味方の弾は当たらない
					if (IsSamePlatoon(bullet.ActionDump.Actor, character))
						continue;
					Vector3 bp = bullet.HaveCollision.Node.position;
					Vector3 p = character.HaveCollision.Capsule.ClosestPoint(bp);
					//衝突していない
					if ((bp - p).sqrMagnitude > BulletHitDistanceSq)
						continue;


					//弾が狙った敵に当たっているとは限らないのでここで上書き
					bullet.ActionDump.Target = character;
					//バトル計算
					BattleCombat.CalculateCombat(bullet.ActionDump);
					bullet.Hit(character);
					bullet.HaveCollision.IsAvailable = false;
					bullet.ActionDump = null;
					break;
				}

			}
		}

		/// <summary>
		/// ほかのキャラと衝突してHP減少するフラグをリセット
		/// </summary>
		private static void ResetLossFlag(CharacterEntity[] characters, int max) {
			//
			for (int i = 0; i < max; i++) {
				characters[i].HaveBlackboard.ConditionBoard.IsLossHP = false;
			}
		}

		/// <summary>
		/// 移動座標がフィールドの外に出ていたら補正する
		/// </summary>
		private static void CorrectDeplayCharacter(CharacterEntity[] characters, int max, FieldEntity field) {
			//
			for (int i = 0; i < max; i++) {
				characters[i].ClampDeployPos(field.StageArea);
			}
		}
		/// <summary>
		/// 敵の更新処理
		/// </summary>
		private static void PreCalculateNearEnemy(CharacterEntity[] characters, int max) {
			//敵のヘイトリストとか持たせて攻撃対象決めたほうがいいのだが、微妙な違いしか出なさそうなので一旦一番近い敵だけ探す
            //@todo 全キャラの敵情報を毎回更新するのではなく少しタイミングをずらして一定周期で更新をするようにして計算量を減らす
			for (int i = 0; i < max; i++) {
				if (!characters[i].HaveUnitParam.Fight.IsAlive())
					continue;
				CharacterEntity alpha = characters[i];
                Vector3 posAlpha = alpha.GetPosition();

				//一番近いエネミーを検出する
				float lenSq = float.MaxValue;
				CharacterEntity target = null;
				for (int j = 0; j < max; j++) {
                    //自分自身は無視
                    if (i == j)
                        continue;
					CharacterEntity bravo = characters[j];
					//死んでいるキャラは対象外
					if (!bravo.HaveUnitParam.Fight.IsAlive())
						continue;

					//敵チームでない場合は無視
					if (alpha.Platoon == bravo.Platoon)
						continue;
					//ターゲットが遠い場合は無視
					float sq = (posAlpha - bravo.GetPosition()).sqrMagnitude;
					if (sq > lenSq)
						continue;
					//if(!alpha.IsInRange(bravo, alpha.HaveParam.Physical.SearchRange))
					//	continue;

					lenSq = sq;
					target = bravo;

				}
				alpha.HaveBlackboard.EnemyBoard.SetNearEnemy(target);
			}
        }
        /// <summary>
        /// 移動前のコリジョン計算
        /// </summary>
        private static void PreCalculateMove(CharacterEntity[] characters, int max) {
			for (int i = 0; i < max; i++) {
                CharacterEntity alpha = characters[i];
				//コリジョン計算しないキャラの場合は無視
				if (IgnoreCollision(alpha))
					continue;
				float posA = alpha.GetPosition().z;
                Vector3 pos1 = alpha.GetPosition() + alpha.MoveVector;
                float radius1 = alpha.HaveCollision.Radius;


				for (int j = 0; j < max; j++) {
					if (i == j)
						continue;
					CharacterEntity bravo = characters[j];
					//コリジョン計算しないキャラの場合は無視
					if (IgnoreCollision(bravo))
						continue;
					//同一の分隊に属しているときは計算無視
					if (IsSameSquad(alpha, bravo))
						continue;

					float az = alpha.MoveVector.z;
                    float bz = bravo.MoveVector.z;
					if (az == 0f && bz == 0f)
						continue;

					float posB = bravo.GetPosition().z;
                    Vector3 pos2 = bravo.GetPosition() + bravo.MoveVector;
                    float radius2 = bravo.HaveCollision.Radius;
                    //当たりをチェック
                    float dx = (radius1 + radius2) * ToleranceRatio - Mathf.Abs(pos1.x - pos2.x);
                    if (dx < GameConst.Battle.EPSILON)
                        continue;
                    float dz = (radius1 + radius2) * ToleranceRatio - Mathf.Abs(pos1.z - pos2.z);
                    if (dz < GameConst.Battle.EPSILON)
                        continue;

					//同一チームの場合
					if(alpha.Platoon.Index == bravo.Platoon.Index) {
						float toward = alpha.Platoon.TowardSign;
						//alphaが前列の場合
						if (alpha.Squad.Orbat < bravo.Squad.Orbat) {
							//進行方向が逆の場合はあたりを無視する
							if(az * toward >= 0f && bz * toward <= 0f)
								continue;
						} else {
							if (az * toward <= 0f && bz * toward >= 0f)
								continue;
						}
					}


					//進行方向同じで重なった場合
					if (az * bz >= 0f) {
                        //進行方向が正の場合
                        if((az != 0f && az > 0f) || (bz != 0f && bz > 0f)) {
                            //alpha側が前にいる
                            if(posA > posB) {
                                bravo.CorrectMove(bz - dz);
                                bravo.HaveUnitParam.Physical.CurrentSpeed = 0f;
								bravo.HaveBlackboard.KnockbackBorad.KnockBackSpeed = 0f;
							//bravo側が前にいる
							} else {
                                alpha.CorrectMove(az - dz);
                                alpha.HaveUnitParam.Physical.CurrentSpeed = 0f;
								alpha.HaveBlackboard.KnockbackBorad.KnockBackSpeed = 0f;
							}
							//進行方向が負の場合
						} else {
                            //alphaが前にいる
                            if (posA < posB) {
                                bravo.CorrectMove(bz + dz);
                                bravo.HaveUnitParam.Physical.CurrentSpeed = 0f;
								bravo.HaveBlackboard.KnockbackBorad.KnockBackSpeed = 0f;
								//bravoが前にいる
							} else {
                                alpha.CorrectMove(az + dz);
                                alpha.HaveUnitParam.Physical.CurrentSpeed = 0f;
								alpha.HaveBlackboard.KnockbackBorad.KnockBackSpeed = 0f;
							}
						}
                    //進行方向逆で重なった場合
                    } else {
                        //速度の割合に応じて譲歩の割合を変える
                        float total = Mathf.Abs(az) + Mathf.Abs(bz);
                        //alphaの進行方向が正の場合
                        if (az > 0f) {
                            alpha.CorrectMove(az - Mathf.Abs(bz) / total);
                        } else {
                            alpha.CorrectMove(az + Mathf.Abs(bz) / total);
                        }
                        //bravoの進行方向が正の場合
                        if (bz > 0f) {
                            bravo.CorrectMove(bz - Mathf.Abs(az) / total);
                        } else {
                            bravo.CorrectMove(bz + Mathf.Abs(az) / total);
                        }
                    }

                }
            }
        }
		/// <summary>
		/// コリジョン計算無視するかチェック
		/// </summary>
		private static bool IgnoreCollision(CharacterEntity entity) {
			//対象のキャラが死んでいる場合は無視
			if (!entity.HaveUnitParam.Fight.IsAlive())
				return true;
			if (entity.HaveBlackboard.ConditionBoard.IsIgnoewCollision)
				return true;


			return false;
		}
		/// <summary>
		/// 同一の分隊に属しているとき
		/// </summary>
		private static bool IsSameSquad(CharacterEntity alpha, CharacterEntity bravo) {
			if (alpha.Platoon == bravo.Platoon && alpha.Squad == bravo.Squad)
				return true;
			return false;
		}
		/// <summary>
		/// 同一の小隊に属しているとき
		/// </summary>
		private static bool IsSamePlatoon(CharacterEntity alpha, CharacterEntity bravo) {
			if (alpha.Platoon == bravo.Platoon)
				return true;
			return false;
		}

		/// <summary>
		/// HP減少の計算
		/// </summary>
		private static void PostCalculateLoss(CharacterEntity[] characters, int count) {
			float loss = GameConst.Battle.HP_LOSS_PENALTY * Time.deltaTime;
			for (int i = 0, max = count; i < max; i++) {
				CharacterEntity entity = characters[i];
				if (entity.HaveBlackboard.ConditionBoard.IsLossHP) {
					entity.HaveUnitParam.Fight.Hp -= loss;
				}
			}
		}

		/// <summary>
		/// 重なりを解消する補正計算
		/// </summary>
		private static void PostCalculatePush(CharacterEntity[] characters, int count, FieldEntity field) {
			Reset(characters, count);

			//ステージ外、敵側との重なりは重ならない位置に強制的に移動
			FixStage(characters, count, field);

            for (int i = 0, max = count; i < max; i++) {
                CharacterEntity alpha = characters[i];
				if (IgnoreCollision(alpha))
					continue;
				Vector3 pos1 = alpha.GetPosition();
                float radius1 = alpha.HaveCollision.Radius;

                for (int j = i + 1; j < max; j++) {
                    CharacterEntity bravo = characters[j];
					if (IgnoreCollision(bravo))
						continue;
					//特定の条件のキャラ同士は円の押出で計算
					if (alpha.Squad == bravo.Squad || (alpha.Platoon != bravo.Platoon)) {
						//リーダーと同グループキャラとの当たりはチェックしない
						//if (alpha.HaveParam.Party.Boss || bravo.HaveParam.Party.Boss)
						//	continue;

						Vector3 pos2 = bravo.GetPosition();
						float radius2 = bravo.HaveCollision.Radius;
						float dx = pos1.x - pos2.x;
						float dz = pos1.z - pos2.z;

						float lensq = dx * dx + dz * dz;
						float r = radius1 + radius2;
						//円が重ならない距離にいる場合は無視
						if (lensq > r * r)
							continue;

						float len = Mathf.Sqrt(lensq);
						float d = (len - (radius1 + radius2)) * 0.5f;

						alpha.HaveCollision.Correct.x -= dx / len * d;
						bravo.HaveCollision.Correct.x += dx / len * d;

						//alpha.HaveCollision.Correct.z -= dz / len * d;
						//bravo.HaveCollision.Correct.z += dz / len * d;
					//@note ノックバックしたときにまっすぐ後ろに移動して欲しいからAABBで計算
					//AABBによる押出で計算
					} else {

						Vector3 pos2 = bravo.GetPosition();
						float radius2 = bravo.HaveCollision.Radius;

						//当たりをチェック
						float dx = Mathf.Abs(pos1.x - pos2.x) - (radius1 + radius2);
						if (dx > GameConst.Battle.EPSILON)
							continue;
						float dz = Mathf.Abs(pos1.z - pos2.z) - (radius1 + radius2);
						if (dz > GameConst.Battle.EPSILON)
							continue;


						//変化量が少ないほうで押し戻しを行う
						if (dz < dx) {
							//横方向で押出
							CalcHorizontalCorrect(alpha, bravo, dx);
						} else {
							CalcVerticalCorrectInner(alpha, bravo, dz);
						}
						//味方と密集しているのでHPペナルティ
						alpha.HaveBlackboard.ConditionBoard.IsLossHP = true;
						bravo.HaveBlackboard.ConditionBoard.IsLossHP = true;
					}
				}
            }

            //補正値を適用する
            Apply(characters, count);
        }
        /// <summary>
        /// 水平(x)方向押出量計算
        /// </summary>
        private static void CalcHorizontalCorrect(CharacterEntity alpha, CharacterEntity bravo, float dx) {
            float hdx = dx * 0.5f;
            if (alpha.GetPosition().x < bravo.GetPosition().x) {
                alpha.HaveCollision.Correct.x += hdx;
                bravo.HaveCollision.Correct.x += -hdx;
            } else {
                alpha.HaveCollision.Correct.x += -hdx;
                bravo.HaveCollision.Correct.x += hdx;
            }
        }

        /// <summary>
        /// チーム内キャラ同士の垂直(z)方向押出量計算
        /// </summary>
        /// <remarks>
        /// 後方にあるキャラだけが押し出されるようにする
        /// </remarks>
        private static void CalcVerticalCorrectInner(CharacterEntity alpha, CharacterEntity bravo, float dz) {
            if (alpha.Platoon.TowardSign > 0f) {
                if (alpha.GetPosition().z > bravo.GetPosition().z) {
	                if(alpha.Squad.Orbat < bravo.Squad.Orbat)
	                    bravo.HaveCollision.Correct.z += dz;
                } else {
	                if(alpha.Squad.Orbat > bravo.Squad.Orbat)
	                    alpha.HaveCollision.Correct.z += dz;
                }
            } else {
                if (alpha.GetPosition().z > bravo.GetPosition().z) {
	                if(alpha.Squad.Orbat > bravo.Squad.Orbat)
	                    alpha.HaveCollision.Correct.z -= dz;
                } else {
	                if(alpha.Squad.Orbat < bravo.Squad.Orbat)
	                    bravo.HaveCollision.Correct.z -= dz;
                }
            }

        }
        /// <summary>
        /// チーム外キャラ同士の垂直(z)方向押出量計算
        /// </summary>
        /// <remarks>
        /// 双方に押出をする
        /// </remarks>
        private static void CalcVerticalCorrectOuter(CharacterEntity alpha, CharacterEntity bravo, float dz) {
            float hdz = dz * 0.5f;
            if (alpha.GetPosition().z > alpha.GetPosition().z) {
                alpha.HaveCollision.Correct.z += -hdz;
                bravo.HaveCollision.Correct.z += hdz;
            } else {
                alpha.HaveCollision.Correct.z += hdz;
                bravo.HaveCollision.Correct.z += -hdz;
            }
        }

		/// <summary>
		/// ステージをはみ出ないように補正
		/// </summary>
        private static void FixStage(CharacterEntity[] characters, int count, FieldEntity field) {
            for (int i = 0, max = count; i < max; i++) {
				characters[i].ClampCharacterPos(field.StageArea);
            }

        }
		/// <summary>
		/// 補正値をリセット
		/// </summary>
		private static void Reset(CharacterEntity[] characters, int count) {
			for (int i = 0, max = count; i < max; i++) {
				characters[i].HaveCollision.Correct = Vector3.zero;
			}
		}

		/// <summary>
		/// 補正値を適用する
		/// </summary>
		private static void Apply(CharacterEntity[] characters, int count) {
            for (int i = 0, max = count; i < max; i++) {
                CharacterEntity entity = characters[i];
                Vector3 pos = entity.GetPosition() + entity.HaveCollision.Correct * 0.3f;
                entity.SetPosition(pos);
            }
        }
    }
}
