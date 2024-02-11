using UnityEngine;

namespace Project.Mst {
	public partial class MstActionData {
		public void Resolve() {
			AngleRad = Mathf.Deg2Rad * this.AngleDeg;
		}
		public float AngleRad; // 攻撃可能前方範囲(rad)
	}
}
