using System.Collections;
using Project.Lib;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game {
	public class TransitScreen : MonoBehaviour {
		[SerializeField]
		private RawImage rawImage_ = null;

		private const float TRANSIT_TIME = 1f / 0.5f;
		private float alpha_ = 0f;
		private CoroutineTask task_;

		/// <summary>
		/// 起動時処理
		/// </summary>
		private void Awake() {
			task_ = new CoroutineTask();
			rawImage_.color = new Color(1f, 1f, 1f, alpha_);
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		private void Update() {
			task_.Execute();
		}

		/// <summary>
		/// フェード中か
		/// </summary>
		public bool IsFade() {
			return task_.IsEnd();
		}
		/// <summary>
		/// フェードアウト
		/// </summary>
		public void FadeOut(System.Action callback) {
			task_.Play(Out(callback));
		}
		/// <summary>
		/// フェードイン
		/// </summary>
		public void FadeIn(System.Action callback) {
			task_.Play(In(callback));
		}

		/// <summary>
		/// フェードアウト本体
		/// </summary>
		private IEnumerator Out(System.Action callback) {
			while (alpha_ < 1f) {
				alpha_ += Time.deltaTime * TRANSIT_TIME;
				SetAlpha(alpha_);
				yield return null;
			}
			alpha_ = 1f;
			SetAlpha(alpha_);
			callback();
		}
		/// <summary>
		/// フェードイン本体
		/// </summary>
		private IEnumerator In(System.Action callback) {
			while (alpha_ > 0f) {
				alpha_ -= Time.deltaTime * TRANSIT_TIME;
				SetAlpha(alpha_);

				yield return null;
			}
			alpha_ = 0f;
			SetAlpha(alpha_);
			callback();
		}
		/// <summary>
		/// アルファ値設定
		/// </summary>
		private void SetAlpha(float alpha) {
			rawImage_.color = new Color(1f, 1f, 1f, alpha_);

		}

	}
}
