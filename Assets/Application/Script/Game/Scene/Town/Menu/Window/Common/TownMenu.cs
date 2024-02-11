using UnityEngine;
using Project.Lib;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class TownMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.25f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		private const int show = 4;
		private int index_;
		private System.Action draw_ = () => { };

		public TownMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
			draw_ = DrawMain;
		}

		private void DrawMain() {
			CaptionLabel.Draw("タウン");
			if (FitGUILayout.Button("生産施設")) {
				draw_ = DrawFacility<MstFactoryData>;
			}
			if (FitGUILayout.Button("マーケット")) {
				draw_ = DrawFacility<MstMarketData>;
			}
			if (FitGUILayout.Button("住居")) {
				draw_ = DrawFacility<MstResidenceData>;
			}
			if (FitGUILayout.Button("サービス施設")) {
				draw_ = DrawFacility<MstServiceData>;
			}

		}

		private void DrawFacility<T>() {
			var list = BaseDataManager.GetList<T>();
			using (new GUILayout.HorizontalScope()) {
				//一番上にいる場合はボタン無効にする
				if (index_ == 1) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↑")) {
					index_--;
				}
				GUI.enabled = true;
				if (index_ >= list.Count - show) {
					GUI.enabled = false;
				}
				if (FitGUILayout.Button("↓")) {
					index_++;
				}
				GUI.enabled = true;

				if (list.Count > show) {
					index_ = Mathf.Clamp(index_, 1, list.Count - show);
				}
			}

			int max = list.Count < show ? list.Count : show;
			for (int i = 0; i < max; i++) {
				GUILayout.BeginHorizontal();
				int idx = index_ + i;
				(int Id, string name, int price, FacilityType type) data = GetId<T>(list[idx]);
				int number = TownItemData.I.GetItemData((int)data.type, data.Id)?.number ?? 0;
				int price = data.price;
				string cap = string.Format("{0}(${1}) x {2}", data.name, price, number );
				if (FitGUILayout.Button(cap)) {
					TownMessage.CreateFacility.Broadcast(data.type, idx);
				}
				if (FitGUILayout.Button("購入", 100, 0)) {
					TownMessage.PurchaseItem.Broadcast(data.type, data.Id, 1);
				}
				GUILayout.EndHorizontal();

			}

			if (FitGUILayout.Button("戻る")) {
				draw_ = DrawMain;
			}

		}

		private (int, string, int, FacilityType) GetId<T>(T data) {
			switch (data) {
				case MstFactoryData master:
					return (master.Id, master.DbgName, master.PurchasePrice, FacilityType.Factory);
				case MstMarketData master:
					return (master.Id, master.DbgName, master.PurchasePrice, FacilityType.Market);
				case MstResidenceData master:
					return (master.Id, master.DbgName, master.PurchasePrice, FacilityType.Residence);
				case MstStorageData master:
					return (master.Id, master.DbgName, master.PurchasePrice, FacilityType.Storage);
				case MstServiceData master:
					return (master.Id, master.DbgName, master.PurchasePrice, FacilityType.Service);

			}
			return (0, "", 0, FacilityType.None);
		}


		protected override void Draw(int id) {
			draw_();
		}

	}

#endif
}
