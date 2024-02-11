using System.Collections.Generic;
using Project.Http.Mst;
using Project.Lib;
using Project.Mst;
using UnityEngine;


namespace Project.Game {
    /// <summary>
    /// キャラクターの個別パラメータ管理
    /// </summary>
    public class CharacterPersonal : MonoPretender {
	    public int LeaderId { get; private set; }
	    public bool IsLeader() { return LeaderId != 0; }
	    public string CharaName { get; private set; }
	    public int Species { get; private set; }
	    public List<int> Ability { get; private set; }
	    public PHASE Phase { get; private set; }
	    public bool Generate;
	    public string Portrait;
	    public int Grade;

	    private Texture2D protraitTextureCache_;

	    /// <summary>
	    /// キャラクターのポートレート画像をロード
	    /// </summary>
	    public Texture2D LoadPortraitTexture()
	    {
		    if (protraitTextureCache_ == null) {
			    if (IsLeader()) {
				    CharacterPortraitUtil.LoadLeaderTexture(Portrait, (tex) => { protraitTextureCache_ = tex; });
			    } else {
				    protraitTextureCache_ = Generate ? CharacterPortraitUtil.LoadTextureFromPrefs(Portrait) : CharacterPortraitUtil.LoadTextureFromResource(Portrait);
			    }
		    }


		    return protraitTextureCache_;
	    }

	    /// <summary>
	    /// キャラクターの属性アイコン
	    /// </summary>
	    public Texture2D LoadPhaseIcon() {
		    return ResourcePool.GetPhaseIcon(Phase);
	    }

	    /// <summary>
	    /// コンストラクタ
	    /// </summary>
	    public void Init(Network.CharacterData data) {
		    LeaderId = 0;
		    CharaName = data.name;
		    Species = data.species;
		    Ability = data.ability;
		    Phase = (PHASE)data.phase;
		    Generate = data.generate;
		    Portrait = data.portrait;
		    Grade = data.grade;
		    protraitTextureCache_ = null;
	    }
	    /// <summary>
	    /// コンストラクタ
	    /// </summary>
	    public void Init(Network.LeaderData data) {
		    MstLeaderData leaderData = MstLeaderData.GetData(data.leaderMasterId);

		    LeaderId = data.leaderMasterId;
		    CharaName = leaderData.DbgName;			//リーダーはマスターで名前が決まってる
		    Species = 0;
		    Ability = new List<int>();
		    Portrait = "Character/Leader/leader_" + LeaderId.ToString("000");
		    protraitTextureCache_ = null;
	    }
    }
}
