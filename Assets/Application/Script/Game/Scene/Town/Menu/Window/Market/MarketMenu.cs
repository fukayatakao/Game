using Project.Lib;
using Project.Mst;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class MarketMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;
		Rect[] buttonRect_;

		int tab_;
		Market owner_;
		FacilityMenuState state_;

		MarketGoodsMenu goodsMenu_;
		/// <summary>
		/// コンストラク
		/// </summary>
		public MarketMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();

			HideEvent = () => {
				if (goodsMenu_ != null)
					goodsMenu_.Hide();
			};
		}
		/// <summary>
		/// 表示するstorageのインスタンスを設定
		/// </summary>
		public void Init(Market owner, FacilityMenuState state) {
			owner_ = owner;
			state_ = state;
			buttonRect_ = new Rect[owner_.HaveParam.NegativeGoodsIds.Length];
		}

		/// <summary>
		/// 描画
		/// </summary>
		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label(owner_.Name);
			//生産物と生産量の表示
			tab_ = PageTab.Draw(tab_, new string[] { "生産", "情報" }, new System.Action[] { DrawItem, () => { MenuUtil.DrawMarketResult(owner_); } });
		}

		private void DrawItem() {
			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			FitGUILayout.Label("Negativeリスト");
			GUI.color = Color.red;
			;
			//倉庫が管理するgoodsを表示
			for (int i = 0, max = owner_.HaveParam.NegativeGoodsIds.Length; i < max; i++) {
				int goodsId = owner_.HaveParam.NegativeGoodsIds[i];
				if (FitGUILayout.Button(goods[goodsId].DbgName)) {
					goodsMenu_ = DebugWindowManager.Open<MarketGoodsMenu>();
					goodsMenu_.Init(owner_.Data.Article, i, goodsId, owner_.UpdateGoods);
					goodsMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - goodsMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
				}
				//GUIが描画されたときだけ処理
				if (Event.current.type == EventType.Repaint) {
					buttonRect_[i] = GUILayoutUtility.GetLastRect();
				}

			}
			GUI.color = Color.white;

			MenuUtil.DrawFooter(owner_, state_);

		}
	}
#endif
}
