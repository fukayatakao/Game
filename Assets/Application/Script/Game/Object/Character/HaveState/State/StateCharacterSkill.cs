using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// スキル演出状態
    /// </summary>
    public class StateCharacterSkill : IState<CharacterEntity> {
	    private string skill_;
	    public void SetUseSkill(string act) {
		    skill_ = act;
	    }


        /// <summary>
        /// 状態開始時処理
        /// </summary>
        public override void Enter(CharacterEntity owner) {
	        owner.HaveThink.Enable = false;

	        owner.HaveAction.Stop();
	        owner.HaveAction.Play(skill_);
	        BattleMessage.StartSpecialSkill.Broadcast(owner);
        }

        /// <summary>
        /// 状態終了時処理
        /// </summary>
        public override void Exit(CharacterEntity owner) {
	        BattleMessage.EndSpecialSkill.Broadcast(owner);
	        owner.HaveThink.Enable = true;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public override void Execute(CharacterEntity owner) {
	        if (!owner.HaveAction.IsPlay()) {
		        owner.ChangeState(CharacterState.State.Idle);
	        }

        }
    }
}
