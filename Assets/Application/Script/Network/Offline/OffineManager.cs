using Project.Server;



namespace Project.Network {
#if USE_OFFLINE
	public static partial class OfflineManager {
		/// <summary>
		/// オフライン時の関数を登録
		/// </summary>
		public static void Initialize() {
			GetUrlCmd.OfflineFunc = (req) =>{ return new GetUrlResponse(); };
			LoginCmd.OfflineFunc = (req) => { return new LoginResponse(); };
			RegistAccountCmd.OfflineFunc = (req) => { return new RegistAccountResponse(); };

			BaseDataCmd.OfflineFunc = Project.Mst.BaseDataManager.BaseDataOffline;

			TownMainCmd.OfflineFunc = DummyServer.TownMainOffline;
			//建物を建てる
			BuildFactoryCmd.OfflineFunc = DummyServer.BuildFactoryOffline;
			BuildMarketCmd.OfflineFunc = DummyServer.BuildMarketOffline;
			BuildResidenceCmd.OfflineFunc = DummyServer.BuildResidenceOffline;
			BuildStorageCmd.OfflineFunc = DummyServer.BuildStorageOffline;
			BuildServiceCmd.OfflineFunc = DummyServer.BuildServiceOffline;

			//建物を撤去
			DemolishFactoryCmd.OfflineFunc = DummyServer.DemolishFactoryOffline;
			DemolishMarketCmd.OfflineFunc = DummyServer.DemolishMarketOffline;
			DemolishResidenceCmd.OfflineFunc = DummyServer.DemolishResidenceOffline;
			DemolishStorageCmd.OfflineFunc = DummyServer.DemolishStorageOffline;
			DemolishServiceCmd.OfflineFunc = DummyServer.DemolishServiceOffline;

			//建物を再配置
			RelocateFactoryCmd.OfflineFunc = DummyServer.RelocateFactoryOffline;
			RelocateMarketCmd.OfflineFunc = DummyServer.RelocateMarketOffline;
			RelocateResidenceCmd.OfflineFunc = DummyServer.RelocateResidenceOffline;
			RelocateStorageCmd.OfflineFunc = DummyServer.RelocateStorageOffline;
			RelocateServiceCmd.OfflineFunc = DummyServer.RelocateServiceOffline;

			//建物を更新
			UpdateFactoryCmd.OfflineFunc = DummyServer.UpdateFactoryOffline;
			UpdateMarketCmd.OfflineFunc = DummyServer.UpdateMarketOffline;
			UpdateResidenceCmd.OfflineFunc = DummyServer.UpdateResidenceOffline;
			UpdateStorageCmd.OfflineFunc = DummyServer.UpdateStorageOffline;
			UpdateServiceCmd.OfflineFunc = DummyServer.UpdateServiceOffline;
			//キャラクターの建物割り当て更新
			UpdateCharacterCmd.OfflineFunc = DummyServer.UpdateCharacterOffline;

			BattleMainCmd.OfflineFunc = DummyServer.BattleMainOffline;
			BattleResultWinCmd.OfflineFunc = DummyServer.BattleResultWinOffline;
			BattleResultLoseCmd.OfflineFunc = DummyServer.BattleResultLoseOffline;
			QuestBattleCmd.OfflineFunc = DummyServer.QuestBattleOffline;

			StrategyMainCmd.OfflineFunc = DummyServer.StrategyMainOffline;

			UpdateBoardCmd.OfflineFunc = DummyServer.UpdateBoard;

			GachaMainCmd.OfflineFunc = DummyServer.GachaMainOffline;

			OrganizeMainCmd.OfflineFunc = DummyServer.OrganizeMainOffline;
			LotteryOriginalCmd.OfflineFunc = DummyServer.LotteryOriginalOffline;
			LotteryCharacterCmd.OfflineFunc = DummyServer.LotteryCharacterOffline;
			LotteryLeaderCmd.OfflineFunc = DummyServer.LotteryLeaderOffline;
			UpdateCorpsCmd.OfflineFunc = DummyServer.UpdateCorpsOffline;

			PurchaseItemCmd.OfflineFunc = DummyServer.PurchaseItemOffline;
#if DEVELOP_BUILD
			DebugBattleMainCmd.OfflineFunc = DummyServer.DebugBattleMainOffline;
#endif


		}
#endif
	}
}
