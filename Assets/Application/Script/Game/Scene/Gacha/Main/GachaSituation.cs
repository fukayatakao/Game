using Project.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// バトル状況
	/// </summary>
	[System.Serializable]
	public class GachaSituationData {
		static GachaSituationData instance_;
		public static GachaSituationData I { get { return instance_; } }

		//一般キャラクター
		List<CharacterData> characters_ = new List<CharacterData>();
		public List<CharacterData> Characters { get { return characters_; } }
		//リーダー
		List<LeaderData> leaders_ = new List<LeaderData>();
		public List<LeaderData> Leaders { get { return leaders_; } }


		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response res) {
			//シングルトン解放されてないのにインスタンス作ろうとしたらエラー投げる（実害はないが無駄メモリなので
			Debug.Assert(instance_ == null, "not release instance");
			instance_ = new GachaSituationData();
			var response = res as GachaMainResponse;
			instance_.characters_ = response.characters;
			instance_.leaders_ = response.leaders;
		}

		/// <remarks>
		/// インスタンス破棄
		/// </remarks>
		public static void Destroy() {
			instance_ = null;
		}
	}
}
