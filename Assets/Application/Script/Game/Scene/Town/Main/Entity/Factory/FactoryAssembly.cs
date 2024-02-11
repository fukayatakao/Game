using Project.Lib;
using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 施設管理
    /// </summary>
    public class FactoryAssembly : EntityAssembly<Factory, FactoryAssembly> {

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Factory> CreateAsync(FactoryData data, bool isImmediate = false, bool isReserve = false) {
			var mst = BaseDataManager.GetDictionary<int, MstFactoryData>()[data.baseId];
			Factory entity = await base.CreateImplAsync(mst.Model, isImmediate, isReserve);
			entity.Setup(mst, data);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + data.id;
#endif
			return entity;
		}


		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Factory> CreateAsync(int id, Mst.MstFactoryData mst, bool isImmediate = false, bool isReserve = false) {
			Factory entity = await base.CreateImplAsync(mst.Model, isImmediate, isReserve);
			entity.Init(id, mst);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + id;
#endif
			return entity;
		}



	}
}
