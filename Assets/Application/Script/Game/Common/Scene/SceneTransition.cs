using Project.Lib;
using Project.Network;
using UnityEngine;

namespace Project.Game {
	public static class SceneTransition {
		public enum SceneType
		{
			None,
			Title,
			Town,
			Battle,
			Organize,
			Strategy,
		}
		public static Response TransitionData { get; private set; }

		private static TransitScreen screen_;

		/// <summary>
		/// 初期化
		/// </summary>
		public static void Initialize() {
			GameObject obj = UnityUtil.Instantiate(Resources.Load("Transition/TransitionBg") as GameObject); ;
			GameObject.DontDestroyOnLoad(obj);
			screen_ = obj.GetComponent<TransitScreen>();
		}

		/// <summary>
		/// 遷移開始時の通信結果を受け取る
		/// </summary>
		public static void Transition(Response res) {
			TransitionData = res;
		}

		/// <summary>
		/// 遷移終了処理
		/// </summary>
		public static void Termination(System.Action callback) {
			TransitionData = null;
			screen_.FadeIn(callback);
		}

		public static bool IsFinish() {
			return true;
		}

		/// <summary>
		/// ガチャシーンへ遷移
		/// </summary>
		public static void ChangeGacha() {
			BattleMain.PrevScene = SceneType.Town;
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Gacha");
				GachaMainCmd.CreateAsync(new GachaMainRequest(), Transition);
			});
		}


		/// <summary>
		/// タウンシーンへ遷移
		/// </summary>
		public static void ChangeTown() {
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");

			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Town");
				TownMainCmd.CreateAsync(new TownMainRequest(), Transition);
			});
		}


		/// <summary>
		/// バトルシーンへ遷移
		/// </summary>
		public static void ChangeBattle(int invaderPlatoonId, int defenderPlatoonId, int nodeId) {
			BattleMain.PrevScene = SceneType.Strategy;

#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
				BattleMainCmd.CreateAsync(new BattleMainRequest(invaderPlatoonId, defenderPlatoonId, nodeId, 1), Transition);
			});
		}

		/// <summary>
		/// バトルシーンへ遷移
		/// </summary>
		public static void ChangeQuestBattle(int offence, int questId) {
			BattleMain.PrevScene = SceneType.Town;
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
				QuestBattleCmd.CreateAsync(new QuestBattleRequest(offence, questId), Transition);
			});
		}

#if DEVELOP_BUILD
		/// <summary>
		/// バトルシーンへ遷移
		/// </summary>
		public static void ChangeBattle() {
			BattleMain.PrevScene = SceneType.Battle;
			DebugWindowManager.SetActive(false);
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
				DebugBattleMainCmd.CreateAsync(new DebugBattleMainRequest(), Transition);
			});
		}
#endif
		/// <summary>
		/// 編成シーンへ遷移
		/// </summary>
		public static void ChangeOrganize(SceneType prev) {
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			OrganizeMain.PrevScene = prev;
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Organize");
				OrganizeMainCmd.CreateAsync(new OrganizeMainRequest(), Transition);
			});
		}
		/// <summary>
		/// 戦略シーンへ遷移
		/// </summary>
		public static void ChangeStrategy() {
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Strategy");
				StrategyMainCmd.CreateAsync(new StrategyMainRequest(), Transition);
			});
		}
		/// <summary>
		/// 戦略シーンへ遷移(バトル勝利)
		/// </summary>
		public static void ChangeStrategyWin(int invaderId, int defenderId, int nodeId) {
#if DEVELOP_BUILD
			DebugWindowManager.SetActive(false);
#endif
			//ちゃんと解放出来てなかったらエラー出しておく
			Debug.Assert(TransitionData == null, "not release transition data");
			screen_.FadeOut(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Strategy");
				BattleResultWinCmd.CreateAsync(new BattleResultWinRequest(invaderId, defenderId, nodeId), Transition);
			});
		}

#if DEVELOP_BUILD
		/// <summary>
		/// 木人シーンへ遷移
		/// </summary>
		public static void ChangeMockBattle() {
			DebugWindowManager.SetActive(false);

			UnityEngine.SceneManagement.SceneManager.LoadScene("MockBattle");
			DebugBattleMainCmd.CreateAsync(new DebugBattleMainRequest(), Transition);
		}
#endif
	}


}
