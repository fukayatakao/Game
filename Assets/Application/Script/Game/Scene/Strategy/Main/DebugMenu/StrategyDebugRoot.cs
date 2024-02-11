using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
    public static class StrategyDebugRoot {
		static DebugMenu menu =
			DebugMenu.Item("広域マップ",
				DebugMenu.Item("生成", () => { StrategyMessage.GenerateMap.Broadcast(); })
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
