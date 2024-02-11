using Project.Http.Mst;
using Project.Lib;
using UnityEngine;

namespace Project.Game {
	public static class ResourcePool
	{
		//ロードした画像のリソース
		private static  Texture2D[] phaseTexture_ = new Texture2D[(int)PHASE.MAX];

		public static Texture2D GetPhaseIcon(PHASE phase) {
			return phaseTexture_[(int)phase];
		}

		public static void Init() {
			phaseTexture_[(int)PHASE.SUN] = Resources.Load<Texture2D>(ResourcesPath.UI_PHASE_SUN);
			phaseTexture_[(int)PHASE.MOON] = Resources.Load<Texture2D>(ResourcesPath.UI_PHASE_MOON);
			phaseTexture_[(int)PHASE.STAR] = Resources.Load<Texture2D>(ResourcesPath.UI_PHASE_STAR);

		}

	}
}
