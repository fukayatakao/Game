using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// 関係線
    /// </summary>
	public class Link : Entity {
		//矢印UI
		SpriteRenderer arrow_;

		//矢印UIのデフォルトサイズ
		static readonly Vector2 DefaultArrowSize = new Vector2(0.5f, 0.5f);
		//地面から浮かすオフセット
		static readonly Vector3 DefaultOffset = new Vector3(0f, 0.05f, 0f);
		//地面に水平になるように回転
		static readonly Quaternion DefaultRotate = MathUtil.RotateEulerX(-90f);

		Vector3 offset_;
		Facility start_;
		Facility end_;

		const float ARROW_SCALE = 10f;
		const float ARROW_SCALE_INVERSE = 1f / ARROW_SCALE;

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string resName) {
			GameObject obj = UnityUtil.Instantiate(ResourceCache.Load<GameObject>(resName));
			arrow_ = obj.GetComponent<SpriteRenderer>();
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);

			return obj;
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
		}

		public void Setup(Facility start, Facility end) {
			start_ = start;
			end_ = end;
			offset_ = new Vector3(0f, 0f, 10f);
		}

		public void SetOffset(Vector3 offset) {
			offset_ = offset;
		}

		/// <summary>
		/// 実行後処理
		/// </summary>
		public override void LateExecute() {
			CalcStretch(start_, end_, offset_);
		}

		/// <summary>
		/// 始点・終点から矢印の形状を計算
		/// </summary>
		private void CalcStretch(Facility start, Facility end, Vector3 offset) {
			Vector3 vec = end.GetPosition() - start.GetPosition();
			Quaternion quaternion = MathUtil.LookAtY(vec);

			offset.z = start.HaveCollision.GetRadius();
			arrow_.transform.localPosition = start.GetPosition() + DefaultOffset + quaternion * offset;
			arrow_.transform.localRotation = quaternion * DefaultRotate;
			arrow_.transform.localScale = new Vector2(ARROW_SCALE, ARROW_SCALE);
			arrow_.size = new Vector2(DefaultArrowSize.x, (vec.magnitude - (start.HaveCollision.GetRadius() + end.HaveCollision.GetRadius())) * ARROW_SCALE_INVERSE);
		}


#if DEVELOP_BUILD
		/// <summary>
		/// 表示制御
		/// </summary>
		public void SetVisible(bool flag) {
			//on/offすると警告出るので表示を小さくして回避
			if (flag) {
				cacheTrans_.localScale = new Vector3(1f, 1f, 1f);
			} else {
				cacheTrans_.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
			}
			//renderer_.enabled = flag;
		}
#endif
	}
}


