using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class ChangeUnitMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.31f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;


		int abreast_ = 0;
		int assignUnitId_ = 0;
		private List<int> unitId_ = new List<int>();
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ChangeUnitMenu() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(int abreast, int unitId) {
			abreast_ = abreast;
			assignUnitId_ = unitId;
			//リーダー用ユニット
			var leaderUnitId = Mst.MstLeaderData.GetList().Select(x => x.UnitId).ToList();
			//ユニット全体からリーダー用ユニットをはずす
			unitId_ = Mst.MstUnitData.GetList().Where(x => x.Id != unitId).Select(x => x.Id).Except(leaderUnitId).ToList();
		}
		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);

			Dictionary<int, Mst.MstUnitData> mstUnit = Mst.BaseDataManager.GetDictionary<int, Mst.MstUnitData>();
			foreach(int id in unitId_) {
				if (FitGUILayout.Button(mstUnit[id].DbgName)) {
					//誰も配備されてない場合は新規追加
					if (assignUnitId_ == 0) {
						OrganizeMessage.AddSquadUnit.Broadcast(abreast_, id);
					//列の兵科を変更
					} else {
						OrganizeMessage.ChangeSquadUnit.Broadcast(abreast_, id);
					}
					Hide();
				}
			}
			if (FitGUILayout.Button("削除")) {
				//列の兵科を変更
				OrganizeMessage.RemoveSquadUnit.Broadcast(abreast_, 0);
				Hide();

			}
		}
	}
#endif
}
