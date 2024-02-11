using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Project.Lib;
using Project.Network;

namespace Project.Game {
#if DEVELOP_BUILD
	public class ChangeLeaderMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.31f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;


		int abreast_ = 0;
		int assignLeader_ = 0;
		private List<LeaderData> selectable_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ChangeLeaderMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		/// <summary>
		/// 列番号と現在アサインされているリーダーのIDを受け取って表示用データを作る
		/// </summary>
		public void Init(int abreast, int assignLeaderId) {
			abreast_ = abreast;
			assignLeader_ = assignLeaderId;
			selectable_ = OrganizeSituationData.I.leaders.Where(x => x.id != assignLeaderId).ToList();
		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			foreach(LeaderData leader in selectable_) {
				if (FitGUILayout.Button(Mst.MstLeaderData.GetData(leader.leaderMasterId).UnitData.DbgName)) {
					//誰も配備されてない場合は新規追加
					if(assignLeader_ == 0) {
						OrganizeMessage.AddSquadLeader.Broadcast(abreast_, leader);
					//列のリーダーを変更
					} else {
						OrganizeMessage.ChangeSquadLeader.Broadcast(abreast_, leader);
					}

					Hide();
				}
			}

			if (FitGUILayout.Button("削除")) {
				OrganizeMessage.RemoveSquadLeader.Broadcast(abreast_);
				Hide();
			}
		}
	}
#endif
}
