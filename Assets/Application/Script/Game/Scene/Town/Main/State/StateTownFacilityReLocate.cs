using UnityEngine;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 建物の再配置
	/// </summary>
	public class StateTownFacilityReLocate : IState<TownMain> {
		Facility target_;
		private System.Action execute_;

		//再配置前のChainを退避する
		public Vector3 position_;

		/// <summary>
		/// 移動する施設を指定
		/// </summary>
		public void SetTargetFacility(Facility facility) {
			target_ = facility;
			position_ = facility.GetPosition();
		}

		public Vector3 GetLastFacility() {
			return position_;
		}
		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(TownMain owner) {
			TownAlternativeMenu.I.BuildFacilityMenu.Hide();
			TownAlternativeMenu.I.OpenMenu(target_, FacilityMenuState.ReLocate);

			//StateTownMainを抜けるときにOperationSelectFacilityが解放されfalseになるのでもう一度trueを入れる
			target_.HaveArea.SetVisible(true);
			execute_ = OperationFacilityBuild.Create(owner, owner.MainCamera, target_, FacilityMenuState.ReLocate);
			//開始時に建設可能か判定を1回入れる
			target_.CheckBuildable(owner, owner.GridMap);
			Gesture.Enable();
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(TownMain owner) {
			target_.ResolveCollision();

			//操作の振り分けと操作処理
			execute_();
			owner.HaveUserOperation.Execute();
		}

		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(TownMain owner) {
			execute_ = null;
			Gesture.Disable();
			OperationFacilityBuild.Destroy();
			TownAlternativeMenu.I.HideSub();
			TownAlternativeMenu.I.BuildFacilityMenu.Show();
		}



	}

}

