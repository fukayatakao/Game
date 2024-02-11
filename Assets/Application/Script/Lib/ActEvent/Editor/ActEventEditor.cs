using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project.Lib {
    //タイムライン内で処理できない操作リクエストが来た場合リクエストを外に渡す
    public enum LineControl {
        None,
        Add,
        Del,
        Up,
        Down,
        Copy,
		Paste,
    }

    //Editor側で特に用意する関数を分離して管理する
    public static class EditorFuncion {
        public static System.Func<System.Type, GameObject, IEntityActEvent> Create = Editor.ActEventEditorMain.Create;
        public static System.Action Destroy = Editor.ActEventEditorMain.Destroy;
        public static System.Action<System.Type, AnimationClip> AddClip = Editor.ActEventEditorMain.AddClip;
        public static System.Action<AnimationClip, float> SyncTime = Editor.ActEventEditorMain.SyncTime;
        public static System.Action<bool> Pause = Editor.ActEventEditorMain.Pause;
		public static System.Action<bool, bool, bool> CameraReset = Editor.ActEventEditorMain.CameraReset;

        //エディタで使うActEventのデータ名。同じファイル名のデータがあるとバグるはずなのでかぶらないように注意
        public static readonly string TemporaryActEventName = "__temp";
		//エディタ上で使う特殊なアニメーション再生の関数名
		public static readonly string PlayAnimationName = "PlayActEventAnimation";
	}

    //アセットの内容を編集しやすい形で保持
    public class EditorData {
        public void Init(System.Type type) {
            entityType = type;
            editTimeline.Clear();
            editTimeline.Add(new EditTimeline());
        }
        /// <summary>
        /// イベントのリストが空になっているキーを削除する
        /// </summary>
        public void TrimKey() {
            for(int i = 0; i < editTimeline.Count; i++) {
                //新しいインスタンス
                Dictionary<float, List<ActEventEdit>> temp = new Dictionary<float, List<ActEventEdit>>();
                //リストが空ではないキーのみ新しいインスタンスにコピー
                foreach (KeyValuePair<float, List<ActEventEdit>> pair in editTimeline[i].timeEvent) {
                    if(pair.Value.Count != 0) {
                        temp[pair.Key] = pair.Value;
                    }
                }

                editTimeline[i].timeEvent = temp;
            }
        }

        //イベントをセットするEntityのタイプ
        public System.Type entityType;

        public List<EditTimeline> editTimeline = new List<EditTimeline>();
    }

    public class EditTimeline {
		static EditTimeline clipboard = new EditTimeline();
		//時間をkeyにしてそのタイミングで発生するイベントのリストを持つ。さらに複数のtimelineで管理するためにリスト化する
		public Dictionary<float, List<ActEventEdit>> timeEvent;
        public ActEventEdit animationEvent;
        public float selectTime;

        public TimelineControlSub timeline;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditTimeline() {
            timeEvent = new Dictionary<float, List<ActEventEdit>>();
            timeline = new TimelineControlSub();
        }

		/// <summary>
		/// クリップボードにコピー
		/// </summary>
		public void Copy() {
			clipboard.timeEvent = new Dictionary<float, List<ActEventEdit>>();
			foreach(float key in timeEvent.Keys) {
				clipboard.timeEvent[key] = new List<ActEventEdit>();
				for(int i = 0, max = timeEvent[key].Count; i < max; i++) {
					clipboard.timeEvent[key].Add(new ActEventEdit(timeEvent[key][i]));
				}
			}

			clipboard.timeline = new TimelineControlSub(timeline);
		}
		/// <summary>
		/// クリップボードから貼り付け
		/// </summary>
		public void Paste() {
			timeEvent = new Dictionary<float, List<ActEventEdit>>();
			foreach (float key in clipboard.timeEvent.Keys) {
				timeEvent[key] = new List<ActEventEdit>();
				for (int i = 0, max = clipboard.timeEvent[key].Count; i < max; i++) {
					timeEvent[key].Add(new ActEventEdit(clipboard.timeEvent[key][i]));
				}
			}


			timeline = new TimelineControlSub(clipboard.timeline);
			selectTime = clipboard.selectTime;
		}

		/// <summary>
		/// 複数並べたタイムライン表示
		/// </summary>
		public bool DrawMultiTimeline(EditorWindow window, ActEntitySelect selector, out LineControl request) {
            Rect rect;
            using (new GUILayout.HorizontalScope()) {
                request = DrawAnimationBarContorl();
                //イベントの詳細設定
                rect = timeline.DrawSlider(window);
                //コピー
                if (GUILayout.Button("copy", GUILayout.MaxWidth(60f))) {
                    GUI.FocusControl(null);
                    request = LineControl.Copy;
                }
				//貼り付け
				if (GUILayout.Button("paste", GUILayout.MaxWidth(60f))) {
					GUI.FocusControl(null);
					request = LineControl.Paste;
				}
			}

			foreach (float t in timeEvent.Keys) {
				if(t == selectTime) {
					ActEventEditor.DrawTimeLine(rect, t, timeline.MaxTime, Color.green);
					//データが存在する場合は白
				} else if (timeEvent[t].Count > 0) {
                    ActEventEditor.DrawTimeLine(rect, t, timeline.MaxTime, Color.white);
                //Keyだけ打って中が空の場合は赤
                } else {
                    ActEventEditor.DrawTimeLine(rect, t, timeline.MaxTime, Color.red);
                }
            }



            //イベントの詳細設定
            timeline.DrawTime(window, (clip)=>{
                animationEvent = null;
                if (clip != null) {
                    animationEvent = new ActEventEdit(selector.PlayAnimationMethod);
                    animationEvent.SetArgs(new object[] { clip.name });

                    if (EditorApplication.isPlaying) {
                        EditorFuncion.AddClip(selector.EditEntityType, clip);
                    }
                }
            });

            //キー選択
            selectTime = ActEventEditor.DrawKeyControl(ref timeEvent, selectTime, timeline.CurrentTime, timeline.MaxTime);
            return DrawKeyEvent(selector) || request != LineControl.None;
        }

        /// <summary>
        /// タイムラインの移動
        /// </summary>
        private LineControl DrawAnimationBarContorl() {
            //追加
            if (GUILayout.Button("＋", GUILayout.MaxWidth(20f))) {
                GUI.FocusControl(null);
                return LineControl.Add;
            }
            //削除
            if (GUILayout.Button("－", GUILayout.MaxWidth(20f))) {
                GUI.FocusControl(null);
                return LineControl.Del;
            }
            //次のデータ
            if (GUILayout.Button("↑", GUILayout.MaxWidth(20f))) {
                GUI.FocusControl(null);
                return LineControl.Up;
            }
            //前のデータ
            if (GUILayout.Button("↓", GUILayout.MaxWidth(20f))) {
                GUI.FocusControl(null);
                return LineControl.Down;
            }

            return LineControl.None;
        }



        /// <summary>
        /// 時間が同じイベントの表示と編集
        /// </summary>
        private bool DrawKeyEvent(ActEntitySelect selector) {
            if (!timeEvent.ContainsKey(selectTime))
                return false;
            //変更があったか
            bool result = false;
            List<ActEventEdit> data = timeEvent[selectTime];
            //すでに存在しているイベントを表示
            for (int i = 0; i < data.Count; i++) {
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Label("", GUILayout.MaxWidth(80f));

                    //削除ボタンでイベントを消したとき
                    if (GUILayout.Button("-", new GUILayoutOption[] { GUILayout.MaxHeight(14f), GUILayout.MaxWidth(32f) })) {
                        data.RemoveAt(i);
                        i--;
                        result = true;
                        continue;
                    }
                    result |= data[i].DrawGUI(selector.MethodNameArray, selector.MethodInfoList);
                }
            }
            GUILayout.Label("");
            using (new GUILayout.HorizontalScope()) {
                GUILayout.Label("", GUILayout.MaxWidth(480f));
                //新規イベント追加
                if (GUILayout.Button("Add", GUILayout.MaxWidth(80f))) {
                    ActEventEdit edit = new ActEventEdit(selector.MethodInfoList[0]);

                    data.Add(edit);
                    result = true;
                }
            }
            return result;
        }

    }


    /// <summary>
    /// ActEventの編集
    /// </summary>
    public class ActEventEditor : EditorWindow {
		/// <summary>
		/// 編集ダイアログのオープン
		/// </summary>
		[MenuItem("Editor/ActEventditor", false, 98)]
		private static void Open() {
			ActEventEditor window = EditorWindow.GetWindow<ActEventEditor>(false, "ActEventEditor");
            window.Init();
		}
        //シーンの置き場所
        const string EditorScenePath = "Assets/Application/Scene/Editor/ActEventEditor.unity";
		//モデル検索パス
		const string SearchModelPath = "Assets/Application/Addressable/Character/Model";
		//デフォルトのデータ置き場
		string path_ = "Assets/Application/Addressable/Character/Action";
		//タイムラインの表示
		TimelineControlMain masterTimeline_;


        public static readonly string markString = " ※";
		const float LineWidth = 2f;

        IEntityActEvent actEvent_;
        Vector2 scroll_;
		GameObject entityObject_;


		int selectFile_ = 0;
		string[] files_ = new string[0];
		bool[] dirty_ = new bool[0];

		bool isDrawHeader_ = true;
		bool isDrawUpper_ = true;

		//アセット名をkeyにしてそのアセットで行っているすべてのイベントを持つ
		Dictionary<string, EditorData> allEditData_;
		//編集中データ
		EditorData currentEditData_;
		//編集するEntityの切り替え制御クラス
		ActEntitySelect entitySelect_ = new ActEntitySelect();

		bool ready_ = false;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ActEventEditor() {
        }

        /// <summary>
        /// ウィンドウがアクティブになったとき
        /// </summary>
        private void OnEnable() {
			Init();
		}


		/// <summary>
		/// ウィンドウが非アクティブになったとき
		/// </summary>
		private void OnDisable() {
		}

        /// <summary>
        /// マスターのスライダー操作からスレイブを同期
        /// </summary>
        private static TimelineControlSub SyncSliderMaster(EditorData editData, TimelineControlMain master) {
            TimelineControlSub ret = null;
            for (int i = 0; i < editData.editTimeline.Count; i++) {
                EditTimeline et = editData.editTimeline[i];
                if (master.CurrentTime < et.timeline.HeadTime) {
                    et.timeline.CurrentTime = 0f;
                } else if (master.CurrentTime > et.timeline.HeadTime + et.timeline.MaxTime) {
                    et.timeline.CurrentTime = et.timeline.MaxTime;
                } else {
                    et.timeline.CurrentTime = master.CurrentTime - et.timeline.HeadTime;
                    ret = et.timeline;
                }
            }

            return ret;
        }

        /// <summary>
        /// 再生ボタンを押した
        /// </summary>
        private void Play() {
            TimelineControlSub currentTimeline = SyncSliderMaster(currentEditData_, masterTimeline_);
            //EditorFuncion.SyncTime(currentTimeline.Clip, currentTimeline.CurrentTime);
            //EditorFuncion.Pause(false);

            if (!Application.isPlaying)
                return;
            if (actEvent_ == null)
                return;
            actEvent_.Add(EditorFuncion.TemporaryActEventName, CreateActEventData(currentEditData_));
            actEvent_.Play(EditorFuncion.TemporaryActEventName, 1f);
            actEvent_.Seek(masterTimeline_.CurrentTime);

        }
        /// <summary>
        /// 再生が終了した
        /// </summary>
        private void Stop() {
            if (!Application.isPlaying)
                return;
            if (actEvent_ == null)
                return;
            actEvent_.Stop();
            EditorFuncion.Pause(true);
        }
        /// <summary>
        /// 所定の時間にスキップ
        /// </summary>
        private void Seek(float t) {
            if (!Application.isPlaying)
                return;

            masterTimeline_.CurrentTime = t;
            TimelineControlSub currentTimeline = SyncSliderMaster(currentEditData_, masterTimeline_);
            EditorFuncion.SyncTime(currentTimeline.Clip, currentTimeline.CurrentTime);
            if (!masterTimeline_.IsPlaying) {
                EditorFuncion.Pause(true);
            } else {
                EditorFuncion.Pause(false);
            }
            if (actEvent_ == null)
                return;

            actEvent_.Seek(t);
        }
        /// <summary>
        /// 初期化
        /// </summary>
        protected void Init() {
            masterTimeline_ = new TimelineControlMain(Play, Stop, Seek);


            TimelineControlSub.SeekEvent = Seek;



            entitySelect_.Init();
			selectFile_ = 0;
			files_ = new string[0];
			dirty_ = new bool[0];
			allEditData_ = new Dictionary<string, EditorData>();
			currentEditData_ = new EditorData();
            currentEditData_.Init(entitySelect_.EditEntityType);

			//Mst.BaseDataManager.Dispatch(() => { ready_ = true; Debug.Log("update finish"); });
			ready_ = true;
		}

		/// <summary>
		/// 表示処理
		/// </summary>
		private void OnGUI() {
			if (!ready_)
				return;
			//ヘッダー部分の折り畳みチェック
			using (new GUILayout.HorizontalScope()) {
				string cap = isDrawHeader_ ? "∧" : "∨";
				if (GUILayout.Button(cap, GUILayout.MaxWidth(20f))) {
					isDrawHeader_ = !isDrawHeader_;
				}
				EditorUtil.DrawLine();
			}
			//ヘッダーを表示する場合
			if(isDrawHeader_){
				DrawHeader();
				//ちょっと隙間を作る
				GUILayout.Space(5);
			}
            scroll_ = EditorGUILayout.BeginScrollView(scroll_);

			//上部GUIの折り畳みチェック
			using (new GUILayout.HorizontalScope()) {
				string cap = isDrawUpper_ ? "∧" : "∨";
				if (GUILayout.Button(cap, GUILayout.MaxWidth(20f))) {
					isDrawUpper_ = !isDrawUpper_;
				}
				EditorUtil.DrawLine();
			}

			//上部GUI表示
			if(isDrawUpper_){
				DrawUpper();
				GUILayout.Space(5);

			}

			//実行中以外は時間操作無効にする
			if (Application.isPlaying){
			    //ちょっと隙間を作る
			    GUILayout.Space(5);
			    EditorUtil.DrawLine();
			    GUILayout.Space(5);
                DrawMainTimeline();
            }
            //ちょっと隙間を作る
            GUILayout.Space(5);
            EditorUtil.DrawLine();
            GUILayout.Space(5);

            //@note seekのときに最大時間が計算しきれていないと正しく動作しない
            //時間の集計は先に終わらせる
            masterTimeline_.MaxTime = 0f;
            for (int i = 0; i < currentEditData_.editTimeline.Count; i++) {
                masterTimeline_.MaxTime += currentEditData_.editTimeline[i].timeline.MaxTime;
            }

            float length = 0f;
            for (int i = 0; i < currentEditData_.editTimeline.Count;i ++){
                LineControl request = LineControl.None;
                bool result = currentEditData_.editTimeline[i].DrawMultiTimeline(this, entitySelect_, out request);
                if (dirty_.Length > selectFile_) {
                    dirty_[selectFile_] |= result;
                }


                currentEditData_.editTimeline[i].timeline.HeadTime = length;

                switch (request) {
                //追加
                case LineControl.Add:
                    currentEditData_.editTimeline.Add(new EditTimeline());
                    break;
                //削除
                case LineControl.Del:
                    if(currentEditData_.editTimeline.Count > 1){
                        currentEditData_.editTimeline.RemoveAt(i);
                        i--;
                    }
                    break;
                //一つ上に移動
                case LineControl.Up:
                    if(i > 0) {
                        EditTimeline et = currentEditData_.editTimeline[i - 1];
                        currentEditData_.editTimeline[i - 1] = currentEditData_.editTimeline[i];
                        currentEditData_.editTimeline[i] = et;
                    }
                    break;
                //一つ下に移動
                case LineControl.Down:
                    if (i < currentEditData_.editTimeline.Count - 1) {
                        EditTimeline et = currentEditData_.editTimeline[i + 1];
                        currentEditData_.editTimeline[i + 1] = currentEditData_.editTimeline[i];
                        currentEditData_.editTimeline[i] = et;
                    }
                    break;
				//テンポラリにコピー
				case LineControl.Copy:
					currentEditData_.editTimeline[i].Copy();
					break;
				//貼り付け
				case LineControl.Paste:
					currentEditData_.editTimeline[i].Paste();
					break;
				}
				//タイムラインのボタン操作ががあった場合は処理をここで終わる
				if (request != LineControl.None)
                    break;

                GUILayout.Space(15);
                length += currentEditData_.editTimeline[i].timeline.MaxTime;
            }


            EditorGUILayout.EndScrollView();
            DrawFooter();
        }
        /// <summary>
        /// ヘッダーGUI表示
        /// </summary>
        private void DrawHeader() {
            GUILayout.Space(5);
            //ビューアシーンロード
            if (GUILayout.Button("Load Scene")) {
                if(!EditorApplication.isPlaying){
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorScenePath);
                    EditorApplication.isPlaying = true;
                    entityObject_ = null;
                }
            }

			//一番下の段でボタン類を表示
			using (new GUILayout.HorizontalScope()) {
				//ビューアに使うプレハブをセット
				EditorGUI.BeginChangeCheck();
				entityObject_ = (GameObject)EditorGUILayout.ObjectField("Prefab", entityObject_, typeof(GameObject), true);
				if (EditorGUI.EndChangeCheck()) {

					//Entity生成
					actEvent_ = EditorFuncion.Create(currentEditData_.entityType, entityObject_);
				}

				if (GUILayout.Button("Search", GUILayout.Width(160))) {
					SearchPrefabWindow.Open(SearchModelPath, ".prefab", (s) => {
						entityObject_ = AssetDatabase.LoadAssetAtPath(s, typeof(GameObject)) as GameObject;
						//Entity生成
						actEvent_ = EditorFuncion.Create(currentEditData_.entityType, entityObject_);

					});
				}

			}

		}

        /// <summary>
        /// 上部GUI表示
        /// </summary>
        private void DrawUpper() {
            //ファイル選択
            string[] showFiles = new string[files_.Length];
            for (int i = 0; i < files_.Length; i++) {
                showFiles[i] = files_[i];
                if (dirty_[i]) {
                    showFiles[i] = showFiles[i] + markString;
                }
            }

			using (new GUILayout.HorizontalScope()) {
				EditorGUI.BeginChangeCheck();
				selectFile_ = EditorGUILayout.Popup("File", selectFile_, showFiles);
				if (EditorGUI.EndChangeCheck()) {
					SelectFile(selectFile_);
				}
				if (GUILayout.Button("new", GUILayout.Width(100))) {
					NewFile();
				}
			}

            //EntityのPrefabと型選択
            //型の変更があった場合は仕込んだイベントをクリア
            if (entitySelect_.DrawGUI()) {
                currentEditData_.Init(entitySelect_.EditEntityType);
                EditorFuncion.Destroy();
                entityObject_ = null;
            }

			//カメラリセット
			using (new GUILayout.HorizontalScope()) {
				GUILayout.Label("カメラリセット");

				if (GUILayout.Button("座標", GUILayout.Width(160))) {
					EditorFuncion.CameraReset(true, false, false);
				}
				if (GUILayout.Button("回転", GUILayout.Width(160))) {
					EditorFuncion.CameraReset(false, true, false);
				}
				if (GUILayout.Button("距離", GUILayout.Width(160))) {
					EditorFuncion.CameraReset(false, false, true);
				}
				if (GUILayout.Button("ALL", GUILayout.Width(160))) {
					EditorFuncion.CameraReset(true, true, true);
				}
			}

		}

		/// <summary>
		/// メインのタイムライン表示
		/// </summary>
		private void DrawMainTimeline() {
            //タイムライン表示
            Rect rect = masterTimeline_.DrawGUI(this);
            for(int i  =0;i < currentEditData_.editTimeline.Count; i ++){
                Dictionary<float, List<ActEventEdit>> ev = currentEditData_.editTimeline[i].timeEvent;
                float head = currentEditData_.editTimeline[i].timeline.HeadTime;
                if (currentEditData_.editTimeline[i].animationEvent != null) {
                    DrawTimeLine(rect, head, masterTimeline_.MaxTime, Color.cyan);
                }

                foreach (float t in ev.Keys) {

                    //データが存在する場合は白
                    if (ev[t].Count > 0) {
                        DrawTimeLine(rect, t + head, masterTimeline_.MaxTime, Color.white);
                        //Keyだけ打って中が空の場合は赤
                    } else {
                        DrawTimeLine(rect, t + head, masterTimeline_.MaxTime, Color.red);
                    }
                }
            }
        }

        /// <summary>
        /// 下部のGUI表示
        /// </summary>
        private void DrawFooter() {
            //下から2番目の段に現在選択しているパスを表示
            using (new GUILayout.HorizontalScope()) {
                path_ = EditorGUILayout.TextField(path_, GUILayout.MaxWidth(512f));
                if (GUILayout.Button("...", GUILayout.Width(40))) {
                    //pathの先頭"Assets"を取り除いてpcのUnityプロジェクトまでのパスを入れる
                    string folder = Application.dataPath + path_.Substring("Assets".Length);
                    //選択したフォルダを取得
                    folder = EditorUtility.OpenFolderPanel("select folder", folder, "");
                    //フォルダを選んでいたら場合は先頭のプロジェクトまでのパスを削除して"Assets"を入れる
                    if (!string.IsNullOrEmpty(folder)) {
                        path_ = "Assets" + folder.Substring(Application.dataPath.Length);

                        //全ロード
                        LoadAll("");
                    }
                    GUI.FocusControl(null);
                }
            }
            //一番下の段でボタン類を表示
            using (new GUILayout.HorizontalScope()) {

                if (GUILayout.Button("Load All", GUILayout.Width(160))) {
                    LoadAll("");
                }
                if (GUILayout.Button("Save All", GUILayout.Width(160))) {
                    SaveAll();
                }
                if (GUILayout.Button("Save As", GUILayout.Width(160))) {
                    SaveAs();
                }
            }
        }

        /// <summary>
        /// 時間をKeyにしてイベントの制御を行う
        /// </summary>
        public static float DrawKeyControl(ref Dictionary<float, List<ActEventEdit>> dict, float selTime, float curTime, float maxTime) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Key", GUILayout.MaxWidth(32f));
			//追加
			if (GUILayout.Button("+", GUILayout.MaxWidth(20f))) {
				//キーがまだ無い時だけ新規作成する　
				if (!dict.ContainsKey(curTime)) {
					dict[curTime] = new List<ActEventEdit>();
                    SortEventList<List<ActEventEdit>>(ref dict);
					selTime = curTime;
				}
				GUI.FocusControl(null);
			}
			//削除
			if (GUILayout.Button("-", GUILayout.MaxWidth(20f))) {
				//今表示しているキーがある時だけ削除する
				if (dict.ContainsKey(selTime)) {
					dict.Remove(selTime);
					SortEventList<List<ActEventEdit>>(ref dict);
					foreach (float t in dict.Keys) {
						selTime = t;
						break;
					}
				}

				GUI.FocusControl(null);
			}
			//次のデータ
			if (GUILayout.Button("<", GUILayout.MaxWidth(20f))) {
				SortEventList<List<ActEventEdit>>(ref dict);
				float tmp = -1f;
				foreach (float t in dict.Keys) {
					if (t < selTime) {
						tmp = t;
					} else {
						if (tmp >= 0f) {
							selTime = tmp;
						}
						break;
					}
				}
				GUI.FocusControl(null);
			}
			//前のデータ
			if (GUILayout.Button(">", GUILayout.MaxWidth(20f))) {
				SortEventList<List<ActEventEdit>>(ref dict);
				foreach (float t in dict.Keys) {
					if (t > selTime) {
						selTime = t;
						break;
					}
				}
				GUI.FocusControl(null);
			}

			//表示用に現在表示しているイベントが時系列で何番目になるかを計算
			int count = 0;
			foreach (float t in dict.Keys) {
				if (t == selTime) {
					break;
				}
				count++;
			}
			//表示しているイベントの発生時間表示
			GUILayout.Label( "Time", GUILayout.Width( 32f ) );
			EditorGUI.BeginChangeCheck();
			float newTime = EditorGUILayout.FloatField (selTime, GUILayout.Width (120f));
			if (EditorGUI.EndChangeCheck()) {
				//変えようとしている時間が範囲内であれば変更を許可
				if (newTime >= 0f && newTime <= maxTime) {
					//発生時間を直接変えた場合はイベント情報そのままで時間を移す
					if (dict.ContainsKey(selTime) && !dict.ContainsKey(newTime)) {
						dict[newTime] = dict[selTime];
						dict.Remove(selTime);
						selTime = newTime;
						SortEventList<List<ActEventEdit>>(ref dict);
					}
				}
			}
			//イベントが一つもない場合はその旨を表示
			if (dict.Keys.Count == 0) {
				GUILayout.Label("No Data", GUILayout.Width(160f));
			//時間でまとめたイベントが何個あるかを表示する
			} else {
				GUILayout.Label("Key : " + (count + 1) + "/" + dict.Keys.Count, GUILayout.Width(160f));
			}

			EditorGUILayout.EndHorizontal();

			return selTime;
		}


		/// <summary>
		/// 実行処理
		/// </summary>
		public void Update() {
			//再描画命令を出す（出さないとアニメーションしたときにGUIがなめらかに更新されない）
			Repaint();
			masterTimeline_.Update();
            if(masterTimeline_.IsPlaying){
                SyncSliderMaster(currentEditData_, masterTimeline_);
            }
        }
		const string NEW_FILE_NAME = "NewActAsset";
		private void NewFile() {
			int suffix = 1;
			string filename = NEW_FILE_NAME;
			for (int i = 0, max = files_.Length; i < max; i++) {
				if(files_[i].Equals(filename)) {
					suffix++;
					filename = NEW_FILE_NAME + suffix;
				}
			}

			AddArray(filename);

			allEditData_[filename] = new EditorData();
			currentEditData_ = allEditData_[filename];
			currentEditData_.Init(entitySelect_.EditEntityType);


			selectFile_ = files_.Length - 1;
		}
		/// <summary>
		/// 新規ファイル用に配列拡張
		/// </summary>
		private void AddArray(string filename) {
			//ファイル名配列を拡張
			{
				List<string> temp = new List<string>(files_);
				temp.Add(filename);
				files_ = temp.ToArray();
			}
			//dirtyフラグ配列を拡張
			{
				List<bool> temp = new List<bool>(dirty_);
				temp.Add(true);
				dirty_ = temp.ToArray();
			}
		}

		/// <summary>
		/// 編集するデータを選択
		/// </summary>
		private void SelectFile(int index) {
			string key = files_[index];
			//データがなかった時
			if (!allEditData_.ContainsKey(key)) {
				//ここは来ないはず
				Debug.LogWarning("not found key");
				allEditData_[key] = new EditorData();
				allEditData_[key].entityType = entitySelect_.EditEntityType;
			} else {
				//サポートしてない型が入っていたら無視する
				bool result = entitySelect_.Select(allEditData_[key].entityType);
				if (!result) {
					Debug.LogError("not support type");
					return;
				}
			}
			selectFile_ = index;
			currentEditData_ = allEditData_[key];
            for(int count = 0; count < currentEditData_.editTimeline.Count; count++){
                EditTimeline tline = currentEditData_.editTimeline[count];
                
                foreach (float t in tline.timeEvent.Keys) {
                    tline.selectTime = t;
				    break;
			    }
                if(tline.animationEvent != null){
                    tline.animationEvent.Refresh(entitySelect_.MethodInfoList);
                }
                foreach (var val in tline.timeEvent) {
				    for (int i = 0; i < val.Value.Count; i++) {
					    bool result = val.Value[i].Refresh(entitySelect_.MethodInfoList);
					    //関数が変更されるなどして見つからなかった場合はロード失敗
					    if (!result) {
						    //関数が存在しない場合はAssert出して設定をしないようにする
						    Debug.Assert(false, "not found method:type = " + currentEditData_.entityType);
						    return;
					    }
				    }
			    }
            }

        }

		/// <summary>
		/// 選択しているフォルダの中にあるイベントファイルを全ロード
		/// </summary>
		private void LoadAll(string current) {
			allEditData_ = new Dictionary<string, EditorData>();

			int index = 0;
			files_ = EditorUtil.GetFilenames(path_);
			for (int i = 0; i < files_.Length; i++) {
				Load(files_[i]);
				//指定したファイルが見つかった場合はそれを選択状態にする
				if (current == files_[i]) {
					index = i;
				}
			}
			//ファイルが１つ以上あったら選択状態で表示する
			if(files_.Length > 0)
				SelectFile(index);

			dirty_ = new bool[files_.Length];
		}

		/// <summary>
		/// ロード処理
		/// </summary>
		private void Load(string name) {
			//データをロード
			ActEventData data = AssetDatabase.LoadAssetAtPath(path_ + "/" + name + ".asset", typeof(ScriptableObject)) as ActEventData;
			if (data == null)
				return;

			var loadData = new EditorData();
            //Editor用のプロジェクトの外にある型なのでAssemblyつきじゃないと正しく取得できない
            loadData.Init(System.Type.GetType(data.entityType + "," + EditorSetting.AssemblyName));
            loadData.editTimeline.Clear();

            EditTimeline current = null;
            current = new EditTimeline();
            current.timeline.HeadTime = 0f;

            for (int i = 0; i < data.eventDataList.Count; i++) {
				string method = data.eventDataList[i].method;

				float time = data.eventDataList[i].execTime;

				ActEventEdit edit = new ActEventEdit(entitySelect_.MethodDict[loadData.entityType].MethodInfoDict[method]);
				edit.Load(method, data.eventDataList[i].args, data.eventDataList[i].types, data.eventDataList[i].argsBinary, data.curveList);


                //Entityにアニメーション再生がない場合はnullが入っている
                System.Reflection.MethodInfo animationMethod = entitySelect_.MethodDict[loadData.entityType].PlayAnimationMethod;
                if (animationMethod != null && edit.Data.method == animationMethod.Name) {
                    //タイムラインを増やしてclipをセット
                    current = new EditTimeline();
                    current.timeline.HeadTime = time;
                    current.animationEvent = edit;


					//モデルが既にセットされている場合はclipのロード先を絞る
					string subdir = "";
					if(entityObject_ != null) {
						subdir = entityObject_.name;
					}
                    current.timeline.Clip = EditorSetting.SearchClip((string)edit.Data.args[0], subdir);
                    EditorFuncion.AddClip(loadData.entityType, current.timeline.Clip);

                } else if (edit.Data.method == ActEventTermination.TerminationMethod) {

                    current.timeline.MaxTime = time - current.timeline.HeadTime;
                    loadData.editTimeline.Add(current);
                    current = new EditTimeline();
                    current.timeline.HeadTime = time;

                } else {
                    //イベント発生時間をキーにしてリスト化する
                    float key = time - current.timeline.HeadTime;
                    if (!current.timeEvent.ContainsKey(key)) {
                        current.timeEvent[key] = new List<ActEventEdit>();
				    }
                    current.timeEvent[key].Add(edit);
                }
            }
            //最後のイベントの実行時間がActEventの最長時間
            masterTimeline_.MaxTime = data.eventDataList[data.eventDataList.Count - 1].execTime;

            loadData.TrimKey();
            allEditData_[name] = loadData;
		}
		/// <summary>
		/// セーブ処理（個別指定）
		/// </summary>
		void SaveAs() {
			string defaultName = "";
			if (files_.Length > 0) {
				defaultName = files_[selectFile_];
			}
			string p = EditorUtility.SaveFilePanel("Open Asset...", path_, defaultName, "asset");

			if (p != string.Empty) {
				p = p.Substring(p.IndexOf("Assets")).Replace(".asset", "");
				string[] str = p.Split('/');
				string saveFileName = str[str.Length - 1];
				path_ = p.Remove(p.Length - saveFileName.Length - 1);

				//currentEditData_.entityType = entitySelect_.EditEntityType;
				Save(saveFileName, currentEditData_);

				LoadAll(saveFileName);
			}
		}
		/// <summary>
		/// セーブ処理（全部）
		/// </summary>
		void SaveAll() {
			foreach(KeyValuePair<string, EditorData> val in allEditData_){
				Save(val.Key,  val.Value);
			}
			dirty_ = new bool[files_.Length];
		}
		/// <summary>
		/// セーブ処理
		/// </summary>
		private void Save(string name, EditorData editData) {
			//データがない場合はセーブしない
			if (editData == null)
				return;
			//キーはあるけどレコードがない場合はセーブしない
			bool result = false;
            for(int i = 0; i < editData.editTimeline.Count; i++){
                EditTimeline tl = editData.editTimeline[i];
                if(tl.animationEvent != null) {
                    result = true;
                    break;
                }
                foreach (KeyValuePair<float, List<ActEventEdit>> val in tl.timeEvent) {
				    if (val.Value.Count > 0) {
					    result = true;
					    break;
				    }
			    }
            }
			if (!result){
                Debug.LogError("data save failed: invalid or empty data");

                return;
            }
            ActEventData data = CreateActEventData(editData);

            //セーブ
            EditorUtility.SetDirty(data);
			AssetDatabase.CreateAsset(data, path_ + "/" + name + ".asset");
			AssetDatabase.SaveAssets();

		}
        /// <summary>
        /// 編集用データから保存用データを生成
        /// </summary>
        private ActEventData CreateActEventData(EditorData editData) {
            ActEventData data = ScriptableObject.CreateInstance<ActEventData>();

			List<string> resourceList = new List<string>();
            data.entityType = editData.entityType.FullName;
            for (int count = 0; count < editData.editTimeline.Count; count++) {
                EditTimeline tl = editData.editTimeline[count];


                //アニメーション再生をセット
                if(tl.animationEvent != null) {
                    ActEventData.EventData ev = tl.animationEvent.Save();
					resourceList.AddRange(tl.animationEvent.CreateResourceString(entitySelect_.EditEntityFunc));

					ev.execTime = tl.timeline.HeadTime;
                    data.eventDataList.Add(ev);
                }

                //そのほかのイベントをセット
                foreach (KeyValuePair<float, List<ActEventEdit>> val in tl.timeEvent) {
                    float time = val.Key + tl.timeline.HeadTime;

                    //再生時間を超えた時間にあるイベントは無視
                    if (time > tl.timeline.MaxTime + tl.timeline.HeadTime)
                        continue;

                    for (int i = 0; i < val.Value.Count; i++) {
                        ActEventData.EventData ev = val.Value[i].Save();
						resourceList.AddRange(val.Value[i].CreateResourceString(entitySelect_.EditEntityFunc));
						ev.execTime = time;
                        data.eventDataList.Add(ev);
                    }
                }
                //アニメーションの終端用イベントを仕込む
                data.eventDataList.Add(CreateTeminationData(tl.timeline.MaxTime + tl.timeline.HeadTime));
            }
            //重複したリソースは削除する
            HashSet<string> hash = new HashSet<string>();
            for(int i = 0; i < resourceList.Count; i++) {
                hash.Add(resourceList[i]);
            }
            data.resourceList = new List<string>(hash);

            return data;
        }


        /// <summary>
        /// 終端イベントを生成
        /// </summary>
        private ActEventData.EventData CreateTeminationData(float t) {
			ActEventData.EventData finalData = new ActEventData.EventData();
			finalData.method = ActEventTermination.TerminationMethod;
			//引数はないけど一応
			MethodArgs.SerializeArgs(new object[0], out finalData.types, out finalData.args, out finalData.argsBinary);
			finalData.execTime = t;
			return finalData;
		}

		/// <summary>
		/// イベントを時間でソートし直す
		/// </summary>
		private static void SortEventList<T>(ref Dictionary<float, T> dict){
            List<KeyValuePair<float, T>> l = new List<KeyValuePair<float, T>> (dict);
			l.Sort((a,b)=>{
				if(a.Key > b.Key)
					return 1;
				else if(a.Key == b.Key)
					return 0;
				else
					return -1;
			});
			dict.Clear ();
			for (int i = 0, max = l.Count; i < max; i++) {
				dict [l [i].Key] = l [i].Value;
			}
		}
		/// <summary>
		/// タイムライン表示
		/// </summary>
		public static void DrawTimeLine(Rect sliderRect, float t, float length, Color col) {
			float x = sliderRect.xMin + sliderRect.width * (t / length);
			Rect r = new Rect(x - LineWidth * 0.5f, sliderRect.yMin, LineWidth, sliderRect.yMax - sliderRect.yMin);
			Handles.DrawSolidRectangleWithOutline(r, col, col);
		}

    }

}
