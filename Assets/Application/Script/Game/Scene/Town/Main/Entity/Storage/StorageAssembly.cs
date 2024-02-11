using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 施設管理
    /// </summary>
    public class StorageAssembly : EntityAssembly<Storage, StorageAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Storage> CreateAsync(StorageData data, bool isStock = true, bool isReserve = false) {
			var mst = BaseDataManager.GetDictionary<int, MstStorageData>()[data.baseId];
			Storage entity = await base.CreateImplAsync(mst.Model, isStock, isReserve);
			entity.Setup(mst, data);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + data.id;
#endif
			return entity;
		}

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Storage> CreateAsync(int id, Mst.MstStorageData data, bool isStock = true, bool isReserve = false) {
			Storage entity = await base.CreateImplAsync(data.Model, isStock, isReserve);
			entity.Init(id, data);
#if UNITY_EDITOR
			entity.gameObject.name = data.DbgName + ":" + id;
#endif
			return entity;
		}
	}
}
