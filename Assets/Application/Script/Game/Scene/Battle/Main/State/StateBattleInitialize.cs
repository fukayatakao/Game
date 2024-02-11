using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateBattleInitialize : IState<BattleMain> {
		/// <summary>
		/// 非同期で初期化処理
		/// </summary>
		private async void InitializeAsync(BattleMain owner) {
			// バトルシーン初期化
			owner.Initialize();
			await owner.CreateLightEntity();
			await owner.CreateFieldEntity();
			await owner.CreateCameraEntity();
			await owner.CreatePlayerCharacterEntity();
			await owner.CreateEnemyCharacterEntity();
			//Entityがそろったあとに行う
			owner.InitializeLater();
#if DEVELOP_BUILD
			// デバッグ機能初期化
			owner.SetupDebug();
#endif
#if DEVELOP_BUILD
			System.Action action = null;
			if (BattleDebugSetting.IsOpeningSkip) {
				action = ()=> { owner.ChangeState(BattleMain.State.Ready); };
			} else {
				action = () => { owner.ChangeState(BattleMain.State.Opening); };
			}
#else
			action = () => { owner.ChangeState(BattleMain.State.Opening); };
#endif
			SceneTransition.Termination(action);

		}

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(BattleMain owner) {
			InitializeAsync(owner);
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(BattleMain owner) {
        }
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(BattleMain owner) {
		}
	}

}
