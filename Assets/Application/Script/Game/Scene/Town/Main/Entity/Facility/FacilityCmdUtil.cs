using UnityEngine;
using Project.Network;

namespace Project.Game
{
	/// <summary>
	/// 施設の通信処理まとめ
	/// </summary>
	public static class FacilityCmdUtil
	{
		/// <summary>
		/// 通信して施設を再配置
		/// </summary>
		public static void Relocate(Facility facility, System.Action<Response> success) {
			switch (facility) {
				case Factory factory:
					RelocateFactoryCmd.CreateAsync(
						new RelocateFactoryRequest(factory.ToNetworkData(), factory.ToChainData()),
						success,
						(res, status) => { });
					break;
				case Market market:
					RelocateMarketCmd.CreateAsync(
						new RelocateMarketRequest(market.ToNetworkData(), market.ToChainData()),
						success,
						(res, status) => { });
					break;
				case Residence residence:
					RelocateResidenceCmd.CreateAsync(
						new RelocateResidenceRequest(residence.ToNetworkData(), residence.ToChainData()),
						success,
						(res, status) => { });
					break;
				case Storage storage:
					RelocateStorageCmd.CreateAsync(
						new RelocateStorageRequest(storage.ToNetworkData(), storage.ToChainData()),
						success,
						(res, status) => { });
					break;
				case Service service:
					RelocateServiceCmd.CreateAsync(
						new RelocateServiceRequest(service.ToNetworkData(), service.ToChainData()),
						success,
						(res, status) => { });
					break;
				default:
					Debug.LogError("invalid facility type:" + facility.GetType());
					break;
			}
		}
	}
}
