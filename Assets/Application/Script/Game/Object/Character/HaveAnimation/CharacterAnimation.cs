using Project.Lib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {

	//SDモーションタイプ
	public enum BattleMotion : int {
		Idle,               //待機
		AttackA,            //攻撃a
		AttackB,            //攻撃b
		Dead,				//死亡
		Damage,				//よろける
        Walk,				//歩く
        Run,				//走る

		Fall,				//落下
		Land,               //着地
		Salute,               //決めポーズ

		Max,
    }

    /// <summary>
    /// キャラ用アニメーション管理クラス
    /// </summary>
    public class CharacterAnimation : PlayableAnimation {
#if UNITY_EDITOR
		//エディタ用の動的クリップ追加場所
		public static Dictionary<string, AnimationClip> EditorAnimationDict = new Dictionary<string, AnimationClip>();
#endif
		AnimationCache<BattleMotion> battleMotion_ { get;  set; }
		Dictionary<string, List<UnityEngine.Object>> assets_ = new Dictionary<string, List<UnityEngine.Object>>();

		/// <summary>
		/// インスタンス生成
		/// </summary>
		public override void Create() {
			base.Create();
			//ちゃんと破棄がされてないんじゃないか疑惑をチェック
			Debug.Assert(assets_.Keys.Count == 0, "animation instance is not null");
			assets_ = new Dictionary<string, List<UnityEngine.Object>>();
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			foreach(var asset in assets_.Values) {
				AddressableAssist.UnLoadAssetGroup(asset);
			}
			assets_.Clear();
			battleMotion_ = null;
			base.Destroy();
		}


		/// <summary>
		/// アセットロード
		/// </summary>
		public async Task LoadAsync(string animLabel) {
			if (assets_.ContainsKey(animLabel))
				return;

			List<UnityEngine.Object> assets = await AddressableAssist.LoadAssetGroupAsync(animLabel);
			assets_[animLabel] = assets;
			AnimationClip[] clip = Array.ConvertAll(assets.ToArray(), c => (AnimationClip)c);
			battleMotion_ = new AnimationCache<BattleMotion>(clip);
			InitPlay(BattleMotion.Idle);

		}
#if UNITY_EDITOR
		/// <summary>
		/// エディタ用空設定
		/// </summary>
		public void Load() {
			battleMotion_ = new AnimationCache<BattleMotion>(new AnimationClip[] {});
		}
#endif
		/// <summary>
		/// 初期アニメーション再生
		/// </summary>
		/// <remarks>
		/// クロスフェードなしでアニメーションを最初から即座に再生
		/// </remarks>
		public void InitPlay(BattleMotion playMotion) {
			if (battleMotion_ == null)
				return;

			InitPlay(battleMotion_.GetClip((int)playMotion));
		}

		/// <summary>
		/// アニメーション再生
		/// </summary>
		public void Play(BattleMotion playMotion, float crossFadeTime = 0.25f) {
			if (battleMotion_ == null)
				return;
            Play(battleMotion_.GetClip((int)playMotion), crossFadeTime);
        }
        /// <summary>
        /// アニメーション再生中か
        /// </summary>
        public bool IsPlay(BattleMotion playMotion) {
	        if (battleMotion_ == null)
		        return false;
            return IsPlay(battleMotion_.GetClipName((int)playMotion));
        }

		/// <summary>
		/// アニメーション再生
		/// </summary>
		public void Play(string playMotion, float crossFadeTime = 0.25f) {
			Play(GetClip(playMotion), crossFadeTime);
		}

		private AnimationClip GetClip(string motionName)
		{
			AnimationClip clip = null;
			if (battleMotion_.IsExist(motionName))
			{
				clip = battleMotion_.GetClip(motionName);
			}
#if UNITY_EDITOR
			if (clip == null)
			{
				clip = EditorAnimationDict[motionName];
			}
#endif
			Debug.Assert(clip != null, "animation not found");
			return clip;
		}
#if UNITY_EDITOR
		/// <summary>
		/// エディタ用アニメーションを追加
		/// </summary>
		public static void AddClipDict(AnimationClip clip) {
			EditorAnimationDict[clip.name] = clip;
		}
#endif

	}
}
