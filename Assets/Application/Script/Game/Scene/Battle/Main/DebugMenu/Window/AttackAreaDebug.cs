using UnityEngine;
using System.Collections.Generic;
using Project.Http.Mst;
using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
	public class AttackAreaDebug : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.6f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperRight;

		private Power powerTab = Power.Player;
		private Abreast abreastTab = Abreast.First;

		private static List<bool> flags_ = new List<bool>(new bool[(int)Abreast.MAX * (int)ACTION_PATTERN.MAX * (int)Power.Max]);

		public class ShowArea
		{
			public List<bool> setting;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AttackAreaDebug() {
			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		private static int GetIndex(Power power, Abreast abreast, ACTION_PATTERN pattern)
		{
			return ((int)Abreast.MAX * (int)ACTION_PATTERN.MAX) * (int)power + ((int)ACTION_PATTERN.MAX) * (int)abreast + (int)pattern;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			CloseButton.Draw(this);
			powerTab = (Power)FitGUILayout.SelectionGrid((int)powerTab, new[] { "プレイヤー", "エネミー" }, 2);
			abreastTab = (Abreast)FitGUILayout.SelectionGrid((int)abreastTab, new[] { "前列", "中列", "後列"}, 3);
			using (new GUIUtil.ChangeCheckScope(() =>
			       {
					   BattleMessage.AttackAreaDisplay.Broadcast(powerTab, abreastTab, ACTION_PATTERN.A, flags_[GetIndex(powerTab, abreastTab, ACTION_PATTERN.A)]);
				       BattleMessage.AttackAreaDisplay.Broadcast(powerTab, abreastTab, ACTION_PATTERN.B, flags_[GetIndex(powerTab, abreastTab, ACTION_PATTERN.B)]);
				       BattleMessage.AttackAreaDisplay.Broadcast(powerTab, abreastTab, ACTION_PATTERN.C, flags_[GetIndex(powerTab, abreastTab, ACTION_PATTERN.C)]);
					   BattleMessage.AttackAreaDisplay.Broadcast(powerTab, abreastTab, ACTION_PATTERN.D, flags_[GetIndex(powerTab, abreastTab, ACTION_PATTERN.D)]);
					   Save();
			       }))
			{
				int index;
				index = GetIndex(powerTab, abreastTab, ACTION_PATTERN.A);
				flags_[index] = ButtonSwitch.Draw(flags_[index], "A");
				index = GetIndex(powerTab, abreastTab, ACTION_PATTERN.B);
				flags_[index] = ButtonSwitch.Draw(flags_[index], "B");
				index = GetIndex(powerTab, abreastTab, ACTION_PATTERN.C);
				flags_[index] = ButtonSwitch.Draw(flags_[index], "C");
				index = GetIndex(powerTab, abreastTab, ACTION_PATTERN.D);
				flags_[index] = ButtonSwitch.Draw(flags_[index], "D");
			}
		}

		/// <summary>
		/// 前回の設定を読み込み
		/// </summary>
		public static void Load() {
			//キーがどれかでも欠けてたら初期設定で上書きする
			if(!PlayerPrefs.HasKey(PrefsKey.ShowAttackAreaKey)) {
				Save();
			}

			string json = PlayerPrefs.GetString(PrefsKey.ShowAttackAreaKey);
			ShowArea show = JsonUtility.FromJson<ShowArea>(json);
			flags_ = show.setting;

			for(int i = 0, max = (int)Power.Max; i< max; i++) {
				for(int j = 0, max2 = (int)Abreast.MAX; j < max2; j++) {
					for(int k = 0, max3 = (int)ACTION_PATTERN.MAX; k < max3; k++) {
						int index = GetIndex((Power)i, (Abreast)j, (ACTION_PATTERN)k);
						BattleMessage.AttackAreaDisplay.Broadcast((Power)i, (Abreast)j, (ACTION_PATTERN)k, flags_[index]);
					}
				}
			}
		}

		/// <summary>
		/// 設定を保存
		/// </summary>
		private static void Save() {
			ShowArea show = new ShowArea();
			show.setting = new List<bool>();
			show.setting.AddRange(flags_);
			PlayerPrefs.SetString(PrefsKey.ShowAttackAreaKey, JsonUtility.ToJson(show));
		}
	}
#endif
}
