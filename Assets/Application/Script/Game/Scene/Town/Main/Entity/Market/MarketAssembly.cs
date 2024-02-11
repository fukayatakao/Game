using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 施設管理
    /// </summary>
    public class MarketAssembly : EntityAssembly<Market, MarketAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Market> CreateAsync(MarketData data, bool isStock = true, bool isReserve = false) {
			var mst = BaseDataManager.GetDictionary<int, MstMarketData>()[data.baseId];
			Market entity = await base.CreateImplAsync(mst.Model, isStock, isReserve);
			entity.Setup(mst, data);
#if UNITY_EDITOR
			entity.gameObject.name = mst.DbgName + ":" + data.id;
#endif
			return entity;
		}

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<Market> CreateAsync(int id, Mst.MstMarketData data, bool isStock = true, bool isReserve = false) {
			Market entity = await base.CreateImplAsync(data.Model, isStock, isReserve);
			entity.Init(id, data);
#if UNITY_EDITOR
			entity.gameObject.name = data.DbgName + ":" + id;
#endif
			return entity;
		}

	}
}
