using Project.Http.Mst;
using UnityEngine;
using Project.Lib;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CharacterInfo : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.35f, 0.4f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerLeft;

		public static CharacterSelectControl CharacterSelect;


		CharacterEntity target_;

		enum MenuMode : int {
			Status1,
			Status2,
			Debug,
		};
		MenuMode mode_ = MenuMode.Status1;

		/// <summary>
		/// キャラ操作
		/// </summary>
		public class CharacterSelectControl : IHaveControl {
			//カメラ
			CameraEntity camera_;
			private CharacterInfo info_;

			// 操作プライオリティ
			public int Priority { get { return (int)OperationPriority.DebugSelectControl; } }
			/// <summary>
			/// レイを飛ばすカメラとUIをセットアップ
			/// </summary>
			public CharacterSelectControl(CameraEntity cam) {
				camera_ = cam;
				//自分を制御振り分け機能に登録要求
				SystemMessage.RegisterControl.Broadcast(this);

				info_ = BattleAlternativeMenu.I.OpenCharacterInfo();
				info_.Hide();
			}
			/// <summary>
			/// 制御開始
			/// </summary>
			public bool Interrupt() {
				if (!Gesture.IsTouchDown(0))
					return false;

				//キャラクターを選択したら操作開始
				RaycastHit hit;
				bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), camera_, 1 << (int)UnityLayer.Layer.Character, out hit);
				if (!result) {
					//instance_.Hide();
					return false;
				}
				info_.Show();
				info_.target_ = hit.collider.GetComponent<CharacterPortal>().Owner;
				//操作を取得する必要はないのでスルー
				return false;
			}

			/// <summary>
			/// 制御開始
			/// </summary>
			public void Begin() {
			}
			/// <summary>
			/// 制御終了
			/// </summary>
			public bool IsEnd() {
				return true;
			}
			/// <summary>
			/// 制御却下
			/// </summary>
			public void Reject() {
			}

			/// <summary>
			/// 実行処理
			/// </summary>
			public void Execute() {
			}
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharacterInfo() {
            base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();


			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			if (target_ == null) {
				//何も表示しないとエラーが出てしまうので空文字を表示
				FitGUILayout.Label("");
				Hide();
				return;
			}
            CloseButton.Draw(this);
			Debug.Assert(target_ != null, "not found CharacterInfo Target");

			//表示切り替えタブ
			mode_ = (MenuMode)FitGUILayout.SelectionGrid((int)mode_, new string[] { "ステータス", "スキル", "デバッグ"}, 3);

			using (var h = new GUILayout.HorizontalScope()) {
				DrawCharacterPortrait();
				if (target_.IsLeader) {
					switch (mode_) {
						case MenuMode.Status1: DrawCharacterStatus(); break;
						case MenuMode.Status2: DrawSkill(); break;
						case MenuMode.Debug: DrawDebugCommand(); break;
					}
				} else {
					switch (mode_) {
						case MenuMode.Status1: DrawCharacterStatus(); break;
						case MenuMode.Status2: DrawAbility(); break;
						case MenuMode.Debug: DrawDebugCommand(); break;
					}
				}
			}
		}

		/// <summary>
		/// キャラクターのサムネイル画像表示
		/// </summary>
		private void DrawCharacterPortrait() {
			//キャラ名とサムネイル
			using (var v = new GUILayout.VerticalScope()) {
				FitGUILayout.Label(target_.HavePersonal.CharaName, 300, 0);
				FitGUILayout.Label("", 300, 300);
				DrawTextureLastRect(target_.HavePersonal.LoadPortraitTexture());
			}
		}

		/// <summary>
		/// パラメータ表示
		/// </summary>
		private void DrawCharacterStatus() {
			using (var v = new GUILayout.VerticalScope()) {
				FitGUILayout.Label("種別：" + target_.HaveUnitMaster.DbgName);
				using (var h = new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("属性：", 120, 0);
					FitGUILayout.Label("", 36, 36);
					DrawTextureLastRect(target_.HavePersonal.LoadPhaseIcon());
				}

				FitGUILayout.Label("ＨＰ：" + target_.HaveUnitParam.Fight.Hp);
				FitGUILayout.Label("ＬＰ：" + target_.HaveUnitParam.Fight.Lp);
				FitGUILayout.Label("攻撃：" + target_.HaveUnitParam.Fight.Attack);
				FitGUILayout.Label("防御：" + target_.HaveUnitParam.Fight.Defence);
			}
		}
		/// <summary>
		/// アビリティ表示
		/// </summary>
		private void DrawAbility() {
			using (var v = new GUILayout.VerticalScope()) {
				FitGUILayout.Label("ability:");
				for (int i = 0, max = target_.HavePersonal.Ability.Count; i < max; i++) {
					int id = target_.HavePersonal.Ability[i];
					FitGUILayout.Label(BaseDataManager.GetDictionary<int, MstAbilityData>()[id].DbgName);
				}
			}
		}
		/// <summary>
		/// スキル表示
		/// </summary>
		private void DrawSkill() {
			using (var v = new GUILayout.VerticalScope()) {
				FitGUILayout.Label("skill:");

				if (target_.Platoon.HaveSpecialPoint.Point < GameConst.Battle.SPECIAL_POINT_CONSUME) {
					GUI.enabled = false;
				}
				for (int i = 0, max = target_.HaveSkill.Skill.Count; i < max; i++) {
					MstSkillData skill = target_.HaveSkill.Skill[i];
					if (FitGUILayout.Button(skill.DbgName)) {
						target_.PlaySkill(skill.ActionName);
					}
				}
				GUI.enabled = true;
			}
		}
		/// <summary>
		/// デバッグ
		/// </summary>
		private void DrawDebugCommand() {
			using (var v = new GUILayout.VerticalScope()) {
				using (var h = new GUILayout.HorizontalScope()) {
					if (FitGUILayout.Button("Dead")) {
						target_.HaveUnitParam.Fight.Lp = 0f;
						target_.Dead();
					}
					if (FitGUILayout.Button("Escape")) {
						target_.HaveUnitParam.Fight.Lp = 0f;
						target_.Escape();
					}
				}

				{
					//索敵範囲円の表示
					bool flag = ButtonSwitch.Draw(target_.HaveDebug.SearchArea.IsVisible(), "索敵範囲");
					target_.HaveDebug.SearchArea.SetVisible(flag);
				}
				{
					//AIの有効・無効
					GUIUtil.BeginChangeCheck();
					target_.HaveThink.Enable = !ButtonSwitch.Draw(!target_.HaveThink.Enable, "AI停止");
					if (GUIUtil.EndChangeCheck()) {
						BattleMessage.CharacterAI.Broadcast(target_, !target_.HaveThink.Enable);
					}
				}
				//ユニットの攻撃範囲表示
				using (var h = new GUILayout.HorizontalScope()) {
					FitGUILayout.Label("攻撃範囲");
					string[] cap = new string[] { "A", "B", "C", "D" };
					for (int i = 0, max = (int)ACTION_PATTERN.MAX; i < max; i++) {
						bool flag = ButtonSwitch.Draw(target_.HaveDebug.AttackArea[i].IsVisible(), cap[i], cap[i]);
						target_.HaveDebug.AttackArea[i].SetVisible(flag);
					}
				}
			}
		}
		/// <summary>
		/// 属性テクスチャ表示
		/// </summary>
		public void DrawTextureLastRect(Texture2D tex) {
			GUI.DrawTexture(new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().width), tex);
		}
		/// <summary>
		/// 表示しているキャラが戦闘不能になったのでウィンドウを閉じる
		/// </summary>
		public void Defeat(CharacterEntity entity) {
			if (target_ != entity)
				return;
			Hide();
		}
	}
#endif
}
