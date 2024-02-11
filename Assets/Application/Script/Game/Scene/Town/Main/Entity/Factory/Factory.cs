using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// 生産施設
	/// </summary>
	/// <remarks>
	/// 生産施設はGoodsを生み出す。原料が必要な場合は原料を消費する。生み出された製品はマーケット・倉庫(輸出)に送られる
	/// Factory->Factory
	/// Factory->Market
	/// Factory->Storage
	/// Storage->Market
	/// </remarks>
	public class Factory : Facility {
		//元データ
		MstFactoryData data_;
		public MstFactoryData Data { get { return data_; } }

		//生産処理
		FactoryParam haveParam_;
		public FactoryParam HaveParam { get { return haveParam_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Factory; } }



		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<FactoryParam>(obj);

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
		public void Init(int id, MstFactoryData mstdata) {
			id_ = id;
			data_ = mstdata;
			haveParam_.Setup(mstdata.Goods, mstdata.MinGrade, mstdata.Output, mstdata.MaxWorker);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
		}

		/// <summary>
		/// データを受け取ってセットアップ
		/// </summary>
		public void Setup(MstFactoryData mstdata, FactoryData factoryData) {
			id_ = factoryData.id;
			SetPosition(factoryData.position);
			data_ = mstdata;
			//
			haveParam_.Setup(factoryData.product, factoryData.grade, factoryData.output, factoryData.worker);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
			haveArea_.SetRadius(factoryData.radius);
		}

		/// <summary>
		/// 近隣リストを作成
		/// </summary>
		public override void CreateChain(ITownFacility townEntity) {
			CreateChainSender(this, townEntity.Factories);
			CreateChainReceiver(this, townEntity.Factories);
			CreateChainSender(this, townEntity.Markets);

			//自分が送り手になるChainを作成
			foreach (Chain chain in sendList_.Values) {
				//受け取り側の種別で分岐
				switch (chain.Receiver) {
				case Factory factory:
					chain.Valid = CheckFactoryToFactory(this, factory);
					break;
				case Market market:
					chain.Valid = CheckFactoryToMarket(this, market);
					break;
				case Service service:
					chain.Valid = CheckFactoryToService(this, service);
					break;
				case Storage storage:
					chain.Valid = CheckFactoryToStorage(this, storage);
					break;
				}
			}

			//自分が受け手になるChainを作成
			foreach (Chain chain in recieveList_.Values) {
				//送り側の種別で分岐
				switch (chain.Sender) {
				case Factory factory:
					chain.Valid = CheckFactoryToFactory(factory, this);
					break;
				case Storage storage:
					chain.Valid = CheckStorageToFactory(storage, this);
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
		public override void UpdateChainByMovePosition(ITownFacility townEntity, out List<Chain> include, out List<Chain> exclude) {
			include = new List<Chain>();
			exclude = new List<Chain>();
			Vector3 pos = GetPosition();
			//生産施設とのチェーン更新
			for (int i = 0, max = townEntity.Factories.Count; i < max; i++) {
				Factory factory = townEntity.Factories[i];
				float lenSq = (pos - factory.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, factory, lenSq, CheckFactoryToFactory, ref include, ref exclude);
				//自分が受け取る側
				UpdateChain(factory, this, lenSq, CheckFactoryToFactory, ref include, ref exclude);
				UpdateAreaVisible(factory, this, lenSq, CheckFactoryToFactory);
			}
			//マーケットとのチェーン更新
			for (int i = 0, max = townEntity.Markets.Count; i < max; i++) {
				Market market = townEntity.Markets[i];
				float lenSq = (pos - market.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, market, lenSq, CheckFactoryToMarket, ref include, ref exclude);
			}
			//サービス施設とのチェーン更新
			for (int i = 0, max = townEntity.Services.Count; i < max; i++) {
				Service service = townEntity.Services[i];
				float lenSq = (pos - service.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, service, lenSq, CheckFactoryToService, ref include, ref exclude);
			}
			//倉庫とのチェーン更新
			for (int i = 0, max = townEntity.Storages.Count; i < max; i++) {
				Storage storage = townEntity.Storages[i];
				float lenSq = (pos - storage.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, storage, lenSq, CheckFactoryToStorage, ref include, ref exclude);
				//自分が受け取る側
				UpdateChain(storage, this, lenSq, CheckStorageToFactory, ref include, ref exclude);
				UpdateAreaVisible(storage, this, lenSq, CheckStorageToFactory);
			}
		}

		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.FactoryData ToNetworkData() {
			Network.FactoryData data = new Network.FactoryData();
			data.id = Id;
			data.baseId = Data.Id;
			data.position = GetPosition();
			data.radius = haveArea_.Radius;
			data.product = haveParam_.Product;
			data.grade = haveParam_.Grade;
			data.output = haveParam_.Output;
			data.worker = haveParam_.Worker;
			return data;
		}
		/// <summary>
		/// 生産物を変更
		/// </summary>
		public void UpdateProduct(int product) {
			haveParam_.Product = product;
			haveParam_.Output = (product == Data.Goods) ? Data.Output : Data.Output2;
			UpdateFactoryCmd.CreateAsync(new UpdateFactoryRequest(ToNetworkData()));
		}
		/// <summary>
		/// 生産物グレードを変更
		/// </summary>
		public void UpdateGrade(int grade) {
			haveParam_.Grade = grade;
			UpdateFactoryCmd.CreateAsync(new UpdateFactoryRequest(ToNetworkData()));
		}
	}
}
