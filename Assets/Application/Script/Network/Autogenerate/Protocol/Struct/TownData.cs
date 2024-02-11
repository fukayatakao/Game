using UnityEngine;
using System.Collections.Generic;


namespace Project.Network
{
	[System.Serializable]
	public class TownData
	{
		//---- レスポンス変数定義 ----
		public int tax; //税率
		public int identifer; //識別子の開始番号
		public TownhallData townhall; //タウンホール
		public List<FactoryData> factories; //Factoryリスト
		public List<StorageData> storages; //Storageリスト
		public List<MarketData> markets; //Marketリスト
		public List<ResidenceData> residences; //Residenceリスト
		public List<ServiceData> services; //Serviceリスト
		public List<ChainData> chainList; //Chainリスト

	}

}
