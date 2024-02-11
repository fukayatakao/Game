using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Project.Lib;

namespace Project.Http.Mst {
	//
	public enum PHASE {
		[Field("日")] SUN = 0,
		[Field("月")] MOON = 1,
		[Field("星")] STAR = 2,
		[Field("最大数")] MAX = 3,
	}
}
