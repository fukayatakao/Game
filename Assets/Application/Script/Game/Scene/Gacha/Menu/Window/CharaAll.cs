using Project.Lib;
using UnityEngine;
using Project.Network;
using Project.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CharaAll : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.25f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;
		int tab_;

		SelectMenuUtil<CharacterData> charaSelect_ = new Game.SelectMenuUtil<CharacterData>();
		SelectMenuUtil<LeaderData> leaderSelect_ = new Game.SelectMenuUtil<LeaderData>();
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharaAll() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init() {
			charaSelect_.Init(GachaSituationData.I.Characters, 8, (data) => { return data.name; }, (data) => { GachaMessage.ShowCharacter.Broadcast(data); });
			leaderSelect_.Init(GachaSituationData.I.Leaders, 8, (data) => { return MstUnitData.GetData(data.leaderMasterId).DbgName; }, (data) => { GachaMessage.ShowLeader.Broadcast(data); });
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			tab_ = FitGUILayout.SelectionGrid(tab_, new string[] { "キャラ", "リーダー" }, 2);
			if(tab_ == 0) {
				charaSelect_.Draw();
			} else {
				leaderSelect_.Draw();
			}
		}

	}
#endif
}
