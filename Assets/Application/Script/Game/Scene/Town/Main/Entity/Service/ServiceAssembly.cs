using Project.Lib;
using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 施設管理
    /// </summary>
    public class ServiceAssembly : EntityAssembly<Service, ServiceAssembly> {

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Service> CreateAsync(ServiceData data, bool isImmediate = false, bool isReserve = false) {
			var mst = BaseDataManager.GetDictionary<int, MstServiceData>()[data.baseId];
			Service entity = await base.CreateImplAsync(mst.Model, isImmediate, isReserve);
			entity.Setup(mst, data);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + data.id;
#endif
			return entity;
		}


		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Service> CreateAsync(int id, Mst.MstServiceData mst, bool isImmediate = false, bool isReserve = false) {
			Service entity = await base.CreateImplAsync(mst.Model, isImmediate, isReserve);
			entity.Init(id, mst);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + id;
#endif
			return entity;
		}



	}
}
