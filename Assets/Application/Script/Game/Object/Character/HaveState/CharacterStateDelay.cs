namespace Project.Game {
	/// <summary>
	/// キャラクターの状態変化遅延
	/// </summary>
	[System.Serializable]
	public class CharacterStateDelay {
		//遅延が有効か(デフォルト有効)
		private bool enable_=true;
		//命令遅延時間(s)
		public float Delay { get; private set; }

		/// <summary>
		/// 遅延を有効にする
		/// </summary>
		public void Enable() {
			enable_ = true;
		}
		/// <summary>
		/// 遅延を無効にする
		/// </summary>
		public void Disable() {
			enable_ = false;
		}
		/// <summary>
		/// 遅延時間をリセット
		/// </summary>
		public void Reset() {
			Delay = 0;
		}

		/// <summary>
		/// 遅延の仕込み
		/// </summary>
		public void CalcDelay() {
			if (!enable_)
				return;
			//自然なばらつきが出るように遅延を入れる
			float r = DeterminateRandom.Range(0f, GameConst.Battle.ORDER_DELAY_TIME);
			Delay = Time.time + r;


		}
	}
}
