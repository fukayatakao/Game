using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	//@note デバッグ機能なのでMonobehaviour継承してこのクラス単体で完結させる
	/// <summary>
	/// 攻撃範囲の扇表示
	/// </summary>
	public class DebugTargetDraw : MonoBehaviour {
		PrimitiveSphere deploy_ = new PrimitiveSphere();

		CharacterEntity owner_;
		Transform trans_;
		Color deployColor_;
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
		public void Init(CharacterEntity owner, Transform trans) {
			owner_ = owner;
			trans_ = trans;
			float scl = owner_.HaveCollision.Radius * 2 / trans_.localScale.x;
			{
				GameObject obj = deploy_.Create("DeployPosition");
				deployColor_ = new Color(0.8f, 0f, 0.8f, 0.8f);
				deploy_.SetWireframe(true);
				deploy_.SetColor(deployColor_);
				deploy_.SetScale(new Vector3(scl * 0.5f, 1f, scl * 0.5f));
				deploy_.SetParent(CharacterAssembly.I.DebugRoot.transform, false);
			}

			SetVisible(false);
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void LateUpdate() {
			deploy_.SetPosition(owner_.HavePosition.DeployPosition);

			float scl = owner_.HaveCollision.Radius * 2 / trans_.localScale.x;
			deploy_.SetScale(new Vector3(scl, 1f, scl));
			//回転は無効化
			deploy_.cacheTrans.rotation = Quaternion.identity;
		}

		/// <summary>
		/// 表示切替
		/// </summary>
		public void SetVisible(bool flag) {
			deploy_.SetActive(flag);
		}
	}
#endif
}
