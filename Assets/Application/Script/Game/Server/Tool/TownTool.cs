using Project.Mst;
using System.Collections.Generic;
using System.Linq;

namespace Project.Server {
	public static class TownTool {
		//産出したグッズの一時置き場：FacilityId, GoodsId, 数量
		public static Dictionary<int, Dictionary<int, ProductData>> CacheMap = new Dictionary<int, Dictionary<int, ProductData>>();
		//生産施設で消費予定のグッズ：FactoryId, GoodsId, 数量
		public static Dictionary<int, Dictionary<int, int>> StockMap = new Dictionary<int, Dictionary<int, int>>();
		//住居のキャラクターが要求する品目：ResidenceId, ArticleId, 要求数x数量
		public static Dictionary<int, Dictionary<int, int[]>> DemandMap = new Dictionary<int, Dictionary<int, int[]>>();
		//FacilityId, 品目, 要求枠, 供給量の記録
		public static Dictionary<int, Dictionary<int, List<ProductData>[]>> SupplyMap = new Dictionary<int, Dictionary<int, List<ProductData>[]>>();

#if DEVELOP_BUILD
		//FacilityId, 生産量の記録
		public static Dictionary<int, ProductData> OutputMap = new Dictionary<int, ProductData>();
		//FacilityId, グレード
		public static Dictionary<int, int> ResultMap = new Dictionary<int, int>();

#endif
		/// <summary>
		/// Chainの届け先をタイプ別に分類して近い順にソート済のリストにする
		/// </summary>
		class ChainReceiver {
			public List<int> processers = new List<int>();				//グッズを生産する施設。必要な原料を距離が近い順に確保する。
			public List<int> distributers = new List<int>();			//分配のみする施設。生産物を等分して確保して消費に備える
			public List<int> consumers = new List<int>();				//消費のみする施設。
			public List<int> services = new List<int>();
		}

		/// <summary>
		/// 生産と消費の計算
		/// </summary>
		public static void Calculate() {
			int times = CalcTimes();
			if (times == 0)
				return;
			CacheMap.Clear();
			StockMap.Clear();
			DemandMap.Clear();
			SupplyMap.Clear();
#if DEVELOP_BUILD
			OutputMap.Clear();
			ResultMap.Clear();
#endif

			//まずは効率考えずそのまま計算する
			//労働者と住人を施設Idをキーに割り振る
			List<UserMemberTable> characters = UserCharacterTableDAO.SelectAll();
			Dictionary<int, List<UserMemberTable>> worker = characters
				.GroupBy(chara => chara.workId)
				.ToDictionary(group => group.Key, group => group.ToList());
			Dictionary<int, List<UserMemberTable>> stayer = characters
				.GroupBy(chara => chara.stayId)
				.ToDictionary(group => group.Key, group => group.ToList());


			List<UserFactoryTable> factories = new List<UserFactoryTable>(UserFactoryTableDAO.SelectAll());
			List<UserResidenceTable> residences = new List<UserResidenceTable>(UserResidenceTableDAO.SelectAll());
			List<UserMarketTable> markets = new List<UserMarketTable>(UserMarketTableDAO.SelectAll());
			List<UserServiceTable> services = new List<UserServiceTable>(UserServiceTableDAO.SelectAll());

			Dictionary<int, MstFactoryData> mstFactory = BaseDataManager.GetDictionary<int, MstFactoryData>();
			Dictionary<int, MstServiceData> mstService = BaseDataManager.GetDictionary<int, MstServiceData>();

			//生産施設の必要消費量を事前計算
			for (int i = 0, max = factories.Count; i < max; i++) {
				MstFactoryData master = mstFactory[factories[i].baseId];
				PreCalcConsume(factories[i].id, factories[i].product, factories[i].output, master.MaxWorker, worker);
			}
			//サービス施設の必要消費量を事前計算
			for (int i = 0, max = services.Count; i < max; i++) {
				MstServiceData master = mstService[services[i].baseId];
				PreCalcConsume(services[i].id, services[i].product, services[i].output, master.MaxWorker, worker);
			}
			//住居に住む住人の需要を事前計算
			for (int i = 0, max = residences.Count; i < max; i++) {
				PreCalcDemand(residences[i].id, residences[i].baseId, stayer);
			}
			//まずはsender, receiveをkeyにした2重Dictionaryを作る
			Dictionary<int, ChainReceiver> chainDict = CreateChainDict(factories, markets, residences, services);

			//@note 他の生産品を消費して生産の計算する場合があるので他の材料になるGoodsを生産するFactoryから計算を行う
			//Goodsの処理優先順に並べる
			factories.Sort((a, b) => GoodsTool.PriorityMap[a.product] - GoodsTool.PriorityMap[b.product]);
			//生産量の計算
			for (int i = 0, max = factories.Count; i < max; i++) {
				//有効なチェインの起点になっていない場合は無視
				if (!chainDict.ContainsKey(factories[i].id))
					continue;
				ChainReceiver cahinReceiver = chainDict[factories[i].id];

				//生産量を算出
				ProductData output = CalcProduct(factories[i].id, factories[i].product, factories[i].grade, factories[i].output, factories[i].worker, worker);
				int amount = output.Amount;
				int grade = output.Grade;
				int product = output.Goods;

				//リストは距離順にソートされている
				foreach (var receiveId in cahinReceiver.processers) {
					//受け取ったGoddsの量の一時保管インスタンスが作られていなかったら作る
					if (!CacheMap.ContainsKey(receiveId))
						CacheMap[receiveId] = new Dictionary<int, ProductData>();
					if (!CacheMap[receiveId].ContainsKey(product))
						CacheMap[receiveId][product] = new ProductData(product);
					//近い順から生産に必要な量を出力から引いてキャッシュする
					int val = StockMap[receiveId][product];
					//必要量を越える出力が残っていた場合
					if (amount > val) {
						CacheMap[receiveId][product].Push(val, grade);
						StockMap[receiveId][product] = 0;
						amount = amount - val;
						//必要量に出力が満たない場合
					} else {
						CacheMap[receiveId][product].Push(amount, grade);
						StockMap[receiveId][product] -= amount;
						amount = 0;
					}
				}
				//届け先がマーケットの場合は出力の残りを按分してキャッシュ
				if (cahinReceiver.distributers.Count > 0) {
					int outputDiv = amount / cahinReceiver.distributers.Count;
					foreach (var receiveId in cahinReceiver.distributers) {
						//受け取ったGoddsの量の一時保管インスタンスが作られていなかったら作る
						if (!CacheMap.ContainsKey(receiveId))
							CacheMap[receiveId] = new Dictionary<int, ProductData>();
						if (!CacheMap[receiveId].ContainsKey(product))
							CacheMap[receiveId][product] = new ProductData(product);
						CacheMap[receiveId][product].Push(outputDiv, grade);
					}
				}
			}
			//生産物の分配
			for (int i = 0, max = markets.Count; i < max; i++) {
				//有効なチェインの起点になっていない場合は無視
				if (!chainDict.ContainsKey(markets[i].id))
					continue;
				//マーケットに生産品が何も集まっていない場合は無視
				if (!CacheMap.ContainsKey(markets[i].id))
					continue;

				//供給されているGoodsを品目でまとめてグレード順にソートする
				Dictionary<int, List<ProductData>> articleClassify = new Dictionary<int, List<ProductData>>();
				foreach (KeyValuePair<int, ProductData> pair in CacheMap[markets[i].id]) {
					int key = GoodsTool.GoodsArticle[pair.Key];
					if (!articleClassify.ContainsKey(key)) {
						articleClassify[key] = new List<ProductData>();
					}
					articleClassify[key].Add(pair.Value);
				}
				foreach (List<ProductData> list in articleClassify.Values) {
					list.Sort((a, b) => b.Grade - a.Grade);
				}


				//Chainが繋がっている住居の需要に対して供給を行ってスコアを加算する
				ChainReceiver cahinReceiver = chainDict[markets[i].id];
				foreach (var receiveId in cahinReceiver.consumers) {
					CalcSupply(receiveId, articleClassify);
				}
			}
			//サービスの計算
			for (int i = 0, max = services.Count; i < max; i++) {
				//有効なチェインの起点になっていない場合は無視
				if (!chainDict.ContainsKey(services[i].id))
					continue;

				ChainReceiver cahinReceiver = chainDict[services[i].id];
				if (cahinReceiver.consumers.Count > 0) {
					//生産量を算出
					ProductData output = CalcProduct(services[i].id, services[i].product, services[i].grade, services[i].output, services[i].worker, worker);
					int articleId = GoodsTool.GoodsArticle[output.Goods];

					//@todo 本当は住人数で割る
					output.Amount = output.Amount / cahinReceiver.consumers.Count;
					foreach (int residenceId in cahinReceiver.consumers) {
						//需要がない場合は計算省略
						if (!DemandMap[residenceId].ContainsKey(articleId))
							continue;


						int[] demandArray = DemandMap[residenceId][articleId];
						//要求枠, 供給量の記録
						List<ProductData>[] rec = new List<ProductData>[demandArray.Length];
						for (int j = 0, max2 = rec.Length; j < max2; j++) {
							rec[j] = new List<ProductData>();
						}


						int maxValue = 0;
						int demandIndex = 0;
						//同一品目内の残り需要が大きい方を採用
						for (int d = 0; d < demandArray.Length; d++) {
							if (demandArray[d] > maxValue) {
								maxValue = demandArray[d];
								demandIndex = d;
							}
						}
						if (maxValue == 0)
							break;

						//需要を供給で相殺
						//残り需要が供給グッズsの量よりも多い=需要が残る場合
						if (demandArray[demandIndex] > output.Amount) {
							rec[demandIndex].Add(output);
							//残り需要から供給された分を減算
							demandArray[demandIndex] -= output.Amount;
						} else {
							rec[demandIndex].Add(output);
							//需要を越える供給があったので残り需要は0
							demandArray[demandIndex] = 0;
						}
						if (!SupplyMap[residenceId].ContainsKey(articleId))
							SupplyMap[residenceId][articleId] = new List<ProductData>[rec.Length];
						for (int j = 0, max2 = rec.Length; j < max2; j++) {
							if (SupplyMap[residenceId][articleId][j] == null)
								SupplyMap[residenceId][articleId][j] = new List<ProductData>();
							SupplyMap[residenceId][articleId][j].AddRange(rec[j]);
						}
					}

				}
			}
			UserTownhallTable townhallTable = UserTownhallTableDAO.Select();
			int gold = townhallTable.gold;
			int tax = townhallTable.tax;
			//需要がどれだけ満たされたか
			for (int i = 0, max = residences.Count; i < max; i++) {
				//誰も住んでいない住居は無視
				if (!stayer.ContainsKey(residences[i].id))
					continue;
				//何も供給されなかった
				if (!SupplyMap.ContainsKey(residences[i].id))
					continue;
				//品目, 要求種類数x平均Grade
				int residenceId = residences[i].id;
				int amount = 0;
				int point = 0;

				foreach (KeyValuePair<int, int[]> demand in DemandMap[residenceId]) {
					int articleId = demand.Key;
					for (int index = 0; index < demand.Value.Length; index++) {
						//満たされなかった需要の量
						amount += DemandMap[residenceId][articleId][index];
						//この品目は供給されなかったので計算なし
						if (!SupplyMap[residenceId].ContainsKey(articleId))
							continue;

						List<ProductData> list = SupplyMap[residenceId][articleId][index];
						//満たされた需要のGrade x 量を蓄積
						foreach (ProductData data in list) {
							point += data.Amount * data.Grade;
							amount += data.Amount;
							gold += CalcTaxGold(data, tax);
						}

					}
				}
				int average = point / amount;
#if DEVELOP_BUILD
				ResultMap[residences[i].id] = average;
#endif
				//キャラに需要グレードの結果を反映
				foreach (var character in stayer[residences[i].id]) {
					//過去の履歴をずらして新しい記録を追加する
					for (int t = GameConst.Town.HISTORY_MAX - 1; t >= 0; t--) {
						//今回更新される履歴
						if (t < times) {
							character.gradeHistory[t] = average;
							//過去の履歴をずらす
						} else {
							character.gradeHistory[t] = character.gradeHistory[t - times];
						}
					}
					character.grade = character.gradeHistory.Sum() / GameConst.Town.HISTORY_MAX;
				}
			}
			townhallTable.gold = gold;
			UserTownhallTableDAO.Update(townhallTable);

			UserDB.Save();
		}
		/// <summary>
		/// Chainの届け先タイプで分類する
		/// </summary>
		private static Dictionary<int, ChainReceiver> CreateChainDict(List<UserFactoryTable> factories, List<UserMarketTable> markets, List<UserResidenceTable> residences, List<UserServiceTable> services) {
			//施設のDictionaryを作る
			Dictionary<int, UserFactoryTable> factoriesDict = factories.ToDictionary(data => data.id);
			Dictionary<int, UserMarketTable> marketsDict = markets.ToDictionary(data => data.id);
			Dictionary<int, UserResidenceTable> residencesDict = residences.ToDictionary(data => data.id);
			Dictionary<int, UserServiceTable> servicesDict = services.ToDictionary(data => data.id);
			//chainリストを距離でソートした状態で取得する
			List<UserChainTable> chains = UserChainTableDAO.SelectAllOrderByDistancesq();

			Dictionary<int, ChainReceiver> dictionary = new Dictionary<int, ChainReceiver>();
			for (int i = 0, max = chains.Count; i < max; i++) {
				//繋がっているけど物のやりとりはない場合は無視
				if (!chains[i].valid)
					continue;
				int senderId = chains[i].senderId;
				int receiveId = chains[i].recieverId;
				//新規起点の場合はインスタンスを作る
				if (!dictionary.ContainsKey(senderId)) {
					dictionary[senderId] = new ChainReceiver();
				}

				if (factoriesDict.ContainsKey(receiveId)) {
					dictionary[senderId].processers.Add(receiveId);
				} else if (marketsDict.ContainsKey(receiveId)) {
					dictionary[senderId].distributers.Add(receiveId);
				} else if (residencesDict.ContainsKey(receiveId)) {
					dictionary[senderId].consumers.Add(receiveId);
				} else if (servicesDict.ContainsKey(receiveId)) {
					dictionary[senderId].services.Add(receiveId);
				}
			}
			return dictionary;
		}


		/// <summary>
		/// 経過時間が単位時間何回分か計算
		/// </summary>
		private static int CalcTimes() {
			//最後に計算した時刻(unixtime(s))
			/*int lastTime = 0;
			//現在時刻(unixtime(s))
			int currentTime = 0;
			//実行間隔(s)
			int interval = 0;
			int times = (currentTime - lastTime) / interval;
			return times;*/
			return 1;

		}
		/// <summary>
		/// 生産に使う原料消費量
		/// </summary>
		private static void PreCalcConsume(int id, int product, int potential, int maxWorker, Dictionary<int, List<UserMemberTable>> worker) {
			StockMap[id] = new Dictionary<int, int>();
			//働く人がいない
			if (!worker.ContainsKey(id)) {
				foreach (KeyValuePair<int, int> resource in GoodsTool.GoodsResource[product]) {
					//労働者がいないので生産量もゼロ、消費予定のグッズもゼロ
					StockMap[id][resource.Key] = 0;
				}
				return;
			}
			//生産グッズ単位量あたりの原料グッズの使用量を計算する
			foreach (KeyValuePair<int, int> resource in GoodsTool.GoodsResource[product]) {
				int useAmount = resource.Value;
				//消費量 = (100当たりの消費量) * (標準生産量 / 100) * (労働者による生産効率)
				useAmount = (int)(useAmount * ((float)potential / GameConst.Town.AMOUNT_SCALE) * ((float)worker[id].Count / maxWorker));
				StockMap[id][resource.Key] = useAmount;
			}
		}



		/// <summary>
		/// 住居ごとの需要を計算
		/// </summary>
		private static void PreCalcDemand(int id, int baseId, Dictionary<int, List<UserMemberTable>> stayer) {
			//住居に誰も住んでいない
			if (!stayer.ContainsKey(id)) {
				DemandMap[id] = new Dictionary<int, int[]>();
				return;
			}
			var baseDemand = PopTool.DemandByResidence[baseId];
			foreach (var data in baseDemand) {
				if (!DemandMap.ContainsKey(id))
					DemandMap[id] = new Dictionary<int, int[]>();
				if (!DemandMap[id].ContainsKey(data.Article))
					DemandMap[id][data.Article] = new int[data.Number];
				//要求数分だけリストを作る
				for (int j = 0; j < data.Number; j++) {
					DemandMap[id][data.Article][j] = data.Amount * stayer[id].Count;
				}
			}
		}

		/// <summary>
		/// 時間経過による生産量
		/// </summary>
		private static ProductData CalcProduct(int id, int product, int grade, int potential, int maxWorker, Dictionary<int, List<UserMemberTable>> worker) {
			ProductData data = new ProductData(product);

			//働く人がいない
			if (maxWorker != 0 && !worker.ContainsKey(id))
				return data;
			float ratio = 1f;
			//労働者数が定員に満たない場合
			if (maxWorker > 0)
				ratio = (float)worker[id].Count / maxWorker;
			//品質は生産施設のグレードとリソースのグレードを
			//品質 = (生産施設のグレード設定 + (リソースのグレード x リソース種類数)) / (1 +  リソース種類数)
			int gradePoint = grade * GameConst.Town.GRADE_SCALE;
			foreach (var pair in GoodsTool.GoodsResource[product]) {
				int resourceId = pair.Key;                              //原料のグッズId
				//必要が原料がキャッシュされてない場合
				if (!CacheMap.ContainsKey(id) || !CacheMap[id].ContainsKey(resourceId)) {
					data.Amount = 0;
					data.Grade = 0;
					return data;
				}
				float consume = (float)pair.Value * potential / GameConst.Town.AMOUNT_SCALE;                //max生産に必要な原料の量

				int currentCache = CacheMap[id][resourceId].Amount;       //キャッシュされている原料の量
				float r = currentCache / consume;
				//小さい方を採用
				ratio = ratio < r ? ratio : r;

				gradePoint += CacheMap[id][resourceId].Grade;
			}
			data.Amount = (int)(potential * ratio);
			data.Grade = gradePoint / (1 + GoodsTool.GoodsResource[product].Count);
#if DEVELOP_BUILD
			OutputMap[id] = data;
#endif

			return data;
		}

		/// <summary>
		/// 住居ごとの供給を計算
		/// </summary>
		private static void CalcSupply(int residenceId, Dictionary<int, List<ProductData>> supply) {
			Dictionary<int, int[]> demand = DemandMap[residenceId];
			//品目, 要求枠, 供給量の記録
			Dictionary<int, List<ProductData>[]> record = new Dictionary<int, List<ProductData>[]>();

			foreach (KeyValuePair<int, int[]> demandData in demand) {
				int demandArticleId = demandData.Key;
				int[] demandList = demandData.Value;

				//この品目のグッズが供給されてない
				if (!supply.ContainsKey(demandArticleId))
					continue;

				//要求枠, 供給量の記録
				List<ProductData>[] rec = new List<ProductData>[demandList.Length];
				for(int i = 0, max = rec.Length; i < max; i++) {
					rec[i] = new List<ProductData>();
				}


				//供給
				for (int s = 0; s < supply[demandArticleId].Count; s++) {
					int maxValue = 0;
					int demandIndex = 0;
					//同一品目内の残り需要が大きい方を採用
					for (int d = 0; d < demandData.Value.Length; d++) {
						if (demandData.Value[d] > maxValue) {
							maxValue = demandData.Value[d];
							demandIndex = d;
						}
					}
					if (maxValue == 0)
						break;

					//需要を供給で相殺
					//残り需要が供給グッズsの量よりも多い=需要が残る場合
					if (demandData.Value[demandIndex] > supply[demandArticleId][s].Amount) {
						ProductData recData = new ProductData(supply[demandArticleId][s].Goods);
						recData.Amount = supply[demandArticleId][s].Amount;
						recData.Grade = supply[demandArticleId][s].Grade;
						rec[demandIndex].Add(recData);
						//残り需要から供給された分を減算
						demandData.Value[demandIndex] -= supply[demandArticleId][s].Amount;
					} else {
						ProductData recData = new ProductData(supply[demandArticleId][s].Goods);
						recData.Amount = demandData.Value[demandIndex];
						recData.Grade = supply[demandArticleId][s].Grade;
						rec[demandIndex].Add(recData);
						//需要を越える供給があったので残り需要は0
						demandData.Value[demandIndex] = 0;
					}

				}
				record[demandArticleId] = rec;
			}
			SupplyMap[residenceId] = record;
		}

		/// <summary>
		/// 消費税計算
		/// </summary>
		private static int CalcTaxGold(ProductData data, int tax) {
			return data.Amount * GoodsTool.GoodsPrice[data.Goods] * tax / GameConst.Town.TAX_SCALE;
		}
	}
}
