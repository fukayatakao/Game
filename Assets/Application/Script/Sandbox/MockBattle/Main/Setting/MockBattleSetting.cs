using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace Project.Game {
#if UNITY_EDITOR
	public class MockBattleSetting : MonoBehaviour {
		[Serializable]
		public class MockSquad {
			public int unitId;
			public int number;
			public int leader;
		}
		public List<MockSquad> Attacker = new List<MockSquad>() { new MockSquad() { unitId = 1, number = 1, leader = 0 } };
		public List<MockSquad> Defender = new List<MockSquad>() { new MockSquad() { unitId = 1, number = 1, leader = 0 } };

		public int BattleTimeSecond;

		//セーブ用
		public class Data {
			public List<MockSquad> Player;
			public List<MockSquad> Enemy;
			public int BattleTimeSecond;
		}

		static readonly string path_ = Application.dataPath.Remove(Application.dataPath.Length - "/Assets".Length) + "/MockBattle";

		public void Save(string filename) {
			Data data = new Data();
			data.Player = Attacker;
			data.Enemy = Defender;
			data.BattleTimeSecond = BattleTimeSecond;

			string json = JsonUtility.ToJson(data, true);
			if (!Directory.Exists(path_)) {
				Directory.CreateDirectory(path_);
			}

			if (Path.IsPathRooted(filename)) {
				File.WriteAllText(filename, json, Encoding.UTF8);
			} else {
				File.WriteAllText(path_ + Path.GetFileName(filename), json, Encoding.UTF8);
			}

		}

		public void Load(string filename) {
			string json = File.ReadAllText(filename, Encoding.UTF8);
			Data data = JsonUtility.FromJson<Data>(json);
			Attacker = data.Player;
			Defender = data.Enemy;
			BattleTimeSecond = data.BattleTimeSecond;
		}

	}
#endif
}
