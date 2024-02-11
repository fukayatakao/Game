using Project.Network;
using System.Collections.Generic;

#if USE_OFFLINE

namespace Project.Server {
	public partial class DummyServer {
		/// <summary>
		/// ガチャシーン開始通信
		/// </summary>
		public static Response GachaMainOffline(Request req) {
			GachaMainResponse response = new GachaMainResponse();

			List<UserMemberTable> characters = UserCharacterTableDAO.SelectAll();

			List<UserLeaderTable> leaders = UserLeaderTableDAO.SelectAll();

			response.characters = DataTransfer.Convert(characters);
			response.leaders = DataTransfer.Convert(leaders);

			return response;
		}
		/// <summary>
		/// キャラ抽選通信(オリジナルキャラ)
		/// </summary>
		public static Response LotteryOriginalOffline(Request req) {
			var request = req as LotteryOriginalRequest;

			UserMemberTable userCharacter = LotteryTool.LotteryCharacter(request.species);
			userCharacter.generate = true;
			userCharacter.portrait = request.portraitKey;
			UserDB.Instance.memberTable.Add(userCharacter);
			UserDB.Save();

			LotteryOriginalResponse response = new LotteryOriginalResponse();
			response.characterData = DataTransfer.Convert(userCharacter);
			return response;
		}



		/// <summary>
		/// キャラ抽選通信
		/// </summary>
		public static Response LotteryCharacterOffline(Request req) {
			var request = req as LotteryCharacterRequest;

			UserMemberTable userCharacter = LotteryTool.LotteryCharacter(request.species);
			UserDB.Instance.memberTable.Add(userCharacter);
			UserDB.Save();

			LotteryCharacterResponse response = new LotteryCharacterResponse();
			response.characterData = DataTransfer.Convert(userCharacter);
			return response;
		}

		/// <summary>
		/// リーダー抽選通信
		/// </summary>
		public static Response LotteryLeaderOffline(Request req) {
			var request = req as LotteryLeaderRequest;

			UserLeaderTable leader = LotteryTool.LotteryLeader();
			UserDB.Instance.leaderTable.Add(leader);
			UserDB.Save();


			LotteryLeaderResponse response = new LotteryLeaderResponse();
			response.leaderData = DataTransfer.Convert(leader);
			return response;
		}
	}
}
#endif
