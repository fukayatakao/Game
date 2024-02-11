using Project.Network;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// バトル状況
	/// </summary>
	[System.Serializable]
	public class BattleSituation{
		static Project.Network.BattleSituationData instance_;
		public static Project.Network.BattleSituationData I { get { return instance_; } }

		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response response) {
			//シングルトン解放されてないのにインスタンス作ろうとしたらエラー投げる（実害はないが無駄メモリなので
			Debug.Assert(instance_ == null, "not release instance");
#if DEVELOP_BUILD
			if (response is DebugBattleMainResponse)
				instance_ = (response as DebugBattleMainResponse).situation;
			else
				instance_ = (response as BattleMainResponse).situation;
#else
			instance_.Setup(response as BattleMainResponse);
#endif

#if RUN_BACKGROUND
			BattleLog.WritePlatoon(instance_.invader, instance_.defender);
#endif
		}

		/// <remarks>
		/// インスタンス破棄
		/// </remarks>
		public static void Destroy() {
			instance_ = null;
		}
	}
}
