using Project.Lib;

namespace Project.Game {
    /// <summary>
    /// 関係線管理
    /// </summary>
    public class LinkAssembly : EntityAssembly<Link, LinkAssembly> {

		/// <summary>
		/// Entity生成
		/// </summary>
		public Link Create(string resName, bool isStock = true, bool isReserve = false) {
			Link entity = base.CreateImplAsync(resName, isStock, isReserve).Result;
			return entity;
		}

	}
}
