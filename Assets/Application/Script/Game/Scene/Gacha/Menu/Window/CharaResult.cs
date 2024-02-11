using Project.Lib;
using UnityEngine;
using Project.Network;
using Project.Http.Mst;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CharaStatus : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.15f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;
		MessageSystem.Receptor receptor_;
		CharacterData character_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharaStatus() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.UserEvent);

			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(CharacterData chara) {
			character_ = chara;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			if (character_ == null)
				return;
			CloseButton.Draw(this);
			FitGUILayout.Label(character_.name);
			FitGUILayout.Label("", 36, 36);
			GUIDrawSprite(new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().width), (PHASE)character_.phase);

			FitGUILayout.Label("ability:");
			for (int i = 0, max = character_.ability.Count; i < max; i++) {
				int id = character_.ability[i];
				FitGUILayout.Label(BaseDataManager.GetDictionary<int, MstAbilityData>()[id].DbgName);
			}

		}

		/// <summary>
		/// 属性テクスチャ表示
		/// </summary>
		public void GUIDrawSprite(Rect rect, PHASE phase) {
			GUI.DrawTexture(rect, ResourcePool.GetPhaseIcon(phase));
		}
	}
#endif
}
