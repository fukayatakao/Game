using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class FacilityChainDebug : DebugWindow {
		Vector2 scrollPos_ = new Vector2();
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.4f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FacilityChainDebug() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
		}

		public void Init(Facility facility) {

		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			CaptionLabel.Draw("チェイン");
			scrollPos_ = FitGUILayout.BeginScrollView(scrollPos_);


			FitGUILayout.EndScrollView();
		}

	}
#endif
}
