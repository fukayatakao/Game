using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 新規建物の設置
	/// </summary>
	public class StateTownFacilityBuild : IState<TownMain> {
		Facility target_;
		private System.Action execute_;
		public void SetTargetFacility(Facility facility) {
			target_ = facility;
		}
		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(TownMain owner) {
			TownAlternativeMenu.I.BuildFacilityMenu.Hide();
			TownAlternativeMenu.I.OpenMenu(target_, FacilityMenuState.Build);

			bool result = CameraUtil.RaycastHit(owner.MainCamera, 1 << (int)UnityLayer.Layer.Field, out var hit);
			if (result) {
				target_.SetPosition(hit.point);
			}
			execute_ = OperationFacilityBuild.Create(owner, owner.MainCamera, target_, FacilityMenuState.Build);
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

