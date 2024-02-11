using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// 定数
	/// </summary>
	public class TownConstConfig : MonoBehaviour {
#if UNITY_EDITOR
		static string path = Application.dataPath + "/Application/Resources/TownConst.json";
		[SerializeField]
		TownConstFile conf;
		/// <summary>
		/// インスペクタの値に変化があった
		/// </summary>
		void OnValidate() {
			Save();
			GameConst.Town = new TownConst();
		}


		/// <summary>
		/// 設定をセーブ
		/// </summary>
		public void Save() {
			using (var sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8)) {
				//Inspectorに表示されているパラメータで保存する
				string text = JsonUtility.ToJson(conf, true);
				sw.WriteLine(text);
			}
		}

		/// <summary>
		/// 設定をロード
		/// </summary>
		public void Load() {
			using (var sr = new System.IO.StreamReader(path)) {
				string json = sr.ReadToEnd();
				conf = JsonUtility.FromJson<TownConstFile>(json);
				GameConst.Town = new TownConst();
			}

		}
#endif
	}
}
