using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
#if UNITY_EDITOR
	public class EvaluateTest: MonoBehaviour {
		private CharacterEntity character_;
		public CharacterEntity TargetCharacter { get { return character_; } }

		static List<System.Reflection.MethodInfo> evaluateMethods_ = new List<System.Reflection.MethodInfo>();
		public List<System.Reflection.MethodInfo> EvaluateMethods { get { return evaluateMethods_; } }

		[SerializeField]
		public GameObject entity;

		static readonly System.Type evaluateClass = typeof(CharacterEvaluate);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void Create(CharacterEntity character) {
			character_ = character;
			entity = character_.gameObject;

			//CharacterEvaluateの関数をキャッシュ
			System.Reflection.MethodInfo[] methods = evaluateClass.GetMethods();
			for (int i = 0, max = methods.Length; i < max; i++) {
				System.Reflection.ParameterInfo[] param = methods[i].GetParameters();
				if (param.Length == 2 && param[0].ParameterType == typeof(CharacterEntity) && param[1].ParameterType == typeof(object[]) && methods[i].ReturnType == typeof(bool)) {
					evaluateMethods_.Add(methods[i]);

					//BehaviorTree<CharacterEntity>.EvaluateMethod evaluate = (BehaviorTree<CharacterEntity>.EvaluateMethod)Delegate.CreateDelegate(typeof(BehaviorTree<CharacterEntity>.EvaluateMethod), methods[i]);
					//evaluateMethods_.Add(methods[i].Name, evaluate);
				}
			}

		}

	}
#endif
}
