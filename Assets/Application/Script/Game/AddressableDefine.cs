namespace Project.Lib {

	/// <summary>
	/// Addressableで使っているラベル/アドレス定義
	/// </summary>
	/// <remarks>
	/// ハードコードされるアセット名はここにまとめる
	/// </remarks>
	public static class AddressableDefine {
		public const string Effect = "Effect";

		public static class Label {
			public const string AnimationCommon = "Animation-Pose";
		}

		public static class Address {
			public const string TOWN_STAGE_SCENE = "Field/Ground";

			//StrategyMapのノードprefab
			public const string START_PREFAB = "WaymapNode/Start";
			public const string GOAL_PREFAB = "WaymapNode/Goal";
			public const string NODE_PREFAB = "WaymapNode/Node";

			public const string CIRCLE_AREA = "Area/CircleArea";
			public const string LINE_FRONT_LINE = "Line/DeployLine";
#if DEVELOP_BUILD
			public const string DEBUG_LINE = "Line/DebugLine";
#endif
		}
	}
}
