
[System.Serializable]
public class TownConstFile {
	//経済計算を行う時間間隔(s)
	public int CALCULATE_TIMESPAN;
	//量の表示スケール(goodsの量をintで管理して表示はAMOUNT_SCALEで除算してデノミして表示する)
	public int AMOUNT_SCALE;
	public int GRADE_SCALE;
	public int HISTORY_MAX;
	public int TAX_SCALE;

	//近くに建物を移動させたときに流通半径を表示する範囲
	public float EXTEND_AREA_RADIUS;

	public int FIELD_SIZE_WIDTH;
	public int FIELD_SIZE_DEPTH;

}
