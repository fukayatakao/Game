using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
	/// <summary>
	/// ActEventのデータ
	/// </summary>
	[System.Serializable]
	public class ActEventData : ScriptableObject {
		[System.Serializable]
		public class EventData {
			public float execTime;				//実行時刻
			public string method;               //呼び出す関数名
			public string[] args;               //引数用の文字列

			//バイナリ保存できる引数データ
			public List<int> types;
			public byte[] argsBinary;
		}
		//適用するEntityの型情報（主に編集・確認用）
		public string entityType;
		//実行する内容
		public List<EventData> eventDataList;
		//使用する曲線リスト
		public List<AnimationCurve> curveList;
		//使用するリソース
		public List<string> resourceList;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ActEventData() {
			//リストのインスタンスだけつくる。nullチェック面倒。
			eventDataList = new List<EventData>();
			curveList = new List<AnimationCurve>();
			resourceList = new List<string>();
		}
		/// <summary>
		/// イベントの発生順でソート
		/// </summary>
		public void Sort() {
			eventDataList.Sort((a, b) =>
			{
				if (a.execTime > b.execTime) return 1;
				else if (a.execTime < b.execTime) return -1;
				else return 0;
			});
		}


	}

}

