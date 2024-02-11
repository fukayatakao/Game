using System.Collections.Generic;

namespace Project.Game {
	public static class TownCalculateUtil {
		public static void CalcStorageGoods(int goodsId, HashSet<Storage> neighbors, out int amount, out float grade) {
			//グレードはグレード * 量の総計を量で除算して平均を出す
			amount = 0;
			grade = 0f;
			foreach (Storage storage in neighbors) {
				//高速化のために可読性をちょっと犠牲にしてTryGetValue
				SlotStatus value;
				if (!storage.HaveParam.SlotDict.TryGetValue(goodsId, out value))
					continue;
				amount += value.Amount;
				grade += value.Grade * value.Amount;
			}
			//0で除算対策
			if (amount != 0) {
				grade = grade / amount;
			}
		}
	}

}
