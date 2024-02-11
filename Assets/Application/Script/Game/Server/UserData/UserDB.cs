using Project.Lib;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Server {
	/// <summary>
	/// なんちゃってDB
	/// </summary>
	[System.Serializable]
	public class UserDB {
		static UserDB instance_;
		public static UserDB Instance { get { return instance_; } }

		//小隊データ
		public int platoonTableCounter;
		public List<UserPlatoonTable> platoonTable;
		public List<UserPlatoonMemberTable> platoonMemberTable;
		public List<UserPlatoonLeaderTable> platoonLeaderTable;
		//一般キャラクターデータ
		public int memberTableCounter;
		public List<UserMemberTable> memberTable;
		//リーダーデータ
		public int leaderTableCounter;
		public List<UserLeaderTable> leaderTable;

		//戦略マップデータ
		public List<UserStrategyMapTable> strategyMapTable;
		public List<UserStrategyLocationTable> strategyLocationTable;

		//タウンデータ
		public UserGlobalDataTable globalDataTable;
		public UserTownhallTable townhallTable;
		public List<UserFactoryTable> factoryTable;
		public List<UserResidenceTable> residenceTable;
		public List<UserStorageTable> storageTable;
		public List<UserMarketTable> marketTable;
		public List<UserServiceTable> serviceTable;
		public List<UserChainTable> chainTable;

		//共通
		public List<UserItemTable> itemTable;


		/// <summary>
		/// データを保存
		/// </summary>
		public static void Save() {
			PlayerPrefsUtil.SetString(PrefsUtilKey.UserDataKey, JsonUtility.ToJson(instance_, true));
		}

		/// <summary>
		/// データをロード
		/// </summary>
		public static void Load() {
			//データが未作成なら一回作る
			if (!PlayerPrefsUtil.HasKey(PrefsUtilKey.UserDataKey)) {
				CreateUserData();
			//データがあるならロード
			} else {
				string json = PlayerPrefsUtil.GetString(PrefsUtilKey.UserDataKey);
				instance_ = JsonUtility.FromJson<UserDB>(json);
			}
		}
		/// <summary>
		/// ユーザーデータ新規作成
		/// </summary>
		private static void CreateUserData() {
			instance_ = new UserDB();
			//カウンタ初期化
			instance_.platoonTableCounter = 0;
			instance_.leaderTableCounter = 0;
			instance_.memberTableCounter = 0;
			//初期キャラクター(部隊配属はなし)
			InitialUserData.CreateInitMember(out var charas);
			instance_.memberTable = charas;


			//初期状態ユーザーデータ作成
			InitialUserData.CreateInitCorps(out var platoons, out var platoonMembers, out var platoonLeaders, out var leaders);
			instance_.platoonTable = platoons;
			instance_.platoonMemberTable = platoonMembers;
			instance_.platoonLeaderTable = platoonLeaders;
			instance_.leaderTable = leaders;

			//マップ関連はデフォルト空
			instance_.strategyMapTable = new List<UserStrategyMapTable>();
			instance_.strategyLocationTable = new List<UserStrategyLocationTable>();

			//タウンの初期状態ユーザーデータ作成
			InitialUserData.CreateInitTown(out var globalDataTable, out var townhallTable);
			instance_.globalDataTable = globalDataTable;
			instance_.townhallTable = townhallTable;

			instance_.factoryTable = new List<UserFactoryTable>();
			instance_.residenceTable = new List<UserResidenceTable>();
			instance_.storageTable = new List<UserStorageTable>();
			instance_.marketTable = new List<UserMarketTable>();
			instance_.serviceTable = new List<UserServiceTable>();
			instance_.chainTable = new List<UserChainTable>();


			//所持アイテム
			InitialUserData.CreateInitItem(out var itemTable);
			instance_.itemTable = itemTable;
			Save();
		}


	}



}
