using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Game {
	public static class CommonDownload {
		/// <summary>
		/// 一番最初に行うプリロード
		/// </summary>
		public static IEnumerator Download(List<string> list, List<string> assetsList, List<string> categoriesList, List<string> scenesList) {
			//ダウンロード開始
			AddressableAssist.StartDownload(list);
			//ダウンロードの終了待ち
			while (AddressableAssist.IsDownloading()) {
				//進捗確認
				Debug.Log("download progress = " + AddressableAssist.DownloadProgress());
				yield return null;
			}
			//@todo プリロード機能を入れるか検討する
			/*bool loop = true;
			//とりあえず一緒にロードまで行う
			AddressableAssist.LoadAssetsAsync(assetsList);
			//ロードの終了待ち
			while (loop) {
				//進捗確認
				Debug.Log("loading");
				yield return null;
			}*/


			yield break;

		}
	}
}
