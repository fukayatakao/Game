using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;

namespace Project.Game {
	public class CharacterSkill : MonoPretender {
		private List<MstSkillData> skillData_ = new List<MstSkillData>();
		public List<MstSkillData> Skill { get { return skillData_; } }


		/// <summary>
		/// スキル設定
		/// </summary>
		/// <param name="data"></param>
		public void Init(LeaderData data) {
			MstLeaderData leader = MstLeaderData.GetData(data.leaderMasterId);
			if (leader.SkillA != 0) {
				skillData_.Add(MstSkillData.GetData(leader.SkillA));
			}
			if (leader.SkillB != 0) {
				skillData_.Add(MstSkillData.GetData(leader.SkillB));
			}
			if (leader.SkillC != 0) {
				skillData_.Add(MstSkillData.GetData(leader.SkillC));
			}
			if (leader.SkillD != 0) {
				skillData_.Add(MstSkillData.GetData(leader.SkillD));
			}
		}
	}
}
