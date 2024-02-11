namespace Project.Game {
	/// <summary>
	/// 攻撃したときの状態を保存
	/// </summary>
	public class ActionDumpData {
		public ActionDumpData(CharacterEntity actor, CharacterEntity target, Mst.MstActionData attack) {
			Actor = actor;
			Target = target;
			Attack = attack;
		}

		public CharacterEntity Actor;
		public CharacterEntity Target;
		public Mst.MstActionData Attack;
	}
}
