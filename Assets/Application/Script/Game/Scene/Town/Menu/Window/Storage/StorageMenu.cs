using UnityEngine;
using Project.Lib;

using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class StorageMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		Storage owner_;
		FacilityMenuState state_;

		Rect[] buttonRect_;

		StorageGoodsMenu goodsMenu_;
		/// <summary>
		/// コンストラク
		/// </summary>
		public StorageMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}
		/// <summary>
		/// 表示するstorageのインスタンスを設定
		/// </summary>
		public void Init(Storage owner, FacilityMenuState state) {
			owner_ = owner;
			state_ = state;
			buttonRect_ = new Rect[owner_.HaveParam.Slots.Length];
		}

		/// <summary>
		/// 描画
		/// </summary>
		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label(owner_.Name);

			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			//倉庫が管理するgoodsを表示
			for (int i = 0, max = owner_.HaveParam.Slots.Length; i < max; i++) {
				Slot slot = owner_.HaveParam.Slots[i];
				if (FitGUILayout.Button(goods[slot.GoodsId].DbgName)) {
					goodsMenu_ = DebugWindowManager.Open<StorageGoodsMenu>();
					goodsMenu_.Init(slot);
					goodsMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - goodsMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
				}
				//GUIが描画されたときだけ処理
				if (Event.current.type == EventType.Repaint) {
					buttonRect_[i] = GUILayoutUtility.GetLastRect();
				}

				GUILayout.BeginHorizontal();
				FitGUILayout.Label("在庫:" + slot.Amount, 200, 0);
				FitGUILayout.Label("グレード:" + slot.Grade.ToString("F"), 250, 0);

				GUILayout.EndHorizontal();
			}

			MenuUtil.DrawFooter(owner_, state_);
		}
		public new void Hide() {
			base.Hide();
			if(goodsMenu_ != null)
				goodsMenu_.Hide();
		}
	}
#endif
}
