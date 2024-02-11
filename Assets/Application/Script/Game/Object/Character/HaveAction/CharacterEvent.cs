using Project.Lib;
using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// ActEventで呼ばれる処理
	/// </summary>
	[UnityEngine.Scripting.Preserve]
	public static class CharacterEvent {
		[Function("攻撃コリジョン設定")]
		[Arg(0, typeof(long), "AttackHitId", (long)0)]
		public static void SetAttackCollision(CharacterEntity entity, object[] args) {
			long dataId = (long)args[0];
		}

        [Function("アニメーション再生")]
        [Arg(0, typeof(BattleMotion), "アニメーション", BattleMotion.Idle)]
        public static void PlayAnimation(CharacterEntity entity, object[] args) {
            entity.HaveAnimation.Play((BattleMotion)args[0]);
        }

        //@note ActEventEditorで特殊な再生方法で処理されるAnimation関数
        [Function("専用アニメーション再生")]
        [Arg(0, typeof(string), "アニメーション", "")]
        public static void PlayActEventAnimation(CharacterEntity entity, object[] args) {
            entity.HaveAnimation.Play((string)args[0]);
        }

        [Function("攻撃当たり判定")]
        public static void Hit(CharacterEntity entity, object[] args) {
            //バトル計算
            BattleCombat.CalculateCombat(entity.HaveBlackboard.ActionDump);
			entity.HaveBlackboard.ActionDump = null;
		}

		[Function("エフェクト再生")]
		[Arg(0, typeof(string), "エフェクト名", "")]
		[Arg(1, typeof(CharacterNodeCache.Part), "接続ノード", CharacterNodeCache.Part.Head)]
		[Arg(2, typeof(bool), "座標同期", true)]
		[Arg(3, typeof(Vector3), "座標オフセット", "0,0,0")]
		[Arg(4, typeof(bool), "回転同期", true)]
		[Arg(5, typeof(Vector3), "回転オフセット", "0,0,0")]
		[Arg(6, typeof(bool), "拡縮同期", true)]
		[Arg(7, typeof(Vector3), "拡縮オフセット", "1,1,1")]
		[Arg(8, typeof(float), "再生時間", 999f)]
		//[Resource((int)ResourcePath.Effect, 0)]
		public static void Effect(CharacterEntity entity, object[] args) {
			string name = (string)args[0];
			CharacterNodeCache.Part node = (CharacterNodeCache.Part)args[1];
			bool constrainPos = (bool)args[2];
			Vector3 pos = (Vector3)args[3];
			bool constrainRot = (bool)args[4];
			Vector3 rot = (Vector3)args[5];
			bool constrainScl = (bool)args[6];
			Vector3 scl = (Vector3)args[7];

			float life = (float)args[8];

			EffectAssembly.I.CreateAsync(name, (effect) => {
				effect.SetLifeTime(life);

				effect.Constrain(entity.HaveCacheNode.GetNode(node));
				if (constrainPos) {
					effect.HaveConstrain.ConstrainPosition(pos);
				}
				if (constrainRot) {
					effect.HaveConstrain.ConstrainRotation(Quaternion.Euler(rot.x, rot.y, rot.z));
				}
				if (constrainScl) {
					effect.HaveConstrain.ConstrainScale(scl);
				}
			});
		}
		[Function("カメラ再生")]
		[Arg(0, typeof(string), "カメラ名", "")]
		[Arg(1, typeof(CharacterNodeCache.Part), "接続ノード", CharacterNodeCache.Part.Root)]
		public static void PlayCamera(CharacterEntity entity, object[] args) {
			string name = (string)args[0];
			CharacterNodeCache.Part node = (CharacterNodeCache.Part)args[1];

			EffectAssembly.I.CreateCameraAsync(name, (effect) => {
				effect.Constrain(entity.HaveCacheNode.GetNode(node));
				effect.HaveConstrain.ConstrainPosition(Vector3.zero);
				effect.HaveConstrain.ConstrainRotation(Quaternion.identity);
			});
		}
		[Function("矢を撃つ")]
		[Arg(0, typeof(string), "モデル名", "")]
		[Arg(1, typeof(string), "開始ノード", "")]
		public static void Shoot(CharacterEntity entity, object[] args) {
			string name = (string)args[0];
			string nodeName = (string)args[1];
			//一旦ローカル変数に受けないとBulletの非同期生成してる間に値が変化する可能性がある
			var dump = entity.HaveBlackboard.ActionDump;
			//エディタではnullの場合があるので適当に作る
#if UNITY_EDITOR
			if (dump == null) {
				dump = new ActionDumpData(entity, null, null);
			}

#endif
			entity.HaveBlackboard.ActionDump = null;
			BulletAssembly.I.CreateAsync(name, (effect) => {
				effect.Shoot(dump, nodeName);
			});
		}

		[Function("現在座標から相対指定でワープ")]
		[Arg(0, typeof(Vector3), "座標オフセット", "0,0,0")]
		public static void SetPositionRelative(CharacterEntity entity, object[] args) {
			Vector3 pos = (Vector3)args[0];
			entity.SetPosition(entity.GetPosition() + pos);
		}

		[Function("移動")]
		[Arg(0, typeof(Vector3), "移動量", "0,0,0")]
		[Arg(1, typeof(float), "移動時間", 0f)]
		public static void Move(CharacterEntity entity, object[] args) {
			Vector3 pos = (Vector3)args[0];
			float time = (float)args[1];
			entity.ParallelTask.Add(CommonCommand.Move(entity, pos, time));
		}

		[Function("アルファ変化")]
		[Arg(0, typeof(float), "初期値", 0f)]
		[Arg(1, typeof(float), "最終値", 0f)]
		[Arg(2, typeof(float), "時間", 0f)]
		public static void ChangeAlphaLinear(CharacterEntity entity, object[] args) {
			float start = (float)args[0];
			float end = (float)args[1];
			float time = (float)args[2];
			entity.ParallelTask.Add(CharacterCommand.ChangeAlphaLinear(entity, start, end, time));
		}

		[Function("なにもしない")]
		public static void None(CharacterEntity entity, object[] args) {
		}


		[Function("テスト")]
        [Arg(0, typeof(string), "文字列", "")]
        [Arg(1, typeof(int), "整数", 0)]
        [Arg(2, typeof(float), "浮動小数", 0f)]
        [Arg(3, typeof(BattleMotion), "選択", BattleMotion.Idle)]
        public static void Dummy(CharacterEntity entity, object[] args) {
            Debug.Log("call dummy");
        }

#if UNITY_EDITOR
		public enum ResourcePath {
			Effect,
			SoundSE,
			SoundVoice,
		}

		/// <summary>
		/// 引数の値からリソースパスを作る
		/// </summary>
		public static string CreateResourcePath(int type, object arg) {
			switch ((ResourcePath)type) {
			case ResourcePath.Effect:
				return "Effect/" + (string)arg;
			default:
				Debug.LogError("not found resource type:" + type);
				return (string)arg;
			}

		}
#endif
	}
}
