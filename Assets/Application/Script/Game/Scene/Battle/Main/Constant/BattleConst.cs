using UnityEngine;
using Project.Lib;

//・設定ファイルで定数を変化させたい
//・定数なので外部から変更させない
//・getのオーバヘッドも嫌
//->readonly変数にしてコンストラクタでファイルをロードして値をセットする
/// <summary>
/// 定数
/// </summary>
public class BattleConst {
	//HPの基本ダメージ量
	public readonly int BASE_DAMAGE;
	//標準ダメージ幅
	public readonly int STANDARD_RANGE;
	//HPダメージ->LPダメージ量の基本倍率
	public readonly float LP_DAMAGE_RATIO;
	//貫通>装甲によるダメージ量の基本倍率
	public readonly float DIRECT_DAMAGE_RATIO;
	//密集したときの1sあたりのHPペナルティ
	public readonly float HP_LOSS_PENALTY;

	//静摩擦力
	public readonly float STATIC_FRICTION;
	//吹き飛び時間->時間は固定。吹き飛び距離はキャラごとに決まる。
	public readonly float KNOCKBACK_TIME;
	//吹き飛びする蓄積量
	public readonly int KNOCKBACK_ACCUMULATE_LIMIT;
	//熟練度倍率補正
	public readonly int EXPERIENCE_SCALE;

	//貯められるSPの最大値
	public readonly int SPECIAL_POINT_MAX;
	//スキル1回で消費するSP
	public readonly int SPECIAL_POINT_CONSUME;

	//歩きから走りに変わる速度割合
	public readonly float CRUISE_RATE;
    //0とみなす極小値
    public readonly float EPSILON;


	//特定の位置にいるとみなすときの許容誤差距離(2乗)
	public readonly float ARRIVAL_DISTANCE;
	public readonly float ARRIVAL_DISTANCE_SQ;
    //進行方向と現在の向きがなす角Θがこの値以下のときは同時に移動も可能(cos(Θ/2))
    public readonly float MOVABLE_ANGLE_COS;

	//分隊の展開距離
	public readonly float DEPLOY_FORWARD_DETECT;
	public readonly float DEPLOY_FORWARD_LINE;
    public readonly float DEPLOY_BACKWARD_LINE;
    public readonly float DEPLOY_BACKWARD_DETECT;

	//指示から行動までの最大遅延時間
	public readonly float ORDER_DELAY_TIME;
	//遠方へ移動させるときの距離値
	public readonly float LONG_DISTANCE;
	//列交代に必要な距離
	public readonly float ENABLE_SWAP_DISTANCE;
	//指定地点移動の指定可能な距離
	public readonly float GROUND_TARGET_DISTANCE;

	//ラインのブレの範囲
	public readonly float LINE_DISTORTION_DEPTH;
	//距離が十分近い場合に味方が壁になって攻撃出来ない状態は無視する
	public readonly float IGNORE_FRIENDLY_ATTACK_DISTANCE;
	//回復量が最大に達する前線との距離
	public readonly float FULL_RECOVERY_DISTANCE;
	//リーダー一人が一般ユニットの何倍の戦力値になるか
	public readonly int LEADER_NUMBER_FOLD;
	//小隊の戦力がこの割合よりも下回ったらLP漸減が始まる
	public readonly float DECAY_START_RANGE;
	//LP漸減の量(/s)
	public readonly float DECAY_LP_DAMAGE;

	//属性時間帯の持続時間
	public readonly float PHASE_DURATION;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BattleConst()
	{
		BattleConstFile file = Load();
		//全部初期値だと0除算とか発生して正常に動かない。
		//エディタなどのツールで正常に動かすためだけのコンストラクタ
		BASE_DAMAGE = file.BASE_DAMGE;
		STANDARD_RANGE = file.STANDARD_RANGE;
		LP_DAMAGE_RATIO = file.LP_DAMAGE_RATIO;
		DIRECT_DAMAGE_RATIO = file.DIRECT_DAMAGE_RATIO;
		HP_LOSS_PENALTY = file.HP_LOSS_PENALTY;

		STATIC_FRICTION = file.STATIC_FRICTION;
		KNOCKBACK_TIME = file.KNOCKBACK_TIME;
		KNOCKBACK_ACCUMULATE_LIMIT = file.KNOCKBACK_ACCUMULATE_LIMIT;

		EXPERIENCE_SCALE = file.EXPERIENCE_SCALE;

		SPECIAL_POINT_MAX = file.SPECIAL_POINT_MAX;
		SPECIAL_POINT_CONSUME = file.SPECIAL_POINT_CONSUME;

		CRUISE_RATE = file.CRUISE_RATE;
		EPSILON = file.EPSILON;

		ARRIVAL_DISTANCE = file.ARRIVAL_DISTANCE;
		ARRIVAL_DISTANCE_SQ = ARRIVAL_DISTANCE * ARRIVAL_DISTANCE;
		MOVABLE_ANGLE_COS = Mathf.Cos(file.MOVABLE_ANGLE * Mathf.Deg2Rad);

		DEPLOY_FORWARD_DETECT = file.DEPLOY_FORWARD_DETECT;
		DEPLOY_FORWARD_LINE = file.DEPLOY_FORWARD_LINE;
		DEPLOY_BACKWARD_LINE = file.DEPLOY_BACKWARD_LINE;
		DEPLOY_BACKWARD_DETECT = file.DEPLOY_BACKWARD_DETECT;

		ORDER_DELAY_TIME = file.ORDER_DELAY_TIME;
		LONG_DISTANCE = file.LONG_DISTANCE;
		ENABLE_SWAP_DISTANCE = file.ENABLE_SWAP_DISTANCE;
		GROUND_TARGET_DISTANCE = file.GROUND_TARGET_DISTANCE;

		LINE_DISTORTION_DEPTH = file.LINE_DISTORTION_DEPTH;
		IGNORE_FRIENDLY_ATTACK_DISTANCE = file.IGNORE_FRIENDLY_ATTACK_DISTANCE;
		FULL_RECOVERY_DISTANCE = file.FULL_RECOVERY_DISTANCE;
		LEADER_NUMBER_FOLD = file.LEADER_NUMBER_FOLD;
		DECAY_START_RANGE = file.DECAY_START_RANGE;
		DECAY_LP_DAMAGE = file.DECAY_LP_DAMAGE;

		PHASE_DURATION = file.PHASE_DURATION;
	}

	/// <summary>
	/// 設定をロード
	/// </summary>
	public static BattleConstFile Load() {
		var json = Resources.Load("BattleConst") as TextAsset;
		return JsonUtility.FromJson<BattleConstFile>(json.text);
	}
}

// ------------------------------------------------
// 陣営
public enum Power : int {
	[Field("味方")] Player,
	[Field("敵")] Enemy,
	[Field("最大数")] Max,
}
//敵味方識別
public enum IFF : int {
	[Field("自分")] FRIEND,
	[Field("相手")] FOE,
	[Field("最大数")] Max,
}
//列の位置
public enum Abreast : int {
	[Field("前列")] First,
	[Field("中列")] Second,
	[Field("後列")] Third,
	[Field("最大数")] MAX,
}

//状態
public enum Condition : int {
	[Field("なし")] None,
	[Field("与ダメージup")] AttackUp,
	[Field("与ダメージdown")] AttackDown,
	[Field("被ダメージup")] DamageUp,
	[Field("被ダメージdown")] DamageDown,
	[Field("ふっとび")] KnockBack,

	[Field("眠り")] Sleep,
	[Field("混乱")] Confuse,
	[Field("マヒ")] Paralysis,
	[Field("どく")] Poison,
	[Field("おびえ")] Fear,

	[Field("最大数")] MAX,
}
