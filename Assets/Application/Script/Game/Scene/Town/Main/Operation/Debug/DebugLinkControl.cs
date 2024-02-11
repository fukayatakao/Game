namespace Project.Game {
#if false
	/// <summary>
	/// キャラ操作
	/// </summary>
	public class DebugLinkControl: IHaveControl {
		public static bool EnableControl = false;
		public static string ArrowColor = ResourcesPath.LINK_ARROW_RED;

		//カメラ
		CameraEntity camera_;
        //操作中か
        bool enable_;

        // 操作プライオリティ
        public int Priority { get { return (int)OperationPriority.CharacterDrag; } }

		Facility start_;

		/// <summary>
		/// レイを飛ばすカメラをセットアップ
		/// </summary>
		public DebugLinkControl(CameraEntity cam) {
			camera_ = cam;
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this, true);
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (!EnableControl)
				return false;
			if (!Gesture.IsTouchDown())
				return false;

			//建物を選択したら操作開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
			if (!result)
				return false;

			FactoryPortal factory = hit.collider.GetComponent<FactoryPortal>();
			if (factory != null) {
				start_ = factory.Owner;
				return true;
			}

			StoragePortal storage = hit.collider.GetComponent<StoragePortal>();
			if (storage != null) {
				start_ = storage.Owner;
				return true;
			}

			ResidencePortal residence = hit.collider.GetComponent<ResidencePortal>();
			if (residence != null) {
				start_ = residence.Owner;
				return true;
			}


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

			//操作終わり
			if (Gesture.IsTouchUp()) {
				//どっちにしろ操作終了
				enable_ = false;
				//建物を選択したら操作開始
				RaycastHit hit;
				bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
				if (!result)
					return;
				Entity end = null;
				FactoryPortal factory = hit.collider.GetComponent<FactoryPortal>();
				if (factory != null) {
					end = factory.Owner;
				}

				StoragePortal storage = hit.collider.GetComponent<StoragePortal>();
				if (storage != null) {
					end = storage.Owner;
				}

				ResidencePortal residence = hit.collider.GetComponent<ResidencePortal>();
				if (residence != null) {
					end = residence.Owner;
				}
				if (end == null)
					return;
				Link link = LinkAssembly.I.Create(ArrowColor);
				link.Setup(start_, end);



			}

		}
	}
#endif
}
