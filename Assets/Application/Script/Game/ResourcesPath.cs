namespace Project.Lib {

	/// <summary>
	/// Resourcesに含まれているアセットへのアクセス管理
	/// </summary>
	public static class ResourcesPath {
		////////////////////////////////////////////////////////////////
		//ハードコードでResourcesからロードするアセット
		////////////////////////////////////////////////////////////////
		//カメラプレハブ
		public const string BATTLE_CAMERA_PREFAB = "Camera/BattleCamera";
		public const string STRATEGY_CAMERA_PREFAB = "Camera/StrategyCamera";
		public const string VIEW_CAMERA_PREFAB = "Camera/VirtualCamera";
		//個別移動用矢印
		public const string MOVE_ARROW_PREFAB = "Character/Arrow/Arrow2D";


		public const string LINK_ARROW_RED = "Character/Arrow/LinkArrowR";
		public const string LINK_ARROW_GREEN = "Character/Arrow/LinkArrowG";
		public const string LINK_ARROW_BLUE = "Character/Arrow/LinkArrowB";

		public const string UI_PHASE_SUN = "UI/Phase/sun";
		public const string UI_PHASE_MOON = "UI/Phase/moon";
		public const string UI_PHASE_STAR = "UI/Phase/star";


		public const string UI_BATTLE_ATLAS = "UI/BattleUIAtlas";
		public const string UI_GAUGE_PREFAB = "UI/Gauge";
		public const string UI_HEAD_TEXT = "UI/HeadText";


		public const string UI_WORLD_CANVAS = "UI/WorldCanvas";


		public const string LIGHT_DIRECTIONAL = "Light/DirectionalLight";





		//ここから下はAddressableに移行する
		public const string FIELD_SKY_MATERIAL = "Field/Sky/rustig_koppie_4k";

		//エディタ用定義
#if UNITY_EDITOR
		public const string MASTER_LEADER_DATA = "MstLeaderData_json";
		public const string MASTER_UNIT_DATA = "MstUnitData_json";
		public const string MASTER_GOODS_DATA = "MstGoodsData_json";
		public const string MASTER_ARTICLE_DATA = "MstArticleData_json";
#endif

	}
}
