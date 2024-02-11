using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// イベントの編集制御
	/// </summary>
	public class ActEventEdit {
		//編集用データ
		public class EditData {
			public string method;
			public object[] args;

			//編集可能な形式にする
			public void Load(string name, string[] argsString, List<int> types, byte[] binary) {
				method = name;
				args = MethodArgs.DeserializeArgs(argsString, types, binary);
			}

			//保存形式に変換
			public ActEventData.EventData Save() {
				ActEventData.EventData savedata = new ActEventData.EventData();
				savedata.method = method;
				MethodArgs.SerializeArgs(args, out savedata.types, out savedata.args, out savedata.argsBinary);
				return savedata;
			}
		}
		ArgAttribute[] argAttributes_;
		//CurveAttribute curveAttribute_;
		ResourceAttribute[] resourceAttributes_;
		
		EditData data_ = new EditData();
        public EditData Data{ get { return data_; } }
		int selectMethod_ = 0;

		EventEditBase eventEdit_ = new EventEditBase();

		//@note 引数が多いとか一部の引数を表示on/offしたり、シーン上のGameObjectにアクセスしたりとか特殊な制御を行いたい場合は個別に編集制御用クラスを作ってそこで処理する
		//個別の編集制御を行うイベント名と制御クラスの組み合わせデータ
		Dictionary<string, System.Type> customEditDict_ = new Dictionary<string, System.Type>() {
			//@note 関数名を文字列直で指定すると関数名を変えた時にバグる。intellisenseの支援も受けれるので回りくどいけどリフレクションを使う。
		//	{((System.Action<DestructibleEntity, object[]>)DestructibleEvent.SetAttackCollision).Method.Name, typeof(EventEditCollision) },
		};

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public ActEventEdit(ActEventEdit src) {
			//シャローでオッケー
			argAttributes_ = src.argAttributes_;
			resourceAttributes_ = src.resourceAttributes_;
			eventEdit_ = src.eventEdit_;

			//ここからディープコピー
			data_.method = src.data_.method;
			List<int> types;
			string[] args;
			byte[] binary;
			MethodArgs.SerializeArgs(src.data_.args, out types, out args, out binary);
			data_.args = MethodArgs.DeserializeArgs(args, types, binary);

			selectMethod_ = src.selectMethod_;
		}



		public ActEventEdit(System.Reflection.MethodInfo methodInfo) {
			//終端関数の場合はnullで入ってくる
			if (methodInfo == null)
				return;
			Create(methodInfo);
		}

		/// <summary>
		/// 新規データとして作る
		/// </summary>
		public void Create(System.Reflection.MethodInfo methodInfo) {
			//個別の編集制御がある関数の場合
			if (customEditDict_.ContainsKey(methodInfo.Name)) {
				//@note 文字列からインスタンス生成。重いのでゲーム内での使用は禁止。
				eventEdit_ = (EventEditBase)Activator.CreateInstance(customEditDict_[methodInfo.Name]);
			}
			//属性を拾う
			argAttributes_ = CreateArgAttributeArray(methodInfo);
            resourceAttributes_ = CreateResourceAttribute(methodInfo);



            data_.method = methodInfo.Name;
			data_.args = CreateDefaultValueArray(argAttributes_);
		}

		/// <summary>
		/// 引数の設定を再設定する
		/// </summary>
		public bool Refresh(List<System.Reflection.MethodInfo> methodInfoList) {
			selectMethod_ = -1;
			//選択Indexを関数名から再設定する
			for (int i = 0; i < methodInfoList.Count; i++) {
				if (data_.method == methodInfoList[i].Name) {
					selectMethod_ = i;
					break;
				}
			}
			if (selectMethod_ < 0)
				return false;
			//引数の属性を拾う
			argAttributes_ = CreateArgAttributeArray(methodInfoList[selectMethod_]);
            resourceAttributes_ = CreateResourceAttribute(methodInfoList[selectMethod_]);
            return true;
		}

		/// <summary>
		/// ロード処理
		/// </summary>
		public void Load(string name, string[] argsString, List<int> types, byte[] binary, List<AnimationCurve> curveList) {
			data_.Load(name, argsString, types, binary);

			eventEdit_.Load(data_, curveList);
		}
		/// <summary>
		/// セーブ処理
		/// </summary>
		public List<string> CreateResourceString(System.Func<int, object, string> resolver) {
			List<string> list = new List<string>();
			for (int i = 0; i < resourceAttributes_.Length; i++) {
				//引数が文字列型以外だとエラーが出る
				int index = resourceAttributes_[i].Index;
				int type = resourceAttributes_[i].Path;

				string path = resolver(type, data_.args[index]); ;
				list.Add(path);
			}

			return list;
		}


		/// <summary>
		/// セーブ処理
		/// </summary>
		public ActEventData.EventData Save() {
			return data_.Save();
		}


		/// <summary>
		/// GUI表示
		/// </summary>
		public bool DrawGUI(string[] methodArray, List<System.Reflection.MethodInfo> MethodInfoList){
			EditorGUI.BeginChangeCheck();
			selectMethod_ = EditorGUILayout.Popup(selectMethod_, methodArray, GUILayout.Width(200f));
			//関数を変えた=イベントを切り替えたときは引数をリセット
			if (EditorGUI.EndChangeCheck()) {
				Create(MethodInfoList[selectMethod_]);
				//変更有ったのでtrueを返す
				return true;
			}

			return eventEdit_.DrawGUI(ref data_, argAttributes_);
		}

        /// <summary>
        /// 直接引き数をセット
        /// </summary>
        public void SetArgs(object[] args) {
            data_.args = args;
        }

		/// <summary>
		/// 引数編集用のカスタム属性を取得する
		/// </summary>
		private static ArgAttribute[] CreateArgAttributeArray(System.Reflection.MethodInfo methodInfo) {
			//引数の属性を拾う
			Attribute[] attr = Attribute.GetCustomAttributes(methodInfo, typeof(ArgAttribute));
			ArgAttribute[] attributes = new ArgAttribute[attr.Length];

			//GetCustomAttributesしたときの配列の順番と引数の順番は一致しないので明示的に並び替える
			for(int i = 0; i < attr.Length; i++){
				ArgAttribute temp = attr [i] as ArgAttribute;
				attributes [temp.Index] = temp;
			}

			return attributes;
		}
		/// <summary>
		/// 曲線データ保存用のカスタム属性を取得する
		/// </summary>
		private static CurveAttribute CreateCurveAttribute(System.Reflection.MethodInfo methodInfo) {
			return (CurveAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(CurveAttribute));
		}
		/// <summary>
		/// リソース情報保存用のカスタム属性を取得する
		/// </summary>
		private static ResourceAttribute[] CreateResourceAttribute(System.Reflection.MethodInfo methodInfo) {
            //引数の属性を拾う
            return (ResourceAttribute[])Attribute.GetCustomAttributes(methodInfo, typeof(ResourceAttribute));
		}

		/// <summary>
		/// デフォルト値配列を生成
		/// </summary>
		public static object[] CreateDefaultValueArray(ArgAttribute[] attributes) {
			object[] args = new object[attributes.Length];
			for (int i = 0; i < attributes.Length; i++) {
				args[attributes[i].Index] = attributes[i].Value;
			}
			return args;
		}
	}
}
