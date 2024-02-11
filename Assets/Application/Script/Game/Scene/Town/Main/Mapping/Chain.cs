using System.Collections.Generic;
using Project.Network;
using UnityEngine;

namespace Project.Game {
	public class Chain {
		//送り手->受け手でChainを探すためのDictionary
		protected static Dictionary<Facility, Dictionary<Facility, Chain>> chains_ = new Dictionary<Facility, Dictionary<Facility, Chain>>();
		//出荷
		public Facility Sender { get; private set; }
		//入荷
		public Facility Receiver { get; private set; }
		//有効か
		public bool Valid { get; set; }
		//距離
		public float Distance { get; private set; }
		/// <summary>
		/// Chainの新規作成
		/// </summary>
		public static Chain Create(Facility sender, Facility receiver, bool valid) {
			//chainを作って登録
			if (!chains_.ContainsKey(sender))
				chains_[sender] = new Dictionary<Facility, Chain>();
			Chain chain = new Chain(sender, receiver, valid);
			chains_[sender][receiver] = chain;
			return chain;
		}
		/// <summary>
		/// Chainの破棄
		/// </summary>
		public static void Destroy(Chain chain) {
			//大元も削除
			chains_[chain.Sender].Remove(chain.Receiver);
			if (chains_[chain.Sender].Count == 0)
				chains_.Remove(chain.Sender);

		}

		/// <summary>
		/// 距離を更新
		/// </summary>
		public void Update(float distanceSq) {
			Distance = Mathf.Sqrt(distanceSq);
		}

		/// <summary>
		/// チェーンを作る
		/// </summary>
		private Chain(Facility sender, Facility receiver, bool valid) {
			Sender = sender;
			Receiver = receiver;
			Valid = valid;
			float distanceSq = (Sender.GetPosition() - Receiver.GetPosition()).sqrMagnitude;
			Distance = Mathf.Sqrt(distanceSq);
		}
		/// <summary>
		/// サーバ送信用のデータを生成
		/// </summary>
		public ChainData ToNetworkData() {
			ChainData data = new ChainData();
			data.senderId = Sender.Id;
			data.recieverId = Receiver.Id;
			data.distance = Distance;
			data.valid = Valid;
			return data;
		}
	}
}
