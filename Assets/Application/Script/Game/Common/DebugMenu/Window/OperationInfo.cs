using UnityEngine;
using System.Collections.Generic;
using Project.Game;

namespace Project.Lib {
#if DEVELOP_BUILD
	public class OperationInfo : DebugWindow {
		static readonly Rect DefaultWindowRect = FitCommon.CalcRect(new Vector2(0.01f, 0.01f), new Vector2(0.3f, 0.2f), FitCommon.Alignment.UpperRight);
		List<IHaveControl> polingList_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OperationInfo() {
			base.Init(DefaultWindowRect);
			SetAutoResize();
			CommonMessage.GetOperationPoling.Broadcast(this);
		}

		public void Init(List<IHaveControl> list) {
			polingList_ = list;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			FitGUILayout.Label("優先度 : 操作クラス");
			if (polingList_ == null)
				return;
			for (int i = 0, max = polingList_.Count; i < max; i++) {
				FitGUILayout.Label(polingList_[i].Priority + ":" + polingList_[i].GetType().Name);
			}
		}

	}
#endif
}
