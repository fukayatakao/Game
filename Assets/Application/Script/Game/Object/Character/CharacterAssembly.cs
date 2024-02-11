using Project.Lib;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// キャラクター管理
	/// </summary>
	public class CharacterAssembly : EntityAssembly<CharacterEntity, CharacterAssembly> {
#if DEVELOP_BUILD
		public GameObject DebugRoot;
#endif

		/// <summary>
		/// セットアップ
		/// </summary>
		public override void Setup(Transform root, string name) {
            base.Setup(root, name);
#if DEVELOP_BUILD
			DebugRoot = new GameObject("CharacterDebugRoot");
#endif

			BehaviorTree<CharacterEntity>.Initiate(typeof(CharacterEvaluate), typeof(CharacterOrder));
			BehaviorSequence<CharacterEntity>.Initiate(typeof(CharacterEvaluate), typeof(CharacterOrder));
			EntityActEvent<CharacterEntity>.Cache.Initiate(typeof(CharacterEvent));
			CharacterChecker.Initiate();
		}
		/// <summary>
		/// クリーンアップ
		/// </summary>
		public override void Cleanup() {
			BehaviorTree<CharacterEntity>.Terminate();
			BehaviorSequence<CharacterEntity>.Terminate();
			EntityActEvent<CharacterEntity>.Cache.Terminate();
			CharacterChecker.Terminate();
			base.Cleanup();
        }

#if UNITY_EDITOR

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public CharacterEntity Create(GameObject obj) {
			CharacterEntity entity = CreateImpl(obj, true, false);
			entity.HaveAnimation.Load();
			return entity;
		}

#endif
		/// <summary>
		/// インスタンス生成
		/// </summary>
		public async Task<CharacterEntity> CreateAsync(string modelName, string animName, string actionName, string aiName, bool isImmediate=false, bool isReserve = false) {
			CharacterEntity entity = await CreateImplAsync(modelName, isImmediate, isReserve);
			await entity.LoadAnimation(animName);
			await entity.LoadAction(actionName);
			await entity.LoadAI(aiName);
			await entity.LoadCircle();
			return entity;
		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public async Task<CharacterEntity> CreateAsync(string modelName, string animName, bool isImmediate = false, bool isReserve = false) {
			CharacterEntity entity = await CreateImplAsync(modelName, isImmediate, isReserve);
			await entity.LoadAnimation(animName);
			return entity;
		}
	}
}
