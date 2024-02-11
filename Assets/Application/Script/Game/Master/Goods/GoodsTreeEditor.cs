#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Project.Lib;
using Project.Mst;
using Project.Network;


namespace Project.Game.Goods {
	public class GoodsTreeEditor : Lib.TreeEditor.TreeEditor {
		/// <summary>
		/// 編集ダイアログのオープン
		/// </summary>
		[MenuItem("Editor/GoodsTreeEditor", false, 98)]
		private static void Open() {
			GoodsTreeEditor window = EditorWindow.GetWindow<GoodsTreeEditor>(false, "GoodsTreeEditor");
		}
		/// <summary>
		/// ウィンドウがアクティブになったとき
		/// </summary>
		private void OnEnable() {
			Init();
		}
		private void OnDestroy() {
		}
		protected override void SetupOperation() {
			operationExecute_.Add(new Lib.TreeEditor.MainControl(this, operation_).Execute);
			operationExecute_.Add(new SelectControl(this, operation_).Execute);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		protected override void Init() {
			base.Init();
			//Goodsマスターをロード
			{
				TextAsset asset = (Resources.Load(ResourcesPath.MASTER_GOODS_DATA) as TextAsset);
				string json = asset.text;
				masterGoodsDict_ = JsonList.FromJsonList<MstGoodsData>(json).ToDictionary(x => x.Id);
				masterGoodsDict_[-1] = new MstGoodsData() { Id = -1, DbgName = "未設定" };
			}
			//品目マスターをロード
			{
				TextAsset asset = (Resources.Load(ResourcesPath.MASTER_ARTICLE_DATA) as TextAsset);
				string json = asset.text;
				masterArticleDict_ = JsonList.FromJsonList<MstArticleData>(json).ToDictionary(x => x.Id);
			}

			//GoodsDataのデータ変形
			foreach (var mst in masterGoodsDict_.Values) {
				mst.Resolve();
			}

			GoodsGUI.Init(masterGoodsDict_);
			selectGoods_ = masterGoodsDict_[0];
			BuildNode(selectGoods_);
		}
		GoodsEditor subWindow_;

		Dictionary<int, MstGoodsData> masterGoodsDict_;
		Dictionary<int, MstArticleData> masterArticleDict_;
		Vector2 scroll_ = Vector2.zero;

		private MstGoodsData selectGoods_;

		private void EditMaster() {
			Node node = Node.control.EditNode;
			int prevGoods = Node.control.PrevGoodsId;
			int changeGoods = node.goodsId;
			int parentGoods = ((Node)node.parent).goodsId;
			if (masterGoodsDict_[parentGoods].ResourceId1 == prevGoods) {
				masterGoodsDict_[parentGoods].ResourceId1 = changeGoods;
			}
			if (masterGoodsDict_[parentGoods].ResourceId2 == prevGoods) {
				masterGoodsDict_[parentGoods].ResourceId2 = changeGoods;
			}
			if (masterGoodsDict_[parentGoods].ResourceId3 == prevGoods) {
				masterGoodsDict_[parentGoods].ResourceId3 = changeGoods;
			}
			masterGoodsDict_[parentGoods].Resolve();
			BuildNode(selectGoods_);
		}

		/// <summary>
		/// 原料の必要数を変更
		/// </summary>
		void ChangeAmount() {
			Node node = Node.control.EditNode;
			if (node.parent == null)
				return;

			int prevGoods = Node.control.PrevGoodsId;
			int parentGoods = ((Node)node.parent).goodsId;
			if (masterGoodsDict_[parentGoods].ResourceId1 == prevGoods) {
				masterGoodsDict_[parentGoods].UseAmount1 = node.amount;
				masterGoodsDict_[parentGoods].Resolve();
			}
			if (masterGoodsDict_[parentGoods].ResourceId2 == prevGoods) {
				masterGoodsDict_[parentGoods].UseAmount2 = node.amount;
				masterGoodsDict_[parentGoods].Resolve();
			}
			if (masterGoodsDict_[parentGoods].ResourceId3 == prevGoods) {
				masterGoodsDict_[parentGoods].UseAmount3 = node.amount;
				masterGoodsDict_[parentGoods].Resolve();
			}
		}

		/// <summary>
		/// 新たに原料を追加
		/// </summary>
		/// <remarks>
		/// 追加直後は未設定グッズとして追加
		/// </remarks>
		void AddNode() {
			Node node = Node.control.EditNode;
			int goods = node.goodsId;
			//ノードが未設定の場合は子階層設定不可
			if (goods < 0)
				return;
			if (masterGoodsDict_[goods].ResourceId1 == 0) {
				masterGoodsDict_[goods].ResourceId1 = -1;
			} else if (masterGoodsDict_[goods].ResourceId2 == 0) {
				masterGoodsDict_[goods].ResourceId2 = -1;
			} else if (masterGoodsDict_[goods].ResourceId3 == 0) {
				masterGoodsDict_[goods].ResourceId3 = -1;
			} else {
				return;
			}
			masterGoodsDict_[goods].Resolve();
			BuildNode(selectGoods_);
		}

		/// <summary>
		/// 原材料を削除
		/// </summary>
		void DelNode() {
			Node node = Node.control.EditNode;
			if (node.parent == null)
				return;

			int goodsId = Node.control.EditNode.goodsId;
			int parentGoods = ((Node)node.parent).goodsId;
			if (masterGoodsDict_[parentGoods].ResourceId1 == goodsId) {
				masterGoodsDict_[parentGoods].ResourceId1 = 0;
			}
			if (masterGoodsDict_[parentGoods].ResourceId2 == goodsId) {
				masterGoodsDict_[parentGoods].ResourceId2 = 0;
			}
			if (masterGoodsDict_[parentGoods].ResourceId3 == goodsId) {
				masterGoodsDict_[parentGoods].ResourceId3 = 0;
			}
			masterGoodsDict_[parentGoods].Resolve();
			BuildNode(selectGoods_);

		}

		/// <summary>
		/// 表示処理
		/// </summary>
		protected override void OnGUI() {
			Node.control.ResetState();
			base.OnGUI();
			//原料の必要数を変更した
			if (Node.control.IsChangeAmount()) {
				ChangeAmount();
			}
			if (Node.control.IsAdd()) {
				AddNode();
			}
			if (Node.control.IsDelete()) {
				DelNode();
			}

			//階層のどこかのノードのグッズが変更された
			if (Node.control.IsChangeGoods()) {
				//一番上のノードは選択グッズの変更として処理
				if (Node.control.EditNode.parent == null) {
					selectGoods_ = masterGoodsDict_[Node.control.EditNode.goodsId];
					BuildNode(selectGoods_);
				} else {
					//有効な変更かチェックする
					bool check = CheckChangeGoods();
					//データ不整合を起こす変更だったので戻す
					if (!check) {
						Node.control.Rollback();
						//問題ないのでマスターに反映
					} else {
						EditMaster();
					}
				}
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("");
			using (new GUILayout.VerticalScope(GUILayout.Width(160))) {
				GUILayout.Label("グッズ一覧");
				if (GUILayout.Button("Open")) {
					subWindow_ = new GoodsEditor();
					subWindow_.Init(masterGoodsDict_, masterArticleDict_, RemoveGoods);
					subWindow_.Show();
				}

				scroll_ = EditorGUILayout.BeginScrollView(scroll_);
				EditorGUILayout.EndScrollView();

				//index = EditorGUILayout.Popup(index, goodsName_, NodeConst.NodeStyle);

				if (GUILayout.Button("Save")) {
					Save();
				}
				if (GUILayout.Button("SaveTsv")) {
					SaveTsv();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void OnFocus() {
			if(subWindow_ != null) {
				subWindow_.Close();
				subWindow_ = null;
			}
		}

		/// <summary>
		/// グッズノードの変更が出来るかチェック
		/// </summary>
		private bool CheckChangeGoods() {
			Node node = Node.control.EditNode;
			int prevGoods = Node.control.PrevGoodsId;
			int changeGoods = node.goodsId;
			int parentGoods = ((Node)node.parent).goodsId;

			//親子で同じグッズの場合は不可
			if (parentGoods == changeGoods) {
				return false;
			}
			if (Node.control.CheckSameChild()) {
				return false;
			}
			//循環してしまう変更は不可
			if (CycleGoods(parentGoods, changeGoods)) {
				return false;
			}


			return true;
		}

		/// <summary>
		/// グッズ変更で生産ツリーが循環しないかチェック
		/// </summary>
		private bool CycleGoods(int parentId, int targetId) {
			List<int> openList = new List<int>();
			List<int> closeList = new List<int>();

			openList.Add(parentId);

			while(openList.Count > 0) {
				int id = openList[0];
				foreach (var mst in masterGoodsDict_.Values) {
					for (int i = 0, max = mst.Resource.Length; i < max; i++) {
						//原料に使っていない場合は無視
						if (mst.Resource[i] != id)
							continue;
						//検査済のidは無視
						if (closeList.FindIndex(x => x == mst.Id) >= 0)
							continue;
						//検査リストに追加
						openList.Add(mst.Id);
						break;
					}
				}
				closeList.Add(id);
				openList.RemoveAt(0);
			}

			return closeList.FindIndex(x => x == targetId) >= 0;
		}

		/// <summary>
		/// 選択したグッズの表示
		/// </summary>
		private void BuildNode(MstGoodsData data) {
			var newRoot = new Node(data.Id, GameConst.Town.AMOUNT_SCALE);
			BuildChild(newRoot, data);

			root = newRoot;
			current = root;
			ResetLayout();
		}

		/// <summary>
		/// 再帰して子階層を作る
		/// </summary>
		private void BuildChild(Node parent, MstGoodsData data) {
			for (int i = 0, max = data.Resource.Length; i < max; i++) {
				if (data.Resource[i] == 0)
					continue;
				Node n = new Node(data.Resource[i], data.UseAmount[i]);
				AddNode(parent, n);
				BuildChild(n, masterGoodsDict_[data.Resource[i]]);
			}
		}

		/// <summary>
		/// グッズを削除
		/// </summary>
		private void RemoveGoods(int id) {
			if(selectGoods_.Id == id) {
				selectGoods_ = masterGoodsDict_[0];
				BuildNode(selectGoods_);
			}
			masterGoodsDict_.Remove(id);
			GoodsGUI.Init(masterGoodsDict_);

		}
		/// <summary>
		/// jsonにして保存
		/// </summary>
		private void Save() {
			string path = Application.dataPath + "/Application/Json/Resources/" + ResourcesPath.MASTER_GOODS_DATA + ".txt";


			var list = new List<MstGoodsData>(masterGoodsDict_.Values);
			//未設定グッズは要らないので削除
			var removeList = list.FindAll(x => x.Id < 0);
			list = list.Except(removeList).ToList();
			for (int i = 0, max = list.Count; i < max; i++) {
				if (list[i].ResourceId1 < 0) {
					list[i].ResourceId1 = 0;
				}
				if (list[i].ResourceId2 < 0) {
					list[i].ResourceId2 = 0;
				}
				if (list[i].ResourceId3 < 0) {
					list[i].ResourceId3 = 0;
				}
			}
			string output = JsonList.ToJsonList(list);
			System.IO.File.WriteAllText(path, output);
		}

		/// <summary>
		/// tsvにして保存
		/// </summary>
		private void SaveTsv() {
			string path = Application.dataPath + "/../Converter/mstdatagen/master/facility/MstGoodsData.txt";


			var list = new List<MstGoodsData>(masterGoodsDict_.Values);
			//未設定グッズは要らないので削除
			var removeList = list.FindAll(x => x.Id < 0);
			list = list.Except(removeList).ToList();
			for (int i = 0, max = list.Count; i < max; i++) {
				if (list[i].ResourceId1 < 0) {
					list[i].ResourceId1 = 0;
				}
				if (list[i].ResourceId2 < 0) {
					list[i].ResourceId2 = 0;
				}
				if (list[i].ResourceId3 < 0) {
					list[i].ResourceId3 = 0;
				}
			}

			// CSV形式のデータを作成
			StringBuilder csvData = new StringBuilder();
			csvData.AppendLine("id\t品名\tモデル名\t品目ID\t品目名\t価格\t必要品1\t品名\t消費量\t必要品2\t品名\t消費量\t必要品3\t品名\t消費量");

			foreach (var data in list)
			{
				//IDから価格まで
				csvData.Append($"{data.Id}\t{data.DbgName}\t\t{data.Article}\t{masterArticleDict_[data.Article].DbgName}\t{data.Price}\t");
				//必要品と消費量
				csvData.Append($"{data.ResourceId1}\t{masterGoodsDict_[data.ResourceId1].DbgName}\t{data.UseAmount1}\t");
				csvData.Append($"{data.ResourceId2}\t{masterGoodsDict_[data.ResourceId2].DbgName}\t{data.UseAmount2}\t");
				csvData.Append($"{data.ResourceId3}\t{masterGoodsDict_[data.ResourceId3].DbgName}\t{data.UseAmount3}\n");
			}

			// ファイルにCSV形式でテキストを書き込む
			File.WriteAllText(path, csvData.ToString(), Encoding.UTF8);
		}
	}
}
#endif
