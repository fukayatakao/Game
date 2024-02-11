using Project.Lib;
using Project.Network;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// 建設済みの建物を撤去する
		/// </summary>
		public static class DemolishFacility {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Facility facility;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Facility e) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { facility = e }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;
				town.GridMap.Clear(data.facility.GetFillTile());
				TownItemData.I.AddItemNumber((int)data.facility.Type, data.facility.BaseId, 1);

				switch (data.facility) {
				case Factory factory:
					DemolishFactoryCmd.CreateAsync(
						new DemolishFactoryRequest(factory.ToNetworkData(), factory.ToChainData()),
						(res) => { town.ChangeState(TownMain.State.Main); },
						(res, status) => { });
					town.DestroyFactory(factory);
					break;
				case Market market:
					DemolishMarketCmd.CreateAsync(
						new DemolishMarketRequest(market.ToNetworkData(), market.ToChainData()),
						(res) => { town.ChangeState(TownMain.State.Main); },
						(res, status) => { });
					town.DestroyMarket(market);
					break;
				case Residence residence:
					DemolishResidenceCmd.CreateAsync(
						new DemolishResidenceRequest(residence.ToNetworkData(), residence.ToChainData()),
						(res) => { town.ChangeState(TownMain.State.Main); },
						(res, status) => { });
					town.DestroyResidence(residence);
					break;
				case Storage storage:
					DemolishStorageCmd.CreateAsync(
						new DemolishStorageRequest(storage.ToNetworkData(), storage.ToChainData()),
						(res) => { town.ChangeState(TownMain.State.Main); },
						(res, status) => { });
					town.DestroyStorage(storage);
					break;
				case Service service:
					DemolishServiceCmd.CreateAsync(
						new DemolishServiceRequest(service.ToNetworkData(), service.ToChainData()),
						(res) => { town.ChangeState(TownMain.State.Main); },
						(res, status) => { });
					town.DestroyService(service);
					break;
				}
				town.CalculateLogistics();
				town.ChainMap.Clear();
				//メニュー処理がちらばってしまってきているのでそのうちどうにかしないと良くない
#if DEVELOP_BUILD
				TownAlternativeMenu.I.HideSub();
#endif

			}
		}
	}
}
