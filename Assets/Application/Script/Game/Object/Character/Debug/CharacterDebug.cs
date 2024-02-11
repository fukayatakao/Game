using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// デバッグインスタンス置き場
	/// </summary>
	public class CharacterDebug : MonoPretender {
		public DebugAIState AIState;
		public DebugAttackArea[] AttackArea;
		public DebugCollisionDraw CollisionDraw;
		public DebugSearchArea SearchArea;
		public DebugTargetDraw TargetDraw;
	}
#endif
}
