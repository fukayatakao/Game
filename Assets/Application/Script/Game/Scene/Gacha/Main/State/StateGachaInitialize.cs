using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateGachaInitialize : IState<GachaMain> {
		/// <summary>
		/// 非同期で初期化処理
		/// </summary>
		private async void InitializeAsync(GachaMain owner) {
			// バトルシーン初期化
			owner.Initialize();
			//Entityがそろったあとに行う
			owner.InitializeLater();
#if DEVELOP_BUILD
			// デバッグ機能初期化
			owner.InitializeDebug();
#endif
			
			SceneTransition.Termination(() => { owner.ChangeState(GachaMain.State.Main); });
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);
		}

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(GachaMain owner) {
			InitializeAsync(owner);
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(GachaMain owner) {
        }
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(GachaMain owner) {
		}
	}

}
