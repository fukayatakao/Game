using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class BaseDataVersion
	{
		//---- レスポンス変数定義 ----
		public string type; //BaseDataタイプ名
		public string version; //チェックハッシュ

	}

}
