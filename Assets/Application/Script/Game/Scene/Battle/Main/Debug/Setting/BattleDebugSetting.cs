using UnityEngine;



namespace Project.Game {

	/// <summary>
	/// バトルのデバッグ設定をまとめたクラス
	/// </summary>
	[DefaultExecutionOrder(-1)]
	public class BattleDebugSetting : MonoBehaviour {
//クラス自体をくくると定義がないときにMissing扱いになるのでクラス定義だけは残るようにする
#if DEVELOP_BUILD
		//隠れシングルトン
		private static BattleDebugSetting singleton_;


		//開始演出スキップ
		[SerializeField]
		private bool isOpeningSkip = false;
		public static bool IsOpeningSkip {
			get {
#if RUN_BACKGROUND
				return true;
#else
				return singleton_.isOpeningSkip;
#endif
			}
		}

		//デバッグ表示の切り替え
		[SerializeField]
		private bool isCollisionDraw = false;


		[SerializeField]
		private bool isTargetDraw = false;
		[SerializeField]
		private bool isLineDraw = false;
		[SerializeField]
		private bool isAIDraw = false;
		[SerializeField]
		private bool isPhaseDraw = false;



		//空のエフェクトを使ってエフェクト表示をoffにする
		[SerializeField]
		private bool isEmptyEffect = false;

		//AI制御
		//キャラクターのAIを止める。動きも止める。
		[SerializeField]
		private bool isDisableCharaAI = false;
		[SerializeField]
		private bool isDisablePlayerAI = false;
		[SerializeField]
		private bool isDisableEnemyAI = false;

		[SerializeField]
		private bool isReport = false;

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
				SettingAI();

				request_ = false;
			}
		}


		public static void Setting() {
			//エフェクトのon/off設定
			EffectAssembly.I.IsEmpty = singleton_.isEmptyEffect;
			CommonMessage.CommonDebugDraw.Broadcast(singleton_.isCollisionDraw, singleton_.isTargetDraw, singleton_.isLineDraw, singleton_.isAIDraw, singleton_.isPhaseDraw, singleton_.isReport);

		}

		public static void SettingAI() {
			BattleMessage.SwitchAutoPlay.Broadcast(Power.Player, singleton_.isDisablePlayerAI);
			BattleMessage.SwitchAutoPlay.Broadcast(Power.Enemy, singleton_.isDisableEnemyAI);
			BattleMessage.CharacterAI.Broadcast(null, singleton_.isDisableCharaAI);
		}
#endif
	}
}
