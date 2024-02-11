#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Project.Mst;
using System.Linq;

namespace Project.Game.Goods {
	public class GoodsEditor : EditorWindow {
		//原料になっいるグッズ=削除不可能
		HashSet<int> resourceGoods;
		Dictionary<int, MstGoodsData> masterGoodsDict_;
		System.Action<int> callback_;

		MstGoodsData newGoods = new MstGoodsData();
		Vector2 scroll_ = Vector2.zero;
		List<MstArticleData> article_;
		string[] articleNames_;

		public void Init(Dictionary<int, MstGoodsData> dict, Dictionary<int, MstArticleData> article, System.Action<int> callback) {
			masterGoodsDict_ = dict;
			callback_ = callback;
			CalcHash();

			article_ = new List<MstArticleData>(article.Values);
			articleNames_ = article_.Select(x => x.DbgName).ToArray();
		}

		private void CalcHash() {
			resourceGoods = new HashSet<int>();

			foreach (int key in masterGoodsDict_.Keys) {
				//調査不要
				if (key <= 0)
					continue;

				foreach (MstGoodsData value in masterGoodsDict_.Values) {
					if (value.ResourceId1 == key || value.ResourceId2 == key || value.ResourceId3 == key) {
						resourceGoods.Add(key);
						break;
					}
				}
			}

		}

		/// <summary>
		/// 表示処理
		/// </summary>
		protected void OnGUI() {
			if (masterGoodsDict_ == null)
				return;

			scroll_ = EditorGUILayout.BeginScrollView(scroll_);
			int removeId = 0;
			GUILayout.BeginHorizontal();
			GUILayout.Label("ID", GUILayout.Width(80));
			GUILayout.Label("品目", GUILayout.Width(80));
			GUILayout.Label("名称", GUILayout.Width(160));
			GUILayout.Label("価格");
			GUILayout.EndHorizontal();
			EditorGUILayout.Separator();
			foreach (var mst in masterGoodsDict_.Values) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(mst.Id.ToString(), GUILayout.Width(80));

				int select = article_.FindIndex(x => x.Id == mst.Article);
				select = EditorGUILayout.Popup(select, articleNames_, GUILayout.Width(80));
				mst.Article = article_[select].Id;

				GUILayout.Label(mst.DbgName, GUILayout.Width(160));
				mst.Price = EditorGUILayout.IntField(mst.Price);

				GUI.enabled = !resourceGoods.Contains(mst.Id) && mst.Id > 0;
				if(GUILayout.Button("削除")){
					removeId = mst.Id;
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
			if (removeId != 0) {
				callback_(removeId);
				CalcHash();
				return;
			}
			GUILayout.Label("");
			GUILayout.Label("");
			GUILayout.Label("新規追加");
			GUILayout.BeginHorizontal();
			newGoods.Id = EditorGUILayout.IntField(newGoods.Id, GUILayout.Width(80));
			{
				int select = article_.FindIndex(x => x.Id == newGoods.Article);
				select = EditorGUILayout.Popup(select, articleNames_, GUILayout.Width(80));
				newGoods.Article = article_[select].Id;
			}
			newGoods.DbgName = EditorGUILayout.TextField(newGoods.DbgName, GUILayout.Width(160));
			newGoods.Price = EditorGUILayout.IntField(newGoods.Price);

			if (GUILayout.Button("追加")) {
				//idがかぶっていないかチェック、品目もちゃんと指定されてるかチェック
				if (!masterGoodsDict_.ContainsKey(newGoods.Id) && newGoods.Article != 0) {
					newGoods.Resolve();
					masterGoodsDict_[newGoods.Id] = newGoods;
					GoodsGUI.Init(masterGoodsDict_);
					newGoods = new MstGoodsData();
				}

			}
			GUILayout.EndHorizontal();

		}
	}
}
#endif
