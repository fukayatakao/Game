using UnityEngine;


/// <summary>
/// 定数
/// </summary>
public class BattleConstConfig : MonoBehaviour {
#if UNITY_EDITOR
	static readonly string path = Application.dataPath + "/Application/Resources/BattleConst.json";

	[SerializeField]
	BattleConstFile conf;
	/// <summary>
	/// インスペクタの値に変化があった
	/// </summary>
	void OnValidate() {
		Save();
		GameConst.Battle = new BattleConst();
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
			conf = JsonUtility.FromJson<BattleConstFile>(json);
			GameConst.Battle = new BattleConst();
		}

	}

#endif
}

