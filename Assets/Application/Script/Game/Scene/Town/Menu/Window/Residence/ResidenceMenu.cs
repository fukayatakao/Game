using Project.Lib;
using Project.Mst;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class ResidenceMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		int tab_;
		Residence owner_;
		FacilityMenuState state_;

		MstPopulationDataSv populationBase_;
		//折り畳みフラグ
		bool[] foldFlag;

		SelectCharacterMenu selectMenu_;
		Rect[] buttonRect_;

		public ResidenceMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
			HideEvent = () => {
				if (selectMenu_ != null)
					selectMenu_.Hide();
			};
		}
		public void Init(Residence owner, FacilityMenuState state) {
			tab_ = 0;
			owner_ = owner;
			state_ = state;
			populationBase_ = BaseDataManager.GetDictionary<int, MstPopulationDataSv>()[owner_.Data.Population];

			foldFlag = new bool[populationBase_.DemandArticle.Count];
			for(int i =0, max = populationBase_.DemandArticle.Count; i < max; i++) {
				foldFlag[i] = true;
			}

			buttonRect_ = new Rect[owner_.Data.RoomCount];
		}

		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label(owner_.Name);

			tab_ = PageTab.Draw(tab_, new string[] { "需要", "住人", "情報"}, new System.Action[] { DrawDemand, DrawResident, ()=> { MenuUtil.DrawResidenceSupply(owner_); } });
			MenuUtil.DrawFooter(owner_, state_);
		}
		/// <summary>
		/// 住人表示
		/// </summary>
		private void DrawResident() {
			//住んでいるキャラ一覧を取得
			List<int> characters = PeopleData.I.CalcStayCharacter(owner_.Id);
			//そこに住んでいないキャラ一覧を取得
			List<int> selectable = PeopleData.I.CalcNotStayCharacter();
			for (int i = 0, max = owner_.Data.RoomCount; i < max; i++) {
				if (i < characters.Count) {
					int charaid = characters[i];
					if (FitGUILayout.Button(PeopleData.I.Characters[charaid].name)) {
						selectMenu_ = DebugWindowManager.Open<SelectCharacterMenu>();
						selectMenu_.Init(selectable, (newCharaid) => { PeopleData.I.UpdateStayCharacter(owner_.Id, charaid, newCharaid); });
						selectMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - selectMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
					}
				} else {
					if (FitGUILayout.Button("なし")) {
						selectMenu_ = DebugWindowManager.Open<SelectCharacterMenu>();
						selectMenu_.Init(selectable, (newCharaid) => { PeopleData.I.UpdateStayCharacter(owner_.Id, -1, newCharaid); });
						selectMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - selectMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
					}

				}

				//GUIが描画されたときだけ処理
				if (Event.current.type == EventType.Repaint) {
					buttonRect_[i] = GUILayoutUtility.GetLastRect();
				}
			}
		}

		/// <summary>
		/// 需要表示
		/// </summary>
		private void DrawDemand() {
			Dictionary<int, MstGoodsData> baseGoods = BaseDataManager.GetDictionary<int, MstGoodsData>();

			for (int i = 0; i < populationBase_.DemandArticle.Count; i++) {
				MstPopulationDemandData demand = BaseDataManager.GetDictionary<int, MstPopulationDemandData>()[populationBase_.DemandArticle[i]];

				var dict = BaseDataManager.GetDictionary<int, MstArticleData>();
				MstArticleData article = BaseDataManager.GetDictionary<int, MstArticleData>()[demand.Article];

				GUILayout.BeginHorizontal();
				if (FitGUILayout.Button(article.DbgName)) {
					foldFlag[i] = !foldFlag[i];
				}
				FitGUILayout.Label("×" + demand.Number + " : " + demand.Amount);
				GUILayout.EndHorizontal();

				//展開する場合は近隣マーケット→倉庫から品目が一致するものをリスト表示する
				if (!foldFlag[i]) {
					DrawMarketGoods(demand.Article);
				}
			}
		}

		/// <summary>
		/// マーケットが管理してるグッズから品目が一致するものを取得
		/// </summary>
		private void DrawMarketGoods(int articleId) {
			if(owner_.RecieveList.Count == 0) {
				FitGUILayout.Label("  供給なし");
				return;
			}

			Dictionary<int, MstGoodsData> goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			//供給元になるMarketを表示する
			foreach(var pair in owner_.RecieveList) {
				List<int> goodsIds = new List<int>();
				Market market = (Market)pair.Key;
				for (int i = 0, max = market.HaveParam.NegativeGoodsIds.Length; i < max; i++) {
					//マーケットが管理しているグッズで品目が一致するものを収集
					int id = market.HaveParam.NegativeGoodsIds[i];
					if (goods[id].Article == articleId) {
						goodsIds.Add(id);
					}
				}

				if(goodsIds.Count > 0) {
					FitGUILayout.Label("  供給元:" + market.Name);
					string showGoods = "  グッズ:";
					showGoods += goods[goodsIds[0]].DbgName;
					for (int i = 1, max = goodsIds.Count; i < max; i++) {
						showGoods += ",";
						showGoods += goods[goodsIds[i]].DbgName + ",";
					}
					FitGUILayout.Label(showGoods);
				} else {
					FitGUILayout.Label("  供給なし");
				}
			}
		}

	}
#endif
}
