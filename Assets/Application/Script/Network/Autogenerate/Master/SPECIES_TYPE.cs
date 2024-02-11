using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//// 種族種別
	public enum SPECIES_TYPE {
		[Field("無効")] NONE = 0,
		[Field("ヒューマン")] HUMAN = 1,
		[Field("センチネル")] SENTINEL = 2,
		[Field("アンデッド")] UNDEAD = 3,
		[Field("ウォーバンド")] WARBAND = 4,
		[Field("グリフィン")] GRIFFIN = 5,
		[Field("攻城兵器")] SIEGE_ENGINE = 6,
		[Field("")] BATTLE_OWL = 7,
		[Field("")] ROCK_GOLEM = 8,
	}
}
