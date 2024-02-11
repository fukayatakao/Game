using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// 住居
	/// </summary>
	/// <remarks>
	/// 住居は住人が住みGoodsを消費してお金を生み出す(税収を得る)
	/// Market->Residence
	/// </remarks>
	public class Residence : Facility {
		ResidenceParam haveParam_;
		public ResidenceParam HaveParam { get { return haveParam_; } }

		MstResidenceData data_;
		public MstResidenceData Data { get { return data_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Residence; } }


		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<ResidenceParam>(obj);

			return obj;
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(haveParam_);
			DestroyImpl();
		}

		/// <summary>
		/// 初期状態でセットアップ
		/// </summary>
		public void Init(int id, Mst.MstResidenceData mstdata) {
			id_ = id;
			data_ = mstdata;

			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.yellow);
			haveParam_.Init();
		}
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(MstResidenceData mstdata, ResidenceData residenceData) {
			id_ = residenceData.id;
			SetPosition(residenceData.position);
			data_ = mstdata;

			//haveParam_.Setup(data.Capacity);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
			haveArea_.SetRadius(residenceData.radius);
		}


		/// <summary>
		/// 近隣リストを更新
		/// </summary>
		public override void CreateChain(ITownFacility townFacility) {
			CreateChainReceiver(this, townFacility.Markets);
			//自分が受け手になるChainを作成
			foreach (Chain chain in recieveList_.Values) {
				//送り側の種別で分岐
				switch (chain.Sender) {
				case Market market:
					chain.Valid = CheckMarketToResidence(market, this);
					break;
				case Service service:
					chain.Valid = CheckServiceToResidence(service, this);
					break;
				}
			}
		}

		/// <summary>
		/// サプライチェーンを更新
		/// </summary>
		/// <remarks>
		/// 建物の移動による更新（生産物を変更するケースを考えない
		/// </remarks>
		public override void UpdateChainByMovePosition(ITownFacility townFacility, out List<Chain> include, out List<Chain> exclude) {
			include = new List<Chain>();
			exclude = new List<Chain>();
			Vector3 pos = GetPosition();
			//マーケットとのチェーン更新
			for (int i = 0, max = townFacility.Markets.Count; i < max; i++) {
				Market market = townFacility.Markets[i];
				float lenSq = (pos - market.GetPosition()).sqrMagnitude;
				//自分が受け取り側
				UpdateChain(market, this, lenSq, CheckMarketToResidence, ref include, ref exclude);
				UpdateAreaVisible(market, this, lenSq, CheckMarketToResidence);
			}
			//サービス施設とのチェーン更新
			for (int i = 0, max = townFacility.Services.Count; i < max; i++) {
				Service service = townFacility.Services[i];
				float lenSq = (pos - service.GetPosition()).sqrMagnitude;
				//自分が受け取り側
				UpdateChain(service, this, lenSq, CheckServiceToResidence, ref include, ref exclude);
				UpdateAreaVisible(service, this, lenSq, CheckServiceToResidence);
			}
		}



		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.ResidenceData ToNetworkData() {
			Network.ResidenceData data = new Network.ResidenceData();
			data.id = Id;
			data.baseId = Data.Id;
			data.position = GetPosition();
			data.radius = haveArea_.Radius;
			return data;
		}

	}
}
