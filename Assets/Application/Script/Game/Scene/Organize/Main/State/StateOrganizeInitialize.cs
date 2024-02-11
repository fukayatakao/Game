using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateOrganizeInitialize : IState<OrganizeMain> {
		/// <summary>
		/// 非同期で初期化処理
		/// </summary>
		private async void InitializeAsync(OrganizeMain owner) {
			// バトルシーン初期化
			owner.Initialize();
			await owner.CreateLightEntity();
			await owner.CreateFieldEntity();
			await owner.CreateCameraEntity();
			await owner.CreatePlayerCharacterEntity(0);
			//Entityがそろったあとに行う
			owner.InitializeLater();
#if DEVELOP_BUILD
			// デバッグ機能初期化
			owner.InitializeDebug();
#endif
			
			SceneTransition.Termination(() => { owner.ChangeState(OrganizeMain.State.Main); });
		}

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(OrganizeMain owner) {
			InitializeAsync(owner);
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(OrganizeMain owner) {
        }
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(OrganizeMain owner) {
		}
	}

}
