using System.Collections.Generic;
using Project.Http.Mst;

namespace Project.Game {
	/// <summary>
	/// 属性情報
	/// </summary>
	[System.Serializable]
	public class PhaseParam {
		int[] phaseCount_ = new int[(int)PHASE.MAX];

		/// <summary>
		/// 分隊の属性値を取得
		/// </summary>
		public int GetPhaseCount(PHASE phase) {
			return phaseCount_[(int)phase];
		}

		/// <summary>
		/// Total値のセットアップ
		/// </summary>
		public void Apply(IReadOnlyList<CharacterEntity> units) {
			for (int i = 0; i < (int)PHASE.MAX; i++) {
				phaseCount_[i] = 0;
			}

			for (int i = 0, max = units.Count; i < max; i++) {
				PHASE phase = units[i].HavePersonal.Phase;
				phaseCount_[(int)phase] += units[i].HaveUnitMaster.Phase;
			}

		}
	}
}
