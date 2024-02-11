using System;
using Project.Lib;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// バトルレポート
	/// </summary>
	public class BattleReport : MonoPretender {
		[Serializable]
		public class Squad {
			public int unitId;
			public int number;
		}

		[Serializable]
		public class Platoon {
			public List<Squad> squads = new List<Squad>();
			public int leader;
		}

		[Serializable]
		public class Progress {
			public float startTime;
			public float finishTime;
			public float firstContact;
		}

		[Serializable]
		public class Status {
			public float playerHp;				//残りプレイヤーユニットのHP合計
			public float playerLp;				//残りプレイヤーユニットのLP合計
			public int playerNumber;			//残りプレイヤーユニット数

			public float enemyHp;				//残りエネミーユニットのHP合計
			public float enemyLp;				//残りエネミーユニットのLP合計
			public int enemyNumber;			//残りエネミーユニット数

		}

		[Serializable]
		public class Data {
			public Data(Platoon player, Platoon enemy, Progress progress, Status initial, Status result) {
				this.player = player;
				this.enemy = enemy;
				this.progress = progress;
				this.initial = initial;
				this.result = result;
			}
			public Platoon player;
			public Platoon enemy;
			public Progress progress;
			public Status initial;
			public Status result;
		}

		private PlatoonEntity player_;
		private PlatoonEntity enemy_;
		private BattleTimer timer_;


		private Progress progress_;
		private Status initial_;
		private Status result_;

		private bool output_;

		public void SetOutput(bool flag) {
			output_ = flag;
		}
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.JournalEvent, MessageGroup.DebugEvent);
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
		}

		public void Init(BattleMain main) {
			player_ = main.Platoon[(int)Power.Player];
			enemy_ = main.Platoon[(int)Power.Enemy];
			timer_ = main.HaveTimer;

			progress_ = new Progress();
			initial_ = new Status();
			result_ = new Status();
		}


#if UNITY_EDITOR
		static readonly string path_ = UnityEngine.Application.dataPath + "/../Report/";
		public void Report() {
			if (!output_)
				return;
			string json = JsonUtility.ToJson(new Data(BattleReportUtil.CreateData(player_), BattleReportUtil.CreateData(enemy_), progress_, initial_, result_), true);
			if (!Directory.Exists(path_)) {
				Directory.CreateDirectory(path_);
			}

			DateTime now = DateTime.Now;
			string date = string.Format("{0:0000}_{1:00}_{2:00}_{3:00}_{4:00}_{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			using (StreamWriter sw = new StreamWriter(path_ + "Report_" + date + ".txt", false, Encoding.UTF8)) {
				sw.Write(json);
			}

		}
#endif
		/// <summary>
		/// ゲーム開始を記録
		/// </summary>
		public void RecStart() {
			progress_.startTime = timer_.PastTime;

			initial_ = BattleReportUtil.CreateStatus(player_, enemy_);
		}
		/// <summary>
		/// ゲーム終了を記録
		/// </summary>
		public void RecFinish() {
			progress_.finishTime = timer_.PastTime;

			result_ = BattleReportUtil.CreateStatus(player_, enemy_);
		}

		/// <summary>
		/// いづれかのユニットが最初に攻撃した時刻
		/// </summary>
		public void RecFirstContact() {
			//firstContact = Time.time;
		}
	}
}
