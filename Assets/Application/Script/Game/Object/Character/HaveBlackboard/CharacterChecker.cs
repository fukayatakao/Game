using System;
using System.Collections.Generic;

namespace Project.Game {
	/// <summary>
	/// キャラクターの行動評価関数
	/// </summary>
	[UnityEngine.Scripting.Preserve]
	public static class CharacterChecker {
		public delegate bool CheckMethod(CharacterEntity entity);
		public static Dictionary<string, CheckMethod> CheckMethodDict;
		/// <summary>
		/// クラス内にある関数のキャッシュを作る
		/// </summary>
		public static void Initiate() {
			Type evaluateClass = typeof(CharacterChecker);
			CheckMethodDict = new Dictionary<string, CheckMethod>();
			System.Reflection.MethodInfo[] methods = evaluateClass.GetMethods();
			for (int i = 0, max = methods.Length; i < max; i++) {
				System.Reflection.ParameterInfo[] param = methods[i].GetParameters();
				if (param.Length == 1 && param[0].ParameterType == typeof(CharacterEntity) && methods[i].ReturnType == typeof(bool)) {
					CheckMethod evaluate = (CheckMethod)Delegate.CreateDelegate(typeof(CheckMethod), methods[i]);
					CheckMethodDict.Add(methods[i].Name, evaluate);
				}
			}
		}

		/// <summary>
		/// 関数情報をクリア
		/// </summary>
		public static void Terminate() {
			CheckMethodDict = null;
		}

		/// <summary>
		/// 速度が走る以上になっているか
		/// </summary>
		public static bool CheckSpeed(CharacterEntity entity) {
			return (entity.HaveUnitParam.Physical.CurrentSpeed > entity.HaveUnitParam.Physical.MaxSpeed * GameConst.Battle.CRUISE_RATE);
		}
	}
}
