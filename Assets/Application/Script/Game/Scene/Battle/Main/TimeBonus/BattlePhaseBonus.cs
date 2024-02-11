using Project.Http.Mst;
using Project.Lib;

namespace Project.Game {

	/// <summary>
	/// 時間帯ボーナス
	/// </summary>
	public class BattlePhaseBonus : MonoPretender {
		public int current;         //現在属性

		private float phaseTime;
		private float cycle;
		private float time;

		public BattlePhaseBonus() {
			phaseTime = GameConst.Battle.PHASE_DURATION;
			cycle = phaseTime * (int)PHASE.MAX;
			time = 0f;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			time += Time.deltaTime;
			current = (int)(time / phaseTime) % (int)PHASE.MAX;

			if (time > cycle) {
				time = time - cycle;
			}
		}

	}
}
