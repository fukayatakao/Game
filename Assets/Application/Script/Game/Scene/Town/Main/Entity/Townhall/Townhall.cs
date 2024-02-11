using UnityEngine;
using System.Collections.Generic;
using Project.Lib;

using Project.Mst;
using Project.Network;

using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// タウンホール
	/// </summary>
	/// <remarks>
	/// 町の中心
	/// </remarks>
	public class Townhall : Facility {
		//元データ
		MstTownhallData data_;
		public MstTownhallData Data { get { return data_; } }

		//パラメータ
		TownhallParam haveParam_;
		public TownhallParam HaveParam { get { return haveParam_; } }

		//マスターID
		[SerializeField]
		public override int BaseId { get { return data_.Id; } }

		//Facility種別
		[SerializeField]
		public override FacilityType Type { get { return FacilityType.Townhall; } }


		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = await CreateImpl(resName);
			haveParam_ = MonoPretender.Create<TownhallParam>(obj);

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
		/// データを受け取ってセットアップ
		/// </summary>
		public void Setup(MstTownhallData mstdata, TownhallData townhallData) {
			id_ = townhallData.id;
			SetPosition(townhallData.position);
			data_ = mstdata;
			//
			haveParam_.Setup(townhallData.gold, townhallData.logistics);
			haveArea_.Setup(cacheTrans_, mstdata.Range, Color.green);
		}

		/// <summary>
		/// 近隣リストを作成
		/// </summary>
		public override void CreateChain(ITownFacility townEntity) {
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
		}

		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public Network.TownhallData ToNetworkData() {
			Network.TownhallData data = new Network.TownhallData();
			data.id = Id;
			data.baseId = Data.Id;
			data.position = GetPosition();
			return data;
		}
	}
}
