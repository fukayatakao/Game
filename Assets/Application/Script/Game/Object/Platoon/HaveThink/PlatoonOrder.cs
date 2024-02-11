using Project.Lib;

namespace Project.Game {
	[UnityEngine.Scripting.Preserve]
	public static class PlatoonOrder {
        [Function("何もしない")]
        public static void Empty(PlatoonEntity entity, object[] args) {
        }
        [Function("AI状態変更")]
        [Arg(0, typeof(PlatoonThink.State), "ステート", PlatoonThink.State.Root)]
		public static void ChangeAIState(PlatoonEntity entity, object[] args) {
			PlatoonThink.State state = (PlatoonThink.State)args[0];
            entity.HaveThink.ChangeState(entity, state);
        }

		[Function("前部の交代を指示")]
		public static void OrderForeSwap(PlatoonEntity entity, object[] args) {
			BattleMessage.SquadForeSwap.Broadcast(entity.Index);
			//入れ替え終わり時間を記録
			entity.HaveBlackboard.SwapExecTime = Time.time;
		}

		[Function("後部の交代を指示")]
		public static void OrderAftSwap(PlatoonEntity entity, object[] args) {
			BattleMessage.SquadAftSwap.Broadcast(entity.Index);
			//入れ替え終わり時間を記録
			entity.HaveBlackboard.SwapExecTime = Time.time;
		}

		[Function("ローテーション交代を指示")]
		public static void OrderRotation(PlatoonEntity entity, object[] args) {
			BattleMessage.SquadRotation.Broadcast(entity.Index);
			//入れ替え終わり時間を記録
			entity.HaveBlackboard.SwapExecTime = Time.time;
		}

	}
}
