using System.Collections.Generic;
//Linq重いから嫌い
using System.Linq;


namespace Project.Mst {

	public static partial class BaseDataManager {
		/// <summary>
		/// マスターのキャッシュが終わった後に行う処理
		/// </summary>
		static partial void PostCacheProcess() {
			CreateDirectory();
			ConvertMstData();
		}
		/// <summary>
		/// マスターのDictionaryを生成
		/// </summary>
		static void CreateDirectory() {
			Cache<int, MstLeaderData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstUnitData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstActionData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstSkillData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstEnemyLeaderParam>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstEnemyMemberParam>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstEnemyCorps>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstAbilityData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstQuestData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });

			Cache<int, MstFieldData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });

			Cache<int, MstFactoryData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstResidenceData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstStorageData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstMarketData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstServiceData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstTownhallData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstGoodsData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstPopulationData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstPopulationDemandData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
			Cache<int, MstArticleData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });

			Cache<int, MstStrategyMapData>.Create((data) => { return data.ToDictionary(mst => mst.Id); });
		}


		/// <summary>
		/// マスターのデータ形式をちょっと変換
		/// </summary>
		/// <remarks>
		/// サーバから取得するようになると一部の項目を配列で取得するようになる
		/// </remarks>
		static void ConvertMstData() {
			//Populationのデータ変形
			Cache<int, MstPopulationDataSv>.Create<MstPopulationData>((data) => {
				Dictionary<int, MstPopulationDataSv> dict = new Dictionary<int, MstPopulationDataSv>();
				for (int i = 0, max = data.Count; i < max; i++) {
					MstPopulationDataSv dataSv = new MstPopulationDataSv(data[i], Cache<MstPopulationDemandData>.GetList());
					dict[data[i].Id] = dataSv;
				}
				return dict;
			});


			//GoodsDataのデータ変形
			foreach (var mst in Cache<MstGoodsData>.GetList()) {
				mst.Resolve();
			}

			//AttackDataのデータ変形
			foreach (var mst in Cache<MstActionData>.GetList()) {
				mst.Resolve();
			}

			//Unitのデータ変形
			foreach (var mst in Cache<MstUnitData>.GetList()) {
				mst.Resolve(Cache<int, MstActionData>.GetDictionary());
			}
			//Leaderのデータ変形
			foreach (var mst in Cache<MstLeaderData>.GetList()) {
				mst.Resolve(Cache<int, MstUnitData>.GetDictionary());
			}
		}


	}


}
