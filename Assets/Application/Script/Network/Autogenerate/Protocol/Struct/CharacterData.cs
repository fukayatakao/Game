using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class CharacterData
	{
		//---- レスポンス変数定義 ----
		public int id; //管理ID
		public string name; //名前
		public int species; //種族
		public bool generate; //生成されたオリジナル画像か
		public string portrait; //サムネイル画像
		public List<int> ability; //アビリティ
		public int phase; //属性
		public int stayId; //住居Id
		public int staySlotId; //住居スロット番号
		public int workId; //職場Id
		public int workSlotId; //職場スロット番号
		public int grade; //生活水準

	}

}
