using UnityEngine;
using System.Collections;


namespace Project.Game {
	public static class CharacterCommand {

		/// <summary>
		/// 移動
		/// </summary>
		public static IEnumerator SetEnableAI(CharacterEntity entity, bool flag) {
			entity.HaveThink.Enable = flag;
			yield break;
		}
		/// <summary>
		/// 状態変更
		/// </summary>
		public static IEnumerator ChangeState(CharacterEntity entity, CharacterState.State state) {
			entity.ChangeState(state);
			yield break;
		}

		/// <summary>
		/// アニメーション再生
		/// </summary>
		public static IEnumerator PlayAnimation(CharacterEntity entity, BattleMotion motion, float crossFadeTime = 0.25f) {
			entity.HaveAnimation.Play(motion, crossFadeTime);
			yield break;
		}
		/// <summary>
		/// アニメーション再生して終了を待つ
		/// </summary>
		public static IEnumerator PlayAnimationWithWait(CharacterEntity entity, BattleMotion motion) {
			entity.HaveAnimation.Play(motion);
			while (entity.HaveAnimation.IsPlay(motion)) {
				yield return null;
			}
			yield break;
		}

		/// <summary>
		/// アクション再生
		/// </summary>
		public static IEnumerator PlayAction(CharacterEntity entity, BattleAction action) {
			entity.HaveAction.Play(action);
			yield break;
		}
		/// <summary>
		/// アニメーション再生
		/// </summary>
		public static IEnumerator PlayEffect(CharacterEntity entity, string effectName, CharacterNodeCache.Part part, Vector3 pos) {
			EffectAssembly.I.CreateAsync(effectName, (effect) => {
				effect.Constrain(entity.HaveCacheNode.GetNode(part));
				effect.HaveConstrain.ConstrainPosition(pos);
			});

			yield break;
		}
		/// <summary>
		/// アニメーション再生
		/// </summary>
		public static IEnumerator PlayEffect(CharacterEntity entity, string effectName, Vector3 pos) {
			EffectAssembly.I.CreateAsync(effectName, (effect) => {
				effect.Constrain(entity.CacheTrans);
				effect.HaveConstrain.ConstrainPosition(pos);
			});

			yield break;
		}
		/// <summary>
		/// α値変更
		/// </summary>
		public static IEnumerator ChangeAlphaLinear(CharacterEntity entity, float start, float end, float time) {
			float stamp = Time.time;
			float diff= end - start;
			while (true) {
				float t = (Time.time - stamp) / time;
				if(t > 1f) {
					entity.SetAlpha(end);
					break;
				}else{
					float alpha = start + t * diff;
					entity.SetAlpha(alpha);
					yield return null;
				}
			}
			yield break;
		}
		/// <summary>
		/// ３Dモデル変更
		/// </summary>
		public static IEnumerator ChangeModel(CharacterEntity entity, Mst.MstUnitData data) {
			bool wait = true;
			entity.ChangeModel(data, () => { wait = false;});
			while (wait)
			{
				yield return null;
			}
			yield break;
		}
	}
}
