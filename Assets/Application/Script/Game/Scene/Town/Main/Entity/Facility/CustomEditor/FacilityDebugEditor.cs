#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Project.Game;

[UnityEditor.CustomEditor(typeof(FacilityDebug))]
public class FacilityDebugEditor : UnityEditor.Editor  {
	FacilityDebug facilityDebug_;

	private void Awake() {
		facilityDebug_ = (FacilityDebug)base.target;
	}

	public override void OnInspectorGUI() {
		Facility facility = facilityDebug_.Owner;
		//Chainの状況を表示
		GUILayout.Label("Sender");
		foreach (Chain send in facility.SendList.Values) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(send.Receiver.Name);
			EditorGUILayout.ObjectField(send.Receiver.gameObject, typeof(GameObject), true);
			GUILayout.EndHorizontal();
		}

		GUILayout.Label("Receiver");
		foreach (Chain send in facility.RecieveList.Values) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(send.Sender.Name);
			EditorGUILayout.ObjectField(send.Sender.gameObject, typeof(GameObject), true);
			GUILayout.EndHorizontal();
		}
	}
}
#endif
