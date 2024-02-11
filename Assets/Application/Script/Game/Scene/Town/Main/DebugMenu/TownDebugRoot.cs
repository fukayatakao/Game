using Project.Lib;
using Project.Server;

namespace Project.Game {
#if DEVELOP_BUILD
    public static class TownDebugRoot {
		static DebugMenu menu =
			DebugMenu.Item("タウン",
				DebugMenu.Item("チェイン情報", typeof(FacilityChainDebug)),
				DebugMenu.Item("時間経過テスト", () =>
				{
					TownTool.Calculate();
					TownMessage.SetGold.Broadcast(UserDB.Instance.townhallTable.gold);
				}),
				DebugMenu.Item("テスト実行", TownTool.Calculate)

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
