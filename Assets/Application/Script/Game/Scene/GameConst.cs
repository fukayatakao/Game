/// <summary>
/// 定数
/// </summary>
public static class GameConst {
	//バトルで使用する定数。ActEventEditorでエラーにならないようにデフォルトを入れる
	public static BattleConst Battle { get; set; } = new BattleConst();

	public static OrganizeConst Organize { get; set; } = new OrganizeConst();

	public static TownConst Town { get; set; } = new TownConst();

	//プレイヤーはマイナス位置から始めてプラス方向に移動
	//エネミーはプラス位置から始めてマイナス方向に移動
	//陣営の前方方向の符号
	public static readonly float[] TOWARD_SIGN = new float[(int)Power.Max] { 1f, -1f };

	//フォネティックコード文字列
	public static readonly string[] Phonetic = new string[] {"Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot" };

}
