using System.Collections.Generic;
using UnityEngine;


namespace Project.Game {
	public class CameraControlTarget : MonoBehaviour{
		//本物のカメラ
		public Camera AuthenticCamera;
		//マニュアル操作するときのカメラ
		public Cinemachine.CinemachineVirtualCamera ManualCamera;


		//仮想カメラリスト
		public List<Cinemachine.CinemachineVirtualCamera> VirtualCameraList;
	}
}
