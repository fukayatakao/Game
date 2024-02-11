using UnityEngine;

namespace Project.Game {

	/// <summary>
	/// タウンのデバッグ設定をまとめたクラス
	/// </summary>
	[DefaultExecutionOrder(-1)]
	public class TownDebugSetting : MonoBehaviour {
//クラス自体をくくると定義がないときにMissing扱いになるのでクラス定義だけは残るようにする
#if DEVELOP_BUILD
		//隠れシングルトン
		private static TownDebugSetting singleton_;

		//建物配置管理のグリッド表示
		[SerializeField]
		private bool isDrawGrid = false;
		//グリッド表示のタイル数
		[SerializeField]
		private int tileMax = 1000;
		//グリッド表示の幅
		[SerializeField]
		private int gridWidth = 30;
		//グリッド表示の奥行
		[SerializeField]
		private int gridDepth = 30;


		//空のエフェクトを使ってエフェクト表示をoffにする
		[SerializeField]
		private bool isEmptyEffect = false;
		public static bool IsEmptyEffect { get { return singleton_.isEmptyEffect; } }

		bool request_;

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		private void Awake() {
			request_ = false;
			//シングルトンのインスタンスが存在してたらエラー
			Debug.Assert(singleton_ == null, "alread create instance");
			singleton_ = this;
		}
		/// <summary>
		/// インスタンス破棄時処理
		/// </summary>
		private void OnDestroy() {
			singleton_ = null;
		}

		/// <summary>
		/// インスペクタの値に変化があった
		/// </summary>
		void OnValidate() {
			if (singleton_ == null)
				return;

			request_ = true;
		}

		private void Update() {
			if (singleton_ == null)
				return;

			if (request_) {
				Setting();

				request_ = false;
			}
		}


		public static void Setting() {
			//エフェクトのon/off設定
			EffectAssembly.I.IsEmpty = IsEmptyEffect;
			TownMessage.TownGridDraw.Broadcast(singleton_.isDrawGrid, singleton_.tileMax, singleton_.gridWidth, singleton_.gridDepth);

		}

#endif
	}
}
