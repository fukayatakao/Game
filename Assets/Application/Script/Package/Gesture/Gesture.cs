using UnityEngine;

namespace Project.Lib {
    //@note DefaultExecutionOrderで他のUpdateよりも前に呼ぶようにする
    /// <summary>
    /// タッチ(とマウス)のメイン処理
    /// </summary>
    [DefaultExecutionOrder(-2)]
    public class Gesture : MonoBehaviour {
        //タッチ制御
        public static bool IsTouchDown(int index = 0){
			Debug.Assert (instance_ != null, "gesture instance is null :IsTouchDown");
			return instance_.detectTouch_[index].IsTouchDown;
		}
		public static bool IsTouchUp(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :IsTouchUp");
			return instance_.detectTouch_ [index].IsTouchUp;
		}
		public static bool IsTouch(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :IsTouch");
			return instance_.detectTouch_ [index].IsTouch;
		}
		public static Vector2 GetTouchPos(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :TouchPos");
			return instance_.detectTouch_ [index].TouchPos;
		}

		//フリック制御
		public static bool IsFlick(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :IsFlick");
			return instance_.controlFlick_ [index].IsFlick;
		}
		public static Vector2 GetFlickDir(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :FlickDir");
			return instance_.controlFlick_ [index].Direct;
		}
		//フリックと判断する時間
		public static float GetFlickAccurateTime(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateTime.get");
			return instance_.controlFlick_[index].AccurateTime;
		}
		//フリックと判断する時間
		public static void SetFlickAccurateTime(float value, int index) {
			Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateTime.set");
			instance_.controlFlick_[index].AccurateTime = value;
		}
		//フリックと判断する距離の2乗
		public static float GetFlickAccurateDistanceSq(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateDistanceSq.get");
			return instance_.controlFlick_[index].AccurateDistanceSq;
		}
		public static void SetFlickAccurateDistanceSq(float value, int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateDistanceSq.set");
			instance_.controlFlick_ [index].AccurateDistanceSq = value;
		}
		//スワイプ制御
		public static bool IsSwipe(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :IsSwipe");
			return instance_.controlSwipe_ [index].IsSwipe;
		}
		public static Vector2 GetSwipeDeltaPos(int index = 0){
			Debug.Assert (instance_ != null, "gesture instance is null :DeltaPos");
			return instance_.controlSwipe_ [index].DeltaPos;
		}
		public static Vector2 GetSwipeDirect(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :Direct");
			return instance_.controlSwipe_ [index].Direct;
		}

		//ロングタップ制御
		public static bool IsLongTap(int index = 0){
			Debug.Assert (instance_ != null, "gesture instance is null :IsLongTap");
			return instance_.controlLongTap_ [index].IsLongTap;
		}
		public static float GetLongTapTime(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :LongTapTime");
			return instance_.controlLongTap_ [index].LongTapTime;
		}
		//ロングタップと判断する時間
		public static float GetLongTapAccurateTime(int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :LongTapAccurateTime.get");
			return instance_.controlLongTap_ [index].AccurateTime;
		}
		public static void SetLongTapAccurateTime(float value, int index = 0) {
			Debug.Assert (instance_ != null, "gesture instance is null :LongTapAccurateTime.set");
			instance_.controlLongTap_[index].AccurateTime = value;
		}
		//ピンチ制御
		public static bool IsPinchIn{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsPinchIn"); return instance_.detectPinch_.IsPinchIn; } }
		public static bool IsPinchOut{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsPinchOut"); return instance_.detectPinch_.IsPinchOut; } }
		public static float PinchDeltaDistance{ get { Debug.Assert (instance_ != null, "gesture instance is null :DeltaDistance"); return instance_.detectPinch_.DeltaDistance; } }


		//入力制御をつけるgameobject名
		const string GestureObjerctName = "Gesture";
		private static GameObject gestureObject_;

		/// <summary>
		/// インスタンス作成
		/// </summary>
		public static GameObject Create() {
			if(gestureObject_ == null){
				//ゲームオブジェクト未作成の場合作る
				gestureObject_ = new GameObject(GestureObjerctName);
				instance_ = gestureObject_.AddComponent<Gesture>();
				GameObject.DontDestroyOnLoad(gestureObject_);
			}

			Enable();

			return gestureObject_;
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public static void Destroy() {
			//シーン遷移では削除されないので明示的に削除する
			if (gestureObject_ != null) {
				GameObject.Destroy(gestureObject_);
				gestureObject_ = null;
			}
		}
		/// <summary>
		/// 処理を有効化
		/// </summary>
		public static void Enable(){
			//検出処理するか設定
			for (int i = 0; i < MAX_DETECT_FINGER; i++) {
				instance_.controlFlick_[i].Enable = true;
				instance_.controlLongTap_[i].Enable = true;
				instance_.controlSwipe_[i].Enable = true;
			}
			instance_.detectPinch_.Enable = true;
		}
		/// <summary>
		/// 処理を無効化
		/// </summary>
		public static void Disable(){
			//検出処理するか設定
			for (int i = 0; i < MAX_DETECT_FINGER; i++) {
				instance_.controlFlick_[i].Enable = false;
				instance_.controlLongTap_[i].Enable = false;
				instance_.controlSwipe_[i].Enable = false;
			}
			instance_.detectPinch_.Enable = false;
		}

		//シングルトン
		private static Gesture instance_;

		const int MAX_DETECT_FINGER = 3;
		//〜検知はInputを直接使って判定、〜操作は検知の結果を使って二次的に判定
		//タッチ検知
		private DeviceDetectMultiTouch[] detectTouch_ = new DeviceDetectMultiTouch[MAX_DETECT_FINGER];
		//フリック操作
		private DeviceControlFlick[] controlFlick_ = new DeviceControlFlick[MAX_DETECT_FINGER];
		//長押し操作
		private DeviceControlLongTap[] controlLongTap_ = new DeviceControlLongTap[MAX_DETECT_FINGER];
		//スワイプ操作
		private DeviceControlSwipe[] controlSwipe_ = new DeviceControlSwipe[MAX_DETECT_FINGER];
		//ピンチ検知
		private DeviceDetectPinch detectPinch_ = new DeviceDetectPinch ();

#if DEVELOP_BUILD
		//タッチ座標がデバッグウィンドウに含まれていたら無効とする
		public static bool IsValid;
#endif
		/// <summary>
		/// インスタンス生成処理
		/// </summary>
		private void Awake(){
			instance_ = this;
#if DEVELOP_BUILD
            IsValid = true;
#endif
			//検出処理するか設定
			for (int i = 0; i < MAX_DETECT_FINGER; i++) {
				detectTouch_ [i] = new DeviceDetectMultiTouch ();
				controlFlick_ [i] = new DeviceControlFlick ();
				controlLongTap_ [i] = new DeviceControlLongTap ();
				controlSwipe_ [i] = new DeviceControlSwipe ();
			}

			//検出処理するか設定
			for (int i = 0; i < MAX_DETECT_FINGER; i++) {
				controlFlick_[i].Enable = true;
				controlLongTap_[i].Enable = true;
				controlSwipe_[i].Enable = true;
			}
			detectPinch_.Enable = true;

		}

		/// <summary>
		/// 実行処理
		/// </summary>
		/// <remarks>
		/// Edit->Project SettingData->Script Execution OrderでdeltaTimeより前(-1)にして他のUpdateよりも前に呼ぶようにする
		/// </remarks>
		private void Update(){
#if DEVELOP_BUILD
			if (!IsValid) {
#if UNITY_EDITOR || UNITY_STANDALONE
                if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
#else
				if (Input.touchCount == 0) {
#endif

					IsValid = true;
				} else {
					return;
				}
			}
#endif
			//各操作の検出
			detectPinch_.Execute ();
			for (int i = 0; i < MAX_DETECT_FINGER; i++) {
				detectTouch_[i].Execute (i);
#if DEVELOP_BUILD
				//タッチした場所がデバッグウィンドウにかかっていたら操作無効にする
				if (DebugWindowManager.InWindowArea(GetTouchPos(i))) {
					detectTouch_[i].TouchCancel();
					IsValid = false;
					continue;
				}
#endif
				controlFlick_[i].Execute (detectTouch_ [i]);
				controlLongTap_[i].Execute (detectTouch_ [i]);
				controlSwipe_[i].Execute (detectTouch_ [i]);
			}
		}

	}


}
