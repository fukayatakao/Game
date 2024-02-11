using UnityEngine;
using System;

namespace Project.Game {
	/// <summary>
	/// 敵関係の記憶
	/// </summary>
	[Serializable]
	public class EnemySubBoard {
		//Editorの場合は確認しやすいようにGameObjectももたせる
#if UNITY_EDITOR
		[SerializeField]
		private GameObject nearEnemyObject_;
#endif
		[SerializeField]
		private CharacterEntity nearEnemy_;
		public void SetNearEnemy(CharacterEntity entity) {
			nearEnemy_ = entity;
#if UNITY_EDITOR
			nearEnemyObject_ = nearEnemy_?.gameObject;
#endif
		}



		//Editorの場合は確認しやすいようにGameObjectももたせる
#if UNITY_EDITOR
		[SerializeField]
		private GameObject lockonEnemyObject_;
#endif
		[SerializeField]
		private CharacterEntity lockonEnemy_;
		public void SetLockonEnemy(CharacterEntity entity) {
			lockonEnemy_ = entity;
#if UNITY_EDITOR
			lockonEnemyObject_ = lockonEnemy_?.gameObject;
#endif
		}


		public CharacterEntity TargetEnemy {
			get {
				if (lockonEnemy_ != null)
					return lockonEnemy_;
				else
					return nearEnemy_;
			}
		}

		//一番近い敵が大将か
		public bool IsNearBoss { get { return TargetEnemy != null && TargetEnemy.HavePersonal.IsLeader(); } }

		/// <summary>
		/// 定期的に近くの敵を検査
		/// </summary>
		public void Execute(CharacterEntity owner) {
			//FindNearEnemy(owner);
		}

	}
}

