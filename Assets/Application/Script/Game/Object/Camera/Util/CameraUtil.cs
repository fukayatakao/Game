using Project.Lib;
using UnityEngine;


namespace Project.Game {
	public static class CameraUtil {
		/// <summary>
		/// タッチ位置へのrayを飛ばしてステージとの当たりを取得する
		/// </summary>
		public static bool RaycastHit(CameraEntity cam, int layerMask, out RaycastHit hit) {
			Vector3 pos = cam.screenToWorld(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
			Vector3 cameraPos = cam.Camera.transform.position;

			bool result = Physics.Raycast(cameraPos, pos - cameraPos, out hit, Mathf.Infinity, layerMask);
			return result;
		}


		/// <summary>
		/// タッチ位置へのrayを飛ばしてステージとの当たりを取得する
		/// </summary>
		public static bool RaycastHit(Vector3 touchPos, CameraEntity cam, int layerMask, out RaycastHit hit) {
#if DEVELOP_BUILD
			//タッチした位置がデバッグウィンドウにかかっていて無効とされていたら
			if (!Gesture.IsValid) {
				hit = new RaycastHit();
				return false;
			}
#endif
			Vector3 pos = cam.screenToWorld(touchPos);
			Vector3 cameraPos = cam.Camera.transform.position;

			bool result = Physics.Raycast(cameraPos, pos - cameraPos, out hit, Mathf.Infinity, layerMask);
            return result;
        }
		/// <summary>
		/// タッチ位置へのrayを飛ばしてステージとの当たりを取得する
		/// </summary>
		public static RaycastHit[] RaycastHitAll(Vector3 touchPos, CameraEntity cam, int layerMask) {
#if DEVELOP_BUILD
			//タッチした位置がデバッグウィンドウにかかっていて無効とされていたら
			if (!Gesture.IsValid) {
				return new UnityEngine.RaycastHit[0];
			}
#endif
			Vector3 pos = cam.screenToWorld(touchPos);
			Vector3 cameraPos = cam.Camera.transform.position;

			return Physics.RaycastAll(cameraPos, pos - cameraPos, Mathf.Infinity, layerMask);
		}
	}
}
