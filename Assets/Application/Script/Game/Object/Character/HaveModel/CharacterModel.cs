using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;


namespace Project.Game {
	public class CharacterModel : MonoPretender {
		UnityEngine.Object asset_;
		private GameObject model_;
		public GameObject Model { get { return model_; } }
		//キャラクターのアニメーション制御
		private CharacterAnimation haveAnimation_;
		public CharacterAnimation HaveAnimation { get{ return haveAnimation_; } }

		private CharacterOutline haveOutline_;
		public CharacterOutline HaveOutline { get { return haveOutline_; } }

		//ノードアクセス高速化のためのキャッシュ
		private CharacterNodeCache haveCacheNode_;
		public CharacterNodeCache HaveCacheNode { get { return haveCacheNode_; } }

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			MonoPretender.Destroy(haveOutline_);
			MonoPretender.Destroy(haveCacheNode_);
			haveAnimation_.Destroy();
			AddressableAssist.UnLoadAsset(asset_);
		}

		public void Init()
		{
			//アニメーション制御クラス生成（初期化はAwakeの中で行われている
			haveAnimation_ = model_.AddComponent<CharacterAnimation>();
			haveAnimation_.Create();

			haveCacheNode_ = MonoPretender.Create<CharacterNodeCache>(model_);
			haveOutline_ = MonoPretender.Create<CharacterOutline>(model_);

			//@note この３つはデータが出来てきたら移動するかも
			InitNode(model_);
			AttachEvent(model_);
		}
		/// <summary>
		/// キャラクターのノードをセット
		/// </summary>
		private void InitNode(GameObject obj) {
			CharacterNode node = obj.GetComponent<CharacterNode>();
			haveCacheNode_.Register(CharacterNodeCache.Part.Root, model_.transform);
			haveCacheNode_.Register(CharacterNodeCache.Part.Head, node.Head);
			haveCacheNode_.Register(CharacterNodeCache.Part.Body, node.Body);
			haveCacheNode_.Register(CharacterNodeCache.Part.LHand, node.LHand);
			haveCacheNode_.Register(CharacterNodeCache.Part.RHand, node.RHand);
			haveCacheNode_.Register(CharacterNodeCache.Part.LFoot, node.LFoot);
			haveCacheNode_.Register(CharacterNodeCache.Part.RFoot, node.RFoot);
		}
		/// <summary>
		/// UnityChanのアセットがAnimationEvent仕込んでいるので無視するために対策
		/// </summary>
		private void AttachEvent(GameObject obj) {
			obj.AddComponent<AnimationEventCallback>();
		}


		/// <summary>
		/// 3Dモデル生成
		/// </summary>
		public async Task LoadAsync(Transform parent, string assetName) {
			asset_ = await AddressableAssist.LoadAssetAsync(assetName);
			model_ = UnityUtil.InstantiateChild(parent, (GameObject)asset_);
		}

#if UNITY_EDITOR
		/// <summary>
		/// 3Dモデル生成
		/// </summary>
		public void Create(Transform parent, GameObject resObject) {
			//デバッグ専用
			model_ = UnityUtil.InstantiateChild(parent, resObject);
		}
#endif
		/// <summary>
		/// ボディの入れ替え
		/// </summary>
		public void Change(CharacterEntity owner, UnityEngine.Object asset) {
			Destroy();
			Transform parent = model_.transform.parent;
			GameObject.Destroy(model_);

			model_ = UnityUtil.InstantiateChild(parent, (GameObject)asset);
			//コンポーネントからEntityを手繰る用
			model_.AddComponent<CharacterPortal>().Init(owner);

			Init();


			asset_ = asset;
		}
	}
}
