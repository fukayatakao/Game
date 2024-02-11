using Project.Lib;

namespace Project.Game {

    /// <summary>
    /// 状態マシン
    /// </summary>
    public class CharacterStateMachine : StateMachine<CharacterEntity> {

        private int changeRequestState_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterStateMachine(int max) : base(max) {
            changeRequestState_ = -1;
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        public void LateExecute(CharacterEntity owner) {
            Change(owner);
        }


        /// <summary>
        /// 状態変更(リクエストのみ)
        /// </summary>
        public new void ChangeState(CharacterEntity owner, int stateNo, bool force = false) {
            //同じ状態への遷移は無視
            if (currentNo_ == stateNo && !force)
                return;
            //すでにリクエスト済と同じものである場合は無視
            if (changeRequestState_ == stateNo)
                return;

            changeRequestState_ = stateNo;
        }
        /// <summary>
        /// 状態変更(実処理)
        /// </summary>
		private void Change(CharacterEntity owner) {
            if (changeRequestState_ < 0)
                return;

			base.ChangeState(owner, changeRequestState_, true);

            changeRequestState_ = -1;
        }
    }


}
