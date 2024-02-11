#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Project.Game;
using Project.Mst;
using Project.Network;
using System.Linq;
using Project.Lib;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[UnityEditor.CustomEditor(typeof(MockBattleSetting))]
public class MockBattleSettingEditor : Editor {
	MockBattleSetting setting_;

	int selectCorps_;

	List<MstLeaderData> masterLeaderData_;
	string[] leaderSelector;

	List<MstUnitData> masterUnitData_;
	string[] unitSelector;
	string[] numberSelector = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
	string[] timeLimit = new string[] { "無限", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100", "110", "120" };


	private void Awake() {
		setting_ = (MockBattleSetting)base.target;
		selectCorps_ = 0;
		{
			TextAsset asset = (Resources.Load(ResourcesPath.MASTER_LEADER_DATA) as TextAsset);
			string json = asset.text;
			masterLeaderData_ = JsonList.FromJsonList<MstLeaderData>(json);
			leaderSelector = masterLeaderData_.Select(x => x.DbgName).ToArray();
		}
		{
			TextAsset asset = (Resources.Load(ResourcesPath.MASTER_UNIT_DATA) as TextAsset);
			string json = asset.text;
			masterUnitData_ = JsonList.FromJsonList<MstUnitData>(json);
			unitSelector = masterUnitData_.Select(x => x.DbgName).ToArray();
		}
	}

	public override void OnInspectorGUI() {
		if (masterUnitData_ == null)
			return;

		using (new GUILayout.HorizontalScope()) {
			if (GUILayout.Button("セーブ")) {
				string folder = Application.dataPath.Replace("/Assets", "/MockBattle");
				folder = EditorUtility.SaveFilePanel("select", folder, "mock", "txt");
				if (!string.IsNullOrEmpty(folder)) {
					setting_.Save(folder);
				}
			}
			if (GUILayout.Button("ロード")) {
				//選択したフォルダを取得
				string folder = Application.dataPath.Replace("/Assets", "/MockBattle");
				folder = EditorUtility.OpenFilePanel("select", folder, "txt");
				if (!string.IsNullOrEmpty(folder)) {
					setting_.Load(folder);
				}
			}
		}

		GUIUtil.BeginChangeCheck();
		setting_.BattleTimeSecond = (EditorGUILayout.Popup("制限時間", setting_.BattleTimeSecond / 10, timeLimit)) * 10;

		GUILayout.Space(12);
		selectCorps_ = GUILayout.SelectionGrid(selectCorps_, new string[] { "プレイヤー", "エネミー" }, 2);

		List<MockBattleSetting.MockSquad> units = selectCorps_ == 0 ? setting_.Attacker : setting_.Defender;
		for (int i = 0, max = units.Count; i < max; i++) {
			GUILayout.BeginHorizontal();
			//列削除ボタン
			if (GUILayout.Button("x", GUILayout.Width(20f))) {
				//最低１列は残す
				if (units.Count > 1) {
					units.RemoveAt(i);
				}
				break;
			}
			//ユニット種別を選択
			int select = masterUnitData_.FindIndex(x => x.Id == units[i].unitId);
			select = EditorGUILayout.Popup(select, unitSelector);
			units[i].unitId = masterUnitData_[select].Id;
			//ユニットの数を設定
			if (units[i].unitId != 0) {
				//最低1体は必要
				units[i].number = EditorGUILayout.Popup(units[i].number - 1, numberSelector) + 1;
			} else {
				//ユニット指定なしの場合は0体固定
				units[i].number = EditorGUILayout.Popup(0, new string[]{"0"});
			}
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(20f));
			int leader = masterLeaderData_.FindIndex(x => x.Id == units[i].leader);
			leader = EditorGUILayout.Popup(leader, leaderSelector);
			units[i].leader = masterLeaderData_[leader].Id;
			GUILayout.EndHorizontal();
		}
		if (units.Count < (int)Abreast.MAX) {
			if (GUILayout.Button("追加")) {
				units.Add(new MockBattleSetting.MockSquad() { unitId = 1, number = 1, leader = 0 });
			}
		}

		if (GUIUtil.EndChangeCheck()) {
			EditorSceneManager.MarkSceneDirty( SceneManager.GetActiveScene() );
		}
	}


}
#endif
