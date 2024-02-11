using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// イベントの編集の個別制御
	/// </summary>
	public class EventEditBase {
		/// <summary>
		/// ロード処理
		/// </summary>
		public virtual void Load(ActEventEdit.EditData data, List<AnimationCurve> curveList) {
		}
		/// <summary>
		/// セーブ処理
		/// </summary>
		public virtual void Save(ref ActEventEdit.EditData data, ref List<AnimationCurve> curveList, ref List<string> resourceList) {
		}
		/// <summary>
		/// GUI表示
		/// </summary>
		public virtual bool DrawGUI(ref ActEventEdit.EditData data, ArgAttribute[] attributes){
			EditorGUI.BeginChangeCheck();
			using (new GUILayout.HorizontalScope()) {
				GUILayout.Space(160);
				using (new GUILayout.VerticalScope()) {
					for (int i = 0; i < data.args.Length; i++) {
						using (new GUILayout.HorizontalScope()) {
							try {
								EditorGUILayout.LabelField(attributes[i].Text, GUILayout.MaxWidth(128f));
								if (attributes[i].Type == typeof(string)) {
									data.args[i] = EditorGUILayout.TextField((string)data.args[i], GUILayout.MaxWidth(256f));
								} else if (attributes[i].Type == typeof(int)) {
									data.args[i] = EditorGUILayout.IntField((int)data.args[i], GUILayout.MaxWidth(128f));
								} else if (attributes[i].Type == typeof(long)) {
									data.args[i] = EditorGUILayout.LongField((long)data.args[i], GUILayout.MaxWidth(128f));
								} else if (attributes[i].Type == typeof(float)) {
									data.args[i] = EditorGUILayout.FloatField((float)data.args[i], GUILayout.MaxWidth(128f));
								} else if (attributes[i].Type == typeof(bool)) {
									data.args[i] = EditorGUILayout.Toggle((bool)data.args[i], GUILayout.MaxWidth(32f));
								} else if (attributes[i].Type == typeof(Vector3)) {
									data.args[i] = EditorGUILayout.Vector3Field("", (Vector3)data.args[i], GUILayout.MaxWidth(192));
									//enumの場合popuplist使う
								} else if (attributes[i].Type.IsEnum) {
									string[] names = Enum.GetNames(attributes[i].Type);
									string str = Enum.ToObject(attributes[i].Type, data.args[i]).ToString();


									int sel = Array.IndexOf(names, str);
									sel = EditorGUILayout.Popup(sel, names, GUILayout.MaxWidth(128f));
									data.args[i] = Enum.Parse(attributes[i].Type, names[sel]);
								}
							} catch (Exception e) {
								Debug.LogError(e);
								Debug.LogError("引数のキャストエラーが発生したのでデフォルト値を代入します");


								data.args = ActEventEdit.CreateDefaultValueArray(attributes);

								return false;
							}

						}
					}
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



	/// <summary>
	/// イベントの編集の個別制御
	/// </summary>
	public class EventEditCollision : EventEditBase{
		AnimationCurve sizeCurve_;
		/// <summary>
		/// ロード処理
		/// </summary>
		public override void Load(ActEventEdit.EditData data, List<AnimationCurve> curveList) {
			int sizeCurveIndex = (int)data.args[3];
			sizeCurve_ = curveList[sizeCurveIndex];

		}
		/// <summary>
		/// セーブ処理
		/// </summary>
		public override void Save(ref ActEventEdit.EditData data, ref List<AnimationCurve> curveList, ref List<string> resourceList) {
			//使用しているカーブを保存用に追加
			curveList.Add(sizeCurve_);
			//3番目の引数にカーブのIndexを保存
			data.args[3] = curveList.Count;
		}
		/// <summary>
		/// GUI表示
		/// </summary>
		public override bool DrawGUI(ref ActEventEdit.EditData data, ArgAttribute[] attributes){
			return base.DrawGUI(ref data, attributes);
		}
	}
}
