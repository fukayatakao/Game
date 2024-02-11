using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// タウンホールパラメータ
	/// </summary>
	[System.Serializable]
	public class TownhallParam : MonoPretender
	{
		public int Gold;

		public int Tax;
		//流通ポイント
		public int Logistics;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public void Setup(int gold, int logistics) {
			Logistics = logistics;
			Gold = gold;
		}
	}
}
