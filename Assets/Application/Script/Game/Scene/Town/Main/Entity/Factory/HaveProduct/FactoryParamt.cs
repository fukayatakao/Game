using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// 生産施設パラメータ
	/// </summary>
	[System.Serializable]
	public class FactoryParam : MonoPretender {
		//生産物情報
		public int Product;
		//生産物グレード
		public int Grade;
		//生産量
		public int Output;
		//最大労働者数
		public int Worker;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public void Setup(int goodsId, int grade, int output, int worker) {
			Product = goodsId;
			Debug.Assert(grade > 0, "grade value error:" + grade);
			Grade = grade;
			Output = output;
			Worker = worker;
		}
	}
}
