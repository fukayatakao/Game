using System;
using System.Collections.Generic;
using UnityEditor;

namespace Project.Lib {
	/// <summary>
	/// 編集するEntityの切り替え用クラス
	/// </summary>
	public class ActEntitySelect {

        public class EntityMethod {
            //イベント関数のリスト
            public List<System.Reflection.MethodInfo> MethodInfoList = new List<System.Reflection.MethodInfo>();
		    public string[] MethodNameArray = new string[0];
            public System.Reflection.MethodInfo PlayAnimationMethod;
			//検索しやすくするためにDictionaryも用意する
			public Dictionary<string, System.Reflection.MethodInfo> MethodInfoDict = new Dictionary<string, System.Reflection.MethodInfo>();

            public void Create(EditorSetting.EntitySetting setting) {
                MethodInfoList = CreateMethodList(setting.entityType, setting.eventType);


				for (int i = 0, max = MethodInfoList.Count; i < max; i++) {
					MethodInfoDict[MethodInfoList[i].Name] = MethodInfoList[i];
				}
				//終端関数を別途登録
				MethodInfoDict[ActEventTermination.TerminationMethod] = null;


				PlayAnimationMethod = null;
                //アニメーション再生を特別扱いするためにリストから拾う
                for (int i = 0; i < MethodInfoList.Count; i++) {
					if(MethodInfoList[i].Name == EditorFuncion.PlayAnimationName) {
						PlayAnimationMethod = MethodInfoList[i];
						MethodInfoList.RemoveAt(i);
						break;
					}
                }

				MethodNameArray = MethodInfoList.ConvertAll(x => ((FunctionAttribute)Attribute.GetCustomAttribute(x, typeof(FunctionAttribute))).Text).ToArray();
			}
			/// <summary>
			/// 関数リストを取得
			/// </summary>
			private static List<System.Reflection.MethodInfo> CreateMethodList(System.Type entityType, Type EventClass) {
                List<System.Reflection.MethodInfo> method = new List<System.Reflection.MethodInfo>();
                System.Reflection.MethodInfo[] list = EventClass.GetMethods();
                for (int i = 0, max = list.Length; i < max; i++) {
                    System.Reflection.ParameterInfo[] param = list[i].GetParameters();
                    if (param.Length == 2 && param[0].ParameterType == entityType && param[1].ParameterType == typeof(object[])) {
                        method.Add(list[i]);
                    }
                }
                return method;
            }


        }

        public Dictionary<System.Type, EntityMethod> MethodDict = new Dictionary<Type, EntityMethod>();

        //イベント関数のリスト
        public List<System.Reflection.MethodInfo> MethodInfoList = new List<System.Reflection.MethodInfo>();
        public string[] MethodNameArray = new string[0];
        public System.Reflection.MethodInfo PlayAnimationMethod;

        //現在選択中のEntityタイプ
        int selectIndex_ = 0;
		//現在編集中のEntityタイプ
		public System.Type EditEntityType { get { return EditorSetting.EntitySettingList[selectIndex_].entityType; } }
		public System.Func<int, object, string> EditEntityFunc { get { return EditorSetting.EntitySettingList[selectIndex_].createResourcePath; } }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ActEntitySelect() {
            for (int i = 0; i < EditorSetting.EntitySettingList.Count; i++) {
                EntityMethod method = new EntityMethod();
                method.Create(EditorSetting.EntitySettingList[i]);
                MethodDict[EditorSetting.EntitySettingList[i].entityType] = method;
            }
        }

        ///
        public void Init() {
			selectIndex_ = 0;
			Select(selectIndex_);
		}

		/// <summary>
		/// 表示処理
		/// </summary>
		public bool DrawGUI() {
			//Entityタイプの設定
			EditorGUI.BeginChangeCheck();
			selectIndex_ = EditorGUILayout.Popup("Entity", selectIndex_, EditorSetting.EntitySettingList.ConvertAll(x => x.entityType.Name).ToArray());
			//変更有った時
			if (EditorGUI.EndChangeCheck()) {
				Select(selectIndex_);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Entityのタイプを変えた時の処理
		/// </summary>
		public bool Select(System.Type type) {
			for (int i = 0; i < EditorSetting.EntitySettingList.Count; i++) {
				if (type == EditorSetting.EntitySettingList[i].entityType) {
					selectIndex_ = i;
					Select(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Entityのタイプを変えた時の処理
		/// </summary>
		private void Select(int index) {
            System.Type type = EditorSetting.EntitySettingList[index].entityType;

            MethodInfoList = MethodDict[type].MethodInfoList;
            MethodNameArray = MethodDict[type].MethodNameArray;
            PlayAnimationMethod = MethodDict[type].PlayAnimationMethod;
        }
    }
}
