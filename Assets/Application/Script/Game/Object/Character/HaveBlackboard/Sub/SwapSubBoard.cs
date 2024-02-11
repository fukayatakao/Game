using System;

namespace Project.Game {
	/// <summary>
	/// 交代用情報
	/// </summary>
	[Serializable]
	public class SwapSubBoard {
		// 入れ替え中か
		public bool IsSwap;
		public SquadEntity SwapTarget;
	}
}
