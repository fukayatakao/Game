using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class StrategyLocationData
	{
		//---- レスポンス変数定義 ----
		public int nodeId; //配置ノード
		public PlatoonData platoon; //配置部隊

	}

}
