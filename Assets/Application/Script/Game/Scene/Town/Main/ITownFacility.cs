using System.Collections.Generic;


namespace Project.Game {
	/// <summary>
	/// Facilityのみを見えるようにするためのインタフェース
	/// </summary>
	public interface ITownFacility {
		//生産施設
		public Townhall CenterTownhall { get; }
		//生産施設
		public List<Factory> Factories { get; }
		//住居施設
		public List<Residence> Residences { get; }
		//倉庫
		public List<Storage> Storages { get; }
		//マーケット
		public List<Market> Markets { get; }
		//サービス
		public List<Service> Services { get; }

	}

}
