using System.Linq;


namespace Project.Game {
	public static class BattleReportUtil {
		public static BattleReport.Platoon CreateData(PlatoonEntity entity) {
			BattleReport.Platoon platoon = new BattleReport.Platoon();
			for (int i = 0, max = entity.Squads.Count; i < max; i++) {
				BattleReport.Squad squad = new BattleReport.Squad();
				squad.unitId = entity.Squads[i].UnitData.Id;
				squad.number = entity.Squads[i].Members.Count;
				platoon.squads.Add(squad);
			}
			return platoon;
		}

		public static BattleReport.Status CreateStatus(PlatoonEntity player, PlatoonEntity enemy) {
			BattleReport.Status status = new BattleReport.Status();

			status.playerHp = player.Squads.Sum(x => x.HaveParam.Hp);
			status.playerLp = player.Squads.Sum(x => x.HaveParam.Lp);
			status.playerNumber = player.Squads.Sum(x => x.Members.Count);

			status.enemyHp = enemy.Squads.Sum(x => x.HaveParam.Hp);
			status.enemyLp = enemy.Squads.Sum(x => x.HaveParam.Lp);
			status.enemyNumber = enemy.Squads.Sum(x => x.Members.Count);

			return status;
		}
	}
}
