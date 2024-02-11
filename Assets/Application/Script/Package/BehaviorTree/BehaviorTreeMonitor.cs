using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Lib {
#if UNITY_EDITOR
	public class BehaviorTreeMonitor : MonoBehaviour {
		public System.Func<int> func;
		public BehaviorTreeData Data;

		public void Init<T>(BehaviorTree<T> tree) {
			func = tree.GetCurrentId;
			Data = tree.Data;
		}

	}
#endif
}
