using UnityEngine;

#if UNITY_EDITOR
namespace Project.Game {
	/// <summary>
	/// Facilityのデバッグ表示用
	/// </summary>
	public class FacilityDebug : MonoBehaviour
	{
		public Facility Owner;
		public void Init(Facility owner) {
			Owner = owner;
		}
	}
}
#endif
