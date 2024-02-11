using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateStrategyInitialize : IState<StrategyMain> {
		/// <summary>
		/// 非同期で初期化処理
		/// </summary>
		private async void InitializeAsync(StrategyMain owner) {
			// バトルシーン初期化
			owner.Initialize();
			await owner.CreateLightEntity();
			await owner.CreateFieldEntity();
			await owner.CreateCameraEntity();
			await owner.CreatePieceEntity();
			//await owner.CreateEnemyCharacterEntity();
			//Entityがそろったあとに行う
			owner.InitializeLater();
#if DEVELOP_BUILD
			// デバッグ機能初期化
			owner.SetupDebug();
#endif
			SceneTransition.Termination(() => {
				if (StrategySituation.I.turn == 0) {
					StrategyMessage.StartInvaderLocation.Broadcast();
				} else {
					StrategyMessage.StartStrategyMain.Broadcast();
				}
			});

		}
		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(StrategyMain owner) {
			InitializeAsync(owner);
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(StrategyMain owner) {
        }
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
		}
	}

}
