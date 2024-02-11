using UnityEngine;
using Project.Lib;

//・設定ファイルで定数を変化させたい
//・定数なので外部から変更させない
//・getのオーバヘッドも嫌
//->readonly変数にしてコンストラクタでファイルをロードして値をセットする
/// <summary>
/// 定数
/// </summary>
public class TownConst {
	//経済計算を行う時間間隔(s)
	public readonly int CALCULATE_TIMESPAN;
	//量の表示スケール(goodsの量をintで管理して表示はAMOUNT_SCALEで除算してデノミして表示する)
	public readonly int AMOUNT_SCALE;
	public readonly int GRADE_SCALE;
	public readonly int HISTORY_MAX;
	public readonly int TAX_SCALE;

	//近くに建物を移動させたときに流通半径を表示する範囲
	public readonly float EXTEND_AREA_RADIUS;

	public readonly int FIELD_SIZE_WIDTH;
	public readonly int FIELD_SIZE_DEPTH;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public TownConst() {
		//全部初期値だと0除算とか発生して正常に動かない。
		//エディタなどのツールで正常に動かすためだけのコンストラクタ
		TownConstFile file = Load();

		CALCULATE_TIMESPAN = file.CALCULATE_TIMESPAN;
		AMOUNT_SCALE = file.AMOUNT_SCALE;
		GRADE_SCALE = file.GRADE_SCALE;
		HISTORY_MAX = file.HISTORY_MAX;
		TAX_SCALE = file.TAX_SCALE;
		EXTEND_AREA_RADIUS = file.EXTEND_AREA_RADIUS;
		FIELD_SIZE_WIDTH = file.FIELD_SIZE_WIDTH;
		FIELD_SIZE_DEPTH = file.FIELD_SIZE_DEPTH;
	}

	/// <summary>
	/// 設定をロード
	/// </summary>
	public static TownConstFile Load() {
		var json = Resources.Load("TownConst") as TextAsset;
		return JsonUtility.FromJson<TownConstFile>(json.text);
	}

}

// ------------------------------------------------
// 施設タイプ
public enum FacilityType : int {
	[Field("無効")] None,
	[Field("工場")] Factory,
	[Field("マーケット")] Market,
	[Field("住居")] Residence,
	[Field("サービス施設")] Service,
	[Field("倉庫")] Storage,
	[Field("タウンホール")] Townhall,
	[Field("最大数")] Max,
}

public enum DemandCategory : int {
	[Field("必需品")] Necessity,
	[Field("奢侈品")] Luxury,
	[Field("最大数")] Max,
}

