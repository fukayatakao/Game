using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// 状態変化クラス
	/// </summary>
	public class CharacterStatus : MonoPretender {
		StatusCondition[] status_ = new StatusCondition[(int)Condition.MAX];
		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
		}

	}
}
