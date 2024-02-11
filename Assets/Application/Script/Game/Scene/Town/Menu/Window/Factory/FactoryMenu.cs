using Project.Lib;
using Project.Mst;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class FactoryMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		int tab_;
		Factory owner_;
		FacilityMenuState state_;

		int selectProduct_;
		string[] selectableProductName_;
		Rect[] buttonRect_;

		SelectCharacterMenu selectMenu_;

		int currentGrade_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FactoryMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
			HideEvent = () => {
				if (selectMenu_ != null)
					selectMenu_.Hide();
			};
		}
		/// <summary>
		/// 情報表示するEntityを受け取る
		/// </summary>
		public void Init(Factory owner, FacilityMenuState state) {
			owner_ = owner;
			state_ = state;

			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			if(owner.Data.Goods2 != 0) {
				selectableProductName_ = new string[] { goods[owner.Data.Goods].DbgName, goods[owner.Data.Goods2].DbgName, };
				selectProduct_ = owner.HaveParam.Product == owner.Data.Goods ? 0 : 1;
			} else {
				selectableProductName_ = new string[] { goods[owner.Data.Goods].DbgName };
				selectProduct_ = 0;
			}

			currentGrade_ = owner.HaveParam.Grade;
			buttonRect_ = new Rect[owner_.Data.MaxWorker];
		}

		/// <summary>
		/// 表示
		/// </summary>
		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label(owner_.Name);



			//生産物と生産量の表示
			tab_ = PageTab.Draw(tab_, new string[] { "生産", "労働者", "情報" }, new System.Action[] { DrawProduct, DrawWorker, ()=> { MenuUtil.DrawProduct(owner_); } });

		}


		/// <summary>
		/// 住人表示
		/// </summary>
		private void DrawProduct() {
			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			int product = owner_.HaveParam.Product;
			//生産物切り替え
			GUIUtil.BeginChangeCheck();
			selectProduct_ = FitGUILayout.SelectionGrid(selectProduct_, selectableProductName_, 2);
			if (GUIUtil.EndChangeCheck()) {
				owner_.UpdateProduct(selectProduct_ == 0 ? owner_.Data.Goods : owner_.Data.Goods2);
			}
			//対価を払って生産物グレードを上げることができる
			GUIUtil.BeginChangeCheck();
			currentGrade_ = Selector.Draw("グレード:", currentGrade_, owner_.Data.MinGrade, owner_.Data.MaxGrade);
			if (GUIUtil.EndChangeCheck()) {
				owner_.UpdateGrade(currentGrade_);
			}
			FitGUILayout.Label("生産量:" + (selectProduct_ == 0 ? owner_.Data.Output : owner_.Data.Output2));

			//生産に必要な原料とその必要量を表示
			for (int i = 0, max = goods[product].Resource.Length; i < max; i++) {
				int resource = goods[product].Resource[i];
				if (resource != 0)
					FitGUILayout.Label("材料:" + goods[resource].DbgName + " x" + goods[product].UseAmount[i]);
			}
			//操作移行ボタン表示
			MenuUtil.DrawFooter(owner_, state_);
		}

		/// <summary>
		/// 労働者表示
		/// </summary>
		private void DrawWorker() {
			//住んでいるキャラ一覧を取得
			List<int> characters = PeopleData.I.CalcAssignCharacter(owner_.Id);
			//そこに住んでいないキャラ一覧を取得
			List<int> selectable = PeopleData.I.CalcNotAssignCharacter();
			for (int i = 0, max = owner_.Data.MaxWorker; i < max; i++) {
				if (i < characters.Count) {
					int charaid = characters[i];
					if (FitGUILayout.Button(PeopleData.I.Characters[charaid].name)) {
						selectMenu_ = DebugWindowManager.Open<SelectCharacterMenu>();
						selectMenu_.Init(selectable, (newCharaid) => { PeopleData.I.UpdateAssignCharacter(owner_.Id, charaid, newCharaid); });
						selectMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - selectMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
					}
				} else {
					if (FitGUILayout.Button("なし")) {
						selectMenu_ = DebugWindowManager.Open<SelectCharacterMenu>();
						selectMenu_.Init(selectable, (newCharaid) => { PeopleData.I.UpdateAssignCharacter(owner_.Id, -1, newCharaid); });
						selectMenu_.SetPositionRaw(new Vector2(WindowRect.xMin - selectMenu_.WindowRect.width, WindowRect.yMin + buttonRect_[i].yMin));
					}

				}

				//GUIが描画されたときだけ処理
				if (Event.current.type == EventType.Repaint) {
					buttonRect_[i] = GUILayoutUtility.GetLastRect();
				}
			}
		}
	}
#endif
}
