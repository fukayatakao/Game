using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Network;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CharaMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly Vector2 expandSize = new Vector2(0.3f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;


		MessageSystem.Receptor receptor_;

		CharacterEntity actor_;
		CharacterEntity target_;

		bool expand_ = false;
		Vector2 scroll_;

		private string[] abreast_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharaMenu() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.UserEvent);

			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
			abreast_ = FieldAttribute.GetFields(typeof(Abreast));
		}

		public void Init(CharacterEntity actor, CharacterEntity target) {
			//変化がない場合はサイズ変えさせないためにも設定せず終了
			if (actor_ == actor && target_ == target)
				return;

			actor_ = actor;
			target_ = target;
			Vector2 s = size;
			if (target_ != null) {
				s.x = s.x * 2f;
			}
			base.Init(FitCommon.CalcRect(mergin, s, align));
			expand_ = false;
			SetAutoResize();
		}

		/// <summary>
		/// 選択したキャラの情報を表示
		/// </summary>
		private void DrawCharacter(CharacterEntity character) {
			FitGUILayout.Label("", 200, 200);
			GUI.DrawTexture(new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().width), character.HavePersonal.LoadPortraitTexture());
			using (new GUILayout.VerticalScope()) {
				FitGUILayout.Label(abreast_[(int)character.Squad.Index]);
				FitGUILayout.Label("名前：" + character.HavePersonal.CharaName);

				List<int> ability = character.HavePersonal.Ability;
				for (int i = 0, max = ability.Count; i < max; i++) {
					string str = Mst.BaseDataManager.GetDictionary<int, Mst.MstAbilityData>()[ability[i]].DbgName;
					if (i == 0) {
						FitGUILayout.Label("アビリティ：" + str);
					} else {
						FitGUILayout.Label(str);
					}
				}
			}
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			if (actor_ == null) {
				//何かは描かないとサイズ計算でエラーになるので対策
				FitGUILayout.Label("");
				return;
			}
			using (new GUILayout.HorizontalScope()) {
				DrawCharacter(actor_);
				if (target_ != null) {
					using (new GUILayout.VerticalScope()) {
						if(actor_.HaveUnitMaster.Species != target_.HaveUnitMaster.Species) {
							FitGUILayout.Label("x");
						} else {
							FitGUILayout.Label("<=>");
						}
					}
					DrawCharacter(target_);
				}
			}

			if (target_ == null) {
				if (expand_) {
					if (FitGUILayout.Button("交代リストを隠す")) {
						SetAutoResize();
						base.Init(FitCommon.CalcRect(mergin, size, align));
						expand_ = false;
					}
				} else {
					if (FitGUILayout.Button("交代リストを表示")) {
						SetAutoResize(-1);
						base.Init(FitCommon.CalcRect(mergin, expandSize, align));
						expand_ = true;
					}
				}

				if (expand_) {
					scroll_ = FitGUILayout.BeginScrollView(scroll_);

					for (int i = 0; i < OrganizeSituationData.I.members.Count; i++) {
						CharacterData chara = OrganizeSituationData.I.members[i];
						//ユニットに対応した種族以外は非表示
						if (chara.species != actor_.HaveUnitMaster.Species)
							continue;
						using (new GUILayout.HorizontalScope()) {
							if (FitGUILayout.Button("選択", 80, 0)) {
								OrganizeMessage.ChangePersonal.Broadcast(actor_, chara);
							}
							FitGUILayout.Label(chara.name);
						}
					}
					FitGUILayout.EndScrollView();
				}
			}


		}
	}
#endif
}
