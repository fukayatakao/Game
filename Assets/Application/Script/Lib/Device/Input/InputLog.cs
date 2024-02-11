

//namespace Project.Lib {
#if INPUT_WRAPPER
	[Serializable]
	public class InputLog : ScriptableObject {
		/// <summary>
		/// Input情報クラス
		/// </summary>
		[Serializable]
		public class Record
		{
			public float time;
			public Touch[] touches;
			public int touchCount;
			public bool escapeKey;


			public Touch GetTouch(int index){
				return touches[index];
			}
			public bool GetKey(KeyCode key){
				return key == KeyCode.Escape && escapeKey;
			}


			public Vector3 mousePosition;
			public TouchPhase mousePhase;
			public bool GetMouseButtonDown(int index){
				return mousePhase == TouchPhase.Began;
			}
			public bool GetMouseButton(int index){
				return mousePhase == TouchPhase.Moved || mousePhase == TouchPhase.Stationary;
			}
			public bool GetMouseButtonUp(int index){
				return mousePhase == TouchPhase.Ended;
			}
		}

		//１フレーム単位の入力を時系列でリスト化したもの
		public List<Record> RecordList = new List<Record> ();



	}
#endif

//}


