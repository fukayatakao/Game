using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// アニメーションイベントから呼ばれる関数
	/// </summary>
	public class AnimationEventCallback : MonoBehaviour {
        private void Awake() {
            hideFlags |= HideFlags.HideInInspector;
        }

        public void OnCallChangeFace(string str)
        {
        }
        public void Hit()
        {
        }
    }
}
