using UnityEngine;

namespace Project.Lib {
	public class AttachDebugFacility : MonoBehaviour {
#if DEVELOP_BUILD
		[SerializeField]
		public Vector3Int size_ = new Vector3Int(10, 3, 10);

		[SerializeField]
		public Color color_ = new Color(0.7f, 0.7f, 0.7f, 0.7f);

		//テキスト表示用
		[SerializeField]
		public string text_ = "建物種類";
		[SerializeField]
		public int fontSize_ = 48;

		private void OnDrawGizmos() {
			Color col = Gizmos.color;
			Gizmos.color = Color.red;
			Vector3 pos = transform.position;
			pos.y += size_.y * 0.5f;
			Gizmos.DrawWireCube(pos, size_);
			Gizmos.color = col;
		}
#endif
	}
}
