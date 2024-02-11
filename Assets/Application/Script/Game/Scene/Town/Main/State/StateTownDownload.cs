using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// シーンのアセットダウンロード処理
	/// </summary>
	public class StateTownDownload : IState<TownMain> {
		//プリロードタスク
		CoroutineTaskList downloadTask_;

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(TownMain owner) {
			//初期化タスクを拾って登録する
			downloadTask_ = new CoroutineTaskList();
			downloadTask_.Add(RecieveTransition(owner));

			downloadTask_.Add(BuildDownloadTask(owner));

		}
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(TownMain owner) {
			downloadTask_.Execute();
		}

		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(TownMain owner) {
			downloadTask_ = null;
		}

		/// <summary>
		/// 遷移の受付
		/// </summary>
		private IEnumerator RecieveTransition(TownMain owner) {
			//遷移データが入れられるまで待つ
			while(TownMain.TransitionData == null) {
				yield return null;
			}

			PeopleData.Create(TownMain.TransitionData);
			TownItemData.Create(TownMain.TransitionData);
			yield break;
		}
		/// <summary>
		/// 一番最初に行うプリロード
		/// </summary>
		private IEnumerator BuildDownloadTask(TownMain owner) {
			//個別ダウンロードリストを作って変換
			List<string> assets;
			List<string> labels;
			List<string> scenes;
			TownAssetDownload.CreateDownloadList(TownMain.TransitionData, out assets, out labels, out scenes);

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
		private IEnumerator ChangeNextState(TownMain owner) {
			owner.ChangeState(TownMain.State.Initialize);
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

