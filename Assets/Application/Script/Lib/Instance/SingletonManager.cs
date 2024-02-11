using System;


namespace Project.Lib {
	/// <summary>
	/// Singleton管理
	/// </summary>
	public class SingletonManager {
		//生成・破棄の関数を配列化
		private System.Action[] createInstance_;
		private System.Action[] destroyInstance_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SingletonManager(Type[] singletons) {
			createInstance_ = Array.ConvertAll(singletons, str => (System.Action)Delegate.CreateDelegate(typeof(System.Action), str.GetMethod("CreateInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
			destroyInstance_ = Array.ConvertAll(singletons, str => (System.Action)Delegate.CreateDelegate(typeof(System.Action), str.GetMethod("DestroyInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public void CreateInstance() {
			//明示的にシングルトンのインスタンスを作る
			for (int i = 0, max = createInstance_.Length; i < max; i++) {
				UnityUtil.Call(createInstance_[i]);
			}

		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public void DestroyInstance() {
			//シングルトンのインスタンスを破棄する。明示的に破棄しないとアプリ終わるまで残り続ける。
			for (int i = destroyInstance_.Length - 1; i >= 0; i--) {
				UnityUtil.Call(destroyInstance_[i]);
			}
		}

	}
}
