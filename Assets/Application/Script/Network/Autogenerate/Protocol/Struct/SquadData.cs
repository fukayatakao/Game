using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class SquadData
	{
		//---- レスポンス変数定義 ----
		public int unitId; //ユニットMasterId
		public int enemyMemberParamId; //敵ユニット用上書きパラメータId
		public List<CharacterData> members; //メンバーデータ
		public LeaderData leader; //リーダーデータ

	}

}
