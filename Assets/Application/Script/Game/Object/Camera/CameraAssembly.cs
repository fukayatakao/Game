using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// カメラ管理
	/// </summary>
	public class CameraAssembly : EntityAssembly<CameraEntity, CameraAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<CameraEntity> CreateAsync(string resName, bool isImmediate = false, bool isReserve = false) {
			//本体を生成
			return await CreateImplAsync(resName, isImmediate, isReserve);
		}
	}
}
