using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class BattleSituationData
	{
		//---- レスポンス変数定義 ----
		public PlatoonData invader; //攻撃側情報
		public PlatoonData defender; //防衛側情報
		public string enemyAI; //敵AI
		public int nodeId; //ノードID
		public int stageId; //ステージID

	}

}
