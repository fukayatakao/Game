using System.Collections.Generic;

namespace Project.Mst {
	public partial class MstUnitData {
		public void Resolve(Dictionary<int, MstActionData> attackDict) {
			if (attackDict.TryGetValue(ActionA, out var action_a)) { if(action_a.Id != 0) AttackPattern.Add(action_a); }
			if (attackDict.TryGetValue(ActionB, out var action_b)) { if(action_b.Id != 0) AttackPattern.Add(action_b); }
			if (attackDict.TryGetValue(ActionC, out var action_c)) { if(action_c.Id != 0) AttackPattern.Add(action_c); }
			if (attackDict.TryGetValue(ActionD, out var action_d)) { if(action_d.Id != 0) AttackPattern.Add(action_d); }
			//優先順位(昇順)でソート
			AttackPattern.Sort((a, b) => a.Priority - b.Priority);
		}

		public List<MstActionData> AttackPattern = new List<MstActionData>(); // 攻撃方法のデータ
	}
}
