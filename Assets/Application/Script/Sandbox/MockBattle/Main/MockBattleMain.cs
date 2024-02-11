using System.Collections.Generic;
using Project.Network;
using System.Threading.Tasks;
using Project.Lib;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Game {
#if UNITY_EDITOR
	public class MockBattleMain : BattleMain {

		private MockBattleSetting haveSetting_;
		//デフォルトのデータ置き場
		string path_ = "Assets/Application/Addressable/Character/Action/Dummy";

		//@note Startの使い方は審議
		/// <summary>
		/// 開始直後に１回だけ
		/// </summary>
		public void Start() {
			haveSetting_ = gameObject.GetComponent<MockBattleSetting>();



			DummySetting setting = gameObject.GetComponent<DummySetting>();
			//データをロード
			string actAsset = path_ + "/Dummy_AttackA.asset";
			ActEventData data = AssetDatabase.LoadAssetAtPath(actAsset, typeof(ScriptableObject)) as ActEventData;
			if (data == null)
				return;
			for(int i = 0, max = data.eventDataList.Count; i < max; i++) {
				if(data.eventDataList[i].method == "Hit"){
					data.eventDataList[i].execTime = setting.HitTime;
				}
				if (data.eventDataList[i].method == "Termination") {
					data.eventDataList[i].execTime = setting.FinishTime;
				}
			}

			//セーブ
			EditorUtility.SetDirty(data);
			AssetDatabase.SaveAssets();

		}

		/// <summary>
		/// 検証用の小隊データを生成
		/// </summary>
		PlatoonData CreateDummy(List<MockBattleSetting.MockSquad> squads) {
			PlatoonData data = new PlatoonData();

			data.name = "テスト";
			data.squads = new List<SquadData>();

			for(int i = 0, max = squads.Count; i < max; i++) {
				//ユニット
				List<CharacterData> list = new List<CharacterData>();
				for(int c = 0; c < squads[i].number; c++) {
					list.Add(CharacterUtil.GetEmptyCharacterData());
				}

				//所属リーダー
				LeaderData leaderData = null;
				if (squads[i].leader != 0) {
					leaderData = new LeaderData() {
						leaderMasterId = squads[i].leader,
						id = 0,
					};
				}

				//分隊データを作る
				var squad = new SquadData() { unitId = squads[i].unitId, members = list, leader = leaderData };
				data.squads.Add(squad);
			}

			return data;
		}
		/// <summary>
		/// デバッグ機能セットアップ
		/// </summary>
		public override void SetupDebug() {
			//メニュー表示
			DummyAlternativeMenu.Open(this);
			//デバッグウィンドウで作った仮メニューを表示する
			BattleDebugSetting.Setting();
			HaveTimer.LimitTime = haveSetting_.BattleTimeSecond;
			if (HaveTimer.LimitTime == 0)
				HaveTimer.LimitTime = 9999;

			SetupDummy();
		}
		/// <summary>
		/// デバッグ機能クリーンアップ
		/// </summary>
		public override void CleanupDebug() {
			DummyAlternativeMenu.Close();
		}

		/// <summary>
		/// プレイヤーサイドのキャラクター生成
		/// </summary>
		public override async Task CreatePlayerCharacterEntity() {
			PlatoonData data = CreateDummy(haveSetting_.Attacker);
			int index = (int)Power.Player;
			Platoon[index] = await PlatoonAssembly.I.CreateAsync("Player", "Platoon/AI/basic", data);
			Platoon[index].InitUI(MainCamera, FloatUICanvas);
			Platoon[index].InitParam(Power.Player, StageField.Width, StageField.CenterDepth, StageField.TerritoryDepth);
		}

		/// <summary>
		/// キャラクター生成
		/// </summary>
		public override async Task CreateEnemyCharacterEntity() {
			PlatoonData data = CreateDummy(haveSetting_.Defender);
			int index = (int)Power.Enemy;
			Platoon[index] = await PlatoonAssembly.I.CreateAsync("Enemy", "Platoon/AI/basic", data);
			Platoon[index].InitUI(MainCamera, FloatUICanvas);
			Platoon[index].InitParam(Power.Enemy, StageField.Width, StageField.CenterDepth, StageField.TerritoryDepth);
		}


		private void SetupDummy() {
			//@todo leader
			for(int i = 0, max = Platoon.Length; i < max; i++) {
				//リーダーのAIを止めて棒立ちにさせる
				//Platoon[i].Leader.Character.HaveThink.Enable = false;
				//邪魔にならない位置に避ける
				//Platoon[i].Leader.Character.SetPosition(new Vector3(0f, 0f, 99999f));
				//Platoon[i].Leader.Character.gameObject.SetActive(false);
			}
			//叩かれる側の設定
			//Platoon[(int)Power.Enemy].Squads[0].Characters[0].SetPosition(new Vector3(0f, 0f, 0f));
			//Platoon[(int)Power.Enemy].Squads[0].Characters[0].HaveParam.Fight.isInfinityHp = true;
			//Platoon[(int)Power.Enemy].Squads[0].Characters[0].HaveParam.Fight.isInfinityLp = true;
			//Platoon[(int)Power.Enemy].Squads[0].Characters[0].HaveThink.Enable = false;

			//叩く側の設定
			//Platoon[(int)Power.Player].Squads[0].Characters[0].SetPosition(new Vector3(0f, 0f, -3f));
			//Platoon[(int)Power.Player].Squads[0].Characters[0].SetRotation(Quaternion.Euler(0f, 90f, 0f));

		}

		/// <summary>
		/// バトル終了時処理
		/// </summary>
		public override void GameFinish()
		{
			base.GameFinish();
			SceneTransition.ChangeMockBattle();
		}

	}
#endif
}
