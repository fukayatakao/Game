using Project.Http.Mst;
using Project.Lib;
using Project.Mst;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	//@note デバッグ機能なのでMonobehaviour継承してこのクラス単体で完結させる
	/// <summary>
	/// 攻撃範囲の扇表示
	/// </summary>
	public class DebugAttackArea : MonoBehaviour {
		Mst.MstActionData actionData_;
		PrimitiveFan fan_ = new PrimitiveFan();

		CharacterEntity owner_;
		public CharacterEntity Owner { get { return owner_; } }
		public ACTION_PATTERN pattern;

		static readonly Color[] color_ = new Color[(int)ACTION_PATTERN.MAX] { Color.green, Color.cyan, Color.magenta, Color.yellow };

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
		public void Init(CharacterEntity owner, ACTION_PATTERN type) {
			owner_ = owner;
			actionData_ = BaseDataManager.GetDictionary<int, MstActionData>()[GetAttackId(type)];

			pattern = type;

			GameObject obj = fan_.Create("fan" + type.ToString());
			Color col = color_[(int)type];
			col.a = 0.3f;
			fan_.SetColor(col);
			fan_.SetFan(actionData_.RangeMax + owner.HaveCollision.Radius, actionData_.AngleRad);
			SetVisible(false);
		}
		/// <summary>
		/// 攻撃パターンごとのMstActionDataのIdを取得
		/// </summary>
		public int GetAttackId(ACTION_PATTERN pattern) {
			switch (pattern) {
				case ACTION_PATTERN.A:
					return owner_.HaveUnitMaster.ActionA;
				case ACTION_PATTERN.B:
					return owner_.HaveUnitMaster.ActionB;
				case ACTION_PATTERN.C:
					return owner_.HaveUnitMaster.ActionC;
				case ACTION_PATTERN.D:
					return owner_.HaveUnitMaster.ActionD;
				default:
					return 0;
			}

		}


		/// <summary>
		/// 所有者と同一かチェック
		/// </summary>
		public bool IsOwn(CharacterEntity entity) {
			return owner_ == entity;
		}


		/// <summary>
		/// 親ノード設定
		/// </summary>
		public void SetParent(Transform parent) {
            fan_.SetParent(parent, false);
            //キャラの足元に置く
            fan_.SetPosition(Vector3.up * 0.01f);
        }
		/// <summary>
		/// 実行処理
		/// </summary>
        public void LateUpdate() {
			float len = actionData_.RangeMax;
			fan_.SetFan(len / owner_.CacheTrans.localScale.x, actionData_.AngleRad);
		}


		/// <summary>
		/// 表示切替
		/// </summary>
		public void SetVisible(bool flag) {
			fan_.SetActive(flag);
		}
		/// <summary>
		/// 表示されているか
		/// </summary>
		public bool IsVisible() {
			return fan_.gameObject.activeSelf;
		}
	}
#endif
}
