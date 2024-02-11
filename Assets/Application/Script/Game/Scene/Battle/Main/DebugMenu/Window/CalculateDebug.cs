using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CalculateDebug : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;
		//HPダメージ計算するか
		public static bool IsHpDamage = true;
		//ダメージ計算するか
		public static bool IsLpDamage = true;
		//ノックバック計算するか
		public static bool IsKnockBack = true;
		//貫通ダメージ計算するか
		public static bool IsPircing = true;
		//ノックバック強制するか
		public static bool IsForceKnockBackEnemy = false;
		//ノックバック強制するか
		public static bool IsForceKnockBackPlayer = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CalculateDebug() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			using (new GUIUtil.ChangeCheckScope(() => { Save(); })) {
				IsHpDamage = ButtonSwitch.Draw(IsHpDamage, "HPダメージ");
				IsLpDamage = ButtonSwitch.Draw(IsLpDamage, "LPダメージ");
				IsKnockBack = ButtonSwitch.Draw(IsKnockBack, "ノックバック");
				IsPircing = ButtonSwitch.Draw(IsPircing, "貫通ダメージ");
				IsForceKnockBackEnemy = ButtonSwitch.Draw(IsForceKnockBackEnemy, "強制ノックバック(敵)");
				IsForceKnockBackPlayer = ButtonSwitch.Draw(IsForceKnockBackPlayer, "強制ノックバック(味方)");
			}
		}

		public static void Load() {
			//キーがどれかでも欠けてたら初期設定で上書きする
			if(!(PlayerPrefs.HasKey(PrefsKey.HpDamageKey) &&
				PlayerPrefs.HasKey(PrefsKey.LpDamageKey) &&
				PlayerPrefs.HasKey(PrefsKey.KnockBackKey) &&
				PlayerPrefs.HasKey(PrefsKey.PircingKey) &&
				PlayerPrefs.HasKey(PrefsKey.ForceKnockBackPlayerKey) &&
				PlayerPrefs.HasKey(PrefsKey.ForceKnockBackEnemyKey))) {
				Save();
			}



			IsHpDamage = PlayerPrefs.GetInt(PrefsKey.HpDamageKey) != 0;
			IsLpDamage = PlayerPrefs.GetInt(PrefsKey.LpDamageKey) != 0;
			IsKnockBack = PlayerPrefs.GetInt(PrefsKey.KnockBackKey) != 0;
			IsPircing = PlayerPrefs.GetInt(PrefsKey.PircingKey) != 0;
			IsForceKnockBackEnemy = PlayerPrefs.GetInt(PrefsKey.ForceKnockBackEnemyKey) != 0;
			IsForceKnockBackPlayer = PlayerPrefs.GetInt(PrefsKey.ForceKnockBackPlayerKey) != 0;
		}

		private static void Save() {
			PlayerPrefs.SetInt(PrefsKey.HpDamageKey, IsHpDamage ? 1 : 0);
			PlayerPrefs.SetInt(PrefsKey.LpDamageKey, IsLpDamage ? 1 : 0);
			PlayerPrefs.SetInt(PrefsKey.KnockBackKey, IsKnockBack ? 1 : 0);
			PlayerPrefs.SetInt(PrefsKey.PircingKey, IsPircing ? 1 : 0);
			PlayerPrefs.SetInt(PrefsKey.ForceKnockBackEnemyKey, IsForceKnockBackEnemy ? 1 : 0);
			PlayerPrefs.SetInt(PrefsKey.ForceKnockBackPlayerKey, IsForceKnockBackPlayer ? 1 : 0);

		}
	}
#endif
}
