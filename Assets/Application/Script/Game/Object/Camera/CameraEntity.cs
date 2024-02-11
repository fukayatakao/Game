using UnityEngine;
using System.Collections.Generic;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
    /// <summary>
    /// カメラ
    /// </summary>
	public class CameraEntity : Entity {
        //Unityのカメラ本体
        Camera camera_;
        public Camera Camera{get{ return camera_; } }

        //カメラ制御の共通設定値
        CameraControlSetting controlSetting_;
		public CameraControlSetting ControlSetting{ get{ return controlSetting_; } }
		//カメラ制御クラス
		ICameraControl control_;
		//無操作制御クラス
		NoneControl none_;
		//追随カメラ制御
		FollowControl follow_;
		//注視カメラ制御
		LockOnControl lockon_;
        //ビュー制御クラス
        ViewControl view_;
		//キャラ注視制御
		FollowViewControl followView_;
#if UNITY_EDITOR
		//エディタ用ビュー制御クラス
		EditorViewControl editorView_;
		public EditorViewControl EditorView { get { return editorView_; } }
#endif
		const float DefaultLength = 6f;
        const float DefaultEvaluationDeg = 20f;

		Transform manualTarget_;
		private List<Cinemachine.CinemachineVirtualCamera> virtualCamera_ = new List<Cinemachine.CinemachineVirtualCamera>();
		public List<Cinemachine.CinemachineVirtualCamera> VirtualCamera{get{ return virtualCamera_; }}

		public Quaternion Azimth{ get { return MathUtil.RotateEulerY(controlSetting_.AzimthAngle); } }


		public enum Control{
			Follow,
			LockOn,

		}

		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string name) {
			GameObject obj = UnityUtil.Instantiate(ResourceCache.Load<GameObject>(name, false));

			SetupCamera(obj);

			//カメラ制御で共通の設定値
			controlSetting_ = MonoPretender.Create<CameraControlSetting>(obj);
			controlSetting_.Init(DefaultLength, DefaultEvaluationDeg);
			//制御クラスの生成
			none_ = MonoPretender.Create<NoneControl>(obj);
			follow_ = MonoPretender.Create<FollowControl>(obj);
			lockon_ = MonoPretender.Create<LockOnControl>(obj);
			view_ = MonoPretender.Create<ViewControl>(obj);
			followView_ = MonoPretender.Create<FollowViewControl>(obj);
#if UNITY_EDITOR
			editorView_ = MonoPretender.Create<EditorViewControl>(obj);
#endif
			//デフォルト制御を設定
			control_ = none_;
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);

			return obj;
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
			MonoPretender.Destroy(controlSetting_);
			MonoPretender.Destroy(none_);
			MonoPretender.Destroy(follow_);
			MonoPretender.Destroy(lockon_);
			MonoPretender.Destroy(view_);
			MonoPretender.Destroy(followView_);
#if UNITY_EDITOR
			MonoPretender.Destroy(editorView_);
#endif
		}

		/// <summary>
		/// カメラの本隊と手動操作用のセットアップ
		/// </summary>
		private void SetupCamera(GameObject own) {
			//カメラの設定を取得
			CameraControlTarget ctrl = own.GetComponent<CameraControlTarget>();
			if (ctrl != null) {
				manualTarget_ = ctrl.ManualCamera.transform;
				camera_ = ctrl.AuthenticCamera.GetComponent<Camera>();
				virtualCamera_.Add(ctrl.ManualCamera);
				virtualCamera_.AddRange(ctrl.VirtualCameraList);
			} else {
				manualTarget_ = own.transform;
				camera_ = own.GetComponent<Camera>();
			}
		}

		/// <summary>
		/// Entityの生存判定
		/// </summary>
		public override bool IsExist() {
			return true;
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public override void Execute(){
		}
        /// <summary>
        /// 更新後処理
        /// </summary>
        /// <remarks>
        /// キャラクターの移動処理が終わってから実行する
        /// </remarks>
		public override void LateExecute(){
            CalculateControl();
        }

		/// <summary>
		/// スクリーン座標を2D座標に変換
		/// </summary>
		public Vector3 screenToWorld(Vector3 pos) {
            return camera_.ScreenToWorldPoint(new Vector3(pos.x, pos.y, controlSetting_.Length));
        }

		/// <summary>
		/// 特定の座標を注視する
		/// </summary>
		public void LookAtView(Vector3 target) {
			view_.SetTarget(target);
		}

		/// <summary>
		/// 注視する座標の範囲を指定
		/// </summary>
		public void LimitTargetView(float min_x, float max_x, float min_z, float max_z) {
			view_.SetTargetLimit(min_x, max_x, min_z, max_z);
		}

		/// <summary>
		/// カメラ位置を即更新
		/// </summary>
		private void CalculateControl() {
            control_.Execute(controlSetting_);
			manualTarget_.localPosition = control_.Position;
			manualTarget_.localRotation = control_.Rotation;
        }

        /// <summary>
        /// 無操作に変更
        /// </summary>
        public void ChangeControlNone() {
            none_.Change(control_);
            control_ = none_;
        }

        /// <summary>
        /// 追随制御に変更
        /// </summary>
        public void ChangeControlFollow(Transform target, bool immediate = false){
			follow_.SetTarget (target);
            control_ = follow_;

            if(immediate){
                CalculateControl();
            }
        }

        /// <summary>
        /// ロックオン制御に変更
        /// </summary>
        public void ChangeControlLockOn(Transform player, Transform target, bool immediate = false) {
			lockon_.SetTarget (player, target);
            control_ = lockon_;

            if (immediate) {
                CalculateControl();
            }

        }
        /// <summary>
        /// ビュー制御に変更
        /// </summary>
        public void ChangeControlView(bool immediate = false) {
            control_ = view_;

            if (immediate) {
                CalculateControl();
            }
        }

		/// <summary>
		/// キャラ注視制御に変更
		/// </summary>
		public void ChangeControlFollowView(Transform target, bool immediate = false) {
			followView_.SetTarget(target);
			control_ = followView_;

			if (immediate) {
				CalculateControl();
			}
		}

		public void ChangeVirtualCamera(int index) {
			//今使っているカメラをoff
			virtualCamera_[virtualCameraIndex_].gameObject.SetActive(false);

			virtualCameraIndex_ = index;
			virtualCamera_[virtualCameraIndex_].gameObject.SetActive(true);


			if (virtualCameraIndex_ == 0) {
				ChangeControlView();
			} else {
				ChangeControlNone();
			}
		}

		/// <summary>
		/// ひとつづつカメラを切り替える
		/// </summary>
		int virtualCameraIndex_ = 0;
		public void ChangeVirtualCamera() {
			if (virtualCamera_ == null)
				return;
			//今使っているカメラをoff
			virtualCamera_[virtualCameraIndex_].gameObject.SetActive(false);

			virtualCameraIndex_++;
			if(virtualCameraIndex_ >= virtualCamera_.Count) {
				virtualCameraIndex_ = 0;
			}

			virtualCamera_[virtualCameraIndex_].gameObject.SetActive(true);

			if (virtualCameraIndex_ == 0) {
				ChangeControlView();
			} else {
				ChangeControlNone();
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// エディタ用ビュー制御に変更
		/// </summary>
		public void ChangeControlEditorView(bool immediate = false) {
			controlSetting_.SetLengthMinMax(0f, float.MaxValue);
			controlSetting_.SetAzimthMinMax(float.MinValue, float.MaxValue);
			controlSetting_.SetElevationMinMax(-90f, 90f);
			editorView_.Reset(controlSetting_);

			control_ = editorView_;

			if (immediate) {
				CalculateControl();
			}
		}

		public void ResetEditor(bool isTarget, bool isRot, bool isLen) {
			editorView_.Reset(controlSetting_, isTarget, isRot, isLen);
		}

#endif
	}

}
