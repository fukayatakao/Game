using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// 建物を選択してメニューを表示する
	/// </summary>
	public class OperationSelectFacility : IHaveControl {
		//隠れシングルトン（シングルトンにする必要性はないのだが多重でインスタンス作られるとバグるのでチェックのためにも用意
		static OperationSelectFacility instance_;
		//カメラ
		CameraEntity camera_;
		ChainLinkMap chainMap_;
		//操作中か
		bool enable_;

        // 操作プライオリティ
        public int Priority { get { return (int)TownOperationPriority.SelectFacility; } }
		//選択された建物
		Facility target_;

		MessageSystem.Receptor receptor_;
		/// <summary>
		/// 操作処理のインスタンスを生成して操作振り分けに登録する
		/// </summary>
		public static void Create(ChainLinkMap chainMap, CameraEntity cam) {
			//隠れシングルトンなので２重でインスタンス作ろうとしたらエラー出しておく
			Debug.Assert(instance_ == null, "instance is already alive");
			//インスタンス作ってパラメータをセット
			instance_ = new OperationSelectFacility();
			instance_.chainMap_ = chainMap;
			instance_.camera_ = cam;
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(instance_, true);

			//メッセージ受容体作成
			instance_.receptor_ = MessageSystem.CreateReceptor(instance_, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// 操作処理の破棄
		/// </summary>
		public static void Destroy() {
			instance_.Exit();
			SystemMessage.UnRegisterControl.Broadcast(instance_);
			MessageSystem.DestroyReceptor(instance_.receptor_);
			instance_ = null;
		}


		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (!Gesture.IsTouchDown())
				return false;

			Exit();

			//建物を選択したら操作開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Facility, out hit);
			if (!result)
				return false;

			//対象のインスタンスを取得
			var portal = hit.collider.GetComponent<FacilityPortal>();
			if (portal == null)
				return false;

			Enter(portal.Owner);

			return false;

		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
            enable_ = true;
        }
        /// <summary>
        /// 制御終了
        /// </summary>
        public bool IsEnd() {
            return !enable_;
        }
        /// <summary>
        /// 制御却下
        /// </summary>
        public void Reject() {
            enable_ = false;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
            if (!enable_)
                return;
			enable_ = false;
		}



		/// <summary>
		/// 開始
		/// </summary>
		public void Enter(Facility target) {
			target_ = target;
			//メニュー開く
#if DEVELOP_BUILD
			TownAlternativeMenu.I.OpenMenu(target, FacilityMenuState.Default);
#endif
			chainMap_.Clear();
			chainMap_.InitLink(target.SendList.Values, target.RecieveList.Values, ChainLinkMap.Arrow.BLUE);

			target.HaveArea.SetVisible(true);
		}
		/// <summary>
		/// 終了
		/// </summary>
		public void Exit() {
			if (target_ == null)
				return;
#if DEVELOP_BUILD
			TownAlternativeMenu.I.HideSub();
#endif
			target_.HaveArea.SetVisible(false);
			chainMap_.Clear();
			target_ = null;
		}

	}
}
