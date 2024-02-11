using Project.Lib;


namespace Project.Game {
    /// <summary>
    /// キャラクターのユニットから来るパラメータ管理
    /// </summary>
    public class CharacterUnitParam : MonoPretender {
		public PhysicalParam Physical = new PhysicalParam();
		public FightParam Fight = new FightParam();

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity owner) {
            Fight.Execute(owner);
        }

    }
}
