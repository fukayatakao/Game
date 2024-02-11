

//namespace Project.Lib {
#if INPUT_WRAPPER
	/// <summary>
	/// UnityのInputをラッピング
	/// </summary>
	/// <remarks>
	/// 入力情報を記録・再生する処理を挟み込む等で使う予定
	/// </remarks>
	public class InputRecorder : MonoBehaviour{
	//出力するときのファイル名
		[SerializeField]
		private string filename_ ="test";
		enum State{
			None,
			Rec,
			Play,
		}

		State state_;
		InputLog log_;

		IEnumerator recorder_;

		void Awake(){
			state_ = State.None;
		}

		public void RecStart(){
			state_ = State.Rec;
			log_ = new InputLog ();
			recorder_ = Rec(log_);

		}

		public void RecEnd(){
			state_ = State.None;
#if UNITY_EDITOR
			recorder_ = null;
			Save(filename_, log_);
#endif
		}

		public void PlayStart(){
			state_ = State.Play;
#if UNITY_EDITOR
			log_ = Load (filename_);
			recorder_ = Play (log_);
#endif
		}

		public void PlayEnd(){
			state_ = State.None;
			Input.UseNativeInput = true;
		}


		void Update(){
			if (Input.GetKeyDown (KeyCode.S) && state_ != State.Play) {
				if (state_ == State.None) {
					//DebugLogDisplay.Display ("rec start");
					RecStart ();
				} else {
					//DebugLogDisplay.Display ("rec end");
					RecEnd ();
				}
			}
			if (Input.GetKeyDown (KeyCode.A) && state_ != State.Rec) {
				if (state_ == State.None) {
					//DebugLogDisplay.Display ("play start");
					PlayStart ();
				} else {
					//DebugLogDisplay.Display ("play end");
					PlayEnd ();
				}
			}


			if (recorder_ != null) {
				bool ret = recorder_.MoveNext ();
				if (!ret)
					recorder_ = null;
			}
		}


		/// <summary>
		/// Escキー(Androidのバックキー)が押された情報を記録
		/// </summary>
		private static bool RecEscKeyDown(ref InputLog.Record data){
			//情報を記録
			if (Input.GetKeyDown (KeyCode.Escape)) {
				data.escapeKey = Input.GetKeyDown (KeyCode.Escape);
				return true;
			//入力ないならfalse
			} else {
				return false;
			}
		}

		/// <summary>
		/// タッチ情報を記録
		/// </summary>
		private static bool RecTouchInfo(ref InputLog.Record data){
			//情報を記録
			if (Input.touchCount > 0) {
				data.touches = Input.touches;
				data.touchCount = Input.touchCount;
				return true;
			//入力ないならfalse
			} else {
				return false;
			}
		}
		/// <summary>
		/// マウス情報を記録
		/// </summary>
		private static bool RecMouseInfo(ref InputLog.Record data){
			//情報を記録
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButton (0) || Input.GetMouseButtonUp (0)) {
				if (Input.GetMouseButtonDown (0)) {
					data.mousePhase = TouchPhase.Began;
				} else if (Input.GetMouseButton (0)) {
					data.mousePhase = TouchPhase.Moved;
				} else if (Input.GetMouseButtonUp (0)) {
					data.mousePhase = TouchPhase.Ended;
				}
				data.mousePosition = Input.mousePosition;
				return true;
				//入力ないならfalse
			} else {
				return false;
			}
		}
		/// <summary>
		/// 入力情報の記録処理
		/// </summary>
		public static IEnumerator Rec(InputLog log){
			float recStartTime = Time.time;
			bool emptyFlag = true;
			while (true) {
				bool outputFlag = false;
				InputLog.Record data = new InputLog.Record ();
				data.time = Time.time - recStartTime;

				outputFlag |= RecEscKeyDown (ref data);
				outputFlag |= RecTouchInfo (ref data);
				outputFlag |= RecMouseInfo (ref data);

				//何らかの入力があったとき
				if(outputFlag){
					log.RecordList.Add (data);
					emptyFlag = true;
				//前フレームで出力が有ったとき
				} else if(emptyFlag){
					data.mousePhase = TouchPhase.Canceled;
					log.RecordList.Add (data);
					emptyFlag = false;
				}
				yield return null;
			}
		}

		/// <summary>
		/// 記録された入力情報の再生
		/// </summary>
		/// <param name="log">Log.</param>
		IEnumerator Play(InputLog log){
			float currentTime = 0f;
			int recordIndex = 0;

			Input.UseNativeInput = false;
			while (log.RecordList.Count > recordIndex) {
				currentTime += Time.deltaTime;
				if (log.RecordList [recordIndex].time < currentTime) {
					Input.CurrentRecord = log.RecordList [recordIndex];
					recordIndex++;
				}
				yield return null;
			}
		}


#if UNITY_EDITOR

		private const string InputAssetDirectoryPath = "Assets/ExternalResources/Input/";
		public void Save(string name, InputLog log)
		{
			string parent = InputAssetDirectoryPath;
			//ディレクトリがなければ作成
			if (!System.IO.Directory.Exists(parent + name)) {
				System.IO.Directory.CreateDirectory(parent + name);
			}
			// 該当パスにオブジェクトアセットを生成
			string outputPath = parent + name + "/" + name + ".asset";
			UnityEditor.AssetDatabase.CreateAsset(log, outputPath);

			// 未保存のアセットをアセットデータベースに保存
			UnityEditor.AssetDatabase.SaveAssets();


		}


		public InputLog Load(string name)
		{
			string outputPath = InputAssetDirectoryPath + name + "/" + name + ".asset";
			return (InputLog)UnityEditor.AssetDatabase.LoadAssetAtPath (outputPath, typeof(InputLog));
		}
#endif

	}

#endif



//}


