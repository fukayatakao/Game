using Project.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// タウンで使用する情報
	/// </summary>
	[System.Serializable]
	public class TownItemData {
		//シングルトン
		static TownItemData instance_;
		public static TownItemData I { get { return instance_; } }

		//アイテムデータ
		private Dictionary<int, Dictionary<int, ItemData>> items_;
		public Dictionary<int, Dictionary<int, ItemData>> Items { get { return items_; } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private TownItemData(){}
		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response response) {
			instance_ = new TownItemData();
			instance_.Setup(response as TownMainResponse);
		}

		/// <remarks>
		/// インスタンス破棄
		/// </remarks>
		public static void Destroy() {
			instance_ = null;
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		private void Setup(TownMainResponse response) {
			items_ = new Dictionary<int, Dictionary<int, ItemData>>();
			for (int i = 0, max = response.items.Count; i < max; i++) {
				ItemData item = response.items[i];
				if (!items_.ContainsKey(item.type)) {
					items_[item.type] = new Dictionary<int, ItemData>();
				}

				items_[item.type][item.id] = item;
			}

		}

		public bool IsExist(int type, int id) {
			return items_.ContainsKey(type) && items_[type].ContainsKey(id);
		}

		public ItemData GetItemData(int type, int id)
		{
			if (IsExist(type, id))
				return items_[type][id];
			else
				return null;
		}

		/// <summary>
		/// アイテムの数量変化
		/// </summary>
		public void AddItemNumber(int type, int id, int number)
		{
			if (IsExist(type, id))
			{
				items_[type][id].number += number;
			}
			else
			{
				if (!items_.ContainsKey(type))
				{
					items_[type] = new Dictionary<int, ItemData>();
				}

				items_[type][id] = new ItemData(){type = type, id = id, number = number};
			}

			Debug.Assert(items_.ContainsKey(type), "item type dictionary error:" + type);
			Debug.Assert(items_[type].ContainsKey(id), "item id dictionary error:" + id);
			Debug.Assert(items_[type][id].number >= 0, "invalid item number:" + number);

		}
	}
}
