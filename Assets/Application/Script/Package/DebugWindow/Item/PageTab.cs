namespace Project.Lib {
#if DEVELOP_BUILD
	/// <summary>
	/// 切り替え選択
	/// </summary>
	public class PageTab {
		public static int Draw(int index, string[] text, System.Action[] draw) {
			index = FitGUILayout.SelectionGrid(index, text, text.Length);
			draw[index]();
			return index;
		}
	}
#endif
}
