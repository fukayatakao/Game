using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Http.Mst;

namespace Project.Game {
	public static class CharacterUtil {
		/// <summary>
		/// ターゲットキャラクターの方向に向き直る
		/// </summary>
		public static Quaternion CalcAimTarget(Vector3 ownerPos, Quaternion ownerRot, Vector3 targetPos, float aimSpeedDeg) {
			Quaternion lookForward = MathUtil.LookAtY(targetPos - ownerPos);
			return Quaternion.RotateTowards(ownerRot, lookForward, aimSpeedDeg * Time.deltaTime);
		}
		private static Network.CharacterData emptyData_ = null;

		public static Network.CharacterData GetEmptyCharacterData(){
			if (emptyData_ == null) {
				emptyData_ = new Network.CharacterData() {
					name = "名無し",
					generate = false,
					portrait = "chara_0000",
					ability = new List<int>()
				};
			}
			//属性は常にランダムで抽選する
			emptyData_.phase = UnityEngine.Random.Range(0, (int)PHASE.MAX);
			return emptyData_;
		}

	}
}
