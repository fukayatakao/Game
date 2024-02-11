using UnityEngine;
using Project.Lib;

namespace Project.Game {
#if DEVELOP_BUILD
    /// <summary>
    /// キーボードを使ったデバッグ操作
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class BattleDebugKeyControl : MonoBehaviour{
        //ゲームメイン
        BattleMain main_;

		/// <summary>
		/// 生成処理
		/// </summary>
		public static void Create(BattleMain main) {
			GameObject obj = new GameObject("Debug");
			//デバッグ用キー操作
			obj.AddComponent<BattleDebugKeyControl>().Init(main);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(BattleMain main) {
            main_ = main;
        }
		/// <summary>
		/// 実行処理
		/// </summary>
		private void Update() {
			//終了処理テスト
			if (Input.GetKeyDown(KeyCode.Escape)) {
				switch (BattleMain.PrevScene) {
					//バトルから直接開始していたらバトルをループする
					case SceneTransition.SceneType.None:
					case SceneTransition.SceneType.Battle:
						SceneTransition.ChangeBattle();
						break;
					//タウンから来たらタウンに戻る
					case SceneTransition.SceneType.Town:
						SceneTransition.ChangeTown();
						break;
					//それ以外は戦略マップに移動
					default:
						int invaderId = BattleSituation.I.invader.id;
						int defenderId = BattleSituation.I.defender.id;
						int nodeId = BattleSituation.I.nodeId;
						SceneTransition.ChangeStrategyWin(invaderId, defenderId, nodeId);
						break;
				}
			}

			//キャラクター無敵切り替え
			/*if (Input.GetKey(KeyCode.LeftControl)) {
				CharacterEntity entity = ClickCharacter();
				if (entity != null) {
					//無敵フラグを反転
					bool flag = !entity.HaveUnitParam.Fight.isInfinityHp;
					entity.HaveUnitParam.Fight.isInfinityHp = flag;
					entity.HaveUnitParam.Fight.isInfinityLp = flag;

					if (flag) {
						entity.HaveDebugFloatUI.Show();
					} else {
						entity.HaveDebugFloatUI.Hide();
					}
				}
			}*/
#if MANIPULATE_TIME
			Time.DebugKey();
#endif
		}

		/// <summary>
		/// キャラクリックを検出
		/// </summary>
		private CharacterEntity ClickCharacter() {
			if (!Gesture.IsTouchDown(0))
				return null;
			//キャラクターを選択したら操作開始
			RaycastHit hit;
			bool result = CameraUtil.RaycastHit(Gesture.GetTouchPos(0), main_.MainCamera, 1 << (int)UnityLayer.Layer.Character, out hit);
			if (!result)
				return null;

			return hit.collider.GetComponent<CharacterPortal>().Owner;

		}
	}
#endif
		}
