using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	//@note デバッグ機能なのでMonobehaviour継承してこのクラス単体で完結させる
	/// <summary>
	/// 攻撃範囲の扇表示
	/// </summary>
	public class DebugCollisionDraw : MonoBehaviour {
		PrimitiveCube cube_ = new PrimitiveCube();

		CharacterEntity owner_;
		Transform trans_;
		Color color_;
		Color selectColor_;
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
			GameObject obj = cube_.Create("coll");
			color_ = new Color(0.8f, 0.8f, 0.2f, 0.4f); //SelectColor(owner.HaveParam.Party.Squad.Platoon.Index);
			selectColor_ = new Color(0.8f, 0.8f, 0.2f, 0.4f);

			color_.a = 0.2f;
			cube_.SetColor(color_);

			float scl = owner_.HaveCollision.Radius * 2 / trans_.localScale.x;
			cube_.SetScale(new Vector3(scl, 1f, scl));
			SetVisible(false);
		}

		/// <summary>
		/// 親ノード設定
		/// </summary>
		public void SetParent(Transform parent) {
			cube_.SetParent(parent, false);
			//キャラの足元に置く
			cube_.SetPosition(Vector3.up * 0.51f);
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void LateUpdate() {
			float scl = owner_.HaveCollision.Radius * 2 / trans_.localScale.x;
			cube_.SetScale(new Vector3(scl, 1f, scl));
			//AABBなので回転は無効化
			cube_.cacheTrans.rotation = Quaternion.identity;
		}
		/// <summary>
		/// 表示切替
		/// </summary>
		public void SetVisible(bool flag) {
			cube_.SetActive(flag);
		}

		/// <summary>
		/// 列ごとに色を変える
		/// </summary>
		private Color SelectColor(Power team) {
			if(team == Power.Player) {
				return new Color(0f, 0.2f, 0.8f);
			} else {
				return new Color(0.8f, 0.2f, 0f);
			}
		}
	}
#endif
}
