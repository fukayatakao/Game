using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// ライトのインスタンス管理クラス
	/// </summary>
	public class LightAssembly : EntityAssembly<LightEntity, LightAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<LightEntity> CreateAsync(string resName, bool isImmediate = false, bool isReserve = false) {
			//本体を生成
			return await CreateImplAsync(resName, isImmediate, isReserve);
		}
	}

}
