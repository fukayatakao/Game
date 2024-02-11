using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// ノードに部隊配置
	/// </summary>
	public class SelectNode : IHaveControl {
		//カメラ
		CameraEntity camera_;

		// 操作プライオリティ
		public int Priority { get { return (int)OperationPriority.NodeSelect; } }
		/// <summary>
		/// レイを飛ばすカメラとUIをセットアップ
		/// </summary>
		public SelectNode(CameraEntity cam) {
			camera_ = cam;
		}

		public void Register() {
			//自分を制御振り分け機能に登録要求
			SystemMessage.RegisterControl.Broadcast(this);
		}
		public void UnRegister() {
			//自分を制御振り分け機能に除名要求
			SystemMessage.UnRegisterControl.Broadcast(this);
		}

		/// <summary>
		/// 制御開始
		/// </summary>
		public bool Interrupt() {
			if (!Gesture.IsTouchDown(0))
				return false;

			//ノードを選択したら開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Collision, out hit);
			if (!result) {
				return false;
			}

			int nodeId = hit.collider.gameObject.GetComponent<NodePortal>().Id;
			StrategyMessage.PutInvader.Broadcast(nodeId);

			//操作を取得する必要はないのでスルー
			return false;
		}
		/// <summary>
		/// 制御開始
		/// </summary>
		public void Begin() {
		}
		/// <summary>
		/// 制御終了
		/// </summary>
		public bool IsEnd() {
			return true;
		}
		/// <summary>
		/// 制御却下
		/// </summary>
		public void Reject() {
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
		}
	}
}
