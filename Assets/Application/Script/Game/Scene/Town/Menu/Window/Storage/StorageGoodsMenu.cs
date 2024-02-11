using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class StorageGoodsMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		Slot slot_;
		int select_;
		string[] text_;
		int[] ids_;
		public StorageGoodsMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			var dict = BaseDataManager.GetDictionary<int, MstGoodsData>();
			List<int> ids = new List<int>();
			List<string> names = new List<string>();
			foreach (MstGoodsData data in dict.Values) {
				ids.Add(data.Id);
				names.Add(data.DbgName);
			}
			ids_ = ids.ToArray();
			text_ = names.ToArray();

			SetAutoResize();
		}

		public void Init(Slot slot) {
			slot_ = slot;
			for(int i = 0,max = ids_.Length; i < max; i++) {
				if(ids_[i] == slot.GoodsId) {
					select_ = i;
					break;
				}
			}
		}


		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label("Goods選択");

			GUIUtil.BeginChangeCheck();
			select_ = FitGUILayout.SelectionGrid(select_, text_, 4);
			if (GUIUtil.EndChangeCheck()) {
				slot_.GoodsId = ids_[select_];
			}
			FitGUILayout.Label("在庫");
			slot_.Amount = Selector.Draw(slot_.Amount, 0, 1000, 1, 100);
			FitGUILayout.Label("グレード");
			slot_.Grade = Selector.Draw(slot_.Grade, 0f, 4f, 0.1f, 1f);
		}

	}
#endif
}
