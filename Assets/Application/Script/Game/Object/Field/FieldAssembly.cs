using Project.Lib;
using System.Threading.Tasks;

namespace Project.Game {
	/// <summary>
	/// フィールドのインスタンス管理クラス
	/// </summary>
	public class FieldAssembly : EntityAssembly<FieldEntity, FieldAssembly> {
		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<FieldEntity> CreateAsync(string stage, string sky, bool isImmediate = false, bool isReserve = false) {
			//フィールド生成
			FieldEntity entity = await CreateImplAsync("FieldEntity", isImmediate, isReserve);
			await entity.Load(stage, sky);

			return entity;
		}
		/// <summary>
		/// Strategyシーン用の経路情報付きフィールド
		/// </summary>
		public async Task<FieldEntity> CreateAsync(string stage, string sky, string waymap, bool isImmediate = false, bool isReserve = false) {
			//フィールド生成
			FieldEntity entity = await CreateImplAsync("FieldEntity", isImmediate, isReserve);
			await entity.Load(stage, sky, waymap);

			return entity;
		}
	}
}
