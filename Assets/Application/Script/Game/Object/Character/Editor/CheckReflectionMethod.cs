using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Project.Lib;

namespace Project.Game {
	public class CheckReflectionMethod {
		const string ACTION_PATH = "Assets/Application/Addressable/Character/Action";
		const string METHOD_CACHE_NAME = "methodCache_";                     //キャッシュのメンバ変数名


		[MenuItem("Editor/Check/CharacterAct")]
		public static void CharacterAct() {
			Debug.Log("check CharacterAct");
			EntityActEvent<CharacterEntity>.Cache.Initiate(typeof(CharacterEvent));

			//使っている関数名を取得
			HashSet<string> useMethod = new HashSet<string>();
			string[] dirs = EditorUtil.GetDirectories(ACTION_PATH);
			for(int i = 0; i < dirs.Length; i++) {
				string[] files = EditorUtil.GetFilenames(ACTION_PATH + "/" + dirs[i]);
				for(int j = 0; j < files.Length; j++) {
					string p = ACTION_PATH + "/" + dirs[i] + "/" + files[j];
					//データをロード
					ActEventData data = AssetDatabase.LoadAssetAtPath(p + ".asset", typeof(ScriptableObject)) as ActEventData;
					for(int k = 0; k < data.eventDataList.Count; k++) {
						useMethod.Add(data.eventDataList[k].method);
					}
				}
			}

			//privateをこじ開ける
			System.Reflection.FieldInfo fieldInfo = typeof(ActEventCache<CharacterEntity>).GetField(METHOD_CACHE_NAME, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			var methods = fieldInfo.GetValue(EntityActEvent<CharacterEntity>.Cache) as Dictionary<string, ActEventCache<CharacterEntity>.MethodFunc>;


			//プログラム的には存在しないが、データ上は使用している関数はエラー
			foreach (string method in useMethod) {
				if (methods.ContainsKey(method))
					continue;

				Debug.LogError("event method not found:" + method);
			}
			//データ上使っていないが関数は存在する場合は警告
			foreach (string method in methods.Keys) {
				if (useMethod.Contains(method))
					continue;

				Debug.LogWarning("event method not used:" + method);
			}

			EntityActEvent<CharacterEntity>.Cache.Terminate();
			Debug.Log("finish CharacterAct");

		}


		const string BEHAVIOUR_PATH = "Assets/Application/Addressable/Character/AI";

		const string CACHE_NAME = "cache_";                     //キャッシュのメンバ変数名
		const string EVALUATE_NAME = "evaluateMethods_";        //評価関数のdictionaryメンバ変数名
		const string ORDER_NAME = "orderMethods_";              //実行関数のdictionaryメンバ変数名

		[MenuItem("Editor/Check/CharacterBehaviour")]
		public static void CharacterBehaviour() {
			Debug.Log("check CharacterBehaviour");
			BehaviorSequence<CharacterEntity>.Initiate(typeof(CharacterEvaluate), typeof(CharacterOrder));

			//privateをこじ開ける
			System.Reflection.FieldInfo fieldInfo = typeof(BehaviorSequence<CharacterEntity>).GetField(CACHE_NAME, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var obj = fieldInfo.GetValue(null);
			var evaluateField = obj.GetType().GetField(EVALUATE_NAME, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var evaluate = evaluateField.GetValue(obj) as Dictionary<string, Decision<CharacterEntity>.EvaluateFunc>;
			var orderField = obj.GetType().GetField(ORDER_NAME, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var order = orderField.GetValue(obj) as Dictionary<string, Decision<CharacterEntity>.OrderFunc>;


			//実際に使用しているメソッド名
			HashSet<string> useEvaluate = new HashSet<string>();
			HashSet<string> useOrder = new HashSet<string>();
			string[] files = EditorUtil.GetFilenames(BEHAVIOUR_PATH);
			for(int i = 0, max = files.Length; i < max; i++) {
				//データをロード
				BehaviorSequenceData data = AssetDatabase.LoadAssetAtPath(BEHAVIOUR_PATH + "/" + files[i] + ".asset", typeof(ScriptableObject)) as BehaviorSequenceData;

				for(int j = 0; j < data.StateDataList.Count; j++) {
					for (int k = 0; k < data.StateDataList[j].EnterDecision.Count; k++) {
						useEvaluate.Add(data.StateDataList[j].EnterDecision[k].EvaluateFuncNo);
						useOrder.Add(data.StateDataList[j].EnterDecision[k].OrderFuncNo);
					}
					for (int k = 0; k < data.StateDataList[j].ExecuteDecision.Count; k++) {
						useEvaluate.Add(data.StateDataList[j].ExecuteDecision[k].EvaluateFuncNo);
						useOrder.Add(data.StateDataList[j].ExecuteDecision[k].OrderFuncNo);
					}
					for (int k = 0; k < data.StateDataList[j].ExitDecision.Count; k++) {
						useEvaluate.Add(data.StateDataList[j].ExitDecision[k].EvaluateFuncNo);
						useOrder.Add(data.StateDataList[j].ExitDecision[k].OrderFuncNo);
					}

				}
			}
			//プログラム的には存在しないが、データ上は使用している関数はエラー
			foreach (string method in useEvaluate) {
				if (evaluate.ContainsKey(method))
					continue;

				Debug.LogError("evaluate method not found:" + method);
			}
			foreach (string method in useOrder) {
				if (order.ContainsKey(method))
					continue;

				Debug.LogError("order method not found:" + method);
			}

			//データ上使っていないが関数は存在する場合は警告
			foreach (string method in evaluate.Keys) {
				if (useEvaluate.Contains(method))
					continue;

				Debug.LogWarning("evaluate method not used:" + method);

			}
			foreach (string method in order.Keys) {
				if (useOrder.Contains(method))
					continue;

				Debug.LogWarning("order method not used:" + method);

			}

			BehaviorSequence<CharacterEntity>.Terminate();
			Debug.Log("finish CharacterBehaviour");
		}



	}


}
