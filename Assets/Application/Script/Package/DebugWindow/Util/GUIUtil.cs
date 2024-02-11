using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
#if DEVELOP_BUILD
	public static class GUIUtil {
		//GUIに変更があったか監視するときのスタック
		static Stack<bool> enable_ = new Stack<bool>();

		/// <summary>
		/// 変更の監視開始
		/// </summary>
		public static void BeginChangeCheck() {
			enable_.Push(GUI.changed);
			GUI.changed = false;
		}

		/// <summary>
		/// Begin以降に変更があったかチェック
		/// </summary>
		public static bool EndChangeCheck() {
			bool result = GUI.changed;
			GUI.changed = enable_.Pop() || GUI.changed;
			return result;
		}

		/// <summary>
		/// スコープを抜けたときに変更があったらActionを実行するクラス
		/// </summary>
		public class ChangeCheckScope : System.IDisposable{
			System.Action action_;
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public ChangeCheckScope(System.Action action) {
				//Actionを登録
				action_ = action;
				BeginChangeCheck();
			}
			/// <summary>
			/// デストラクタ
			/// </summary>
			public void Dispose()
			{
				//ここまでで変更があったらActionを実行
				if (EndChangeCheck()) {
					action_();
				}
			}

		}
	}

#endif

}
