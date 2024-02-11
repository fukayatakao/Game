using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//
	public enum ABILITY {
		[Field("与ダメup")] GIVE_DAMAGE＿UP = 0,
		[Field("与ダメdown")] GIVE_DAMAGE＿DOWN = 1,
		[Field("被ダメup")] RECIEVE_DAMAGE_UP = 2,
		[Field("被ダメdown")] RECIEVE_DAMAGE_DOWN = 3,
		[Field("属性影響力up")] PHASE_EFFICACY_UP = 4,
		[Field("属性影響力down")] PHASE_EFFICACY_DOWN = 5,
		[Field("与ノックバック量up")] GIVE_KNOCKBACK_UP = 6,
		[Field("与ノックバック量down")] GIVE_KNOCKBACK_DOWN = 7,
		[Field("被ノックバック量up")] RECIEVE_KNOCKBACK_UP = 8,
		[Field("被ノックバック量down")] RECIEVE_KNOCKBACK_DOWN = 9,
	}
}
