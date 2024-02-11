using Project.Lib;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 分隊の展開ライン制御
	/// </summary>
	public class SquadDeployLine : MonoPretender {
#if DEVELOP_BUILD
		LineMesh lineMesh_ = new LineMesh();
#endif
		//ラインの設定限度(後方)
		[SerializeField]
		private float limit_;
		[SerializeField]
		private float depth_;

		//リーダー配置用の奥行オフセット
		private float offset_;

		//メッセージ処理システムに組み込むための受容体
		MessageSystem.Receptor receptor_;


		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
#if DEVELOP_BUILD
			lineMesh_.UnLoad();
#endif
		}

		public async Task LoadAsync(Transform parent) {
#if DEVELOP_BUILD
			//展開するべきラインのインスタンスを生成
			await lineMesh_.LoadAsync(AddressableDefine.Address.DEBUG_LINE);
			lineMesh_.Create(parent);
			lineMesh_.Hide();
			//メッセージ受容体作成
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
#endif
		}

		/// <summary>
		/// ライン限度を変更
		/// </summary>
		public void SetLimit(float depth) {
			limit_ = depth;
		}

		/// <summary>
		/// 展開ラインの更新
		/// </summary>
		public void UpdateLine(float depth, float sign) {
			//ステージ外に出ないように丸め込み
			depth_ = ClampDepth(depth, sign);
			Distortion(depth_);
#if DEVELOP_BUILD
			lineMesh_.Calculate();
#endif
		}
		/// <summary>
		/// ラインの設定限度で丸める
		/// </summary>
		private float ClampDepth(float depth, float sign) {
			return (depth - limit_) * sign < 0f ? limit_ : depth;
		}

		/// <summary>
		/// リーダーの展開座標を計算
		/// </summary>
		public Vector3 CalcLeaderPos() {
			Vector3 pos = Vector3.zero;
			pos.x = CalcEvenDistanceX(0, 1);
			pos.z = depth_ - offset_;
			return pos;
		}

		/// <summary>
		/// x座標から曲線上のz座標を計算
		/// </summary>
		public Vector3 CalcUnitPos(int index, int max) {
			Vector3 pos = Vector3.zero;
			pos.x = CalcEvenDistanceX(index, max);
			pos.z = Evaluate(pos.x);

			return pos;
		}


		const int DISTORTION_POINT_MAX = 5;
		const float DISTORTION_INV = 1f / (DISTORTION_POINT_MAX - 1);

		//制御点の情報
		[System.Serializable]
		private class CurvePoint {
			public Vector3 p;           //座標
			public Vector3 v;           //傾き
		}
		[SerializeField]
		CurvePoint[] point_ = new CurvePoint[DISTORTION_POINT_MAX];
		//ステージ左端から右端までの距離
		float size_;
		float halfSize_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public void Init(float size) {
			size_ = size;
			halfSize_ = size_ * 0.5f;

			for (int i = 0; i < DISTORTION_POINT_MAX; i++) {
				point_[i] = new CurvePoint();
			}
#if DEVELOP_BUILD
			lineMesh_.Init(Evaluate, size, 1f, 0.01f);
#endif
		}
		/// <summary>
		/// リーダーの展開位置オフセットを設定
		/// </summary>
		public void InitOffset(CharacterEntity leader, float unitRadius, float sign) {
			if (leader == null)
				return;
			offset_ = sign * (GameConst.Battle.LINE_DISTORTION_DEPTH + leader.HaveCollision.Radius + unitRadius);
		}

		/// <summary>
		/// 制御点をリセットして歪みをなくす
		/// </summary>
		public void InitDepth(float z) {
			for (int i = 0; i < DISTORTION_POINT_MAX; i++) {
				float x = i * DISTORTION_INV;
				point_[i].p = new Vector3(x * size_ - halfSize_, 0f, z);
				point_[i].v = new Vector3(1f, 0f, 0f);
			}
			depth_ = z;
		}

		/// <summary>
		/// 歪み付きの制御点リセット
		/// </summary>
		private void Distortion(float depth) {
			for (int i = 0; i < DISTORTION_POINT_MAX; i++) {
				float x = i * DISTORTION_INV;
				float z = DeterminateRandom.Range(-1f, 1f) * GameConst.Battle.LINE_DISTORTION_DEPTH + depth;
				point_[i].p = new Vector3(x * size_ - halfSize_, 0f, z);
			}

			//両端の傾きはx方向に水平
			point_[0].v = new Vector3(1f, 0f, 0f);
			point_[DISTORTION_POINT_MAX - 1].v = new Vector3(1f, 0f, 0f);

			for (int i = 1; i < DISTORTION_POINT_MAX - 1; i++) {
				//一つ前の点の傾きを現在の点と２つ前の点から計算
				point_[i].v = point_[i + 1].p - point_[i - 1].p;
				point_[i].v.Normalize();

			}
		}


		/// <summary>
		/// 一列に均等に配置したときのx座標を計算
		/// </summary>
		private float CalcEvenDistanceX(int index, int max) {
			float t = (float)index / max;
			float offset = halfSize_ / max;
			//0<t<1の範囲を実際のx座標に変換
			return size_ * t - halfSize_ + offset;
		}

		/// <summary>
		/// x座標から曲線上のz座標を計算
		/// </summary>
		private float Evaluate(float x) {
			for (int i = 1; i < DISTORTION_POINT_MAX; i++) {
				if (point_[i].p.x < x) {
					continue;
				}
				float t = (x - point_[i - 1].p.x) / (point_[i].p.x - point_[i - 1].p.x);
				Vector3 pos = MathUtil.CalcHermite(point_[i - 1].p, point_[i - 1].v, point_[i].p, point_[i].v, t);

				return pos.z;
			}
			return 0f;
		}
#if DEVELOP_BUILD
		/// <summary>
		/// 表示制御
		/// </summary>
		public void SetVisible(bool flag) {
			//on/offすると警告出るので表示を小さくして回避
			if (flag) {
				lineMesh_.Show();
			} else {
				lineMesh_.Hide();
			}
		}
#endif

	}
}

