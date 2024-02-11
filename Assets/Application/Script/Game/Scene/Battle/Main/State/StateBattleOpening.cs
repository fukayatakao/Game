using UnityEngine;
using UnityEngine.Playables;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 初期化ステート
	/// </summary>
	public class StateBattleOpening : IState<BattleMain> {
#if DEVELOP_BUILD
		OpeningSkipMenu menu_;
#endif
		PlayableDirector director_;

		/// <summary>
		/// 開始
		/// </summary>
		public override void Enter(BattleMain owner) {
#if DEVELOP_BUILD
			menu_ = DebugWindowManager.Open<OpeningSkipMenu>();
#endif
			owner.MainCamera.ChangeControlView(true);


			GameObject obj = new GameObject("timeline");

			director_ = obj.AddComponent<PlayableDirector>();
			var res = Resources.Load("Opening");
			PlayableAsset asset = Resources.Load("Opening") as PlayableAsset;

			//@todo leader
			CharacterEntity leader = owner.Platoon[(int)Power.Player].Squads[^1].Leader;
			for (int i = 0; i < owner.Platoon[(int)Power.Player].Squads.Count; i++) {
				SquadEntity group = owner.Platoon[(int)Power.Player].Squads[i];
				for (int j = 0; j < group.Members.Count; j++) {
					CharacterEntity entity = group.Members[j];
					entity.SetAlpha(0f);
					entity.ChangeState(CharacterState.State.None, true);
				}
			}

			foreach (var b in asset.outputs) {
				switch (b.streamName) {
				case "Walk":
					director_.SetGenericBinding(b.sourceObject, leader.HaveModel.Model);
					break;
				case "Move":
					director_.SetGenericBinding(b.sourceObject, leader.HaveModel.Model);
					break;
				case "ManualCamera":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[0].gameObject);
					break;
				case "VirtualCamera1":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[1].gameObject);
					break;
				case "VirtualCamera2":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[2].gameObject);
					break;
				case "VirtualCamera3":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[3].gameObject);
					break;
				case "VirtualCamera4":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[4].gameObject);
					break;
				case "CameraRoot":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.gameObject);
					break;
				case "LastCamera":
					director_.SetGenericBinding(b.sourceObject, owner.MainCamera.VirtualCamera[4].gameObject);
					break;
				case "Signal":
					director_.SetGenericBinding(b.sourceObject, owner.gameObject);
					break;
				}
			}

			director_.Play(asset);
			director_.stopped += EndEvent;

			owner.MainCamera.VirtualCamera[1].LookAt = leader.HaveModel.Model.transform;
			owner.MainCamera.VirtualCamera[2].LookAt = leader.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head);
			owner.MainCamera.VirtualCamera[3].Follow = leader.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head);
			owner.MainCamera.VirtualCamera[4].Follow = leader.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head);

			owner.MainCamera.ChangeVirtualCamera(1);

		}
		bool end = false;
		private void EndEvent(PlayableDirector dir) {
			end = true;
		}

		float t = 0f;
		/// <summary>
		/// 更新
		/// </summary>
		public override void Execute(BattleMain owner) {
			t = t + Time.deltaTime;
			if(t > 7f) {
				//func(owner);
				t = float.MinValue;
			}

			for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterEntity entity = CharacterAssembly.I.Current[i];
				if (entity.HaveAction.IsPlay())
					return;
			}

			if (end) {
				owner.ChangeState(BattleMain.State.Ready);
			}
		}
		/// <summary>
		/// 終了
		/// </summary>
		public override void Exit(BattleMain owner) {
			director_.Stop();
			for (int i = 0, max = CharacterAssembly.I.Count; i < max; i++) {
				CharacterAssembly.I.Current[i].ChangeState(CharacterState.State.Idle);
			}
			//Timelineで勝手にカメラonにされるので全部強制的にoffにしてからカメラ切替を行う
			for (int i = 0; i < owner.MainCamera.VirtualCamera.Count; i++) {
				owner.MainCamera.VirtualCamera[i].gameObject.SetActive(false);
			}
			owner.MainCamera.SetPosition(Vector3.zero);
			owner.MainCamera.ChangeVirtualCamera(0);
			owner.Platoon[(int)Power.Player].Squads[^1].Leader.HaveModel.Model.transform.localPosition = Vector3.zero;
			owner.Platoon[(int)Power.Player].InitLocation(owner.StageField.CenterDepth, owner.StageField.TerritoryDepth);
			//キャラが非表示になっているのでonにする
			for (int i = 0; i < owner.Platoon[(int)Power.Player].Squads.Count; i++) {
				SquadEntity group = owner.Platoon[(int)Power.Player].Squads[i];
				for (int j = 0; j < group.Members.Count; j++) {
					if (!group.Members[j].IsLeader) {
						group.Members[j].SetAlpha(1f);
						group.Members[j].SetActive(true);
						group.Members[j].HaveAction.Stop();
						group.Members[j].TaskList.Clear();
						group.Members[j].ParallelTask.Clear();
					}
				}
			}
#if DEVELOP_BUILD
			menu_.Hide();
#endif
		}


	}

}
