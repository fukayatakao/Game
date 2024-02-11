using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 駒管理
    /// </summary>
    public class PieceAssembly : EntityAssembly<PieceEntity, PieceAssembly> {
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

			BehaviorTree<PieceEntity>.Initiate(typeof(CharacterEvaluate), typeof(CharacterOrder));
			BehaviorSequence<PieceEntity>.Initiate(typeof(CharacterEvaluate), typeof(CharacterOrder));
			EntityActEvent<PieceEntity>.Cache.Initiate(typeof(CharacterEvent));
		}
		/// <summary>
		/// クリーンアップ
		/// </summary>
		public override void Cleanup() {
			BehaviorTree<PieceEntity>.Terminate();
			BehaviorSequence<PieceEntity>.Terminate();
			EntityActEvent<PieceEntity>.Cache.Terminate();
			base.Cleanup();
        }

#if UNITY_EDITOR

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public PieceEntity Create(GameObject obj) {
			PieceEntity entity = CreateImpl(obj, true, false);
			entity.HaveModel.HaveAnimation.Load();
			return entity;
		}

#endif
		/// <summary>
		/// インスタンス生成
		/// </summary>
		public async Task<PieceEntity> CreateAsync(string modelName, string animName, bool isImmediate = false) {
			PieceEntity entity = await CreateImplAsync(modelName, isImmediate, false);
			await entity.HaveModel.HaveAnimation.LoadAsync(animName);
			return entity;
		}
	}
}
