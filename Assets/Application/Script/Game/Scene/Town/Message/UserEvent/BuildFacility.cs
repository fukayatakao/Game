using UnityEngine;
using Project.Lib;
using Project.Network;

namespace Project.Game {
	public static partial class TownMessage {
		//新規建設ふろー
		//Create->Build(決定)
		//Create->Destroy(キャンセル)
		/// <summary>
		/// 建物を建てる
		/// </summary>
		public static class CreateFacility {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public FacilityType type;
				public int id;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(FacilityType t, int id) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { type = t, id = id }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				//残り数量をチェック
				ItemData item = TownItemData.I.GetItemData((int)data.type, data.id);
				if (item == null || item.number == 0)
					return;

				Create(town, data.type, data.id);
			}

			private static async void Create(TownMain town, FacilityType type, int baseId) {
				//Clinet側で先に作ってChainの計算もクライアントで行ってサーバに送る
				Facility entity;
				switch (type) {
				case FacilityType.Factory:
					entity = await town.CreateFactory(baseId);
					break;
				case FacilityType.Market:
					entity = await town.CreateMarket(baseId);
					break;
				case FacilityType.Residence:
					entity = await town.CreateResidence(baseId);
					break;
				case FacilityType.Storage:
					entity = await town.CreateStorage(baseId);
					break;
				case FacilityType.Service:
					entity = await town.CreateService(baseId);
					break;
				default:
					entity = null;
					Debug.LogError("facility type error:" + type);
					break;
				}

				town.CalculateLogistics();
				//Townの制御を新規設置状態に移行する
				(town.HaveState.GetState((int)TownMain.State.FacilityBuild) as StateTownFacilityBuild).SetTargetFacility(entity);
				town.ChangeState(TownMain.State.FacilityBuild);
			}

		}

		/// <summary>
		/// CreateFacilityした後にBuildで確定してない建物を削除する
		/// </summary>
		/// <remarks>
		/// BuildFaclityした建物はDemolishFacilityで撤去する
		/// </remarks>
		public static class DestroyFacility {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Facility entity;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Facility e) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { entity = e }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;
				switch (data.entity) {
					case Factory factory:
						town.DestroyFactory(factory);
						break;
					case Market market:
						town.DestroyMarket(market);
						break;
					case Residence residence:
						town.DestroyResidence(residence);
						break;
					case Storage storage:
						town.DestroyStorage(storage);
						break;
					case Service service:
						town.DestroyService(service);
						break;
				}


				town.CalculateLogistics();
				town.ChangeState(TownMain.State.Main);
			}
		}
		/// <summary>
		/// 生成した建物を確定させてサーバにも記録させる
		/// </summary>
		public static class BuildFacility {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Facility facility;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Facility f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { facility = f }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				town.GridMap.Fill(data.facility.GetFillTile());
				TownItemData.I.AddItemNumber((int)data.facility.Type, data.facility.BaseId, -1);

				switch (data.facility) {
					case Factory factory:
						BuildFactoryCmd.CreateAsync(
							new BuildFactoryRequest(factory.ToNetworkData(), factory.ToChainData()),
							(res) => { town.ChangeState(TownMain.State.Main); },
							(res, status)=> { });
						break;
					case Market market:
						BuildMarketCmd.CreateAsync(
							new BuildMarketRequest(market.ToNetworkData(), market.ToChainData()),
							(res) => { town.ChangeState(TownMain.State.Main); },
							(res, status) => { });
						break;
					case Residence residence:
						BuildResidenceCmd.CreateAsync(
							new BuildResidenceRequest(residence.ToNetworkData(), residence.ToChainData()),
							(res) => { town.ChangeState(TownMain.State.Main); },
							(res, status) => { });
						break;
					case Storage storage:
						BuildStorageCmd.CreateAsync(
							new BuildStorageRequest(storage.ToNetworkData(), storage.ToChainData()),
							(res) => { town.ChangeState(TownMain.State.Main); },
							(res, status) => { });
						break;
					case Service service:
						BuildServiceCmd.CreateAsync(
							new BuildServiceRequest(service.ToNetworkData(), service.ToChainData()),
							(res) => { town.ChangeState(TownMain.State.Main); },
							(res, status) => { });
						break;
				}
			}
		}
	}
}
