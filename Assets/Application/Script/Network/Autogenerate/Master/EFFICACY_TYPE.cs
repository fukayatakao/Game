using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//// 効果種別
	public enum EFFICACY_TYPE {
		[Field("無効")] NONE = 0,
		[Field("攻撃力")] ATTACK = 1,
		[Field("防御力")] DEFFENSE = 2,
		[Field("最大数")] MAX = 3,
	}
}
