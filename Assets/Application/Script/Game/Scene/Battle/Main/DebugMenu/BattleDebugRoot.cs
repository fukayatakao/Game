using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
    public static class BattleDebugRoot {
	    private static DebugMenu menu =
		    DebugMenu.Item("バトル",
			    DebugMenu.Item("敵編成", typeof(EnemyChangeDebug)),
			    DebugMenu.Item("分隊デバッグ", typeof(SquadDebug)),
			    DebugMenu.Item("バトル計算", typeof(CalculateDebug)),
			    DebugMenu.Item("攻撃範囲", typeof(AttackAreaDebug)),

				DebugMenu.Item("SP最大", () => { BattleMessage.SPMax.Broadcast(Power.Player); })

			);

		public static void Add() {
			DebugRootMenu.I.Add(menu);
		}
		public static void Remove() {
			DebugRootMenu.I.Remove(menu);
		}

	}
#endif
}
