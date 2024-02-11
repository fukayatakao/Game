using UnityEditor;
using UnityEngine;

namespace Project.Lib {
	public class SearchPrefabWindow : EditorWindow {
		//シングルトン
		private static SearchPrefabWindow instance_;
		//検索するフォルダ
		private string path_;
		//Loadボタンを押したときの処理
		private System.Action<string> onSelect_;



		private string searchText_="";
		private string searchExt_ ="";
		private string[] strList;
		private int currentIndex = 0;

		Vector2 scrollPos_ = Vector2.zero;
		/// <summary>
		/// EditorWindowを開く(デバッグ用)
		/// </summary>
		[MenuItem("Editor/Sample/SearchPrefabTest")]
		public static void Open() {
			Open("Assets", "", null);
		}

		/// <summary>
		/// 検索ウィンドウを開く
		/// </summary>
		public static SearchPrefabWindow Open(string path, string ext, System.Action<string> onSelect) {
			//インスタンスが作られていなかったら生成
			if (instance_ == null) {
				instance_ = CreateInstance<SearchPrefabWindow>();
			}
			instance_.ShowUtility();
			instance_.Init(path, ext, onSelect);
			instance_.Show();
			instance_.Focus();
			return instance_;
		}
		/// <summary>
		/// 初期設定
		/// </summary>
		private void Init(string path, string ext, System.Action<string> onSelect) {
			instance_ = this;

			path_ = Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length) + path;
			path_.TrimEnd('/', '\\');
			onSelect_ = onSelect;
			searchExt_ = ext;
			if (!searchExt_.StartsWith(".")) {
				searchExt_ = "." + searchExt_;
			}
			strList = CreateFileList(path_, "", searchExt_);
		}

		private void Update() {
		}
		/// <summary>
		/// インスタンス破棄時処理
		/// </summary>
		public void OnDestroy() {
			instance_ = null;
		}

		/// <summary>
		/// GUI表示
		/// </summary>
		public void OnGUI() {
			GUILayout.Label("path:" + path_);

			EditorGUI.BeginChangeCheck();
			searchText_ = EditorUtil.SearchField(searchText_);
			if (EditorGUI.EndChangeCheck()) {
				strList = CreateFileList(path_, searchText_, searchExt_);
			}
			EditorUtil.DrawLine();

			scrollPos_ = GUILayout.BeginScrollView(scrollPos_);
			currentIndex = EditorUtil.SelectList(strList, currentIndex);
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load", GUILayout.Width(150))) {
				instance_.Close();
				if (onSelect_ != null) {
					string p = path_.Substring(Application.dataPath.Length - "Assets".Length);
					onSelect_(p + "/" + strList[currentIndex] + searchExt_);
				}
			}
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// フォルダ下にあるファイル名一覧を取得
		/// </summary>
		private static string[] CreateFileList(string path, string text, string ext) {
			string[] list = System.IO.Directory.GetFiles(path, "*" + text + "*" + ext, System.IO.SearchOption.TopDirectoryOnly);
			for (int i = 0, max = list.Length; i < max; i++) {
				list[i] = System.IO.Path.GetFileNameWithoutExtension(list[i]);
			}

			return list;
		}

	}
}


/*namespace SearchBoxTest
{
    using UnityEngine;
    using UnityEditor;

    public class SearchBoxTestWindow : EditorWindow
    {
        private static GUIStyle toolbarSearchField;
        private static GUIStyle toolbarSearchFieldCancelButton;
        private static GUIStyle toolbarSearchFieldCancelButtonEmpty;
        private string text = "";

        [MenuItem("EditorWindow/SearchBoxTest")]
        static void Open()
        {
            // ウィンドウを開く
            GetWindow<SearchBoxTestWindow>();
        }

        void OnGUI()
        {
			EditorGUI.BeginChangeCheck();
			// 検索ボックスを表示
			//this.text = SearchField(this.text);
			text = Project.Lib.DebugUtil.SearchField(text);
			if (EditorGUI.EndChangeCheck()) {

				Debug.Log(text);
			}

        }
		/// <summary>
		/// 検索ボックス
		/// </summary>
        static string SearchField(string text)
        {
            if (toolbarSearchField == null) { toolbarSearchField = GUI.skin.FindStyle("ToolbarSeachTextField"); }
            if (toolbarSearchFieldCancelButton == null) { toolbarSearchFieldCancelButton = GUI.skin.FindStyle("ToolbarSeachCancelButton"); }
            if (toolbarSearchFieldCancelButtonEmpty == null) { toolbarSearchFieldCancelButtonEmpty = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty"); }

			Rect rect = GUILayoutUtility.GetRect(16f, 24f, 16f, 16f, new GUILayoutOption[]
            {
                GUILayout.Width(250f) // 検索ボックスのサイズ
            });
            rect.x += 4f;
            rect.y += 4f;

			Rect buttonRect = rect;
            buttonRect.x += rect.width;
            buttonRect.width = 14f;

			//検索文字列
            text = EditorGUI.TextField(rect, text, toolbarSearchField);
			//空文字列の場合はキャンセルボタン表示なし
            if (text == "")
            {
                GUI.Button(buttonRect, GUIContent.none, toolbarSearchFieldCancelButtonEmpty);
            }
			//キャンセルボタン表示
            else
            {
				//キャンセルボタン
                if (GUI.Button(buttonRect, GUIContent.none, toolbarSearchFieldCancelButton))
                {
                    text = "";
                    GUIUtility.keyboardControl = 0;
                }
            }

            return text;
        }

		/// <summary>
		/// 検索エリア表示
		/// </summary>
        static string ToolbarSearchField(Rect position, string text, bool showWithPopupArrow)
        {
            Rect position3 = position;
            position3.x += position.width;
            position3.width = 14f;

			//検索文字列
            text = EditorGUI.TextField(position, text, toolbarSearchField);
			//空文字列の場合はキャンセルボタン表示なし
            if (text == "")
            {
                GUI.Button(position3, GUIContent.none, toolbarSearchFieldCancelButtonEmpty);
            }
			//キャンセルボタン表示
            else
            {
				//キャンセルボタン
                if (GUI.Button(position3, GUIContent.none, toolbarSearchFieldCancelButton))
                {
                    text = "";
                    GUIUtility.keyboardControl = 0;
                }
            }

            return text;
        }

    }
} */

