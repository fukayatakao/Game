using UnityEngine;
using Project.Lib;


namespace Project.Game {
	public static class CharacterTask {
		/// <summary>
		/// キャラ交換
		/// </summary>
		public static void BuildChangeSwitch(CharacterEntity entity, Mst.MstUnitData data) {
			CoroutineTaskList list = entity.TaskList;
			list.Clear();
			list.Add(CharacterCommand.PlayEffect(entity, "Respawn", Vector3.zero));
			list.Add(CommonCommand.WaitFrame(60));
			list.Add(CharacterCommand.ChangeModel(entity, data));

		}

		/// <summary>
		/// キャラ交換
		/// </summary>
		public static void BuildChangeEntry(CharacterEntity entity) {
			CoroutineTaskList list = entity.TaskList;
			list.Clear();
			Vector3 startPos = entity.GetPosition();
			startPos.y += 10f;
			entity.SetPosition(startPos);



			list.Add(CharacterCommand.SetEnableAI(entity, false));
			list.Add(CommonCommand.WaitFrame(2));      //@todo stateの即時変更機能を入れたら直す
			list.Add(CharacterCommand.ChangeState(entity, CharacterState.State.None));
			list.Add(CommonCommand.WaitFrame(2));      //@todo stateの即時変更機能を入れたら直す


			list.Add(CharacterCommand.PlayAnimation(entity, BattleMotion.Fall, 0f));
			list.Add(CommonCommand.WaitFrame(Random.Range(0, 30)));
			list.Add(CommonCommand.Fall(entity, 0f));

			list.Add(CharacterCommand.PlayAnimationWithWait(entity, BattleMotion.Land));
			list.Add(CharacterCommand.PlayAnimationWithWait(entity, BattleMotion.Salute));

			list.Add(CharacterCommand.ChangeState(entity, CharacterState.State.Idle));
			list.Add(CharacterCommand.SetEnableAI(entity, true));
		}
	}
}
