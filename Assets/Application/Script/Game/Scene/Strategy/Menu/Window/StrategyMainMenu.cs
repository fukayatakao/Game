using Project.Lib;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class StrategyMainMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		Dictionary<int, Geopolitics.Way> next_;

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		CoroutineTaskList taskList_ = new CoroutineTaskList();


		StrategyBoard board_;
		int selectNode_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public StrategyMainMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();

			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent, MessageGroup.UserEvent);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public void Init(StrategyBoard board) {
			board_ = board;
			selectNode_ = -1;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			taskList_.Execute();
			CloseButton.Draw(this);
			CaptionLabel.Draw("ターン" + board_.CurrentTurn + "/" + board_.MaxTurn);

			FieldWayMap map = board_.Field.HaveWaymap;

			if(selectNode_ < 0) {
				CaptionLabel.Draw("動かす部隊を選択");

				//盤面の攻撃側キャラを選択
				foreach (KeyValuePair<int, PieceEntity> pair in board_.Invader) {
					if (FitGUILayout.Button(pair.Value.data.name)) {
						//キャラが居るnodeIdを退避
						selectNode_ = pair.Key;
						//キャラをフォーカスする
						StrategyMessage.LookAtCameraOn.Broadcast(pair.Value);
						//キャラがいるノードから次のノードに進む経路を取得
						next_ = map.StartWay(selectNode_);
					}
				}
			} else {
				CaptionLabel.Draw(board_.Invader[selectNode_].data.name);
				if (next_ != null) {
					foreach (KeyValuePair<int, Geopolitics.Way> pair in next_) {
						if (FitGUILayout.Button("node" + pair.Key.ToString() + "へ移動")) {
							StrategyMessage.MovePiece.Broadcast(board_, pair.Value);
							selectNode_ = -1;
							Hide();
						}
					}
				}
				if (FitGUILayout.Button("戻る")) {
					selectNode_ = -1;
				}
			}
			if (FitGUILayout.Button("タウンへ戻る")) {
				SceneTransition.ChangeTown();
			}
		}

	}
#endif
}
