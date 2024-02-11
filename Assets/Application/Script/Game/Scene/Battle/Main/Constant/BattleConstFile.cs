
[System.Serializable]
public class BattleConstFile {
	//HPの基本ダメージ量
	public int BASE_DAMGE;
	//HPの基本ダメージ量
	public int STANDARD_RANGE;
	//HPダメージ->LPダメージ量の基本倍率
	public float LP_DAMAGE_RATIO;
	//貫通>装甲によるダメージ量の基本倍率
	public float DIRECT_DAMAGE_RATIO;
	//密集したときの1sあたりのHPペナルティ
	public float HP_LOSS_PENALTY;
	//静摩擦力
	public float STATIC_FRICTION;
	//吹き飛び時間->時間は固定。吹き飛び距離はキャラごとに決まる。
	public float KNOCKBACK_TIME;
	//吹き飛びする蓄積量
	public int KNOCKBACK_ACCUMULATE_LIMIT;
	//熟練度倍率補正
	public int EXPERIENCE_SCALE;
	//分隊の展開距離
	public float DEPLOY_FORWARD_LINE;
	public float DEPLOY_FORWARD_DETECT;
	public float DEPLOY_BACKWARD_LINE;
	public float DEPLOY_BACKWARD_DETECT;

	//貯められるSPの最大値
	public int SPECIAL_POINT_MAX;
	//スキル1回で消費するSP
	public int SPECIAL_POINT_CONSUME;


	//歩きから走りに変わる速度割合
	public float CRUISE_RATE;
    //0とみなす極小値
    public float EPSILON;


	//特定の位置にいるとみなすときの許容誤差距離(2乗)
	public float ARRIVAL_DISTANCE;
	//進行方向と現在の向きがなす角Θがこの値以下のときは同時に移動も可能(cos(Θ/2))
	public float MOVABLE_ANGLE;


	//指示から行動までの最大遅延時間
	public float ORDER_DELAY_TIME;
	//遠方へ移動させるときの距離値
	public float LONG_DISTANCE;
	//列交代に必要な距離
	public float ENABLE_SWAP_DISTANCE;
	//指定地点移動の指定可能な距離
	public float GROUND_TARGET_DISTANCE;

	//ラインのブレの範囲
	public float LINE_DISTORTION_DEPTH;
	//距離が十分近い場合に味方が壁になって攻撃出来ない状態は無視する
	public float IGNORE_FRIENDLY_ATTACK_DISTANCE;
	//回復量が最大に達する前線との距離
	public float FULL_RECOVERY_DISTANCE;
	//リーダー一人が一般ユニットの何倍の戦力値になるか
	public int LEADER_NUMBER_FOLD;
	//小隊の戦力がこの割合よりも下回ったらLP漸減が始まる
	public float DECAY_START_RANGE;
	//LP漸減の量(/s)
	public float DECAY_LP_DAMAGE;

	//属性時間帯の持続時間
	public float PHASE_DURATION;

}
