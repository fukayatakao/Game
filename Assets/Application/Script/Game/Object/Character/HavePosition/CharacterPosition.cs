using UnityEngine;
using Project.Lib;

namespace Project.Game {
	/// <summary>
	/// ポジション関係の記憶
	/// </summary>
	/// <remarks>
	/// 参照が多く混乱しやすい情報なので隠蔽しやすくするためBlackbordから分離
	/// </remarks>
	public class CharacterPosition : MonoPretender {

		public enum Status {
			Deploy,
			Ground,
			Enemy,
		}

		public Status status{ get; set; }



		//部隊上の展開座標
		public Vector3 DeployPosition;

		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			status = Status.Deploy;
		}

		/// <summary>
		/// 定期的に近くの敵を検査
		/// </summary>
		public void Execute(CharacterEntity owner) {
		}

		/// <summary>
		/// 座標をキャラの半径と移動可能矩形で丸める
		/// </summary>
		public static Vector3 ClampPos(Vector3 pos, float radius, Rect rect) {
			//移動可能エリアの外側が移動先に指定されたら移動可能エリア内に丸める
			if (rect.xMin > pos.x - radius) {
				pos.x = rect.xMin + radius;
			} else if (rect.xMax < pos.x + radius) {
				pos.x = rect.xMax - radius;
			}
			//移動可能エリアの外側が移動先に指定されたら移動可能エリア内に丸める
			if (rect.yMin > pos.z - radius) {
				pos.z = rect.yMin + radius;
			} else if (rect.yMax < pos.z + radius) {
				pos.z = rect.yMax - radius;
			}

			return pos;
		}
		/// <summary>
		/// 目標座標に到達したか
		/// </summary>
		public bool CheckDeployPos(Vector3 pos) {
			Vector3 targetVec = DeployPosition - pos;
			float distSq = targetVec.sqrMagnitude;
			return distSq < GameConst.Battle.ARRIVAL_DISTANCE_SQ;
		}
	}
}

