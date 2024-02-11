using UnityEngine;
using Project.Lib;


namespace Project.Game {
	/// <summary>
	/// メイン処理
	/// </summary>
	public class BattleTimelineEvent : MonoBehaviour {
		[SerializeField]
		private BattleMain owner_ = null;
		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public void Awake() {
			//BattleMainのインスタンスが登録されてなかったらエラー
			Debug.Assert(owner_ != null, "not found BattleMain");
		}
		/// <summary>
		/// タイムラインから呼ばれる
		/// </summary>
		public void func() {
			Vector3 offset = new Vector3(0f, 0f, -3f);
			for (int i = 0; i < owner_.Platoon[(int)Power.Player].Squads.Count; i++) {
				SquadEntity group = owner_.Platoon[(int)Power.Player].Squads[i];
				for (int j = 0; j < group.Members.Count; j++) {
					CharacterEntity entity = group.Members[j];
					if (!group.Members[j].IsLeader) {
						entity.TaskList.Clear();
						entity.TaskList.Add(CommonCommand.Wait(DeterminateRandom.Range(0f, 2f)));
						entity.TaskList.Add(CommonCommand.SetPosition(entity, entity.GetPosition() + offset));
						entity.TaskList.Add(CommonCommand.SetActive(entity, true));
						entity.TaskList.Add(CharacterCommand.PlayAction(entity, BattleAction.OpeningEntry));
					}
				}
			}

		}



	}
}
