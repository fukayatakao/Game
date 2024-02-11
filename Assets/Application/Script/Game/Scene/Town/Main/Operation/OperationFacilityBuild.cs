using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// 建物の移動操作
	/// </summary>
	public class OperationFacilityBuild : IHaveControl {
		//隠れシングルトン（シングルトンにする必要性はないのだが多重でインスタンス作られるとバグるのでチェックのためにも用意
		static OperationFacilityBuild instance_;
		//カメラ
		CameraEntity camera_;
		//操作対象の建物
		Facility target_;
		//最後に選択した建物
		Facility lastSelect_;

		//操作中か
		bool enable_;

		Vector3 offset_;

		FacilityMenuState menuState_;
		// 操作プライオリティ
		public int Priority { get { return (int)TownOperationPriority.FacilityBuild; } }

		TownMain owner_;
		MessageSystem.Receptor receptor_;
		/// <summary>
		/// 操作処理のインスタンスを作る
		/// </summary>
		public static System.Action Create(TownMain main, CameraEntity cam, Facility target, FacilityMenuState menuState) {
			//隠れシングルトンなので２重でインスタンス作ろうとしたらエラー出しておく
			Debug.Assert(instance_ == null, "instance is already alive");
			//インスタンス作ってパラメータをセット
			instance_ = new OperationFacilityBuild();
			instance_.owner_ = main;
			instance_.camera_ = cam;
			instance_.target_ = target;
			instance_.lastSelect_ = null;

			instance_.menuState_ = menuState;
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(instance_, true);

			//メッセージ受容体作成
			instance_.receptor_ = MessageSystem.CreateReceptor(instance_, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);
			target.HaveModel.Select();
			return instance_.Execute;
		}

		/// <summary>
		/// 操作処理の破棄
		/// </summary>
		public static void Destroy() {
			instance_.target_.HaveModel.UnSelect();

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
			return SelectTarget();
		}

		/// <summary>
		/// 操作対象を選択
		/// </summary>
		/// <returns></returns>
		private bool SelectTarget() {
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
			if (target_ != portal.Owner) {
				Enter(portal.Owner);
				return false;
			}
			//タッチしたときの位置と移動させるオブジェクトのブレをオフセットとして保存して選択した瞬間にちょっとワープするのを防ぐ
			result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Field, out hit);
			if (result) {
				offset_ = hit.point - target_.GetPosition();
			} else {
				offset_ = Vector3.zero;
			}
			Enter(target_);

			return true;

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
			Exit();
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
            if (!enable_)
                return;

			//クリック以外の操作を検知したら終了
			if(Gesture.IsTouchDown(1) || Gesture.IsPinchIn || Gesture.IsPinchOut) {
				enable_ = false;
				Exit();
				return;
			}
			//操作終わり
			if (Gesture.IsTouchDown()) {
				Exit();
				enable_ = SelectTarget();
			//タッチ操作中
			} else if (Gesture.IsTouch()) {
				//タッチ位置から配置座標を計算
				RaycastHit hit;
				bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Field, out hit);
				if (result) {
					Vector3 roundPos = hit.point - offset_;
					roundPos.x = (int)(roundPos.x / 4) * 4;
					roundPos.y = (int)(roundPos.y / 4) * 4;
					roundPos.z = (int)(roundPos.z / 4) * 4;
					target_.SetPosition(roundPos);
					Execute(target_);
				}
			}

		}



		/// <summary>
		/// 開始
		/// </summary>
		private void Enter(Facility target) {
			owner_.ChainMap.InitLink(target.SendList.Values, target.RecieveList.Values, ChainLinkMap.Arrow.BLUE);
			target.HaveArea.SetVisible(true);
			lastSelect_ = target;
#if DEVELOP_BUILD
			TownAlternativeMenu.I.OpenMenu(target, target == target_ ? menuState_ : FacilityMenuState.None);
#endif
		}
		/// <summary>
		/// 更新
		/// </summary>
		private void Execute(Facility target) {
			target.UpdateChainByMovePosition(owner_, out var includeChain, out var excludeChain);
			target.CheckBuildable(owner_, owner_.GridMap);
			owner_.ChainMap.Linking(includeChain, excludeChain, ChainLinkMap.Arrow.BLUE);
		}

		/// <summary>
		/// 終了
		/// </summary>
		private void Exit() {
			if (lastSelect_ == null)
				return;
#if DEVELOP_BUILD
			TownAlternativeMenu.I.OpenMenu(target_, menuState_);
#endif
			lastSelect_.HaveArea.SetVisible(false);
			owner_.ChainMap.Clear();
			lastSelect_ = null;
		}


	}
}
