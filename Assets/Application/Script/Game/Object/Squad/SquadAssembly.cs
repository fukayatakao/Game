using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {

	/// <summary>
	/// 分隊管理
	/// </summary>
	public class SquadAssembly : EntityAssembly<SquadEntity, SquadAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<SquadEntity> CreateAsync(string resName, SquadData data, bool isImmediate = false, bool isReserve = false) {
			//ユニットがいない且つリーダーもいない分隊はデータがおかしい
			Debug.Assert(!((data.unitId == 0 || data.members.Count == 0) && data.leader == null), "invalid squad data");
			//本体を生成
			SquadEntity entity = await CreateImplAsync(resName, isImmediate, isReserve);
			//下にいるキャラクターと展開ラインを生成
			MstUnitData mstUnit = BaseDataManager.GetDictionary<int, MstUnitData>()[data.unitId];
			await entity.CreateMember(mstUnit, data.members, isImmediate);
			//リーダーがいるときのみ生成
			if(data.leader != null)
				await entity.CreateLeader(data.leader, isImmediate);

			return entity;
		}
	}
}
