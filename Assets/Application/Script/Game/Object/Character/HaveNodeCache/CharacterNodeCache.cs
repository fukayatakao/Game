using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// 検索したノードをキャッシュする
	/// </summary>
	public class CharacterNodeCache : MonoPretender {
		Transform root_;
        private Transform[] cacheTransformArray_;
        private Dictionary<string, Transform> node_ = new Dictionary<string, Transform>();


        public enum Part{
	        Root,
            Head,
            Body,
            LHand,
            RHand,
			LFoot,
			RFoot,
		}
		private string[] PART_NAME = System.Enum.GetNames(typeof(Part));


        /// <summary>
        /// インスタンス生成
        /// </summary>
		protected override void Create(GameObject root) {
            root_ = root.transform;
            cacheTransformArray_ = root_.GetComponentsInChildren<Transform>();
        }

        /// <summary>
        /// ノードを動的に追加したときにリストを更新する
        /// </summary>
        public void Refresh() {
            cacheTransformArray_ = root_.GetComponentsInChildren<Transform>();
        }

        /// <summary>
        /// ノードを登録
        /// </summary>
        public void Register(Part key, Transform trans) {
            string part = PART_NAME[(int)key];
            node_[part] = trans;
        }

        /// <summary>
        /// ノードを取得
        /// </summary>
        public Transform GetNode(Part key) {
            string part = PART_NAME[(int)key];
            return node_[part];
        }
        /// <summary>
        /// ノードを探して取得
        /// </summary>
        public Transform SearchNode(string name) {
			if (node_.ContainsKey(name)) {
				return node_ [name];
			} else {

				for (int i = 0, max = cacheTransformArray_.Length; i < max; i++) {
					if (cacheTransformArray_[i].name == name) {
						node_.Add (name, cacheTransformArray_[i]);
						return cacheTransformArray_[i];
					}
				}
			}

			Debug.Assert (false, "not found node");
			return null;

		}

	}
}
