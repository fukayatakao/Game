using Project.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if USE_OFFLINE

namespace Project.Server {
	public partial class DummyServer {
		//@todo 置き場所審議
		/// <summary>
		/// データ送信
		/// </summary>
		public static TownData SerializeTownDataResponse() {
			TownData data = new TownData();

			data.identifer = UserDB.Instance.globalDataTable.id;

			data.townhall = DataTransfer.ConvertData(UserDB.Instance.townhallTable);

			//生産施設
			data.factories = new List<FactoryData>();
			for (int i = 0, max = UserDB.Instance.factoryTable.Count; i < max; i++) {
				data.factories.Add(DataTransfer.ConvertData(UserDB.Instance.factoryTable[i]));
			}
			//マーケット
			data.markets = new List<MarketData>();
			for (int i = 0, max = UserDB.Instance.marketTable.Count; i < max; i++) {
				data.markets.Add(DataTransfer.ConvertData(UserDB.Instance.marketTable[i]));
			}
			//住居
			data.residences = new List<ResidenceData>();
			for (int i = 0, max = UserDB.Instance.residenceTable.Count; i < max; i++) {
				data.residences.Add(DataTransfer.ConvertData(UserDB.Instance.residenceTable[i]));
			}
			//倉庫
			data.storages = new List<StorageData>();
			for (int i = 0, max = UserDB.Instance.storageTable.Count; i < max; i++) {
				data.storages.Add(DataTransfer.ConvertData(UserDB.Instance.storageTable[i]));
			}
			//サービス
			data.services = new List<ServiceData>();
			for (int i = 0, max = UserDB.Instance.serviceTable.Count; i < max; i++) {
				data.services.Add(DataTransfer.ConvertData(UserDB.Instance.serviceTable[i]));
			}


			data.chainList = DataTransfer.ConvertData(UserDB.Instance.chainTable);

			return data;
		}


		/// <summary>
		/// シーン開始通信
		/// </summary>
		public static Response TownMainOffline(Request req) {
			var request = req as TownMainRequest;

			TownMainResponse response = new TownMainResponse();
			response.townData = SerializeTownDataResponse();
			response.characters = DataTransfer.Convert(UserCharacterTableDAO.SelectAll());
			response.items = DataTransfer.Convert(UserItemTableDAO.SelectAll());
			return response;
		}

		/// <summary>
		/// 生産施設を建てる
		/// </summary>
		public static Response BuildFactoryOffline(Request req) {
			var request = req as BuildFactoryRequest;

			UserDB.Instance.globalDataTable.id = request.factoryData.id;
			UserFactoryTable table = DataTransfer.ConvertData(request.factoryData);

			//使用したアイテムを減らす
			UserItemTable itemTable = UserItemTableDAO.SelectById(request.factoryData.baseId, (int)FacilityType.Factory);
			itemTable.number -= 1;
			Debug.Assert(itemTable.number >= 0, "item number error:" + itemTable.number);

			UserFactoryTableDAO.Insert(table);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));
			UserDB.Save();
			return new BuildFactoryResponse();
		}
		/// <summary>
		/// マーケットを建てる
		/// </summary>
		public static Response BuildMarketOffline(Request req) {
			var request = req as BuildMarketRequest;

			UserDB.Instance.globalDataTable.id = request.marketData.id;
			UserMarketTable table = DataTransfer.ConvertData(request.marketData);
			//使用したアイテムを減らす
			UserItemTable itemTable = UserItemTableDAO.SelectById(request.marketData.baseId, (int)FacilityType.Market);
			itemTable.number -= 1;
			Debug.Assert(itemTable.number >= 0, "item number error:" + itemTable.number);

			UserMarketTableDAO.Insert(table);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));


			UserDB.Save();

			return new BuildMarketResponse();
		}

		/// <summary>
		/// 住居を建てる
		/// </summary>
		public static Response BuildResidenceOffline(Request req) {
			var request = req as BuildResidenceRequest;

			UserDB.Instance.globalDataTable.id = request.residenceData.id;
			UserResidenceTable table = DataTransfer.ConvertData(request.residenceData);
			//使用したアイテムを減らす
			UserItemTable itemTable = UserItemTableDAO.SelectById(request.residenceData.baseId, (int)FacilityType.Residence);
			itemTable.number -= 1;
			Debug.Assert(itemTable.number >= 0, "item number error:" + itemTable.number);

			UserResidenceTableDAO.Insert(table);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));


			UserDB.Save();

			return new BuildResidenceResponse();
		}

		/// <summary>
		/// 倉庫を建てる
		/// </summary>
		public static Response BuildStorageOffline(Request req) {
			var request = req as BuildStorageRequest;

			UserDB.Instance.globalDataTable.id = request.storageData.id;
			UserStorageTable table = DataTransfer.ConvertData(request.storageData);
			//使用したアイテムを減らす
			UserItemTable itemTable = UserItemTableDAO.SelectById(request.storageData.baseId, (int)FacilityType.Residence);
			itemTable.number -= 1;
			Debug.Assert(itemTable.number >= 0, "item number error:" + itemTable.number);

			UserStorageTableDAO.Insert(table);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));


			UserDB.Save();

			return new BuildStorageResponse();
		}

		/// <summary>
		/// サービス施設を建てる
		/// </summary>
		public static Response BuildServiceOffline(Request req) {
			var request = req as BuildServiceRequest;

			UserDB.Instance.globalDataTable.id = request.serviceData.id;
			UserServiceTable table = DataTransfer.ConvertData(request.serviceData);
			//使用したアイテムを減らす
			UserItemTable itemTable = UserItemTableDAO.SelectById(request.serviceData.baseId, (int)FacilityType.Service);
			itemTable.number -= 1;
			Debug.Assert(itemTable.number >= 0, "item number error:" + itemTable.number);

			UserServiceTableDAO.Insert(table);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));


			UserDB.Save();

			return new BuildServiceResponse();
		}

		//@note 建物はクエストの報酬で得られる予定なのでDemolishでマップから取り除いた建物は所持品に戻るイメージ
		/// <summary>
		/// 生産施設を取り除く
		/// </summary>
		public static Response DemolishFactoryOffline(Request req) {
			var request = req as DemolishFactoryRequest;
			//労働者の割り当てを解放
			foreach (var chara in UserCharacterTableDAO.SelectByWorkId(request.factoryData.id)) {
				chara.workId = -1;
			}
			//取り除いた建物はアイテムに戻す
			UserItemTable table = UserItemTableDAO.SelectById(request.factoryData.baseId, (int)FacilityType.Factory);
			table.number += 1;

			//@todo 一旦Deleteで対応する
			UserFactoryTableDAO.Delete(request.factoryData.id);
			//Chainは再構築可能なのでdeleteでオッケー、と思うけどサーバ詳しい人に聞きたいところ
			//データ量とも相談だと思うのでまずは作る
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);

			UserDB.Save();

			return new DemolishFactoryResponse() { };
		}

		/// <summary>
		/// マーケットを取り除く
		/// </summary>
		public static Response DemolishMarketOffline(Request req) {
			var request = req as DemolishMarketRequest;

			//取り除いた建物はアイテムに戻す
			UserItemTable table = UserItemTableDAO.SelectById(request.marketData.baseId, (int)FacilityType.Market);
			table.number += 1;

			//@todo 一旦Deleteで対応する
			UserMarketTableDAO.Delete(request.marketData.id);
			//Chainは再構築可能なのでdeleteでオッケー、と思うけどサーバ詳しい人に聞きたいところ
			//データ量とも相談だと思うのでまずは作る
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);

			UserDB.Save();

			return new DemolishMarketResponse() { };
		}
		/// <summary>
		/// 住居を取り除く
		/// </summary>
		public static Response DemolishResidenceOffline(Request req) {
			var request = req as DemolishResidenceRequest;
			foreach (var chara in UserCharacterTableDAO.SelectByStayId(request.residenceData.id)) {
				chara.stayId = -1;
				chara.workId = -1;
			}
			//取り除いた建物はアイテムに戻す
			UserItemTable table = UserItemTableDAO.SelectById(request.residenceData.baseId, (int)FacilityType.Residence);
			table.number += 1;

			//@todo 一旦Deleteで対応する
			UserResidenceTableDAO.Delete(request.residenceData.id);
			//Chainは再構築可能なのでdeleteでオッケー、と思うけどサーバ詳しい人に聞きたいところ
			//データ量とも相談だと思うのでまずは作る
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);

			UserDB.Save();

			return new DemolishResidenceResponse() { };
		}
		/// <summary>
		/// 倉庫を取り除く
		/// </summary>
		public static Response DemolishStorageOffline(Request req) {
			var request = req as DemolishStorageRequest;
			//取り除いた建物はアイテムに戻す
			UserItemTable table = UserItemTableDAO.SelectById(request.storageData.baseId, (int)FacilityType.Storage);
			table.number += 1;
			//@todo 一旦Deleteで対応する
			UserStorageTableDAO.Delete(request.storageData.id);
			//Chainは再構築可能なのでdeleteでオッケー、と思うけどサーバ詳しい人に聞きたいところ
			//データ量とも相談だと思うのでまずは作る
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);

			UserDB.Save();

			return new DemolishStorageResponse() { };
		}

		/// <summary>
		/// サービス施設を取り除く
		/// </summary>
		public static Response DemolishServiceOffline(Request req) {
			var request = req as DemolishServiceRequest;
			//取り除いた建物はアイテムに戻す
			UserItemTable table = UserItemTableDAO.SelectById(request.serviceData.baseId, (int)FacilityType.Service);
			table.number += 1;
			//@todo 一旦Deleteで対応する
			UserServiceTableDAO.Delete(request.serviceData.id);
			//Chainは再構築可能なのでdeleteでオッケー、と思うけどサーバ詳しい人に聞きたいところ
			//データ量とも相談だと思うのでまずは作る
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);

			UserDB.Save();

			return new DemolishServiceResponse() { };
		}

		/// <summary>
		/// 生産施設を再配置する
		/// </summary>
		public static Response RelocateFactoryOffline(Request req) {
			var request = req as RelocateFactoryRequest;

			//再配置前のchainは削除する
			UserChainTableDAO.DeleteBySenderOrReciever(request.factoryData.id);
			UserFactoryTableDAO.Update(DataTransfer.ConvertData(request.factoryData));
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chains));

			UserDB.Save();

			return new RelocateFactoryResponse() { };
		}

		/// <summary>
		/// マーケットを再配置する
		/// </summary>
		public static Response RelocateMarketOffline(Request req) {
			var request = req as RelocateMarketRequest;

			//再配置前のchainは削除する
			UserChainTableDAO.DeleteBySenderOrReciever(request.marketData.id);
			UserMarketTableDAO.Update(DataTransfer.ConvertData(request.marketData));
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chains));

			UserDB.Save();

			return new RelocateMarketResponse() { };
		}

		/// <summary>
		/// 住居を再配置する
		/// </summary>
		public static Response RelocateResidenceOffline(Request req) {
			var request = req as RelocateResidenceRequest;

			//再配置前のchainは削除する
			UserChainTableDAO.DeleteBySenderOrReciever(request.residenceData.id);
			UserResidenceTableDAO.Update(DataTransfer.ConvertData(request.residenceData));
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chains));

			UserDB.Save();

			return new RelocateResidenceResponse() { };
		}

		/// <summary>
		/// 倉庫を再配置する
		/// </summary>
		public static Response RelocateStorageOffline(Request req) {
			var request = req as RelocateStorageRequest;

			//再配置前のchainは削除する
			UserChainTableDAO.DeleteBySenderOrReciever(request.storageData.id);
			UserStorageTableDAO.Update(DataTransfer.ConvertData(request.storageData));
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chains));

			UserDB.Save();

			return new RelocateStorageResponse() { };
		}

		/// <summary>
		/// サービス施設を再配置する
		/// </summary>
		public static Response RelocateServiceOffline(Request req) {
			var request = req as RelocateServiceRequest;

			//再配置前のchainは削除する
			UserChainTableDAO.DeleteBySenderOrReciever(request.serviceData.id);
			UserServiceTableDAO.Update(DataTransfer.ConvertData(request.serviceData));
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chains));

			UserDB.Save();

			return new RelocateServiceResponse() { };
		}

		/// <summary>
		/// 生産施設を更新する
		/// </summary>
		public static Response UpdateFactoryOffline(Request req) {
			var request = req as UpdateFactoryRequest;
			UserFactoryTableDAO.Update(DataTransfer.ConvertData(request.factoryData));

			UserDB.Save();

			return new UpdateFactoryResponse() { };
		}

		/// <summary>
		/// マーケットを更新する
		/// </summary>
		public static Response UpdateMarketOffline(Request req) {
			var request = req as UpdateMarketRequest;
			UserMarketTableDAO.Update(DataTransfer.ConvertData(request.marketData));

			//Chainの更新。仮なので削除->新規作成で実装する
			List<(int, int)> selectList = request.chain.Select(c => (c.senderId, c.recieverId)).ToList<(int, int)>();
			UserChainTableDAO.Delete(selectList);
			UserChainTableDAO.Insert(DataTransfer.ConvertData(request.chain));

			UserDB.Save();

			return new UpdateMarketResponse() { };
		}

		/// <summary>
		/// 住居を更新する
		/// </summary>
		public static Response UpdateResidenceOffline(Request req) {
			var request = req as UpdateResidenceRequest;
			UserResidenceTableDAO.Update(DataTransfer.ConvertData(request.residenceData));

			UserDB.Save();

			return new UpdateResidenceResponse() { };
		}

		/// <summary>
		/// 倉庫を更新する
		/// </summary>
		public static Response UpdateStorageOffline(Request req) {
			var request = req as UpdateStorageRequest;
			UserStorageTableDAO.Update(DataTransfer.ConvertData(request.storageData));

			UserDB.Save();

			return new UpdateStorageResponse() { };
		}
		/// <summary>
		/// サービス施設を更新する
		/// </summary>
		public static Response UpdateServiceOffline(Request req) {
			var request = req as UpdateServiceRequest;
			UserServiceTableDAO.Update(DataTransfer.ConvertData(request.serviceData));

			UserDB.Save();

			return new UpdateServiceResponse() { };
		}
		/// <summary>
		/// キャラクターを更新する
		/// </summary>
		public static Response UpdateCharacterOffline(Request req) {
			var request = req as UpdateCharacterRequest;
			UserCharacterTableDAO.Update(DataTransfer.Convert(request.characters));

			UserDB.Save();

			return new UpdateCharacterResponse() { };
		}
	}
}
#endif
