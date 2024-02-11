using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	//@note デバッグ機能なのでMonobehaviour継承してこのクラス単体で完結させる
	/// <summary>
	/// 攻撃範囲の扇表示
	/// </summary>
	public class DebugAIState : MonoBehaviour {
		GameObject text_;
		TMPro.TextMeshPro textMesh_;

		static readonly Vector3 size = new Vector3(0.2f, 0.2f, 0.2f);

		CharacterEntity owner_;
		Camera camera_;
		private const float OFFSET_Y = 0.4f;

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;

		void Awake() {

			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}

		private void OnDestroy() {
			MessageSystem.DestroyReceptor(receptor_);
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		public void Init(CharacterEntity owner, Camera camera) {
			owner_ = owner;
			camera_ = camera;
			GameObject obj = ResourceCache.Load<GameObject>(ResourcesPath.UI_HEAD_TEXT);
			text_ = UnityUtil.Instantiate(obj);
			textMesh_ = text_.GetComponent<TMPro.TextMeshPro>();
			text_.transform.localScale = size;

			SetVisible(false);
		}

		/// <summary>
		/// 親ノード設定
		/// </summary>
		public void SetParent(Transform parent) {
			text_.transform.SetParent(parent, false);
            //キャラの足元に置く
            //fan_.SetPosition(Vector3.up * 0.01f);
        }
		/// <summary>
		/// 実行処理
		/// </summary>
        public void LateUpdate() {
			if (!text_.activeSelf)
				return;
			if (!owner_.IsAlive) {
				SetVisible(false);
				return;
			}

			Vector3 pos = owner_.HaveCacheNode.GetNode(CharacterNodeCache.Part.Head).transform.position;
			pos.y = pos.y + OFFSET_Y;

			textMesh_.text = owner_.HaveThink.CurrentLabel();

			text_.transform.position = pos;
			text_.transform.rotation = camera_.transform.rotation;
		}


		/// <summary>
		/// 表示切替
		/// </summary>
		public void SetVisible(bool flag) {
			text_.SetActive(flag);
		}

	}
#endif
}
