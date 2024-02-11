using Project.Network;
using System.Collections.Generic;
using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// バトル状況
	/// </summary>
	[System.Serializable]
	public class OrganizeSituationData{
		static OrganizeSituationData instance_;
		public static OrganizeSituationData I { get { return instance_; } }


		public List<PlatoonData> platoons;
		public List<LeaderData> leaders; //予備リーダー
		public List<CharacterData> members; //予備キャラクターデータ

		public int stageId; //ステージモデル

		public int SelectPlatoon;

		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response response) {
			instance_ = new OrganizeSituationData();
			instance_.Setup(response);
		}

		/// <summary>
		/// Responseを解釈してデータをセットする
		/// </summary>
		private void Setup(Response res) {
			var response = res as OrganizeMainResponse;
			Debug.Assert(response != null, "response type error:" + res.GetType().Name);
			platoons = response.platoons;
			leaders = response.leaders;
			members = response.reserves;
			stageId = response.stageId;
		}

		/// <summary>
		/// 小隊情報をサーバに送れる形式にして保存する
		/// </summary>
		public void Update(PlatoonEntity platoon) {
			platoons[SelectPlatoon] = platoon.ConvertData();
		}


		/// <summary>
		/// 分隊に割り当てられていたキャラクターの配属を解除
		/// </summary>
		public void FreeMember(IReadOnlyList<CharacterEntity> members) {
			for(int i = 0, max = members.Count; i < max; i++) {
				//無名キャラは戻さない
				if (members[i].Id < 0)
					continue;
				CharacterData data = members[i].ConvertCharacterData();
				this.members.Add(data);
			}
		}

		/// <summary>
		/// 分隊に割り当てられていたキャラクターの配属を解除
		/// </summary>
		public LeaderData AllocLeader(LeaderData leader) {
			LeaderData data = leaders.Find(x => x.id == leader.id);
			leaders.Remove(data);
			return data;
		}
		/// <summary>
		/// 分隊に割り当てられていたキャラクターの配属を解除
		/// </summary>
		public void FreeLeader(CharacterEntity leader) {
			//リーダーが所属してない場合は無視
			if (leader == null)
				return;
			LeaderData data = leader.ConvertLeaderData();
			leaders.Add(data);
		}
	}
}
