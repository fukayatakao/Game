using UnityEngine;
using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {

    /// <summary>
    /// ライト
    /// </summary>
    public class LightEntity : Entity {
		/// <summary>
		/// インスタンス生成時処理
		/// </summary>
		public async override Task<GameObject> CreateAsync(string name) {
			GameObject obj = UnityUtil.Instantiate(ResourceCache.Load<GameObject>(name));
			//非同期の警告消し
			await Task.CompletedTask.ConfigureAwait(false);
			return obj;
		}
		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public override void Destroy() {
		}

    }
}

