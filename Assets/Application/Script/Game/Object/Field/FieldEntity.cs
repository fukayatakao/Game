using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {

	/// <summary>
	/// フィールド
	/// </summary>
	public class FieldEntity : Entity {
		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;
		//フィールド用シーン
		private FieldScene haveScene_;
		public FieldScene HaveScene { get { return haveScene_; } }
		//空のテクスチャ管理
		private FieldSky haveSky_;
		public FieldSky HaveSky { get { return haveSky_; } }
		//経路
		private FieldWayMap haveWaymap_;
		public FieldWayMap HaveWaymap { get { return haveWaymap_; } }

		//ステージサイズ
		private Vector2 size_;
		public float Width { get { return size_.x; } }
		public float Depth { get { return size_.y; } }
		//ステージ中央部分の奥行
		private float centerDepth_;
		public float CenterDepth { get { return centerDepth_; } }
		//各陣地の奥行
		private float territoryDepth_;
		public float TerritoryDepth { get { return territoryDepth_; } }

		private Rect stageArea_;
		public Rect StageArea { get { return stageArea_; } }

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = new GameObject(resName);
			haveScene_ = MonoPretender.Create<FieldScene>(obj);
			haveSky_ = MonoPretender.Create<FieldSky>(obj);

			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.GameEvent);

			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);
			return obj;
		}
		/// <summary>
		/// インスタンス破棄処理
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(haveWaymap_);
			MonoPretender.Destroy(haveScene_);
			MonoPretender.Destroy(haveSky_);
		}
		/// <summary>
		/// 生成と同時に行う初期化
		/// </summary>
		public async Task Load(string stage, string sky) {
			await haveScene_.LoadAsync(stage);
			haveSky_.Load(sky);
		}

		/// <summary>
		/// 生成と同時に行う初期化
		/// </summary>
		public async Task Load(string stage, string sky, string waymap) {
			await Load(stage, sky);

			haveWaymap_ = MonoPretender.Create<FieldWayMap>(gameObject_);
			GameObject obj = new GameObject("Waymap");
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, haveScene_.Scene);
			await haveWaymap_.LoadAsync(obj, waymap);



		}

		/// <summary>
		/// 直接ステージのサイズをセット
		/// </summary>
		public void InitSize(Vector2 size) {
			size_ = size;
		}

		/// <summary>
		/// パラメータの設定
		/// </summary>
		public void InitData(Mst.MstFieldData fieldData) {
			size_.x = fieldData.Width;
			size_.y = fieldData.Depth + fieldData.BackDepth * 2;
			centerDepth_ = fieldData.Depth;
			territoryDepth_ = fieldData.BackDepth;

			stageArea_ = new Rect();
			stageArea_.xMin = -size_.x * 0.5f;
			stageArea_.xMax = size_.x * 0.5f;
			stageArea_.yMin = -size_.y * 0.5f;
			stageArea_.yMax = size_.y * 0.5f;
		}
	}
}
