using UnityEngine;

namespace Project.Server {
	public partial class DummyServer : MonoBehaviour {
		public static void Login() {
			GoodsTool.Init();
			PopTool.Init();
			UserDB.Load();
		}
	}


}
