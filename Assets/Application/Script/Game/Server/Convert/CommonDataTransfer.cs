using Project.Network;
using System.Collections.Generic;

namespace Project.Server {
	public static partial class DataTransfer {
		/// <summary>
		/// キャラクターデータをクライアントに送る形に整形
		/// </summary>
		public static CharacterData Convert(UserMemberTable table) {
			CharacterData data = new CharacterData();

			//---- レスポンス変数定義 ----
			data.id = table.id;
			data.name = table.name;
			data.species = table.species;
			data.generate = table.generate;
			data.portrait = table.portrait;
			data.ability = new List<int>() { table.ability1, table.ability2, table.ability3 };
			data.phase = table.phase;

			data.stayId = table.stayId;
			data.staySlotId = table.staySlotId;
			data.workId = table.workId;
			data.workSlotId = table.workSlotId;
			data.grade = table.grade;

			return data;
		}

		/// <summary>
		/// キャラクターデータをクライアントに送る形に整形
		/// </summary>
		public static UserMemberTable Convert(CharacterData data) {
			UserMemberTable table = new UserMemberTable();

			//---- レスポンス変数定義 ----
			table.id = data.id;
			table.name = data.name;
			table.species = data.species;
			table.generate = data.generate;
			table.portrait = data.portrait;
			table.ability1 = data.ability.Count > 0 ? data.ability[0] : 0;
			table.ability2 = data.ability.Count > 1 ? data.ability[1] : 0;
			table.ability3 = data.ability.Count > 2 ? data.ability[2] : 0;
			table.phase = data.phase;

			table.stayId = data.stayId;
			table.staySlotId = data.staySlotId;
			table.workId = data.workId;
			table.workSlotId = data.workSlotId;
			table.grade = data.grade;
			return table;
		}

		/// <summary>
		/// キャラクターデータをクライアントに送る形に整形
		/// </summary>
		public static List<CharacterData> Convert(List<UserMemberTable> tables) {
			List<CharacterData> data = new List<CharacterData>();
			for (int i = 0, max = tables.Count; i < max; i++) {
				data.Add(Convert(tables[i]));
			}
			return data;
		}

		/// <summary>
		/// 送られたデータをサーバで使える形に整形
		/// </summary>
		public static List<UserMemberTable> Convert(List<CharacterData> tables) {
			List<UserMemberTable> data = new List<UserMemberTable>();
			for (int i = 0, max = tables.Count; i < max; i++) {
				data.Add(Convert(tables[i]));
			}
			return data;
		}

		/// <summary>
		/// リーダーデータをクライアントに送る形に整形
		/// </summary>
		public static LeaderData Convert(UserLeaderTable table) {
			LeaderData data = new LeaderData();
			data.id = table.id;
			data.leaderMasterId = table.leaderMasterId;
			return data;
		}

		/// <summary>
		/// リーダーデータをクライアントに送る形に整形
		/// </summary>
		public static List<LeaderData> Convert(List<UserLeaderTable> tables) {
			List<LeaderData> data = new List<LeaderData>();
			for (int i = 0, max = tables.Count; i < max; i++) {
				data.Add(Convert(tables[i]));
			}
			return data;
		}




		/// <summary>
		/// アイテムデータをクライアントに送る形に整形
		/// </summary>
		public static ItemData Convert(UserItemTable table) {
			ItemData data = new ItemData();

			//---- レスポンス変数定義 ----
			data.id = table.id;
			data.classify = 0;
			data.type = table.type;
			data.number = table.number;
			return data;
		}

		/// <summary>
		/// 送られたデータをサーバで使える形に整形
		/// </summary>
		public static UserItemTable Convert(ItemData data) {
			UserItemTable table = new UserItemTable();

			//---- レスポンス変数定義 ----
			table.id = data.id;
			//data.classify = 0;
			table.type = data.type;
			table.number = data.number;
			return table;
		}
		/// <summary>
		/// アイテムデータをクライアントに送る形に整形
		/// </summary>
		public static List<ItemData> Convert(List<UserItemTable> tables) {
			List<ItemData> data = new List<ItemData>();
			for (int i = 0, max = tables.Count; i < max; i++) {
				data.Add(Convert(tables[i]));
			}
			return data;
		}

		/// <summary>
		/// 送られたデータをサーバで使える形に整形
		/// </summary>
		public static List<UserItemTable> Convert(List<ItemData> tables) {
			List<UserItemTable> data = new List<UserItemTable>();
			for (int i = 0, max = tables.Count; i < max; i++) {
				data.Add(Convert(tables[i]));
			}
			return data;
		}



	}


}
