using UnityEngine;
using System.Collections.Generic;
using Project.Lib;


namespace Project.Game {

	/// <summary>
	/// 列制御
	/// </summary>
	public class PlatoonDeploy : MonoPretender {
		private float[] abreastDepth_ = new float[(int)Abreast.MAX];

		//展開予定ラインと展開しなおす検知用ライン
		public float ForwardLine;
		public float ForwardDetect;

		public float BackwardLine;
		public float BackwardDetect;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PlatoonDeploy() {
			ForwardLine = GameConst.Battle.DEPLOY_FORWARD_LINE;
			ForwardDetect = GameConst.Battle.DEPLOY_FORWARD_DETECT;
			BackwardLine = GameConst.Battle.DEPLOY_BACKWARD_LINE;
			BackwardDetect = GameConst.Battle.DEPLOY_BACKWARD_DETECT;
		}
		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(PlatoonEntity owner) {
			NormalExecute(owner);
		}
		/// <summary>
		/// 展開ラインの更新
		/// </summary>
		private void NormalExecute(PlatoonEntity owner) {
			IReadOnlyList<SquadEntity> groups = owner.Squads;

			//列が全滅していたら何もしない
			if (groups.Count == 0)
				return;
			if (owner.IsSwap()) {
				return;
			}


			//突撃する列は敵陣最奥に移動先を設定
			if (owner.HaveBlackboard.RushLine) {
				for (int i = 0, max = groups.Count; i < max; i++) {
					groups[i].UpdateLine(owner.TowardSign * GameConst.Battle.LONG_DISTANCE);
				}
			} else {
				groups[0].UpdateLine(owner.TowardSign * GameConst.Battle.LONG_DISTANCE);

				if (groups.Count > 2 && groups[0].IsBackward(groups[1].HaveLimit.ForwardLimit, ForwardDetect)) {
					float d = groups[0].HaveLimit.BackwardLimit - ForwardLine * owner.TowardSign;
					groups[1].UpdateLine(d);
				}


				//3列目以降
				for (int i = 2, max = groups.Count; i < max; i++) {
					SquadEntity front = groups[i - 1];
					SquadEntity current = groups[i];
					//検査する列がひとつ前の列から遠すぎる=>展開ラインを前に設定しなおす
					if (front.IsBackward(current.HaveLimit.ForwardLimit, BackwardDetect)) {
						float d = front.HaveLimit.BackwardLimit - BackwardLine * owner.TowardSign;
						groups[i].UpdateLine(d);
					}
				}

			}

			//owner.Leader.UpdateLine();
		}
		/// <summary>
		/// 前方入れ替え
		/// </summary>
		public void ForeSwap(PlatoonEntity owner) {
			//入れ替わった状態で入ってくるので現前列は2列目にいる
			SquadEntity front = owner.Squads[(int)Abreast.Second];
			SquadEntity back = owner.Squads[(int)Abreast.First];

			//現在の後列展開ラインを退避
			float backDepth = back.HaveLimit.BackwardLimit;

			//後列の前進開始
			back.UpdateLine(owner.TowardSign * GameConst.Battle.LONG_DISTANCE);

			//現前列と後列が一定距離以下になったら前列の展開ラインを退避したラインに設定


			//後列のラインが近すぎる場合は距離を空ける
			if(Mathf.Abs(front.HaveLimit.BackwardLimit - backDepth) < BackwardLine) {
				//２列目
				float second = front.HaveLimit.BackwardLimit - ForwardLine * owner.TowardSign;
				front.UpdateLine(second);
				front.SwapBackward(back);
				//３列目も設定しなおし
				if (owner.Squads.Count > (int)Abreast.Third) {
					SquadEntity aft = owner.Squads[(int)Abreast.Third];
					float third = second - BackwardLine * owner.TowardSign;
					aft.UpdateLine(third);
				}
			} else {
				front.UpdateLine(backDepth);
				front.SwapBackward(back);
			}
		}
		/// <summary>
		/// 後方入れ替え開始
		/// </summary>
		public void AftSwap(PlatoonEntity owner) {
			//入れ替わった状態で入ってくるので中列は3列目、後列は2列目になっている
			SquadEntity middle = owner.Squads[(int)Abreast.Third];
			SquadEntity back = owner.Squads[(int)Abreast.Second];

			float second = middle.HaveLimit.ForwardLimit;
			float third = back.HaveLimit.BackwardLimit;

			back.UpdateLine(second);

			middle.UpdateLine(third);
			middle.SwapBackward(back);
		}
		/// <summary>
		/// 輪番入れ替え開始
		/// </summary>
		public void Rotation(PlatoonEntity owner) {
			if (owner.Squads.Count < 3) {
				ForeSwap(owner);
				return;
			}
			//入れ替え済の状態で入ってくる
			SquadEntity front = owner.Squads[(int)Abreast.Third];
			SquadEntity middle = owner.Squads[(int)Abreast.First];
			SquadEntity back = owner.Squads[(int)Abreast.Second];


			//中列の前進開始
			middle.UpdateLine(owner.TowardSign * GameConst.Battle.LONG_DISTANCE);

			//２列目
			float middleDepth = front.HaveLimit.BackwardLimit - ForwardLine * owner.TowardSign;
			//３列目
			float backDepth = middleDepth - BackwardLine * owner.TowardSign;
			front.UpdateLine(backDepth);
			front.SwapBackward(back);

			back.UpdateLine(middleDepth);
			back.SwapBackward(front);
		}
	}
}
