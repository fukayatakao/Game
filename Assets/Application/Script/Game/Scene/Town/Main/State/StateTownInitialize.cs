using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateTownInitialize : IState<TownMain> {
		/// <summary>
		/// 非同期で初期化処理
		/// </summary>
		private async void InitializeAsync(TownMain owner) {

			// シーン初期化
			owner.Initialize();
			await owner.CreateLightEntity();
			await owner.CreateFieldEntity();
			await owner.CreateCameraEntity();
			await owner.LoadTownAsync(TownMain.TransitionData.townData);

			/*initTask_.Add(owner.CreateEnemyCharacterEntity());
				initTask_.Add(owner.CreatePlayerCharacterEntity());*/

			//Entityがそろったあとに行う
			owner.InitializeLater();

#if DEVELOP_BUILD
			// デバッグ機能初期化
			owner.InitializeDebug();
#endif
			SceneTransition.Termination(() => { owner.ChangeState(TownMain.State.Main); });
		}


		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(TownMain owner) {
			InitializeAsync(owner);
		}

		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(TownMain owner) {
        }
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(TownMain owner) {
		}
	}

}
