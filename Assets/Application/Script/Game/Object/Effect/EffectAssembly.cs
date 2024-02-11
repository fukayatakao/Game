using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// エフェクトのインスタンス管理クラス
	/// </summary>
	public class EffectAssembly : EntityAssembly<EffectEntity, EffectAssembly> {
#if DEVELOP_BUILD
		const string EmptyEffect = "Effect/Empty";
		public bool IsEmpty;
#endif
		const string EFFECT_PATH = "Effect/";
		const string CAMERA_PATH = "Camera/";


		/// <summary>
		/// Entity生成
		/// </summary>
		/// <remarks>
		/// 非同期で呼び出しっぱなしにする
		/// </remarks>
		public async void CreateAsync(string resName, System.Action<EffectEntity> callback) {
			try {
#if DEVELOP_BUILD
				//エフェクト表示をoffにするときは空のエフェクトを作って返す
				if (IsEmpty) {
					EffectEntity entity = await CreateImplAsync(EmptyEffect, true, false);
					callback(entity);
					return;
				}
#endif
				{
					resName = EFFECT_PATH + resName;
					EffectEntity entity = await CreateImplAsync(resName, true, false);
					callback(entity);
				}
			} catch (System.OperationCanceledException e) {
				storage_.Clear();
				Debug.LogWarning($"{nameof(System.OperationCanceledException)} thrown with message: {e.Message}");
			}
		}
		/// <summary>
		/// Entity生成
		/// </summary>
		/// <remarks>
		/// 非同期で呼び出しっぱなしにする
		/// </remarks>
		public async void CreateCameraAsync(string resName, System.Action<EffectEntity> callback) {
			try {
	#if DEVELOP_BUILD
				//エフェクト表示をoffにするときは空のエフェクトを作って返す
				if (IsEmpty) {
					EffectEntity entity = await CreateImplAsync(EmptyEffect, true, false);
					callback(entity);
					return;
				}
	#endif
				{
					resName = CAMERA_PATH + resName;
					EffectEntity entity = await CreateImplAsync(resName, true, false);
					callback(entity);
				}
			} catch (System.OperationCanceledException e) {
				storage_.Clear();
				Debug.LogWarning($"{nameof(System.OperationCanceledException)} thrown with message: {e.Message}");
			}
		}


	}





}
