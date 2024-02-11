using UnityEngine;

namespace Project.Game {
	public static class StrategyUtil {
		/// <summary>
		/// 接地計算
		/// </summary>
		public static Vector3 Grounding(Vector3 pos, float height = 10f, float distance = 100f) {
			//height分上げて下にray(distanceの距離)を飛ばして接地座標を計算
			pos.y += height;
			bool result = Physics.Raycast(pos, Vector3.down, out var hit, distance, 1 << (int)UnityLayer.Layer.Field);
			if (result) {
				pos.y = hit.point.y;
			}
			return pos;
		}

	}
}
