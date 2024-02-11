using Project.Mst;
using UnityEngine;


namespace Project.Master
{
	public static class MasterUtil
	{
		/// <summary>
		/// 施設の購入値段
		/// </summary>
		public static int GetFacilityPrice(FacilityType type, int id)
		{
			int price = 0;
			switch (type)
			{
				case FacilityType.Factory:
					price = MstFactoryData.GetData(id).PurchasePrice;
					break;
				case FacilityType.Residence:
					price = MstResidenceData.GetData(id).PurchasePrice;
					break;
				case FacilityType.Storage:
					price = MstStorageData.GetData(id).PurchasePrice;
					break;
				case FacilityType.Market:
					price = MstMarketData.GetData(id).PurchasePrice;
					break;
				case FacilityType.Service:
					price = MstServiceData.GetData(id).PurchasePrice;
					break;
				default:
					Debug.LogError("invalid facility type");
					break;
			}

			return price;
		}

	}

}
