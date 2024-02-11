using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {

	public static class CommonCommand {
		/// <summary>
		/// 特定フレーム数待機
		/// </summary>
		public static IEnumerator WaitFrame(int count) {
			for(int i = 0; i < count; i++){
				yield return null;
			}
			yield break;
		}

		/// <summary>
		/// 待機
		/// </summary>
		public static IEnumerator Wait(float t) {
			while (t > 0f) {
				t -= Time.deltaTime;
				yield return null;
			}
			yield break;
		}

		/// <summary>
		/// 準備完了フラグを立てる
		/// </summary>
		public static IEnumerator Ready(List<bool> ready, int index) {
			ready[index] = true;
			yield break;
		}

		/// <summary>
		/// アクティブ制御
		/// </summary>
		public static IEnumerator SetActive(Entity entity, bool flag) {
			entity.SetActive(flag);
			yield break;
		}
		/// <summary>
		/// 指定座標にセット
		/// </summary>
		public static IEnumerator SetPosition(Entity entity, Vector3 pos) {
			entity.SetPosition(pos);
			yield break;
		}

		/// <summary>
		/// 指定回転にセット
		/// </summary>
		public static IEnumerator SetRotate(Entity entity, Quaternion rot) {
			entity.SetRotation(rot);
			yield break;
		}

		/// <summary>
		/// 指定スケールにセット
		/// </summary>
		public static IEnumerator SetScale(Entity entity, Vector3 scl) {
			entity.SetScale(scl);
			yield break;
		}

		/// <summary>
		/// 移動
		/// </summary>
		public static IEnumerator Move(Entity entity, Vector3 distance, float time) {
			float t = 0f;
			Vector3 startPos = entity.GetPosition();
			while (t < time) {
				t = t + Time.deltaTime;
				Vector3 pos = startPos + t / time * distance;
				entity.SetPosition(pos);

				yield return null;
			}
		}
		/// <summary>
		/// 自由落下
		/// </summary>
		public static IEnumerator Fall(Entity entity, float y) {
			Vector3 pos = entity.GetPosition();
			float v = 0f;
			float g = -9.8f;
			while(pos.y > y) {
				entity.SetPosition(pos);
				v += g * Time.deltaTime;
				pos.y += v * Time.deltaTime;
				yield return null;
			}
			pos.y = y;
			entity.SetPosition(pos);
			yield break;
		}

	}
}
