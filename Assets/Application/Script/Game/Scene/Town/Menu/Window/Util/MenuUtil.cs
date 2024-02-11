using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Mst;
using Project.Server;
using System;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 分類難しいメニューの部分機能
	/// </summary>
	public static class MenuUtil {
		public static void DrawProduct(Facility facility) {
			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			FitGUILayout.Label("前回生産");
			if (Project.Server.TownTool.OutputMap.ContainsKey(facility.Id)) {
				int product = Project.Server.TownTool.OutputMap[facility.Id].Goods;
				int amount = Project.Server.TownTool.OutputMap[facility.Id].Amount;
				float grade = Project.Server.TownTool.OutputMap[facility.Id].Grade / (float)GameConst.Town.GRADE_SCALE;
				FitGUILayout.Label(goods[product].DbgName + " x " + amount + " ☆" + Math.Round(grade, 2));
			} else {
				FitGUILayout.Label("なし");
			}

			if (Project.Server.TownTool.StockMap.ContainsKey(facility.Id)) {
				FitGUILayout.Label("原料 (供給(不足) : ☆) ");
				foreach (KeyValuePair<int, int> consume in Project.Server.TownTool.StockMap[facility.Id]) {
					int goodsId = consume.Key;
					ProductData val = Project.Server.TownTool.CacheMap[facility.Id][goodsId];
					FitGUILayout.Label(goods[goodsId].DbgName + " x " + val.Amount + "(" + consume.Value + ") : ☆" + val.Grade / (float)GameConst.Town.GRADE_SCALE);
				}
			}
		}
		/// <summary>
		/// 分配可能なグッズを表示
		/// </summary>
		public static void DrawMarketResult(Market market) {
			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			FitGUILayout.Label("供給 ☆");
			//一度も計算してない場合はデータがないのでエラー回避
			if (!Project.Server.TownTool.CacheMap.ContainsKey(market.Id)) {
				return;
			}

			foreach (KeyValuePair<int, ProductData> cache in Project.Server.TownTool.CacheMap[market.Id]) {
				int goodsId = cache.Key;
				FitGUILayout.Label(goods[goodsId].DbgName + " x " + cache.Value.Amount + " : ☆" + cache.Value.Grade / (float)GameConst.Town.GRADE_SCALE);
			}
		}
		/// <summary>
		/// 住居の住人に提供されたグッズを表示
		/// </summary>
		public static void DrawResidenceSupply(Residence residence) {
			var article = BaseDataManager.GetDictionary<int, MstArticleData>();
			var goods = BaseDataManager.GetDictionary<int, MstGoodsData>();
			//一度も計算してない場合はデータがないのでエラー回避
			if (!Project.Server.TownTool.SupplyMap.ContainsKey(residence.Id)) {
				return;
			}
			FitGUILayout.Label("結果グレード：" + Project.Server.TownTool.ResultMap[residence.Id] / (float)GameConst.Town.GRADE_SCALE);

			foreach (KeyValuePair<int, List<ProductData>[]> supply in Project.Server.TownTool.SupplyMap[residence.Id]) {
				FitGUILayout.Label("品目：" + article[supply.Key].DbgName);
				for(int i = 0, max = supply.Value.Length; i < max; i++) {
					FitGUILayout.Label("要求枠" + (i + 1));
					for(int j = 0, max2 = supply.Value[i].Count; j < max2; j++) {
						ProductData data = supply.Value[i][j];
						FitGUILayout.Label(goods[data.Goods].DbgName + " x " + data.Amount + " : ☆" + data.Grade / (float)GameConst.Town.GRADE_SCALE);
					}
				}
			}
		}

		/// <summary>
		/// 新規作成・移動などの建物共通のボタンを表示する
		/// </summary>
		public static void DrawFooter(Facility facility, FacilityMenuState state) {
			FitGUILayout.Label("", 0, 8);
			float r = facility.HaveArea.Radius;
			FitGUILayout.Label("流通半径:" + r.ToString("0.00"));
			GUIUtil.BeginChangeCheck();
			r  = FitGUILayout.Slider(r, 0, facility.HaveArea.MaxRadius);

			if (GUIUtil.EndChangeCheck()) {
				TownMessage.UpdateArea.Broadcast(facility, r);
			}

			switch (state) {
			//建築時の表示
			case FacilityMenuState.Build:
				//ほかの建物と衝突している場合は建てれないようにボタンを無効にする
				GUI.enabled = facility.IsResolveCollision();
				if (FitGUILayout.Button("決定")) {
					TownMessage.BuildFacility.Broadcast(facility);
				}
				GUI.enabled = true;
				if (FitGUILayout.Button("キャンセル")) {
					TownMessage.DestroyFacility.Broadcast(facility);
				}
				break;
			//再配置時の表示
			case FacilityMenuState.ReLocate:
				//ほかの建物と衝突している場合は建てれないようにボタンを無効にする
				GUI.enabled = facility.IsResolveCollision();
				if (FitGUILayout.Button("確定")) {
					TownMessage.CommitLocateFacility.Broadcast(facility);
				}
				GUI.enabled = true;
				if (FitGUILayout.Button("キャンセル")) {
					TownMessage.CancelLocateFacility.Broadcast(facility);
				}
				break;
			//デフォルト状態
			case FacilityMenuState.Default:
				if (FitGUILayout.Button("再配置")) {
					TownMessage.ReLocateFacility.Broadcast(facility);
				}
				if (FitGUILayout.Button("撤去")) {
					TownMessage.DemolishFacility.Broadcast(facility);
				}

				break;
			//建築時or再配置時に対象ではない建物の情報を表示したとき
			case FacilityMenuState.None:
				break;
			}
		}
	}
#endif
}
