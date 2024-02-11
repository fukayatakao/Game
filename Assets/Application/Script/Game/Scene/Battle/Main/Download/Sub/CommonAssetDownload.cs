using Project.Lib;
using System.Collections.Generic;

namespace Project.Game {
	public static class CommonAssetDownload {

		/// <summary>
		/// ダウンロードリスト作成
		/// </summary>
		public static List<string> CreateAssetsList() {
			//ダウンロードリスト作成
			List<string> downloadList = new List<string>();
			downloadList.Add("Effect/Shock");
			downloadList.Add("Effect/Empty");

			downloadList.Add("Character/AI/basic");
			downloadList.Add("Platoon/AI/basic");
			downloadList.Add(AddressableDefine.Address.CIRCLE_AREA);


			return downloadList;
		}

		/// <summary>
		/// ダウンロードリスト作成
		/// </summary>
		public static List<string> CreateCategoryList() {
			//ダウンロードリスト作成
			List<string> downloadList = new List<string>();

			downloadList.Add("Character/AI/basic");
			downloadList.Add("Platoon/AI/basic");
			downloadList.Add(AddressableDefine.Address.CIRCLE_AREA);


			return downloadList;
		}

	}
}
