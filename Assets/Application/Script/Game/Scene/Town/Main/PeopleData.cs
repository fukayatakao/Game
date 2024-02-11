using Project.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// タウンで使用する情報
	/// </summary>
	[System.Serializable]
	public class PeopleData {
		//シングルトン
		static PeopleData instance_;
		public static PeopleData I { get { return instance_; } }

		//キャラクターデータ
		private Dictionary<int, CharacterData> characters_;
		public Dictionary<int, CharacterData> Characters { get { return characters_; } }
		/// <remarks>
		/// サーバから受け取ったデータを元に生成
		/// </remarks>
		public static void Create(Response response) {
			instance_ = new PeopleData();
			instance_.Setup(response as TownMainResponse);
		}

		/// <remarks>
		/// インスタンス破棄
		/// </remarks>
		public static void Destroy() {
			instance_ = null;
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		private void Setup(TownMainResponse response) {
			characters_ = response.characters.ToDictionary(chara => chara.id);
		}

		/// <summary>
		/// 生産施設にいるキャラ一覧を計算
		/// </summary>
		public List<int> CalcAssignCharacter(int factoryId) {
			var list = characters_.Values
				.Where(data => data.workId == factoryId)
				.OrderBy(x => x.workSlotId)
				.Select(data => data.id).ToList();
			//住居に登録されてないのに働いているキャラがいたらエラー出す
			Debug.Assert(!characters_.Values.Any(data => data.workId != -1 && data.stayId == -1), "found illegal character data");
			return list;
		}

		/// <summary>
		/// 生産施設にいないキャラ一覧を計算
		/// </summary>
		public List<int> CalcNotAssignCharacter() {
			return characters_.Values.Where(data => data.workId == -1 && data.stayId != -1).Select(data => data.id).ToList();
		}
		/// <summary>
		/// 生産施設のキャラ割り当てを更新
		/// </summary>
		public void UpdateAssignCharacter(int factoryId, int oldCharaId, int newCharaId) {
			List<CharacterData> send = new List<CharacterData>();

			int slot = oldCharaId != -1 ? characters_[oldCharaId].workSlotId : CalcAssignCharacter(factoryId).Count;

			//旧住人の割り当て解除
			if (oldCharaId >= 0) {
				characters_[oldCharaId].workId = -1;
				characters_[oldCharaId].workSlotId = -1;
				send.Add(characters_[oldCharaId]);
			}
			//新住人を割り当て
			if (newCharaId > 0) {
				characters_[newCharaId].workId = factoryId;
				characters_[newCharaId].workSlotId = slot;
				send.Add(characters_[newCharaId]);
			}

			//変更があったキャラの情報をサーバに送る
			UpdateCharacterCmd.CreateAsync(new UpdateCharacterRequest(send));
		}
		/// <summary>
		/// 生産施設にいるキャラの労働割り当てを解除
		/// </summary>
		public void ResetAssign(int factoryId) {
			foreach (CharacterData chara in characters_.Values) {
				if (chara.workId == factoryId)
					chara.workId = -1;
			}
		}

		/// <summary>
		/// 住居にいるキャラ一覧を計算
		/// </summary>
		public List<int> CalcStayCharacter(int residenceId) {
			var list = characters_.Values
				.Where(data => data.stayId == residenceId)
				.OrderBy(x=>x.staySlotId)
				.Select(data => data.id).ToList();
			return list;
		}

		/// <summary>
		/// 住居にいないキャラ一覧を計算
		/// </summary>
		public List<int> CalcNotStayCharacter() {
			return characters_.Values.Where(data => data.stayId == -1).Select(data => data.id).ToList();
		}

		/// <summary>
		/// 住居のキャラ割り当てを更新
		/// </summary>
		public void UpdateStayCharacter(int residenceId, int oldCharaId, int newCharaId) {
			List<CharacterData> send = new List<CharacterData>();

			int slot = oldCharaId != -1 ? characters_[oldCharaId].staySlotId : CalcStayCharacter(residenceId).Count;

			//旧住人の割り当て解除
			if (oldCharaId >= 0) {
				characters_[oldCharaId].stayId = -1;
				characters_[oldCharaId].staySlotId = -1;
				send.Add(characters_[oldCharaId]);
			}
			if(newCharaId > 0) {
				//新住人を割り当て
				characters_[newCharaId].stayId = residenceId;
				characters_[newCharaId].staySlotId = slot;
				send.Add(characters_[newCharaId]);
			}

			//変更があったキャラの情報をサーバに送る
			UpdateCharacterCmd.CreateAsync(new UpdateCharacterRequest(send));
		}
		/// <summary>
		/// 住居にいるキャラのキャラ割り当てを解除
		/// </summary>
		public void ResetStay(int residenceId) {
			foreach (CharacterData chara in characters_.Values) {
				if (chara.stayId == residenceId) {
					chara.stayId = -1;
					chara.workId = -1;
				}
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private PeopleData() {

		}
	}
}
