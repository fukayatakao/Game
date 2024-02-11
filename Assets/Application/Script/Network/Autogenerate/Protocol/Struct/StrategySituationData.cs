using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class StrategySituationData
	{
		//---- レスポンス変数定義 ----
		public List<PlatoonData> invader; //未配置攻撃側部隊情報
		public List<PlatoonData> defender; //未配置守備側部隊情報(?)
		public List<StrategyLocationData> invaderLocation; //配置済攻撃部隊
		public List<StrategyLocationData> defenderLocation; //配置済守備部隊
		public int mapId; //マップモデル
		public int turn; //ターン
		public List<int> available; //行動可能部隊

	}

}
