using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using Project.Lib;



public class AddressableProfile : MonoBehaviour {
	UnityEngine.Object asset;
	List<UnityEngine.Object> assets;
	Dictionary<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation, int> dict = new Dictionary<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation, int>();
	public List<string> remains = new List<string>();

	private async void Start() {
		// DiagnosticCallbackを登録
		Addressables.ResourceManager.RegisterDiagnosticCallback(DiagnosticCallback);
		// アセットを読み込む
		var handle = Addressables.LoadAssetAsync<UnityEngine.Object>("Character/AI/edit");
		await handle.Task;

		Addressables.Release(handle);

		// DiagnosticCallbackを登録解除
		//Addressables.ResourceManager.UnregisterDiagnosticCallback(DiagnosticCallback);
	}

	private async void Load() {
		asset = await AddressableAssist.LoadAssetAsync("Character/AI/edit");
	}
	private async void Load2() {
		assets = await AddressableAssist.LoadAssetGroupAsync("Action-Normal");

		//Addressables.ResourceManager.ProvideResources<UnityEngine.Object>(new List<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>(), false);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			Load();
		}
		if (Input.GetKeyDown(KeyCode.Q)) {
			AddressableAssist.UnLoadAsset(asset);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			Load2();
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			AddressableAssist.UnLoadAssetGroup(assets);
		}
	}


	private void DiagnosticCallback(ResourceManager.DiagnosticEventContext context) {
		// 参照カウントの変更通知だけログ出力する
		if (context.Type == ResourceManager.DiagnosticEventType.AsyncOperationReferenceCount) {
			Debug.Log(
				"Reference count changed" +
				$"{Environment.NewLine}Location: {context.Location}" +
				$"{Environment.NewLine}Reference count: {context.EventValue}");
			if (context.Location == null)
				return;
			dict[context.Location] = context.EventValue;
			if (context.EventValue == 0)
				dict.Remove(context.Location);
			remains.Clear();
			foreach (var k in dict.Keys) {
				remains.Add(k.PrimaryKey);
			}
		}
	}
}
