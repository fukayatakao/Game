using Project.Lib;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 攻撃範囲の扇表示
	/// </summary>
	public class DebugSearchArea : MonoBehaviour {
		CharacterCircle circle_;
		CharacterEntity owner_;

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;
		void Awake() {
			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
		}
		private void OnDestroy() {
			MonoPretender.Destroy(circle_);
			MessageSystem.DestroyReceptor(receptor_);
		}
		/// <summary>
		/// 初期設定
		/// </summary>
		public async void Init(CharacterEntity owner, GameObject obj) {
			owner_ = owner;
			circle_ = MonoPretender.Create<CharacterCircle>(gameObject);
			await circle_.LoadAsync();
			circle_.Setup(owner.CacheTrans, owner.HaveUnitParam.Physical.SearchRange, Color.cyan);
			SetVisible(false);
		}

		/// <summary>
		/// 所有者と同一かチェック
		/// </summary>
		public bool IsOwn(CharacterEntity entity) {
			return owner_ == entity;
		}

		/// <summary>
		/// 表示切替
		/// </summary>
		public void SetVisible(bool flag) {
			circle_.SetVisible(flag);
		}

		/// <summary>
		/// 表示されているか
		/// </summary>
		public bool IsVisible() {
			return circle_.IsVisible();
		}
	}
#endif
}
