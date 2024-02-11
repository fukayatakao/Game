

// Prefsキー定義
public static class PrefsKey {
#if DEVELOP_BUILD
	public const string LpDamageKey = "LpDamage";
	public const string HpDamageKey = "HpDamage";
	public const string KnockBackKey = "KnockBack";
	public const string PircingKey = "Pircing";
	public const string ForceKnockBackPlayerKey = "ForceKnockBackPlayer";
	public const string ForceKnockBackEnemyKey = "ForceKnockBackEnemy";

	public const string ShowAttackAreaKey = "ShowAttackArea";

#endif

}
public static class PrefsUtilKey {
	public const string UserDataKey = "UserData";
#if DEVELOP_BUILD
	public const string EditBattlePlatoon = "EditBattlePlatoon";
	public const string SelectBattlePlatoon = "SelectBattlePlatoon";
	public const string CharacterPortrait = "CharacterPortrait";

#endif
}


