using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class LeaderData
	{
		//---- レスポンス変数定義 ----
		public int id; //管理ID
		public int leaderMasterId; //リーダーマスターのId
		public int enemyLeaderParamId; //敵リーダー用上書きパラメータId
		public int lv; //レベル
		public int attack; //レベルによる攻撃補正
		public int defense; //レベルによる防御補正

	}

}
