using Project.Lib;
using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 施設管理
    /// </summary>
    public class TownhallAssembly : EntityAssembly<Townhall, TownhallAssembly> {

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Townhall> CreateAsync(TownhallData data, bool isImmediate = false, bool isReserve = false) {
			var mst = BaseDataManager.GetDictionary<int, MstTownhallData>()[data.baseId];
			Townhall entity = await base.CreateImplAsync(mst.Model, isImmediate, isReserve);
			entity.Setup(mst, data);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + data.id;
#endif
			return entity;
		}


	}
}
