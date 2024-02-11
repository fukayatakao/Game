using Project.Lib;
using System;
using UnityEngine;

public class EntityTest : MonoPretender {
}

public class AddressableSandbox : MonoBehaviour {
	GameObject obj;
	EntityTest test;
	private void Start() {
		obj = new GameObject();
		test = MonoPretender.Create<EntityTest>(obj);
	}
	public void ttt(GameObject t) {
		t = null;
	}
	// Update is called once per frame
	void Update()
    {

		//Debug.Log(AddressableAssistOld.DownloadProgress());

		if (Input.GetKeyDown(KeyCode.S)) {
			//comp.release();
			ttt(obj);
			GameObject.Destroy(obj);
			test = null;
			GC.Collect();
		}
	}
}
