using UnityEngine;

namespace Project.Lib {
    //@note DefaultExecutionOrderで他のUpdateよりも前に呼ぶようにする
    /// <summary>
    /// タッチ(とマウス)のメイン処理
    /// </summary>
    [DefaultExecutionOrder(-2)]
	public class SingleGesture : MonoBehaviour {
		//タッチ制御
		public static bool IsTouchDown{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsTouchDown");  return instance_.detectTouch_.IsTouchDown; } }
		public static bool IsTouchUp{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsTouchUp");  return instance_.detectTouch_.IsTouchUp; } }
		public static bool IsTouch{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsTouch");  return instance_.detectTouch_.IsTouch; } }
		public static Vector2 TouchPos{ get { Debug.Assert (instance_ != null, "gesture instance is null :TouchPos"); return instance_.detectTouch_.TouchPos; } }

		//フリック制御
		public static bool IsFlick{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsFlick"); return instance_.controlFlick_.IsFlick; } }
		public static Vector2 FlickDir{ get { Debug.Assert (instance_ != null, "gesture instance is null :FlickDir"); return instance_.controlFlick_.Direct; } }
		//フリックと判断する時間
		public static float FlickAccurateTime {
			get { Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateTime.get"); return instance_.controlFlick_.AccurateTime; }
			set { Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateTime.set"); instance_.controlFlick_.AccurateTime = value; }
		}
		//フリックと判断する距離の2乗
		public static float FlickAccurateDistanceSq {
			get { Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateDistanceSq.get"); return instance_.controlFlick_.AccurateDistanceSq; }
			set { Debug.Assert (instance_ != null, "gesture instance is null :FlickAccurateDistanceSq.set"); instance_.controlFlick_.AccurateDistanceSq = value; }
		}

		//スワイプ制御
		public static bool IsSwipe{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsSwipe"); return instance_.controlSwipe_.IsSwipe; } }
		public static Vector2 DeltaPos{ get { Debug.Assert (instance_ != null, "gesture instance is null :DeltaPos"); return instance_.controlSwipe_.DeltaPos; } }
		public static Vector2 Direct{ get { Debug.Assert (instance_ != null, "gesture instance is null :Direct"); return instance_.controlSwipe_.Direct; } }

		//ロングタップ制御
		public static bool IsLongTap{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsLongTap"); return instance_.controlLongTap_.IsLongTap; } }
		public static float LongTapTime{ get { Debug.Assert (instance_ != null, "gesture instance is null :LongTapTime"); return instance_.controlLongTap_.LongTapTime; } }
		//ロングタップと判断する時間
		public static float LongTapAccurateTime {
			get { Debug.Assert (instance_ != null, "gesture instance is null :LongTapAccurateTime.get"); return instance_.controlLongTap_.AccurateTime; }
			set { Debug.Assert (instance_ != null, "gesture instance is null :LongTapAccurateTime.set"); instance_.controlLongTap_.AccurateTime = value; }
		}

		//ピンチ制御
		public static bool IsPinchIn{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsPinchIn"); return instance_.detectPinch_.IsPinchIn; } }
		public static bool IsPinchOut{ get { Debug.Assert (instance_ != null, "gesture instance is null :IsPinchOut"); return instance_.detectPinch_.IsPinchOut; } }
		public static float DeltaDistance{ get { Debug.Assert (instance_ != null, "gesture instance is null :DeltaDistance"); return instance_.detectPinch_.DeltaDistance; } }

		//シングルトン
		private static SingleGesture instance_;

        //DeviceDetect〜はInputを直接使って判定、DeviceControl〜は検知の結果を使って二次的に判定
        //タッチ検知
        private DeviceDetectTouch detectTouch_ = new DeviceDetectTouch ();
		//フリック操作
		private DeviceControlFlick controlFlick_ = new DeviceControlFlick();
		//長押し操作
		private DeviceControlLongTap controlLongTap_ = new DeviceControlLongTap ();
		//スワイプ操作
		private DeviceControlSwipe controlSwipe_ = new DeviceControlSwipe ();
		//ピンチ検知
		private DeviceDetectPinch detectPinch_ = new DeviceDetectPinch ();

		/// <summary>
		/// インスタンス生成処理
		/// </summary>
		private void Awake(){
			instance_ = this;

			//検出処理するか設定
			controlFlick_.Enable = true;
			controlLongTap_.Enable = true;
			controlSwipe_.Enable = true;
			detectPinch_.Enable = true;

		}

		/// <summary>
		/// 実行処理
		/// </summary>
		private void Update(){
			//各操作の検出
			detectTouch_.Execute ();
			detectPinch_.Execute ();
			controlFlick_.Execute (detectTouch_);
			controlLongTap_.Execute (detectTouch_);
			controlSwipe_.Execute (detectTouch_);
		}

		//@note この辺のイベントは必要があれば順次実装する
		//タッチイベント関数
		public System.Action<Vector2> OnTouchDown;
		public System.Action<Vector2> OnTouchUp;
		public System.Action<Vector2> OnTouch;

		/// <summary>
		/// タッチイベント関数を実行
		/// </summary>
		private void OnTouchEvent(){
			if (IsTouchDown && OnTouchDown != null) {
				OnTouchDown (TouchPos);
			} else if (IsTouchUp && OnTouchUp != null) {
				OnTouchUp (TouchPos);
			} else if (IsTouch && OnTouch != null) {
				OnTouch (TouchPos);
			}
		}

	}

}
