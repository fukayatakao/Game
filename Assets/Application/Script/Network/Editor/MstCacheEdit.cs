using UnityEditor;
using UnityEngine;
using Project.Lib;

namespace Project.Mst {
	public static class MstCacheEdit {
		[MenuItem("Editor/Delete/BaseDataCache")]
		public static void ClearCache() {
			BaseDataManager.ClearCache();
		}
		[MenuItem("Editor/Delete/PlayerPrefs")]
		public static void SaveClear() {
			PlayerPrefs.DeleteAll();
			PlayerPrefsUtil.DeleteAll();
		}
	}
}
