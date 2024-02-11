using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using Project.Game;
using UnityEngine.SceneManagement;
using System.Threading;


#if UNITY_EDITOR

namespace Project.Editor {
	/// <summary>
	/// エディタメイン処理
	/// </summary>
	public class ActEventEditorMain : MonoBehaviour {
        //初期化完了時コールバック
        private System.Action onComplete_;
        //Entity集合クラス
        AssemlyManager assemblyManager_ = new AssemlyManager(
            new List<System.Func<Transform, CancellationToken, IEntityAssembly>>(){
                CameraAssembly.CreateInstance,
                CharacterAssembly.CreateInstance,
				EffectAssembly.CreateInstance,
				BulletAssembly.CreateInstance,
		   }
		);

        //本体のTransform
        protected Transform cacheTrans_;
        public Transform CacheTrans { get { return cacheTrans_; } }

		private UserOperation userOperation_;

		public static GameObject PrefabObject;

		CameraEntity mainCamera_;

        static Entity currentEntity;
        static bool pause_;


		public static bool IsActEventEditor() {
			return SceneManager.GetActiveScene().name == "ActEventEditor";

		}

		/// <summary>
		/// 操作するEntityを生成
		/// </summary>
		public static IEntityActEvent Create(System.Type type, GameObject entityObject) {
            if (entityObject == null)
                return null;

            Destroy();

            if(type == typeof(CharacterEntity)) {
                //Entity生成
                if (CharacterAssembly.I != null) {
                    currentEntity = Project.Game.CharacterAssembly.I.Create(entityObject);


                    return ((CharacterEntity)currentEntity).HaveAction.ActEvent;
                }

            }

            return null;
        }
        /// <summary>
        /// 操作しているEntityを破棄
        /// </summary>
        public static void Destroy() {
            if (currentEntity == null)
                return;

            //型に応じた破棄処理を実行
            if(currentEntity.GetType() == typeof(CharacterEntity)) {
                if (CharacterAssembly.I == null)
                    return;

                Project.Game.CharacterAssembly.I.Destroy((CharacterEntity)currentEntity);

            }

        }

        /// <summary>
        /// 指定の時間にseekしたときの処理
        /// </summary>
        public static void SyncTime(AnimationClip clip, float t) {
            if (currentEntity == null)
                return;

			//アニメーションの現在時間をセットする
			if (currentEntity.GetType() == typeof(CharacterEntity)) {
                PlayableAnimation playable = ((CharacterEntity)currentEntity).HaveAnimation;
                playable.Play(clip, 0f);
                playable.SetTime(t);

            }

        }
        /// <summary>
        /// 指定の時間にseekしたときの処理
        /// </summary>
        public static void Pause(bool flag) {
            pause_ = flag;
            if (currentEntity == null)
                return;
            //アニメーションの現在時間をセットする
            if (currentEntity.GetType() == typeof(CharacterEntity)) {
				CharacterEntity entity = ((CharacterEntity)currentEntity);

				PlayableAnimation playable = entity.HaveAnimation;
                if(flag){
					entity.HaveAction.ActEvent.Pause();
					playable.Pause();
                }else{
					entity.HaveAction.ActEvent.Resume();
					playable.Resume();
				}

			}

        }
        /// <summary>
        /// AnimationClipを管理下に追加
        /// </summary>
        public static void AddClip(System.Type type, AnimationClip clip) {
            if (type == typeof(CharacterEntity)) {
	            CharacterAnimation.AddClipDict(clip);
            }
        }
		/// <summary>
		/// カメラ設定のリセットをする
		/// </summary>
		public static void CameraReset(bool target, bool rot, bool len) {
			if (CameraAssembly.I == null)
				return;

			CameraEntity entity = CameraAssembly.I.Current[0];
			//カメラをリセット
			entity.ResetEditor(target, rot, len);

		}

		/// <summary>
		/// インスタンス生成
		/// </summary>
		private async void Awake() {
#if MANIPULATE_TIME
			Time.Setup();
#endif
			//システムに近いので早めに生成
			Gesture.Create();
			VirtualScreen.Create();
			MessageSystem.Create(new BattleMessageSetting());
			//初めにインスタンス作らないとメッセージをスルーされる
			userOperation_ = new UserOperation((int)MessageGroup.TouchControl);


			cacheTrans_ = transform;
			assemblyManager_.Create(cacheTrans_);

			Startup(() => { });
			mainCamera_ = await CameraAssembly.I.CreateAsync(ResourcesPath.VIEW_CAMERA_PREFAB, true);
			mainCamera_.ChangeControlEditorView(true);

			//デフォルト制御を登録
			userOperation_.SetDefault((int)Project.Game.OperationPriority.CameraEditorViewing);
		}

        /// <summary>
        /// インスタンス破棄
        /// </summary>
        private void OnDestroy() {
			assemblyManager_.Destroy();
			MessageSystem.Destroy();
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		private void Update() {
			userOperation_.Execute();
			assemblyManager_.Execute();
			MessageSystem.Execute();

			assemblyManager_.Evaluate();
		}
		/// <summary>
		/// 更新後処理
		/// </summary>
		private void LateUpdate() {
            assemblyManager_.LateExecute();
#if MANIPULATE_TIME
			Time.Execute();
#endif
		}

		/// <summary>
		/// 物理計算更新処理
		/// </summary>
		private void FixedUpdate() {
            //    character_.FixedExecute();
        }

        /// <summary>
        /// 初期化処理
        /// 初期化が終わったら m_OnComplete を呼ぶ
        /// </summary>
        public void Startup(System.Action onComplete) {
            onComplete_ = onComplete;


        }


        /// <summary>
        /// 初期化完了時に呼ばれる
        /// </summary>
        public void OnInitialized() {
            UnityUtil.Call(onComplete_);
            onComplete_ = null;
        }
    }

}
#endif

