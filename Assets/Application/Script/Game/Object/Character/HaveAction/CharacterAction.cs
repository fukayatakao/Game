using UnityEngine;
using System;
using System.Collections.Generic;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
	public enum BattleAction {
		OpeningSetup,
		OpeningEntry,
	}


	/// <summary>
	/// キャラクターアクション制御
	/// </summary>
	public class CharacterAction : MonoPretender {
		Dictionary<BattleAction, string> actionDict_ = new Dictionary<BattleAction, string>();

		List<UnityEngine.Object> assets_;
		//ActEvent処理クラス
		private EntityActEvent<CharacterEntity> act_;
#if UNITY_EDITOR
		public EntityActEvent<CharacterEntity> ActEvent{get {return act_; } }
#endif
		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
			act_ = new EntityActEvent<CharacterEntity>();
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			AddressableAssist.UnLoadAssetGroup(assets_);
			assets_ = null;
			act_ = null;
			base.Destroy();
		}

		/// <summary>
		/// アセットロード
		/// </summary>
		public async Task LoadAsync(string actionName) {
			if(assets_ != null)
				AddressableAssist.UnLoadAssetGroup(assets_);
			//アクションデータをタイプ名をキーにして登録
			assets_ = await AddressableAssist.LoadAssetGroupAsync(actionName);
			for (int i = 0, max = assets_.Count; i < max; i++) {
				act_.Add(assets_[i].name, (ActEventData)assets_[i]);

				//_で分割した文字列がenum登録されている場合はenum指定で再生出来るようにdictionaryに登録する
				string key = assets_[i].name.Split('_')[1];
				bool result = Enum.TryParse(typeof(BattleAction), key, out object type);
				if(result)
					actionDict_[(BattleAction)type] = assets_[i].name;
			}
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(CharacterEntity entity) {
			act_.Execute(entity);
		}
		/// <summary>
		/// アクション再生
		/// </summary>
		public void Play(BattleAction action) {
			act_.Play(actionDict_[action]);
		}

		/// <summary>
		/// アクション再生
		/// </summary>
		public void Play(string action) {
			act_.Play(action);
		}
		/// <summary>
		/// アクション再生中か
		/// </summary>
		public bool IsPlay() {
			return act_.IsPlay();
		}

		/// <summary>
		/// アクション停止
		/// </summary>
		public void Stop() {
			act_.Stop();
		}
		/// <summary>
		/// アクション停止
		/// </summary>
		public void Skip(CharacterEntity entity) {
			act_.Skip(entity);
		}
		/// <summary>
		/// キャンセル可能か
		/// </summary>
		public bool IsCancelable()
		{
			if (act_ == null)
				return true;
			float normal = act_.NormalizeTime();
			return normal <= 0 || normal >= 0.7f;
		}

	}
}
