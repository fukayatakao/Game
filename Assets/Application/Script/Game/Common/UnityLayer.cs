

// レイヤーの定義
public static class UnityLayer {
	public enum Layer : int{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Water = 4,
		UI = 5,

        ExtendUI = 10,
        WorldUI = 11,
        Field = 12,					//Battle,Townの地面、フィールド
        Character = 13,				//Battle,Townのキャラクター
		Collision = 14,				//ゲーム制御で使うそのほかコリジョン
		Facility = 15,				//Townの建物
	};


}


