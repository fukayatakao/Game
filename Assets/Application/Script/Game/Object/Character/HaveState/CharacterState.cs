using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// キャラクターの状態管理
	/// </summary>
	public class CharacterState : MonoPretender {
		public enum State : int {
			[Field("なし")] None,
			[Field("待機")] Idle,
			[Field("移動")] Move,
			[Field("敵に接近")] Approach,
			[Field("攻撃")] Attack,


			[Field("死亡")] Dead,
			[Field("逃亡")] Escape,

			[Field("ダメージ")] Damage,

			[Field("スキル")] Skill,

			[Field("最大数")] Max,
		}
		public CharacterStateDelay StateDelay = new CharacterStateDelay();

		// ステート管理
		protected CharacterStateMachine machine_ = new CharacterStateMachine((int)State.Max);

        public State CurrentState{get{ return (State)machine_.CurrentStateNo; } }
		/// <summary>
		/// 状態チェック
		/// </summary>
		public bool IsCurrentState(State state) {
			return state == (State)machine_.CurrentStateNo;
		}
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			Register(obj);

			HideComponent();
		}
		/// <summary>
		/// ステートの登録
		/// </summary>
		private void Register(GameObject obj) {
			machine_.Register<StateCharacterNone>(obj, (int)State.None);
			machine_.Register<StateCharacterApproach>(obj, (int)State.Approach);
			machine_.Register<StateCharacterMove>(obj, (int)State.Move);
			machine_.Register<StateCharacterIdle>(obj, (int)State.Idle);
			machine_.Register<StateCharacterAttack>(obj, (int)State.Attack);
			machine_.Register<StateCharacterDamage>(obj, (int)State.Damage);
			machine_.Register<StateCharacterDead>(obj, (int)State.Dead);
			machine_.Register<StateCharacterEscape>(obj, (int)State.Escape);
			machine_.Register<StateCharacterSkill>(obj, (int)State.Skill);

			machine_.SetFirstState((int)State.None);
		}
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Destroy() {
			machine_.UnRegisterAll();
		}
		/// <summary>
		/// 更新処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
			machine_.Execute(owner);
		}

        /// <summary>
        /// 更新後処理
        /// </summary>
        public void LateExecute(CharacterEntity owner) {
            machine_.LateExecute(owner);
        }

        /// <summary>
        /// 現在の状態クラスを取得
        /// </summary>
        public T GetState<T>(State state) where T : IState<CharacterEntity>{
			return (T)machine_.GetState((int)state);
		}

        /// <summary>
        /// ステート変更
        /// </summary>
        public void ChangeState(CharacterEntity owner, State state, bool force=false) {
            machine_.ChangeState(owner, (int)state, force);
        }
	}
}
