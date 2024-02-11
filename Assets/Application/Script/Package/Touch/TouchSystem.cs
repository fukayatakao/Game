using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// タッチ操作
	/// </summary>
	public class TouchSystem : Singleton<TouchSystem> {
        TouchController controller_;

        /// <summary>
        /// ぷニコン的コントローラーで得られた値
        /// </summary>
		public static class Controller{
			public static float Strength{ get { return I.controller_.TargetY; } }
			public static Quaternion Direction{ get { return I.controller_.RotationXZ; } }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TouchSystem(){
			GameObject obj = Gesture.Create();
			controller_ = obj.AddComponent<TouchController>();
		}

		/// <summary>
		/// 処理を有効化
		/// </summary>
		public void Enable(){
			Gesture.Enable();
        }
		/// <summary>
		/// 処理を無効化
		/// </summary>
		public void Disable(){
			Gesture.Disable();
		}
	}

}
