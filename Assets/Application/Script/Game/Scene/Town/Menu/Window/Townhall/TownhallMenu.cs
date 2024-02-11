using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class TownhallMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.11f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		Townhall owner_;
		FacilityMenuState state_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TownhallMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}
		/// <summary>
		/// 情報表示するEntityを受け取る
		/// </summary>
		public void Init(Townhall owner, FacilityMenuState state) {
			owner_ = owner;
			state_ = state;
		}

		/// <summary>
		/// 表示
		/// </summary>
		protected override void Draw(int id) {
			CloseButton.Draw(this);
			FitGUILayout.Label(owner_.Name);


			//操作移行ボタン表示
			MenuUtil.DrawFooter(owner_, state_);
		}
	}
#endif
}
