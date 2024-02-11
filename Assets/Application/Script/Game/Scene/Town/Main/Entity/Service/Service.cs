using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {

	/// <summary>
	/// サービス施設
	/// </summary>
	/// <remarks>
	/// サービス施設はGoodsを生み出す。原料が必要な場合は原料を消費する。生み出されたグッズは直接住居の住人に消費される
	/// Factory->Service
	/// Service->Residence
	/// </remarks>
	public class Service : Facility {
		//元データ
		MstServiceData data_;
		public MstServiceData Data { get { return data_; } }

		//パラメータ
		ServiceParam haveParam_;
		public ServiceParam HaveParam { get { return haveParam_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Service; } }


		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<ServiceParam>(obj);

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
		public void Init(int id, MstServiceData mstdata) {
			id_ = id;
			data_ = mstdata;
			haveParam_.Setup(mstdata.Goods, mstdata.MinGrade, mstdata.Output, mstdata.MaxWorker);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
		}

		/// <summary>
		/// データを受け取ってセットアップ
		/// </summary>
		public void Setup(MstServiceData mstdata, ServiceData serviceData) {
			id_ = serviceData.id;
			SetPosition(serviceData.position);
			data_ = mstdata;
			//
			haveParam_.Setup(serviceData.product, serviceData.grade, serviceData.output, serviceData.worker);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
			haveArea_.SetRadius(serviceData.radius);
		}

		/// <summary>
		/// 近隣リストを作成
		/// </summary>
		public override void CreateChain(ITownFacility townFacility) {
			/*CreateChainSender(this, factories);
			CreateChainReceiver(this, factories);
			CreateChainSender(this, markets);

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
			}*/
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
				UpdateChain(factory, this, lenSq, CheckFactoryToService, ref include, ref exclude);
				UpdateAreaVisible(factory, this, lenSq, CheckFactoryToService);
			}
			//住居とのチェーン更新
			for (int i = 0, max = townFacility.Residences.Count; i < max; i++) {
				Residence residence = townFacility.Residences[i];
				float lenSq = (pos - residence.GetPosition()).sqrMagnitude;
				//自分が送る側
				UpdateChain(this, residence, lenSq, CheckServiceToResidence, ref include, ref exclude);
			}
		}

		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.ServiceData ToNetworkData() {
			Network.ServiceData data = new Network.ServiceData();
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
		/// サービスを変更
		/// </summary>
		public void UpdateProduct(int product) {
			haveParam_.Product = product;
			UpdateServiceCmd.CreateAsync(new UpdateServiceRequest(ToNetworkData()));
		}
		/// <summary>
		/// サービスのグレードを変更
		/// </summary>
		public void UpdateGrade(int grade) {
			haveParam_.Grade = grade;
			UpdateServiceCmd.CreateAsync(new UpdateServiceRequest(ToNetworkData()));
		}
	}
}
