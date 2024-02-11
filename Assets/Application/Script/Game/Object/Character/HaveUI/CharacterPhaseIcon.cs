using UnityEngine;
using Project.Lib;
using UnityEngine.UI;
using Project.Http.Mst;

namespace Project.Game {
	/// <summary>
	/// キャラクタの属性アイコン表示
	/// </summary>
	public class CharacterPhaseIcon : MonoPretender {
		//UIが表示されるカメラ
		Camera camera_;
		//表示される画像
		GameObject ImageObj_;
		Image image_;

		private bool enable_ = false;
		//表示位置オフセット
		private static readonly Vector2 HEIGHT_OFFSET = new Vector2(0f, 0.8f);
		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		/// <summary>
		/// 生成
		/// </summary>
		protected override void Create(GameObject obj) {
			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.UserEvent);
		}
		/// <summary>
		/// 破棄
		/// </summary>
		protected override void Destroy() {
			MessageSystem.DestroyReceptor(receptor_);

			GameObject.Destroy(ImageObj_);
			enable_ = false;
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public void Init(Camera camera, Transform canvas) {
			camera_ = camera;
			GameObject prefab = ResourceCache.Load<GameObject>("UI/PhaseUI");
			//HP無敵状態の表示
			ImageObj_ = UnityUtil.InstantiateChild(canvas, prefab);
			image_ = ImageObj_.GetComponent<Image>();
			Hide();
		}

		/// <summary>
		/// 属性に対応する画像をspriteに設定
		/// </summary>
		public void SetPhaseSprite(PHASE phase) {
			var tex = ResourcePool.GetPhaseIcon(phase);
			image_.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			if (!enable_)
				return;
			CalcPosition(owner.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head));
		}
		/// <summary>
		/// UI表示
		/// </summary>
		public void Show() {
			enable_ = true;
			image_.enabled = true;
		}
		/// <summary>
		/// UI隠す
		/// </summary>
		public void Hide() {
			enable_ = false;
			image_.enabled = false;
		}
		/// <summary>
		/// 表示位置を計算する
		/// </summary>
		private void CalcPosition(Transform node) {
			Vector3 pos = node.position;
			float x = pos.x;
			pos.y = pos.y + HEIGHT_OFFSET.y;

			pos.x = x + HEIGHT_OFFSET.x;
			image_.transform.position = pos;
			image_.transform.rotation = camera_.transform.rotation;
		}
	}

}
