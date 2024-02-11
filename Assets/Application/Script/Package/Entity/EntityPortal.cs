using UnityEngine;

namespace Project.Game {

	/// <summary>
	/// Entityへのアクセス経路
	/// </summary>
	[DisallowMultipleComponent]
	public class EntityPortal<T> : MonoBehaviour {
        public T Owner;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake() {
	        //Inspector要らない
	        hideFlags |= HideFlags.HideInInspector;
        }
        /// <summary>
        /// owner登録
        /// </summary>
        public void Init(T owner) {
            Owner = owner;
        }
    }
}
