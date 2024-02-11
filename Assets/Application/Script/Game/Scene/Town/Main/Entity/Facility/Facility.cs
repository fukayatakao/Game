using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using System.Threading.Tasks;
using Project.Mst;

namespace Project.Game {
	/// <summary>
	/// 施設
	/// </summary>
	public abstract class Facility : Entity {
		//識別ID
		[SerializeField]
		protected int id_;
		public int Id { get { return id_; } }

		//マスターID
		public abstract int BaseId { get; }

		//Facility種別
		public abstract FacilityType Type { get; }

		//自分が送り手になるChain
		protected Dictionary<Facility, Chain> sendList_ = new Dictionary<Facility, Chain>();
		public IReadOnlyDictionary<Facility, Chain> SendList { get { return sendList_; } }

		//自分が受け手になるChain
		protected Dictionary<Facility, Chain> recieveList_ = new Dictionary<Facility, Chain>();
		public IReadOnlyDictionary<Facility, Chain> RecieveList { get { return recieveList_; } }

		//ロードしたアセット(破棄が必要)
		Object asset_;

		//効果範囲円
		protected CircleArea haveArea_;
		public CircleArea HaveArea { get { return haveArea_; } }

		protected FacilityCollision haveCollision_;
		public FacilityCollision HaveCollision { get { return haveCollision_; } }

		//建設可能な範囲にいるか
		private bool isBuildable_;
		//建設・移動をキャンセルしたときに建設不可表示をクリアする
		public void ClearBuildable() { haveModel_.SetBuildable(true); }
		public bool IsResolveCollision() { return isBuildable_; }
		public void ResolveCollision() { haveModel_.SetBuildable(isBuildable_); }

		protected FacilityModel haveModel_;
		public FacilityModel HaveModel { get { return haveModel_; } }

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		protected async Task<GameObject> CreateImpl(string resName) {
			asset_ = await AddressableAssist.LoadAssetAsync(resName);
			GameObject root = new GameObject(resName);
			GameObject obj = UnityUtil.InstantiateChild(root.transform, asset_ as GameObject);
			//コリジョンからアクセス用
			obj.AddComponent<FacilityPortal>().Init(this);
			//円範囲表示
			haveArea_ = MonoPretender.Create<CircleArea>(root);
			await haveArea_.LoadAsync();

			//表示モデル
			haveModel_ = MonoPretender.Create<FacilityModel>(obj);
			haveCollision_ = MonoPretender.Create<FacilityCollision>(obj);
#if UNITY_EDITOR
			root.AddComponent<FacilityDebug>().Init(this);
#endif
			return root;
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected void DestroyImpl() {
			MonoPretender.Destroy(haveModel_);
			MonoPretender.Destroy(haveArea_);
			MonoPretender.Destroy(haveCollision_);
			AddressableAssist.UnLoadAsset(asset_);
		}

		public (int, int, int, int) GetFillTile()
		{
			Vector3 pos = GetPosition();
			Vector3 size = haveCollision_.GetSize();
			return ((int)pos.x, (int)pos.z, (int)size.x, (int)size.z);
		}

		//@note いずれ外に出すかも
		/// <summary>
		/// Chainを生成
		/// </summary>
		public static Chain EnChain(Facility send, Facility receive, bool enable=false) {
			Chain chain = Chain.Create(send, receive, enable);
			send.sendList_[receive] = chain;
			receive.recieveList_[send] = chain;

			return chain;
		}

		/// <summary>
		/// Chainを破棄
		/// </summary>
		public static void UnChain(Chain chain) {
			chain.Sender.sendList_.Remove(chain.Receiver);
			chain.Receiver.recieveList_.Remove(chain.Sender);
			Chain.Destroy(chain);
		}

		/// <summary>
		/// この建物が所持しているChainの情報
		/// </summary>
		public List<Network.ChainData> ToChainData() {
			List<Network.ChainData> list = new List<Network.ChainData>();
			foreach (Chain chain in sendList_.Values) {
				list.Add(chain.ToNetworkData());
			}
			foreach (Chain chain in recieveList_.Values) {
				list.Add(chain.ToNetworkData());
			}
			return list;
		}

		/// <summary>
		/// 近隣リストを作成
		/// </summary>
		public abstract void CreateChain(ITownFacility townEntity);

		/// <summary>
		/// 近隣リストを破棄
		/// </summary>
		public void DestroyChain() {
			//sendList_,recieveList_をループ回してUnChainするとコレクションが変化するためエラーになるので一旦別リストを作る
			List<Chain> list = new List<Chain>();
			list.AddRange(sendList_.Values);
			list.AddRange(recieveList_.Values);
			//出来ているChainを全部破棄する
			for (int i = 0, max = list.Count; i < max; i++) {
				UnChain(list[i]);
			}
		}

		/// <summary>
		/// サプライチェーンを更新
		/// </summary>
		public abstract void UpdateChainByMovePosition(ITownFacility townEntity, out List<Chain> include, out List<Chain> exclude);

		/// <summary>
		/// タウンホールから遠すぎる場合は建設不可
		/// </summary>
		public void CheckBuildable(ITownFacility townEntity, TownGridMap map)
		{
			Townhall center = townEntity.CenterTownhall;
			float lenSq = (GetPosition() - center.GetPosition()).sqrMagnitude;
			isBuildable_ = lenSq < center.Data.Range * center.Data.Range;

			Vector3 pos = GetPosition();
			Vector3 size = haveCollision_.GetSize();
			isBuildable_ &= map.IsVacant((int)pos.x, (int)pos.z, (int)size.x, (int)size.z);

		}

		/// <summary>
		/// ownerが送り手になるChainを作成
		/// </summary>
		protected static void CreateChainSender<T>(Facility owner, List<T> list) where T : Facility {
			Vector3 pos = owner.GetPosition();
			for (int i = 0, max = list.Count; i < max; i++) {
				//生産物が受け取れるか
				Facility facility = list[i];
				//自分は除外
				if (facility == owner)
					continue;
				//届ける側の効果半径の中に居るかで判定
				float lenSq = (pos - facility.GetPosition()).sqrMagnitude;
				//出荷
				if (owner.haveArea_.RadiusSq > lenSq) {
					EnChain(owner, facility);
				}
			}

		}
		/// <summary>
		/// ownerが受け手になるChainを作成
		/// </summary>
		protected static void CreateChainReceiver<T>(Facility owner, List<T> list) where T : Facility {
			Vector3 pos = owner.GetPosition();
			for (int i = 0, max = list.Count; i < max; i++) {
				//生産物が受け取れるか
				Facility facility = list[i];
				//自分は除外
				if (facility == owner)
					continue;

				//届ける側の効果半径の中に居るかで判定
				float lenSq = (pos - facility.GetPosition()).sqrMagnitude;
				//入荷
				if (facility.haveArea_.RadiusSq > lenSq) {
					EnChain(facility, owner);
				}
			}
		}
		protected static void UpdateChain<T, U>(T sender, U reciever, float lenSq, System.Func<T, U, bool> checkChain, ref List<Chain> include, ref List<Chain> exclude) where T : Facility where U : Facility {
			if (sender == reciever)
				return;
			//仕入れる側の効果半径の中に居るかで判定
			//範囲に入った
			if (sender.HaveArea.RadiusSq > lenSq) {
				//Chainが有る場合は距離だけ更新
				if (sender.SendList.TryGetValue(reciever, out var chain)) {
					chain.Update(lenSq);
				//Chainがない場合は新規追加
				} else {
					bool valid = checkChain(sender, reciever);
					Chain newChain = EnChain(sender, reciever, valid);
					include.Add(newChain);
				}
				//範囲から外れた
			} else {
				//Chainが有る場合は外れる
				if (sender.SendList.TryGetValue(reciever, out var chain)) {
					UnChain(chain);
					exclude.Add(chain);
					//Chainが無い場合は変化ないので何もしない
				} else {

				}
			}
		}

		/// <summary>
		/// 十分近づいたら流通半径を表示する
		/// </summary>
		protected void UpdateAreaVisible<T, U>(T sender, U reciever, float lenSq, System.Func<T, U, bool> checkChain) where T : Facility where U : Facility {
			if (sender == reciever)
				return;
			//Chainは作られないが十分近づいたら流通半径を表示する
			sender.HaveArea.SetVisible(sender.HaveArea.RadiusExSq > lenSq && sender.HaveArea.RadiusSq < lenSq && checkChain(sender, reciever));

		}

		/// <summary>
		/// senderの生産品とreceiverの原料が合致するか
		/// </summary>
		protected static bool CheckFactoryToFactory(Factory sender, Factory receiver) {
			//同一の物が入って来る可能性があるのでチェック
			if (sender == receiver)
				return false;

			int[] list = BaseDataManager.GetDictionary<int, MstGoodsData>()[receiver.HaveParam.Product].Resource;
			for (int i = 0, max = list.Length; i < max; i++) {
				int goods = list[i];
				//原料になるものを生産しているか
				if (sender.HaveParam.Product == goods) {
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// senderの生産品とreceiverの取り扱い種別が合致するか
		/// </summary>
		protected static bool CheckFactoryToMarket(Factory sender, Market receiver) {
			//品目が一致していない場合はチェーンつながない
			if (MstGoodsData.GetData(sender.HaveParam.Product).Article != receiver.Data.Article)
				return false;

			int[] list = receiver.HaveParam.NegativeGoodsIds;
			for (int i = 0, max = list.Length; i < max; i++) {
				int goods = list[i];
				//非取り扱いグッズを生産している場合はチェーンつながない
				if (sender.HaveParam.Product == goods) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// senderの生産品とreceiverの取り扱い種別が合致するか
		/// </summary>
		protected static bool CheckFactoryToService(Factory sender, Service receiver) {
			int[] list = BaseDataManager.GetDictionary<int, MstGoodsData>()[receiver.HaveParam.Product].Resource;
			for (int i = 0, max = list.Length; i < max; i++) {
				int goods = list[i];
				//原料になるものを生産しているか
				if (sender.HaveParam.Product == goods) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// senderの生産品とreceiverの取り扱い種別が合致するか
		/// </summary>
		protected static bool CheckMarketToResidence(Market sender, Residence receiver) {
			return true;
		}

		/// <summary>
		/// senderの生産品とreceiverの原料が合致するか
		/// </summary>
		protected static bool CheckStorageToFactory(Storage sender, Factory receiver) {
			int[] list = BaseDataManager.GetDictionary<int, MstGoodsData>()[receiver.HaveParam.Product].Resource;
			for (int i = 0, max = list.Length; i < max; i++) {
				int goods = list[i];
				//原料になるものを倉庫で管理しているか
				if (sender.HaveParam.SlotDict.ContainsKey(goods)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// senderの生産品とreceiverの原料が合致するか
		/// </summary>
		protected static bool CheckFactoryToStorage(Factory sender, Storage receiver) {
			Slot[] list = receiver.HaveParam.Slots;
			for (int i = 0, max = list.Length; i < max; i++) {
				int goods = list[i].GoodsId;
				//倉庫で管理しているGoodsを生産しているか
				if (sender.HaveParam.Product == goods) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// senderの生産品とreceiverの原料が合致するか
		/// </summary>
		protected static bool CheckStorageToMarket(Storage sender, Market receiver) {
			return true;
		}

		/// <summary>
		/// senderの生産品とreceiverの取り扱い種別が合致するか
		/// </summary>
		protected static bool CheckServiceToResidence(Service sender, Residence receiver) {
			return true;
		}


	}
}
