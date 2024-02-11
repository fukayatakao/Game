using Project.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 戦略マップ状況
	/// </summary>
	[System.Serializable]
	public class StrategySituation {
		static Project.Network.StrategySituationData instance_;
		public static Project.Network.StrategySituationData I { get { return instance_; } }

		//---- レスポンス変数定義 ----
		public List<PlatoonData> invader; //未配置攻撃側部隊情報
		public List<PlatoonData> defender; //未配置守備側部隊情報(?)
		public List<StrategyLocationData> invaderLocation; //配置済攻撃部隊
		public List<StrategyLocationData> defenderLocation; //配置済守備部隊
		public int mapId; //マップモデル
		public int state; //ゲームの状態(STRATEGY_STATE)

		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response response) {
			//シングルトン解放されてないのにインスタンス作ろうとしたらエラー投げる（実害はないが無駄メモリなので
			Debug.Assert(instance_ == null, "not release instance");
			switch (response) {
			case StrategyMainResponse:
				instance_ = (response as StrategyMainResponse).situation;
				break;
			case BattleResultWinResponse:
				instance_ = (response as BattleResultWinResponse).situation;
				break;
			}

		}

		/// <remarks>
		/// インスタンス破棄
		/// </remarks>
		public static void Destroy() {
			instance_ = null;
		}
	}
}
