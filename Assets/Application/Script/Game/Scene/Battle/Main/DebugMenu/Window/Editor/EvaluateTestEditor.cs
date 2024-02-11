#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Project.Lib;
using Project.Game;
using Project.Editor;

[UnityEditor.CustomEditor(typeof(EvaluateTest))]
public class EvaluateTestEditor : Editor {
	int select_;
	string[] label = new string[] { "aaa,", "bbb", "ccc" };
	EvaluateTest evaluate_;
	const int MethodArgsFontSize = 16;


	ArgAttribute[] argAttributes_;
	Dictionary<int, string[]> argEnumNames_ = new Dictionary<int, string[]>();
	object[] args_;
	bool result_;

	private void Awake() {
		evaluate_ = (EvaluateTest)base.target;


		var list = new List<string>();
		for (int i = 0, max = evaluate_.EvaluateMethods.Count; i < max; i++) {
			list.Add(evaluate_.EvaluateMethods[i].Name);
		}

		label = list.ToArray();
	}

	public override void OnInspectorGUI() {
		EditorGUILayout.ObjectField(evaluate_.entity.gameObject, typeof(GameObject), true);

		EditorGUI.BeginChangeCheck();
		select_ = EditorGUILayout.Popup(select_, label);

		if (EditorGUI.EndChangeCheck()) {
			System.Reflection.MethodInfo method = evaluate_.EvaluateMethods[select_];

			//通常関数と真偽切り替え用関数で分岐
			if (Attribute.GetCustomAttribute(method, typeof(FunctionAttribute)) as FunctionAttribute != null) {
				FunctionAttribute funcAttributes_ = Attribute.GetCustomAttribute(method, typeof(FunctionAttribute)) as FunctionAttribute;
				//引数の属性を拾う
				argAttributes_ = MethodUtil.CreateArgAttributeArray(method);

			} else {
				CheckFunctionAttribute funcAttributes_ = Attribute.GetCustomAttribute(method, typeof(CheckFunctionAttribute)) as CheckFunctionAttribute;
				argAttributes_ = new ArgAttribute[] { new ArgAttribute(0, typeof(bool), "真偽値", true) };
			}

			for (int i = 0; i < argAttributes_.Length; i++) {
				argEnumNames_[i] = FieldAttribute.GetFields(argAttributes_[i].Type);
			}
			args_ = CreateDefaultValueArray();
		}
		DrawArgGUI(ref args_);
		DrawExecGUI(ref args_);
	}

	/// <summary>
	/// 引数GUI表示
	/// </summary>
	public void DrawExecGUI(ref object[] args) {
		GUILayout.Label("");
		if (GUILayout.Button("チェック")) {
			System.Reflection.MethodInfo method = evaluate_.EvaluateMethods[select_];
			object[] invokeArgs = new object[] { evaluate_.TargetCharacter, args };
			result_ = (bool)method.Invoke(null, invokeArgs);
		}
		GUILayout.Label("結果:" + result_);
	}

	/// <summary>
	/// デフォルト値配列を生成
	/// </summary>
	object[] CreateDefaultValueArray() {
		return MethodUtil.CreateDefaultValueArray(argAttributes_);
	}

	/// <summary>
	/// 引数GUI表示
	/// </summary>
	public bool DrawArgGUI(ref object[] args) {
		if (args == null)
			return false;
		//関数に変更があったなどで引数が一致しない場合
		if (args.Length != argAttributes_.Length) {
			Debug.LogError("引数の数が一致しないのでデフォルト値で初期化します:" );
			args = MethodUtil.CreateDefaultValueArray(argAttributes_);
		}
		EditorGUI.BeginChangeCheck();
		for (int i = 0; i < args.Length; i++) {
			try {
				EditorGUILayout.LabelField(argAttributes_[i].Text, new GUIStyle("Box") { alignment = TextAnchor.MiddleCenter, fontSize = MethodArgsFontSize, normal = new GUIStyleState() { textColor = Color.white } });
				if (argAttributes_[i].Type == typeof(string)) {
					args[i] = EditorGUILayout.TextField((string)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
				} else if (argAttributes_[i].Type == typeof(int)) {
					args[i] = EditorGUILayout.IntField((int)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
				} else if (argAttributes_[i].Type == typeof(long)) {
					args[i] = EditorGUILayout.LongField((long)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
				} else if (argAttributes_[i].Type == typeof(float)) {
					args[i] = EditorGUILayout.FloatField((float)args[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
				} else if (argAttributes_[i].Type == typeof(bool)) {
					args[i] = EditorGUILayout.Toggle((bool)args[i], new GUIStyle("Toggle") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
				} else if (argAttributes_[i].Type == typeof(Vector3)) {
					args[i] = EditorGUILayout.Vector3Field("", (Vector3)args[i]);
					//enumの場合popuplist使う
				} else if (argAttributes_[i].Type.IsEnum) {
					string[] names = Enum.GetNames(argAttributes_[i].Type);
					string str = Enum.ToObject(argAttributes_[i].Type, args[i]).ToString();


					int sel = Array.IndexOf(names, str);
					sel = EditorGUILayout.Popup(sel, argEnumNames_[i], new GUIStyle("Button") { alignment = TextAnchor.MiddleLeft, fontSize = MethodArgsFontSize });
					args[i] = Enum.Parse(argAttributes_[i].Type, names[sel]);
				}
			} catch (Exception e) {
				Debug.LogError(e);
				Debug.LogError("引数のキャストエラーが発生したのでデフォルト値を代入します:");


				args = MethodUtil.CreateDefaultValueArray(argAttributes_);

				return false;
			}
		}
		//変更有ったのでtrueを返す
		if (EditorGUI.EndChangeCheck()) {
			return true;
		} else {
			return false;
		}
	}

}
#endif
