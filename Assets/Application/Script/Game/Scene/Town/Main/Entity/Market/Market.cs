using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// マーケット(小売り)
	/// </summary>
	/// <remarks>
	/// マーケットは生産施設・倉庫(輸入)からGoodsを受け取り住居に届ける
	/// Factory->Market
	/// Storage->Market
	/// Market->Residence
	/// </remarks>
	public class Market : Facility {
		MstMarketData data_;
		public MstMarketData Data { get { return data_; } }

		MarketParam haveParam_;
		public MarketParam HaveParam { get{ return haveParam_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Market; } }

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<MarketParam>(obj);
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
		/// 近隣リストを更新
		/// </summary>
		public override void CreateChain(ITownFacility townFacility) {
			CreateChainSender(this, townFacility.Residences);
			CreateChainReceiver(this, townFacility.Factories);
			UpdateChain();
		}
		/// <summary>
		/// チェインの内容を更新
		/// </summary>
		private void UpdateChain() {
			//自分が送り手になるChainを作成
			foreach (Chain chain in sendList_.Values) {
				//受け取り側の種別で分岐
				switch (chain.Receiver) {
				case Residence residence:
					chain.Valid = CheckMarketToResidence(this, residence);
					break;
				}
			}

			//自分が受け手になるChainを作成
			foreach (Chain chain in recieveList_.Values) {
				//送り側の種別で分岐
				switch (chain.Sender) {
				case Factory factory:
					chain.Valid = CheckFactoryToMarket(factory, this);
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
			//生産施設とのチェーン更新
			for (int i = 0, max = townFacility.Factories.Count; i < max; i++) {
				Factory factory = townFacility.Factories[i];
				float lenSq = (pos - factory.GetPosition()).sqrMagnitude;
				//自分が受け取る側
				UpdateChain(factory, this, lenSq, CheckFactoryToMarket, ref include, ref exclude);
				UpdateAreaVisible(factory, this, lenSq, CheckFactoryToMarket);
			}
			//住居とのチェーン更新
			for (int i = 0, max = townFacility.Residences.Count; i < max; i++) {
				Residence residence = townFacility.Residences[i];
				float lenSq = (pos - residence.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, residence, lenSq, CheckMarketToResidence, ref include, ref exclude);
			}
		}


		/// <summary>
		/// 初期状態でセットアップ
		/// </summary>
		public void Init(int id, MstMarketData mstdata) {
			id_ = id;
			data_ = mstdata;

			haveParam_.Setup(mstdata.Capacity);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
		}
		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(MstMarketData mstdata, MarketData marketData) {
			id_ = marketData.id;
			SetPosition(marketData.position);
			data_ = mstdata;

			haveParam_.Setup(mstdata.Capacity, marketData.goodsIds);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
			haveArea_.SetRadius(marketData.radius);
		}


		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.MarketData ToNetworkData() {
			Network.MarketData data = new Network.MarketData();
			data.id = Id;
			data.baseId = Data.Id;
			data.position = GetPosition();
			data.radius = haveArea_.Radius;
			data.goodsIds = new List<int>(haveParam_.NegativeGoodsIds);
			return data;
		}
		/// <summary>
		/// 陳列Goodsの更新をしてチェインを精査
		/// </summary>
		public void UpdateGoods(int index, int goodsId) {
			//既に同じグッズを別スロットで指定していたときは指定をクリア
			for (int i = 0, max = haveParam_.NegativeGoodsIds.Length; i < max; i++) {
				if (haveParam_.NegativeGoodsIds[i] == goodsId) {
					haveParam_.NegativeGoodsIds[i] = 0;
				}
			}

			haveParam_.NegativeGoodsIds[index] = goodsId;
			//取り扱うグッズが変わったのでチェインのグッズについても精査
			UpdateChain();
			UpdateMarketCmd.CreateAsync(new UpdateMarketRequest(ToNetworkData(), ToChainData()));
		}

	}
}
