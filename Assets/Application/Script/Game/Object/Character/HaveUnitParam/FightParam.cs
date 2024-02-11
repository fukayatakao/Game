using UnityEngine;

namespace Project.Game {
    /// <summary>
    /// キャラクターの戦闘能力
    /// </summary>
	[System.Serializable]
    public class FightParam {
#if DEVELOP_BUILD
		public bool isInfinityHp;
		public bool isInfinityLp;
#endif

		//HP
		[SerializeField]
        private float hp_;
        public float Hp {
            get { return hp_; }
            set {
#if DEVELOP_BUILD
				//HP変化無効のデバッグ機能
				if (isInfinityHp)
					return;
#endif
				hp_ = value;
                if (hp_ < 0f) hp_ = 0f;
                else if (hp_ > MaxHp) hp_ = MaxHp;
            }
        }

        public float MaxHp;

        //LP
        [SerializeField]
        private float lp_;
        public float Lp {
            get { return lp_; }
            set {
#if DEVELOP_BUILD
				//LP変化無効のデバッグ機能
				if (isInfinityLp)
					return;
#endif
				lp_ = value;
                if (lp_ < 0f)
                    lp_ = 0f;
                else if (lp_ > MaxLp)
                    lp_ = MaxLp;
            }
        }

        public float MaxLp;

        //@note 置き場所は考える。頻繁に使う用ならEntity直下あたりに移動する
        /// <summary>
        /// 生きているか
        /// </summary>
        public bool IsAlive() {
			return lp_ > 0f;
		}
        //インフレ要素
        //攻撃力
        public int Attack;
        //防御力
        public int Defence;

		//時間要素
        //HPの回復力(%/s)
        public float Recovery;

        //補助パラメータ
        //ライフダメージ用パラメータ
        //貫通(ライフポイントに直接ダメージを与える率)
        public int Pircing;
        //装甲(貫通に対する防御率)
        public int Armor;

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(Mst.MstUnitData unit) {
			MaxHp = unit.MaxHp;
			Hp = MaxHp;

			MaxLp = unit.MaxLp;
			Lp = MaxLp;
            //攻撃力
            Attack = unit.Attack;
			//防御力
			Defence = unit.Defence;

			//HPの回復力
			Recovery = MaxHp * unit.Recovery / 100f;

			//貫通
			Pircing = unit.Pircing;
			//装甲
			Armor = unit.Armor;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			//時間経過による回復
			if (owner.HaveBlackboard.ConditionBoard.IsRecoverState) {
				BattleCombat.CalculateRecover(owner);
			}

			//旗色が悪い時の逃亡計算
			BattleCombat.CalculateDecay(owner);
		}
    }
}
