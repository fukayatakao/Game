using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// シーンのアセットダウンロード処理
	/// </summary>
	public class StateStrategyDownload : IState<StrategyMain> {
		//プリロードタスク
		CoroutineTaskList downloadTask_;

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(StrategyMain owner) {
			downloadTask_ = new CoroutineTaskList();
			downloadTask_.Add(RecieveTransition());
			//downloadTask_.Add(BuildDownloadTask(owner));
			//次のステートに移行
			downloadTask_.Add(ChangeNextState(owner));
		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(StrategyMain owner) {
			downloadTask_.Execute ();
		}


		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(StrategyMain owner) {
			downloadTask_ = null;
		}

		/// <summary>
		/// 遷移の受付
		/// </summary>
		private IEnumerator RecieveTransition() {
			//遷移データが入れられるまで待つ
			while (SceneTransition.TransitionData == null) {
				yield return null;
			}

			StrategySituation.Create(SceneTransition.TransitionData);
			yield break;
		}
		/// <summary>
		/// 一番最初に行うプリロード
		/// </summary>
		private IEnumerator BuildDownloadTask(StrategyMain owner) {
			//個別ダウンロードリストを作って変換
			List<string> assets;
			List<string> labels;
			List<string> scenes;
			BattleAssetDownload.CreateDownloadList(BattleSituation.I, out assets, out labels, out scenes);

			List<string> list = new List<string>();
			list.AddRange(assets);
			list.AddRange(labels);
			list.AddRange(scenes);

			DownloadWait wait = new DownloadWait();
			wait.Lock();
			//ダウンロード待機
			downloadTask_.Add(WaitDownload(wait));
			//ダウンロード実行
			downloadTask_.Add(CommonDownload.Download(list, assets, labels, scenes));
			//次のステートに移行
			downloadTask_.Add(ChangeNextState(owner));

			//準備が完了したらダウンロードサイズの表示リクエスト
			AddressableAssist.CalcDownloadSizeAsync(list, (size) => {
				//ダウンロードサイズ表示
				CommonMessage.ShowDownloadSize.Broadcast(size);
			});
			yield break;
		}


		/// <summary>
		/// 次のステートに変化する
		/// </summary>
		private IEnumerator ChangeNextState(StrategyMain owner) {
			owner.ChangeState(StrategyMain.State.Initialize);
			yield break;
		}

		/// <summary>
		/// ダウンロード待機
		/// </summary>
		private IEnumerator WaitDownload(DownloadWait wait) {
			//待機用オブジェクトのロックが解除されるまで待つ
			while (wait.IsWait()) {
				yield return null;
			}
			//後始末
			wait.Release();
		}
	}



}
