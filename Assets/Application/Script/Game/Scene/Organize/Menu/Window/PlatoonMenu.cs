using System.Collections.Generic;
using UnityEngine;
using Project.Lib;
using Project.Network;

namespace Project.Game {
#if DEVELOP_BUILD
	public class PlatoonMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		static Texture2D gauge_ = TextureUtil.CreateColorTexture(new Color(0.1f, 0.9f, 0.1f));
		MessageSystem.Receptor receptor_;

		private Rect experienceGauge_;
		private int selectPlatoon_;
		private int selectAbreast_;
		OrganizeMain owner_;
		ChangeUnitMenu changeUnit_;
		ChangeLeaderMenu changeLeader_;
		private readonly string[] abreastLabel_;

		private const int MAX_CHARACTER_LIST = 8;
		private int memberIndex_;
		private int leaderIndex_;

		enum State
		{
			Platoon,
			Character,
		}

		private State state_;
		private int drawAssign_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PlatoonMenu() {
			//メッセージを受け取れるように登録
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();

			var field = FieldAttribute.GetFields(typeof(Abreast));
			abreastLabel_ = new string[field.Length - 1];
			for (int i = 0, max = field.Length - 1; i < max; i++) {
				abreastLabel_[i] = field[i];
			}
		}

		public void Init(OrganizeMain owner) {
			owner_ = owner;
			selectAbreast_ = (int)Abreast.First;
			selectPlatoon_ = OrganizeSituationData.I.SelectPlatoon;

			changeUnit_ = DebugWindowManager.Open<ChangeUnitMenu>();
			changeUnit_.Hide();

			changeLeader_ = DebugWindowManager.Open<ChangeLeaderMenu>();
			changeLeader_.Hide();

			state_ = State.Platoon;
			memberIndex_ = 0;
			leaderIndex_ = 0;
		}

		public new void Show() {
			base.Show();
		}

		public new void Hide() {
			base.Hide();
			changeUnit_.Hide();
			changeLeader_.Hide();
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			PlatoonEntity platoon = owner_.Platoon;
			GUIUtil.BeginChangeCheck();
			selectPlatoon_ = FitGUILayout.SelectionGrid(selectPlatoon_, new string[] { "Team A", "Team B", "Team C", "Team D" }, 4);
			if (GUIUtil.EndChangeCheck()) {
				OrganizeMessage.ChangePlatoon.Broadcast(selectPlatoon_);
			}

			//切り替え中などはここで終了
			if (!platoon.IsActive())
				return;

			FitGUILayout.Label("編成情報");
			platoon.PlatoonName = FitGUILayout.TextField(platoon.PlatoonName);

			FitGUILayout.Label("熟練度");
			FitGUILayout.Label("");
			if (Event.current.type == EventType.Repaint) {
				experienceGauge_ = GUILayoutUtility.GetLastRect();
			}

			float scale = (float)platoon.Experience / GameConst.Battle.EXPERIENCE_SCALE;
			GUI.DrawTexture(new Rect(experienceGauge_.x, experienceGauge_.y, experienceGauge_.width * scale, experienceGauge_.height), gauge_);
			if (state_ == State.Platoon) {
				DrawPlatoonEdit(platoon);
			} else {
				DrawCharacter();
			}
			if (GUILayout.Button("切り替え")) {
				state_ = state_ == State.Platoon ? State.Character : State.Platoon;
			}

		}

		/// <summary>
		/// キャラクター個別の配備状況を表示
		/// </summary>
		private void DrawCharacter() {
			//drawAssign_ = FitGUILayout.SelectionGrid(drawAssign_, new string[] { "メンバー", "リーダー" }, 2);

			if (drawAssign_ == 0) {
				List<CharacterData> list = new List<CharacterData>();
				//部隊に配備されているメンバー取得
				foreach (PlatoonData platoon in OrganizeSituationData.I.platoons) {
					foreach (SquadData squad in platoon.squads) {
						foreach (CharacterData member in squad.members) {
							//無名キャラは無視
							if (member == null)
								continue;
							list.Add(member);
						}
					}
				}

				//未配備のメンバー取得
				foreach (CharacterData member in OrganizeSituationData.I.members) {
					using (new GUILayout.HorizontalScope()) {
						list.Add(member);
					}
				}
				int allocMember = list.Count - OrganizeSituationData.I.members.Count;
				using (new GUILayout.HorizontalScope()) {
					if (GUILayout.Button("↑")) { memberIndex_--; }
					if (GUILayout.Button("↓")) { memberIndex_++; }

					if (memberIndex_ > list.Count - MAX_CHARACTER_LIST)
						memberIndex_ = list.Count - MAX_CHARACTER_LIST;
					if (memberIndex_ < 0)
						memberIndex_ = 0;
				}
				int max = memberIndex_ + MAX_CHARACTER_LIST;
				if (max >= list.Count) {
					max = list.Count;
				}
				for (int i = memberIndex_; i < max; i++) {
					using (new GUILayout.HorizontalScope()) {
						GUI.color = i < allocMember ? Color.red : Color.cyan;
						GUILayout.Label(i < allocMember ? "配備済" : "未配備");
						GUILayout.Label(list[i].name);
					}
				}
				GUI.color = Color.white;
			} else {

			}

		}

		/// <summary>
		/// 小隊ユニットの編成UI
		/// </summary>
		private void DrawPlatoonEdit(PlatoonEntity platoon) {
			SelectAbreast(platoon);
			if (platoon.Squads.Count > selectAbreast_) {
				SquadEntity squad = platoon.Squads[selectAbreast_];
				using (new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("ユニット");
					if(squad.Members != null && squad.UnitData != null) {
						if (FitGUILayout.Button(squad.UnitData.DbgName)) {
							changeUnit_.Init(selectAbreast_, squad.UnitData.Id);
							changeUnit_.Show();
						}
					} else {
						if (FitGUILayout.Button("追加")) {
							changeUnit_.Init(selectAbreast_, 0);
							changeUnit_.Show();
						}
					}
				}

				using (new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("リーダー");
					if (squad.Leader != null) {
						if (FitGUILayout.Button(squad.Leader.HaveUnitMaster.DbgName)) {
							changeLeader_.Init(selectAbreast_, squad.Leader.Id);
							changeLeader_.Show();
						}
					} else {
						if (FitGUILayout.Button("追加")) {
							changeLeader_.Init(selectAbreast_, 0);
							changeLeader_.Show();
						}
					}
				}
			} else {
				using (new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("ユニット");
					if (FitGUILayout.Button("新規追加")) {
						changeUnit_.Init(selectAbreast_, 0);
						changeUnit_.Show();
					}
				}

				using (new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("リーダー");
					if (FitGUILayout.Button("新規追加")) {
						changeLeader_.Init(selectAbreast_, 0);
						changeLeader_.Show();
					}
				}
			}

		}


		/// <summary>
		/// 列選択の切り替え
		/// </summary>
		private void SelectAbreast(PlatoonEntity platoon) {
			using (new GUILayout.HorizontalScope()) {
				for (int i = 0, max = abreastLabel_.Length; i < max; i++) {
					//分隊数が足りない場合は選択不可
					if (i > platoon.Squads.Count) {
						GUI.enabled = false;
					}

					if (i == platoon.Squads.Count) {
						GUI.color = Color.green;
					}
					if (FitGUILayout.Button(abreastLabel_[i])) {
						selectAbreast_ = i;
						changeUnit_.Hide();
						changeLeader_.Hide();
					}
				}
			}
			GUI.color = Color.white;
			GUI.enabled = true;
		}

	}
#endif
}
