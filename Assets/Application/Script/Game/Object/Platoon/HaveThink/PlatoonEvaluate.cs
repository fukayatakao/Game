using Project.Lib;

namespace Project.Game {
	[UnityEngine.Scripting.Preserve]
	public static class PlatoonEvaluate {
        [Function("無条件にtrue")]
        public static bool Always(PlatoonEntity entity, object[] args) {
            return true;
        }
        [CheckFunction("前列入れ替え可能な状態", "前列入れ替え不可能な状態")]
        public static bool IsForeSwap(PlatoonEntity entity, object[] args) {
	        if ((bool)args[0]) {
		        return !entity.IsDisableForeSwap();
	        } else {
		        return entity.IsDisableForeSwap();
	        }
        }
        [CheckFunction("後列入れ替え可能な状態", "後列入れ替え不可能な状態")]
        public static bool IsAftSwap(PlatoonEntity entity, object[] args) {
	        if ((bool)args[0]) {
		        return !entity.IsDisableAftSwap();
	        } else {
		        return entity.IsDisableAftSwap();
	        }
        }
        [CheckFunction("ローテーション可能な状態", "ローテーション不可能な状態")]
        public static bool IsRotation(PlatoonEntity entity, object[] args) {
	        if ((bool)args[0]) {
		        return !entity.IsDisableRotation();
	        } else {
		        return entity.IsDisableRotation();
	        }
        }

		[Function("自分{0}列目が{1}列目よりも前列に適している")]
		[Arg(0, typeof(Abreast), "列番号", Abreast.First)]
		[Arg(1, typeof(Abreast), "列番号", Abreast.Second)]
		public static bool CheckForwardAptitude(PlatoonEntity entity, object[] args) {
			int index1 = (int)args[0];
			int index2 = (int)args[1];
			//残り１列しかない場合はfalse
			if (entity.Squads.Count <= 1)
				return false;

			return entity.Squads[index1].CalcAptitude() > entity.Squads[index2].CalcAptitude();
		}


		[Function("前回の入れ替えから{0}s経過した")]
		[Arg(0, typeof(float), "時間", 0f)]
		public static bool CheckPastSwapInterval(PlatoonEntity entity, object[] args) {
			float t = (float)args[0];

			return entity.HaveBlackboard.SwapExecTime + t < Time.time;
		}

		[Function("前回の入れ替えから{1}s経過した")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(float), "時間", 0f)]
		public static bool CheckStillSwapInterval(PlatoonEntity entity, object[] args) {
			return !CheckPastSwapInterval(entity, args);
		}

		[Function("{1}のHPが{2}%より上")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(Abreast), "列", Abreast.First)]
		[Arg(2, typeof(int), "割合", 100)]
		public static bool CheckGroupHPMore(PlatoonEntity entity, object[] args) {
			PlatoonEntity platoon = (IFF)args[0] == IFF .FRIEND ? entity : entity.HaveBlackboard.Opponent;
			Abreast group = (Abreast)args[1];
			float rate = (int)args[2];
			if (entity.Squads.Count <= (int)group)
				return false;

			SquadEntity squad = entity.Squads[(int)group];
			return 100 * squad.HaveParam.Hp / squad.HaveParam.MaxHp > rate;
		}

		[Function("{1}のHPが{2}%以下")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(Abreast), "列", Abreast.First)]
		[Arg(2, typeof(int), "割合", 100)]
		public static bool CheckGroupHPLess(PlatoonEntity entity, object[] args) {
			return !CheckGroupHPMore(entity, args);
		}

		[Function("{1}列の残りキャラ数が{2}より上")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(Abreast), "列", Abreast.First)]
		[Arg(2, typeof(int), "残数", 0)]
		public static bool CheckGroupNumMore(PlatoonEntity entity, object[] args) {
			PlatoonEntity platoon = (IFF)args[0] == IFF.FRIEND ? entity : entity.HaveBlackboard.Opponent;
			int group = (int)args[1];
			float num = (int)args[2];

			SquadEntity squad = entity.Squads[group];
			return squad.Members.Count > num;
		}


		[Function("{1}列の残りキャラ数が{2}以下")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(Abreast), "列", Abreast.First)]
		[Arg(2, typeof(int), "残数", 0)]
		public static bool CheckGroupNumLess(PlatoonEntity entity, object[] args) {
			return !CheckGroupNumMore(entity, args);
		}

		[Function("列の数が一定より上")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(int), "列数", 0)]
		public static bool CheckGroupCountMore(PlatoonEntity entity, object[] args) {
			int count = (int)args[0];

			return entity.Squads.Count > count;
		}
		[Function("列の数が一定以下")]
		[Arg(0, typeof(IFF), "自他識別", IFF.FRIEND)]
		[Arg(1, typeof(int), "列数", 0)]
		public static bool CheckGroupCountLess(PlatoonEntity entity, object[] args) {
			return !CheckGroupCountMore(entity, args);
		}
	}
}
