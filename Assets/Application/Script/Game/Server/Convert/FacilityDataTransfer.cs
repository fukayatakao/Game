using Project.Network;
using System.Collections.Generic;

namespace Project.Server {
	public static partial class DataTransfer {
		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserTownhallTable ConvertData(TownhallData data) {
			UserTownhallTable table = new UserTownhallTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.tax = data.tax;
			table.gold = data.gold;
			table.logistics = data.logistics;
			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static TownhallData ConvertData(UserTownhallTable table) {
			TownhallData data = new TownhallData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.tax = table.tax;
			data.gold = table.gold;
			data.logistics = table.logistics;
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserFactoryTable ConvertData(FactoryData data) {
			UserFactoryTable table = new UserFactoryTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.radius = data.radius;
			table.product = data.product;
			table.grade = data.grade;
			table.output = data.output;
			table.worker = data.worker;

			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static FactoryData ConvertData(UserFactoryTable table) {
			FactoryData data = new FactoryData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.radius = table.radius;
			data.product = table.product;
			data.grade = table.grade;
			data.output = table.output;
			data.worker = table.worker;
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserMarketTable ConvertData(MarketData data) {
			UserMarketTable table = new UserMarketTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.radius = data.radius;
			table.goodsIds = new List<int>(data.goodsIds);
			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static MarketData ConvertData(UserMarketTable table) {
			MarketData data = new MarketData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.radius = table.radius;
			data.goodsIds = new List<int>(table.goodsIds);
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserResidenceTable ConvertData(ResidenceData data) {
			UserResidenceTable table = new UserResidenceTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.radius = data.radius;
			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static ResidenceData ConvertData(UserResidenceTable table) {
			ResidenceData data = new ResidenceData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.radius = table.radius;
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserStorageTable ConvertData(StorageData data) {
			UserStorageTable table = new UserStorageTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.radius = data.radius;
			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static StorageData ConvertData(UserStorageTable table) {
			StorageData data = new StorageData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.radius = table.radius;
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static UserServiceTable ConvertData(ServiceData data) {
			UserServiceTable table = new UserServiceTable();
			table.id = data.id;
			table.baseId = data.baseId;
			table.position = data.position;
			table.radius = data.radius;
			table.product = data.product;
			table.grade = data.grade;
			table.output = data.output;
			table.worker = data.worker;

			return table;
		}

		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static ServiceData ConvertData(UserServiceTable table) {
			ServiceData data = new ServiceData();
			data.id = table.id;
			data.baseId = table.baseId;
			data.position = table.position;
			data.radius = table.radius;
			data.product = table.product;
			data.grade = table.grade;
			data.output = table.output;
			data.worker = table.worker;
			return data;
		}

		/// <summary>
		/// 通信用データ形式から保存用テーブル形式に変換
		/// </summary>
		public static List<UserChainTable> ConvertData(List<ChainData> chains) {
			List<UserChainTable> list = new List<UserChainTable>();
			for (int i = 0, max = chains.Count; i < max; i++) {
				ChainData data = chains[i];
				UserChainTable table = new UserChainTable();
				table.senderId = data.senderId;
				table.recieverId = data.recieverId;
				table.valid = data.valid;
				table.distance = data.distance;
				list.Add(table);
			}
			return list;
		}
		/// <summary>
		/// 保存用テーブル形式から通信用データ形式に変換
		/// </summary>
		public static List<ChainData> ConvertData(List<UserChainTable> chains) {
			List<ChainData> list = new List<ChainData>();
			for (int i = 0, max = chains.Count; i < max; i++) {
				ChainData data = new ChainData();
				UserChainTable table = chains[i];
				data.senderId = table.senderId;
				data.recieverId = table.recieverId;
				data.valid = table.valid;
				data.distance = table.distance;
				list.Add(data);
			}
			return list;
		}
	}
}
