namespace Project.Game {
	/// <summary>
	/// バトルの便利計算
	/// </summary>
	public static class BattleUtil {
		/// <summary>
		/// bravoがalphaよりも前にいるか
		/// </summary>
		public static bool IsForward(float alpha, float bravo, float sign, float range = 0f) {
			return (alpha - bravo) * sign < -range;
		}
		/// <summary>
		/// bravoがalphaよりも後ろにいるか
		/// </summary>
		public static　bool IsBackward(float alpha, float bravo, float sign, float range = 0f) {
			return (alpha - bravo) * sign > range;
		}
	}
}
