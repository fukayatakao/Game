using System;
using UnityEngine;

namespace Project.Game {
	//@note 乱数固定したいときはこちらを使う。
	/// <summary>
	/// 決定論的乱数クラス
	/// </summary>
	public class DeterminateRandom : MonoBehaviour {
		private static int seed_;
		private static int seek_ = -1;
		public static int Seek {
			get {
				return seek_;
			}
			set {
				seek_ = value;
				random_ = new System.Random(seed_);
				for (int i = 0; i < seek_; i++){
					random_.Next();
				}
			}
		}

		private static System.Random random_ = new System.Random();

		/// <summary>
		/// 種指定
		/// </summary>
		public static void SetSeed(int seed) {
			if(seed < 0) {
				seed_ = DateTime.Now.Millisecond;
			}else{
				seed_ = seed;
			}

			random_ = new System.Random(seed_);
		}

		/// <summary>
		/// 乱数取得
		/// </summary>
		public static int Range(int min, int max) {
			Debug.Assert(max > min, "random range error");
			return min + Rand() % (max - min);
		}
		public static float Range(float min, float max) {
			Debug.Assert(max >= min, "random range error");
			int value = Rand();
			float t = (float)value / Int32.MaxValue;

			return (t * (max - min)) + min;
		}

		/// <summary>
		/// ダイスを振る
		/// </summary>
		private static int Rand() {
			int val = random_.Next();
			seek_++;
			return val;
		}
	}

}
