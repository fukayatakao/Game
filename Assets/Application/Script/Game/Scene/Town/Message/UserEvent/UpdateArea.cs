using Project.Lib;


namespace Project.Game {
	public static partial class TownMessage {
		/// <summary>
		/// ゲーム更新
		/// </summary>
		public static class UpdateArea {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容
			private class Data
			{
				public Facility facility;
				public float radius;
			}
			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(Facility f, float r) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data(){facility = f, radius = r}),
					(int)MessageGroup.UserEvent
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(TownMain town, MessageObject msg) {
				Data data = (Data)msg.Data;
				data.facility.HaveArea.SetRadius(data.radius);
				data.facility.UpdateChainByMovePosition(town, out var include, out var exclude);
				town.ChainMap.Linking(include, exclude, ChainLinkMap.Arrow.BLUE);
				town.CalculateLogistics();

				FacilityCmdUtil.Relocate(data.facility, (res) => { });
			}
		}
	}
}
