using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// キャラクターの状態管理
	/// </summary>
	public class PieceState : MonoPretender {
		public enum State : int {
			[Field("なし")] None,
			[Field("待機")] Idle,
			[Field("移動")] Move,

			[Field("最大数")] Max,
		}

		// ステート管理
		protected StateMachine<PieceEntity> machine_ = new StateMachine<PieceEntity>((int)State.Max);

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
			machine_.Register<StatePieceNone>(obj, (int)State.None);
			machine_.Register<StatePieceMove>(obj, (int)State.Move);
			machine_.Register<StatePieceIdle>(obj, (int)State.Idle);

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
		public void Execute(PieceEntity owner) {
			machine_.Execute(owner);
		}

        /// <summary>
        /// 現在の状態クラスを取得
        /// </summary>
        public T GetState<T>(State state) where T : IState<PieceEntity> {
			return (T)machine_.GetState((int)state);
		}

        /// <summary>
        /// ステート変更
        /// </summary>
        public void ChangeState(PieceEntity owner, State state, bool force=false) {
            machine_.ChangeState(owner, (int)state, force);
        }
	}
}
