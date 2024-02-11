using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// キャラクター名表示
	/// </summary>
	public class CharacterNameText : MonoPretender {
		static readonly Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);
		private static readonly Vector2 HEIGHT_OFFSET = new Vector2(0f, 0.8f);

		//UIが表示されるカメラ
		Camera camera_;
		GameObject textObj_;
		TMPro.TextMeshPro textMesh_;
		private bool enable_ = false;

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

			GameObject.Destroy(textObj_);
			enable_ = false;
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		public void Init(Camera camera, Transform canvas) {
			camera_ = camera;
			GameObject prefab = ResourceCache.Load<GameObject>(ResourcesPath.UI_HEAD_TEXT);
			textObj_ = UnityUtil.InstantiateChild(canvas, prefab);
			textMesh_ = textObj_.GetComponent<TMPro.TextMeshPro>();
			textMesh_.transform.localScale = size;

			Hide();
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			if (!enable_)
				return;
			textMesh_.text = owner.HavePersonal.CharaName;
			CalcPosition(owner.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head));
		}
		/// <summary>
		/// UI表示
		/// </summary>
		public void Show() {
			enable_ = true;
			textMesh_.enabled = true;
		}
		/// <summary>
		/// UI隠す
		/// </summary>
		public void Hide() {
			enable_ = false;
			textMesh_.enabled = false;
		}
		/// <summary>
		/// 表示位置を計算する
		/// </summary>
		private void CalcPosition(Transform node) {
			Vector3 pos = node.position;
			float x = pos.x;
			pos.y = pos.y + HEIGHT_OFFSET.y;

			pos.x = x + HEIGHT_OFFSET.x;
			textMesh_.transform.position = pos;
			textMesh_.transform.rotation = camera_.transform.rotation;
		}
	}
}
