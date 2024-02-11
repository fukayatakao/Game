using UnityEditor;
using UnityEngine;


namespace Project.Game {
	public class EditorDebugUtil {
		const string BattleScenePath = "Assets/Application/Scene/Game/Battle.unity";
		const string TownScenePath = "Assets/Application/Scene/Game/Town.unity";
		const string TitleScenePath = "Assets/Application/Scene/Game/Title.unity";
		const string OrganizeScenePath = "Assets/Application/Scene/Game/Organize.unity";
		const string StrategyScenePath = "Assets/Application/Scene/Game/Strategy.unity";
		const string GachaScenePath = "Assets/Application/Scene/Game/Gacha.unity";

		//battleシーンをメニューからオープン
		[MenuItem("Editor/Scene/Battle", false, 98)]
		public static void BootBattle() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(BattleScenePath);
		}

		//townシーンをメニューからオープン
		[MenuItem("Editor/Scene/Town", false, 98)]
		public static void BootTown() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TownScenePath);
		}

		//titleシーンをメニューからオープン
		[MenuItem("Editor/Scene/Title", false, 98)]
		public static void BootTitle() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(TitleScenePath);
		}

		//Organizeシーンをメニューからオープン
		[MenuItem("Editor/Scene/Organize", false, 98)]
		public static void BootOrganize() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(OrganizeScenePath);
		}

		//Strategyシーンをメニューからオープン
		[MenuItem("Editor/Scene/Strategy", false, 98)]
		public static void BootStrategy() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(StrategyScenePath);
		}

		//Gachaシーンをメニューからオープン
		[MenuItem("Editor/Scene/Gacha", false, 98)]
		public static void BootGacha() {
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene(GachaScenePath);
		}



		//ReverseAnimationシーンをメニューからオープン
		[MenuItem("Editor/Scene/Sandbox/ReverseAnimation", false, 98)]
		public static void BootReverseAnimation()
		{
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Application/Scene/Sandbox/ReverseAnimation.unity");
		}

		//MockBattleシーンをメニューからオープン
		[MenuItem("Editor/Scene/Sandbox/MockBattle", false, 98)]
		public static void BootMockBattle()
		{
			UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Application/Scene/Sandbox/MockBattle.unity");
		}

		//ゲームスピードを遅くする
		[MenuItem("Editor/Time/Time Slow &u", false, 98)]
		public static void TimeScaleDown() {
#if DEVELOP_BUILD
			TimeControl.SlowScale();
#endif
		}

		//ゲームスピードを早くする
		[MenuItem("Editor/Time/Time Fast &i", false, 98)]
		public static void TimeScaleUp() {
#if DEVELOP_BUILD
			TimeControl.FastScale();
#endif
		}

		//ゲームスピードをリセットする
		[MenuItem("Editor/Time/Time Reset &o", false, 98)]
		public static void TimeScaleReset() {
#if DEVELOP_BUILD
			TimeControl.ResetScale();
#endif
		}



	}
}
