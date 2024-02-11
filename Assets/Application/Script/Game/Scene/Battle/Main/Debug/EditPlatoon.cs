using Project.Lib;
using Project.Mst;
using System.Collections.Generic;
using UnityEngine;

#if DEVELOP_BUILD

namespace Project.Game {
	[System.Serializable]
	public class EditPlatoon {
		public List<int> unit;
		public int leader;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditPlatoon(List<int> unit, int leader) {
			this.unit = unit;
			this.leader = leader;
		}

		/// <summary>
		/// マスターデータの形式にする
		/// </summary>
		public MstEnemyCorps CreateCorps() {
			MstEnemyCorps corps = new MstEnemyCorps();

			corps.FirstUnit = unit[0];
			corps.SecondUnit = unit[1];
			corps.ThirdUnit = unit[2];
			corps.ThirdLeader = leader;
			return corps;
		}

		/// <summary>
		/// データが保存されているか
		/// </summary>
		public static bool IsExist() {
			return PlayerPrefsUtil.HasKey(PrefsUtilKey.EditBattlePlatoon);
		}

		/// <summary>
		/// 編集した部隊を保存する
		/// </summary>
		public static void Save(EditPlatoon platoon) {
			PlayerPrefsUtil.SetString(PrefsUtilKey.EditBattlePlatoon, JsonUtility.ToJson(platoon));
		}
		/// <summary>
		/// 編集した部隊を保存する
		/// </summary>
		public static EditPlatoon Load() {
			string json = PlayerPrefsUtil.GetString(PrefsUtilKey.EditBattlePlatoon);
			return JsonUtility.FromJson<EditPlatoon>(json);
		}
	}
}
#endif
