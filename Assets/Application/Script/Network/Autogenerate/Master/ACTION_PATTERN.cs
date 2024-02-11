using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//// 攻撃パターン定義
	public enum ACTION_PATTERN {
		[Field("パターンA")] A = 0,
		[Field("パターンB")] B = 1,
		[Field("パターンC")] C = 2,
		[Field("パターンD")] D = 3,
		[Field("最大数")] MAX = 4,
	}
}
