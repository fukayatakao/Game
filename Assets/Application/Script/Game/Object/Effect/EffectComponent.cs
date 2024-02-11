using Cinemachine;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// エフェクトに使われているコンポーネントの一覧を管理
	/// </summary>
	public class EffectComponent {
		//public Animation[] Animations;
		public CinemachineDollyCart[] DollyCarts;
		public Animator[] Animators;
		public ParticleSystem[] Particles;

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public EffectComponent(GameObject obj) {
			//Animations = obj.GetComponentsInChildren<Animation>(true);
			DollyCarts = obj.GetComponentsInChildren<CinemachineDollyCart>(true);
			Particles = obj.GetComponentsInChildren<ParticleSystem>(true);
			Animators = obj.GetComponentsInChildren<Animator>(true);
		}

	}


}
