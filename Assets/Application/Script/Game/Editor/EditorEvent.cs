namespace Project.Game {
	/// <summary>
	/// エディタイベントで行う処理
	/// </summary>
	public static class EditorEvent {
        /// <summary>
        /// Indexから名前に変換
        /// </summary>
        public static string BuildEffectName(int index) {
            int dir = index / 100;
            return "ef_" + index.ToString();
        }

        /// <summary>
    }





}
