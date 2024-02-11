using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//// 戦略マップでの所属
	public enum STRATEGY_ATTITUDE {
		[Field("無効")] NONE = 0,
		[Field("攻撃側")] INVADER = 1,
		[Field("防御側")] DEFENDER = 2,
	}
}
