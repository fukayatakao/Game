using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 分隊のAI用記録
    /// </summary>
    public class PlatoonBlackboard : CommonBlackboard {
		//敵対チーム
		public PlatoonEntity Opponent;
		//前回入れ替えを行った時刻
		public float SwapExecTime;
		//ラッシュフラグ
		public bool RushLine;


		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(PlatoonEntity owner) {
		}

	}
}

