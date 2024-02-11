using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// 倉庫
	/// </summary>
	/// <remarks>
	/// 倉庫はマップ外とのGoodsの輸出入を管理する
	/// Storage->Factory
	/// Factory->Storage
	/// Storage->Market
	/// </remarks>
	public class Storage : Facility {
		MstStorageData data_;
		public MstStorageData Data { get { return data_; } }

		StorageParam haveParam_;
		public StorageParam HaveParam{ get{ return haveParam_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Storage; } }


		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<StorageParam>(obj);

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
		public void Init(int id, MstStorageData data) {
			id_ = id;
			data_ = data;

			haveArea_.Setup(cacheTrans_, data.Range, Color.green);
			haveParam_.Setup(data.Capacity);
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Setup(MstStorageData mstdata, StorageData data) {
			id_ = data.id;
			SetPosition(data.position);
			data_ = mstdata;

			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
			haveArea_.SetRadius(data.radius);
		}
		/// <summary>
		/// 近隣リストを更新
		/// </summary>
		public override void CreateChain(ITownFacility townFacility) {
			CreateChainSender(this, townFacility.Residences);
			CreateChainReceiver(this, townFacility.Factories);
			//自分が送り手になるChainを作成
			foreach (Chain chain in sendList_.Values) {
				//受け取り側の種別で分岐
				switch (chain.Receiver) {
				case Factory factory:
					chain.Valid = CheckStorageToFactory(this, factory);
					break;
				case Market market:
					chain.Valid = CheckStorageToMarket(this, market);
					break;
				}
			}

			//自分が受け手になるChainを作成
			foreach (Chain chain in recieveList_.Values) {
				//送り側の種別で分岐
				switch (chain.Sender) {
				case Factory factory:
					chain.Valid = CheckFactoryToStorage(factory, this);
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
				//受け取りチェック
				Factory factory = townFacility.Factories[i];
				float lenSq = (pos - factory.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, factory, lenSq, CheckStorageToFactory, ref include, ref exclude);
				//自分が受け取り側
				UpdateChain(factory, this, lenSq, CheckFactoryToStorage, ref include, ref exclude);
				UpdateAreaVisible(factory, this, lenSq, CheckFactoryToStorage);
			}
			//マーケットとのチェーン更新
			for (int i = 0, max = townFacility.Markets.Count; i < max; i++) {
				Market market = townFacility.Markets[i];
				float lenSq = (pos - market.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, market, lenSq, CheckStorageToMarket, ref include, ref exclude);
			}
		}




		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.StorageData ToNetworkData() {
			Network.StorageData data = new Network.StorageData();
			data.id = Id;
			data.baseId = Data.Id;
			data.position = GetPosition();
			data.radius = haveArea_.Radius;
			return data;
		}


	}
}
