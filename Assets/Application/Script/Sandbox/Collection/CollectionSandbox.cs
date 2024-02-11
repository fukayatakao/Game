using System.Collections.Generic;
using UnityEngine;

public class CollectionSandbox : MonoBehaviour {

	List<int> list = new List<int>() { 1, 2, 3, 4, 5 };

	public void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			IReadOnlyCollection<int> l = null;
			for (int i = 0; i < 10000; i++) {
				l = list;
			}
			foreach (int i in l) {
				Debug.Log(i);
			}

			System.Collections.ObjectModel.ReadOnlyCollection<int> coll = null;
			for (int i = 0; i < 10000; i++) {
				coll = list.AsReadOnly();
			}
			foreach (int i in coll) {
				Debug.Log(i);
			}
		}
	}

}
