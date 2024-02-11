using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//// 戦略マップでの状態
	public enum STRATEGY_STATE {
		[Field("新規開始直後")] INIT = 0,
		[Field("攻撃側配置が終わった")] READY = 1,
		[Field("ゲーム中")] INGAME = 2,
		[Field("終了した")] FINISH = 3,
	}
}
