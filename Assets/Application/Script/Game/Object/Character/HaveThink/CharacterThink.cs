using UnityEngine;
using System;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// キャラクターAIの意思決定
    /// </summary>
    public class CharacterThink : MonoPretender{
        public enum State {
            [Field("ルート")] Root,
			[Field("敵追尾")] Chase,
			[Field("列入替(後退)")] Backward,
		}
		//思考が有効か
		public bool Enable;
		//行動制御データ
		BehaviorTree<CharacterEntity> behaviourTree_;

		static readonly string[] states_ = Enum.GetNames(typeof(State));

#if DEVELOP_BUILD
		[SerializeField]
		private string assetName_;
#endif
		UnityEngine.Object asset_;
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			behaviourTree_ = new BehaviorTree<CharacterEntity>();
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			AddressableAssist.UnLoadAsset(asset_);
			asset_ = null;
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public async Task LoadAsync(string assetName) {
			//前回ロードした内容は破棄
			if(asset_ != null)
				AddressableAssist.UnLoadAsset(asset_);

			asset_ = await AddressableAssist.LoadAssetAsync(assetName) as ScriptableObject;
			behaviourTree_.Create(asset_ as BehaviorTreeData);
#if UNITY_EDITOR && USE_MONOBEHAVIOUR
			gameObject.AddComponent<BehaviorTreeMonitor>().Init<CharacterEntity>(behaviourTree_);
#endif

#if DEVELOP_BUILD
			assetName_ = assetName;
#endif
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			if (!Enable)
				return;
			behaviourTree_.Execute(owner);
		}

        /// <summary>
        /// AI状態の変更
        /// </summary>
        public void ChangeState(CharacterEntity owner, State state, bool force = false) {
			behaviourTree_.ChangeModule(states_[(int)state]);
        }

#if DEVELOP_BUILD
		/// <summary>
		/// AI状態の現在状態確認
		/// </summary>
		public string CurrentLabel() {
			return behaviourTree_.CurrentLabel;
		}
#endif
	}
}

