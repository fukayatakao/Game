using UnityEngine;

/// <summary>
/// 定数
/// </summary>
public class OrganizeConst {
	//リーダー交代により失われる熟練度
	public readonly int LOST_EXPERIENCE_CHANGE_LEADER;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public OrganizeConst() {
		OrganizeConstFile file = Load();
		LOST_EXPERIENCE_CHANGE_LEADER = file.LOST_EXPERIENCE_CHANGE_LEADER;
	}

	/// <summary>
	/// 設定をロード
	/// </summary>
	public static OrganizeConstFile Load() {
		var json = Resources.Load("OrganizeConst") as TextAsset;
		return JsonUtility.FromJson<OrganizeConstFile>(json.text);
	}

}
