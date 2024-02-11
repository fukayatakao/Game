using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class EnemyChangeDebug : DebugWindow {
		Vector2 scrollPos_ = new Vector2();
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.4f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		EditPlatoon editPlatoon_;
		int[] unitIndex_;
		int leaderIndex_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EnemyChangeDebug() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			//デバッグ編成がない場合
			if(!EditPlatoon.IsExist()) {
				editPlatoon_ = new EditPlatoon(new List<int>() { 1, 1, 1 }, 1);
			} else {
				editPlatoon_ = EditPlatoon.Load();
			}
			unitIndex_ = new int[(int)Abreast.MAX];
			for(int i = 0, max = unitIndex_.Length; i < max; i++) {
				int id = editPlatoon_.unit[i];
				unitIndex_[i] = BaseDataManager.GetList<MstEnemyMemberParam>().FindIndex(x => x.Id == id);
			}
			leaderIndex_ = BaseDataManager.GetList<MstEnemyMemberParam>().FindIndex(x => x.Id == editPlatoon_.leader);
		}


		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			CaptionLabel.Draw("編成");
			scrollPos_ = FitGUILayout.BeginScrollView(scrollPos_);

			for(int i = 0; i < editPlatoon_.unit.Count; i++) {
				DrawEdit(i);
			}
			DrawEditLeader();
			if (FitGUILayout.Button("変更")) {
				EditPlatoon.Save(editPlatoon_);
				SceneTransition.ChangeBattle();
			}

			FitGUILayout.EndScrollView();
		}

		private void DrawEdit(int index) {
			List<MstEnemyMemberParam> list = BaseDataManager.GetList<MstEnemyMemberParam>();
			int id = unitIndex_[index];

			using (new GUILayout.HorizontalScope()) {
				string txt = list[id].Id + "(" + id.ToString() + ")";
				FitGUILayout.Label((index + 1) + "列目");
				if (FitGUILayout.Button("<<")) {
					id -= 10;
				}
				if (FitGUILayout.Button("<")) {
					id -= 1;
				}
				FitGUILayout.Label(txt);
				if (FitGUILayout.Button(">")) {
					id  += 1;
				}
				if (FitGUILayout.Button(">>")) {
					id  += 10;
				}
			}
			id = Mathf.Clamp(id, 0, list.Count - 1);
			editPlatoon_.unit[index] = list[id].Id;
			FitGUILayout.Label(list[id].DbgName);
			unitIndex_[index] = id;
		}

		private void DrawEditLeader() {
			List<MstEnemyLeaderParam> list = BaseDataManager.GetList<MstEnemyLeaderParam>();
			int id = leaderIndex_;

			using (new GUILayout.HorizontalScope()) {
				string txt = list[id].Id + "(" + id.ToString() + ")";
				FitGUILayout.Label("リーダー");
				if (FitGUILayout.Button("<<")) {
					id -= 10;
				}
				if (FitGUILayout.Button("<")) {
					id -= 1;
				}
				FitGUILayout.Label(txt);
				if (FitGUILayout.Button(">")) {
					id += 1;
				}
				if (FitGUILayout.Button(">>")) {
					id += 10;
				}
			}
			id = Mathf.Clamp(id, 0, list.Count - 1);
			editPlatoon_.leader = list[id].Id;
			FitGUILayout.Label(list[id].DbgName);
			leaderIndex_ = id;
		}
	}
#endif
}
