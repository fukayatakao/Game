using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Project.Game {
	/// <summary>
	/// フィールドのステージ
	/// </summary>
	public class FieldScene : MonoPretender {
		private SceneInstance scene;
		public Scene Scene { get { return scene.Scene; } }
		public GameObject[] RootObject;


		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			AddressableAssist.UnLoadSceneAsync(scene);
		}

		/// <summary>
		/// セットアップ
		/// </summary>
		public async Task LoadAsync(string stageName) {
			scene = await AddressableAssist.LoadSceneAsync(stageName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			RootObject = scene.Scene.GetRootGameObjects();
		}
	}
}
