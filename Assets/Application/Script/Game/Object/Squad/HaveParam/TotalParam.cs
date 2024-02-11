using System.Collections.Generic;


namespace Project.Game {
	/// <summary>
	/// トータル情報
	/// </summary>
	[System.Serializable]
	public class TotalParam {
		//最大HP・現在HP
		public float MaxHp { get; private set; }
		public float Hp { get; private set; }

		//最大LP・現在LP
		public float MaxLp { get; private set; }
		public float Lp { get; private set; }

		/// <summary>
		/// 列が全滅したので数値をリセット
		/// </summary>
		public void Defeat() {
			Hp = 0f;
			Lp = 0f;
		}

		/// <summary>
		/// Total値のセットアップ
		/// </summary>
		public void Apply(IReadOnlyList<CharacterEntity> units) {
			MaxHp = 0f;
			MaxLp = 0f;
			if (units == null)
				return;

			for (int i = 0, max = units.Count; i < max; i++) {
				MaxHp += units[i].HaveUnitParam.Fight.MaxHp;
				MaxLp += units[i].HaveUnitParam.Fight.MaxLp;
			}
		}

		/// <summary>
		/// 定期的に状態を記録
		/// </summary>
		public void Execute(IReadOnlyList<CharacterEntity> units) {
			if (units == null)
				return;
			float hp = 0f;
			float lp = 0f;

			//所属キャラの情報を精査
			for (int i = 0, count = units.Count; i < count; i++) {
				CharacterEntity unit = units[i];
				//現在HP/LPを集計
				hp += unit.HaveUnitParam.Fight.Hp;
				lp += unit.HaveUnitParam.Fight.Lp;
			}

			Hp = hp;
			Lp = lp;
		}

	}
}
