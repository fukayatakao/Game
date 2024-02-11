using Project.Lib;
using UnityEngine;

namespace Project.Game {
	public class BattleTimer : MonoPretender {
		//経過時間
		private float pastTime_ = 0f;
		public float PastTime { get { return pastTime_; } }

		private float limitTime_ = 180f;
		public float LimitTime {
			get { return limitTime_;}
			set { limitTime_ = value;}
		}

		private bool enable_;
		/// <summary>
		/// サブコンポーネントがある場合はここで生成
		/// </summary>
		protected override void Create(GameObject obj) {
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.UserEvent, MessageGroup.GameEvent, MessageGroup.DebugEvent);
		}

		protected override void Destroy() {
		}


		/// <summary>
		/// ゲーム開始初期化
		/// </summary>
		public void GameStart() {
			enable_ = true;
			pastTime_ = 0f;
		}

		/// <summary>
		/// 演出などで時間を止める
		/// </summary>
		public void Stop() {
			enable_ = false;
		}
		/// <summary>
		/// 演出などで時間を止める
		/// </summary>
		public void Resume() {
			enable_ = true;
		}
		/// <summary>
		/// 時間経過が有効な状態か
		/// </summary>
		public bool IsEnable() {
			return enable_;
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
			//誤差が積み重なるけど一旦無視
			pastTime_ += Time.deltaTime;

			//タイムアップ
			if (pastTime_ > limitTime_) {
				pastTime_ = limitTime_;
				BattleMessage.GameEnd.Broadcast();
			}
		}
	}
}
