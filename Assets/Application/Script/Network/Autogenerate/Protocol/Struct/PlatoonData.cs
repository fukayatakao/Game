using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class PlatoonData
	{
		//---- レスポンス変数定義 ----
		public int id; //管理ID
		public string name; //小隊名
		public int experience; //熟練度
		public List<SquadData> squads; //分隊

	}

}
