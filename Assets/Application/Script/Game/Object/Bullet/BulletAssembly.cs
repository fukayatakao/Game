using Project.Lib;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// エフェクトのインスタンス管理クラス
	/// </summary>
	public class BulletAssembly : EntityAssembly<BulletEntity, BulletAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		/// <remarks>
		/// インスタンスが使いまわせるようなら使いまわす
		/// クラス外から呼び出す用
		/// </remarks>
		public async Task<BulletEntity> CreateAsync(string resName, bool isImmediate = true, bool isReserve = false) {
			return await CreateImplAsync(resName, isImmediate, isReserve);
		}

		/// <summary>
		/// Entity生成
		/// </summary>
		/// <remarks>
		/// 非同期で呼び出しっぱなしにする
		/// </remarks>
		public async void CreateAsync(string resName, System.Action<BulletEntity> callback) {
			try {
				BulletEntity entity = await CreateImplAsync(resName, true, false);
				token_.ThrowIfCancellationRequested();
				callback(entity);
			} catch (System.OperationCanceledException e) {
				storage_.Clear();
				Debug.LogWarning($"{nameof(System.OperationCanceledException)} thrown with message: {e.Message}");
			}
		}

	}





}
