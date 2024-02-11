using Project.Lib;
using UnityEngine;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// 建物再配置
		/// </summary>
		public static class ReLocateFacility {
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

				town.GridMap.Clear(data.facility.GetFillTile());
				//Townの制御を再配置状態に移行する
				(town.HaveState.GetState((int)TownMain.State.FacilityReLocate) as StateTownFacilityReLocate).SetTargetFacility(data.facility);
				town.ChangeState(TownMain.State.FacilityReLocate);

			}

		}
		/// <summary>
		/// 建物再配置
		/// </summary>
		public static class CommitLocateFacility {
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

				FacilityCmdUtil.Relocate(data.facility, (res) => { town.ChangeState(TownMain.State.Main);});
			}
		}
		/// <summary>
		/// 建物再配置
		/// </summary>
		public static class CancelLocateFacility {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data {
				public Facility entity;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Facility f) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { entity = f }),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;

				//再配置前のChain
				Vector3 pos = (town.HaveState.GetState((int)TownMain.State.FacilityReLocate) as StateTownFacilityReLocate).GetLastFacility();

				switch (data.entity) {
					case Factory factory:
						//チェーンを破棄
						factory.DestroyChain();
						factory.SetPosition(pos);
						factory.CreateChain(town);
						break;
					case Market market:
						market.DestroyChain();
						market.SetPosition(pos);
						market.CreateChain(town);
						break;
					case Residence residence:
						residence.DestroyChain();
						residence.SetPosition(pos);
						residence.CreateChain(town);
						break;
					case Storage storage:
						storage.DestroyChain();
						storage.SetPosition(pos);
						storage.CreateChain(town);
						break;
					case Service service:
						service.DestroyChain();
						service.SetPosition(pos);
						service.CreateChain(town);
						break;
				}

				town.GridMap.Fill(data.entity.GetFillTile());
				data.entity.ClearBuildable();
				town.ChangeState(TownMain.State.Main);
			}

		}
	}
}
