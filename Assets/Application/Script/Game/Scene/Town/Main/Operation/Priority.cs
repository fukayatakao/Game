namespace Project.Game {
	//制御優先度
	public enum TownOperationPriority {
		FacilityBuild,              //建物を建てる時の操作
		TownLayout,                 //タウンの建物再配置の操作
		SelectFacility,             //タウンの建物選択してメニューを表示する
	}
}
