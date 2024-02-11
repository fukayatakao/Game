using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class MarketGoodsMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		//マーケットの取扱グッズ配列
		int index_;
		//選択グッズ
		int select_;
		//選択可能グッズのidと表示テキスト
		int[] ids_;
		string[] text_;

		System.Action<int, int> update_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MarketGoodsMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));


			SetAutoResize();
		}

		public void Init(int article, int slotIndex, int currentGoodsId, System.Action<int, int> update) {
			index_ = slotIndex;
			select_ = -1;
			List<MstGoodsData> list = BaseDataManager.GetList<MstGoodsData>();
			List<int> ids = new List<int>();
			List<string> names = new List<string>();
			for (int i = 0, max = list.Count; i < max; i++) {
				//品目が一致しないものは選択できないようにする
				if (list[i].Article != article)
					continue;
				ids.Add(list[i].Id);
				names.Add(list[i].DbgName);
			}
			ids_ = ids.ToArray();
			text_ = names.ToArray();

			for (int i = 0, max = ids_.Length; i < max; i++) {
				if (ids_[i] == currentGoodsId) {
					select_ = i;
					break;
				}
			}
			update_ = update;
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label("Goods選択");

			GUIUtil.BeginChangeCheck();
			select_ = FitGUILayout.SelectionGrid(select_, text_, 4);
			if (GUIUtil.EndChangeCheck()) {
				update_(index_, ids_[select_]);
			}
		}

	}
#endif
}
