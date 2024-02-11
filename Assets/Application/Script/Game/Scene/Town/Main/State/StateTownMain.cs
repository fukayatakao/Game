using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// メイン状態
	/// </summary>
	public class StateTownMain : IState<TownMain> {
		const float Interval = 2f;
		float nextTime_;

		public override void Enter(TownMain owner) {
			nextTime_ = Time.time + Interval;
			OperationSelectFacility.Create(owner.ChainMap, owner.MainCamera);

			Gesture.Enable();
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(TownMain owner) {
			Gesture.Disable();
			OperationSelectFacility.Destroy();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(TownMain owner) {
			//2秒に1回計算にする
			if (nextTime_ < Time.time) {
				Server.TownTool.Calculate();
				TownMessage.SetGold.Broadcast(Server.UserDB.Instance.townhallTable.gold);
				nextTime_ = Time.time + Interval;
			}

			//操作の振り分けと操作処理
			owner.HaveUserOperation.Execute();
		}


	}
}
