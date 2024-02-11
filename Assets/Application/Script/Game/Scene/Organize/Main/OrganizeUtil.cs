namespace Project.Game
{
	public static class OrganizeUtil
	{
		public static bool IsExChangeable(CharacterEntity alpha, CharacterEntity bravo)
		{
			return alpha.HaveUnitMaster.Species == bravo.HaveUnitMaster.Species;
		}
	}
}
