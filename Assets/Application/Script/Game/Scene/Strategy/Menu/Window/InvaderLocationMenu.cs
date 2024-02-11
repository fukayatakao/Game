using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class InvaderLocationMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		StrategyMain main_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public InvaderLocationMenu() {
            base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public void Init(StrategyMain main) {
			main_ = main;
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
            CloseButton.Draw(this);
            CaptionLabel.Draw("部隊を配置");

			for(int i = 0, max = main_.HaveStrategyBoard.Reserve.Count; i < max; i++) {
				if (FitGUILayout.Button(main_.HaveStrategyBoard.Reserve[i].name)) {
					//順番入れ替え
					var data = main_.HaveStrategyBoard.Reserve[i];
					main_.HaveStrategyBoard.Reserve.RemoveAt(i);
					main_.HaveStrategyBoard.Reserve.Insert(0, data);

					break;
				}
			}

			FitGUILayout.Label("");
			if (FitGUILayout.Button("編成")) {
				SceneTransition.ChangeOrganize(SceneTransition.SceneType.Strategy);
			}
			if (FitGUILayout.Button("タウンへ戻る")) {
				SceneTransition.ChangeTown();
			}
		}

	}
#endif
}
