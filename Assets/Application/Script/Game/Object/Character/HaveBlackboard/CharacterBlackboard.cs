using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// キャラクターのAI用記憶
	/// </summary>
	public class CharacterBlackboard : CommonBlackboard {
		//敵に関する記憶
		public EnemySubBoard EnemyBoard;
		//状態に関する記憶
		public ConditionSubBoard ConditionBoard;
		//交代に関する記憶
		public SwapSubBoard SwapBoard;
		// 攻撃選択関係の記憶
		public AttackSubBoard AttackBoard;
		// ノックバック関係の記憶
		public KnockbackSubBoard KnockbackBorad;
		//独立行動中か
		public bool IsMobile;

		//命令遅延(stateの遅延と混在するので混乱しそう。統一できるならしたい)
		public float Delay;

		//移動速度
		public float Speed;


		public ActionDumpData ActionDump;
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			EnemyBoard = new EnemySubBoard();
			ConditionBoard = new ConditionSubBoard();
			SwapBoard = new SwapSubBoard();
			AttackBoard = new AttackSubBoard();
			KnockbackBorad = new KnockbackSubBoard();
		}


		public void Execute(CharacterEntity owner) {
			ConditionBoard.Execute(owner);
			EnemyBoard.Execute(owner);
			AttackBoard.Execute(owner);
		}

	}
}

