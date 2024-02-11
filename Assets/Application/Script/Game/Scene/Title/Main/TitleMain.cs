using UnityEngine;


namespace Project.Game {
	/// <summary>
	/// タイトルシーンのメイン処理
	/// </summary>
	public class TitleMain : MonoBehaviour {
		public void Awake() {
			EntryPoint.Boot();
			EntryPoint.Initialize();
#if DEVELOP_BUILD
			TitleAlternativeMenu.Open(this);
#endif
		}

		public void Update() {
		}

		public void OnDestroy() {
#if DEVELOP_BUILD
			TitleAlternativeMenu.Close();
#endif
		}
	}

}
