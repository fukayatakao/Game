using System.Collections.Generic;
using Project.Lib;

namespace Project.Game {
	public class ChainLinkMap {
		Dictionary<Facility, Dictionary<Facility, Link>> links_ = new Dictionary<Facility, Dictionary<Facility, Link>>();
		public enum Arrow {
			RED,
			GREEN,
			BLUE,
		}
		/// <summary>
		/// リンク表示を更新
		/// </summary>
		public void InitLink(IEnumerable<Chain> sendList, IEnumerable<Chain> recieveList, Arrow arrow) {
			foreach (Chain chain in sendList) {
				if (!chain.Valid)
					continue;
				CreateLink(chain, arrow);
			}
			foreach (Chain chain in recieveList) {
				if (!chain.Valid)
					continue;
				CreateLink(chain, arrow);
			}
		}

		/// <summary>
		/// リンク表示を更新
		/// </summary>
		public void Linking(IReadOnlyCollection<Chain> includeList, IReadOnlyCollection<Chain> excludeList, Arrow arrow) {
			if (excludeList != null) {
				foreach (Chain chain in excludeList) {
					if (!chain.Valid)
						continue;
					DestroyLink(chain);
				}
			}
			if (includeList != null) {
				foreach (Chain chain in includeList) {
					if (!chain.Valid)
						continue;
					CreateLink(chain, arrow);
				}
			}
		}
		/// <summary>
		/// Linkを生成
		/// </summary>
		private Link CreateLink(Chain chain, Arrow arrow) {
			string asset = GetArrowAssetName(arrow);
			Link link = LinkAssembly.I.Create(asset);
			link.Setup(chain.Sender, chain.Receiver);
			if (!links_.ContainsKey(chain.Sender))
				links_[chain.Sender] = new Dictionary<Facility, Link>();
			links_[chain.Sender][chain.Receiver] = link;

			return link;
		}
		/// <summary>
		/// Linkを破棄
		/// </summary>
		private void DestroyLink(Chain key) {
			LinkAssembly.I.Destroy(links_[key.Sender][key.Receiver]);
			links_[key.Sender].Remove(key.Receiver);
			if (links_[key.Sender].Count == 0)
				links_.Remove(key.Sender);

		}
		/// <summary>
		/// Linkを破棄
		/// </summary>
		public void Clear() {
			foreach (Dictionary<Facility, Link> linkDict in links_.Values) {
				foreach (Link link in linkDict.Values) {
					LinkAssembly.I.Destroy(link);
				}
			}
			links_.Clear();
		}


		/// <summary>
		/// 矢印のタイプから対応するアセット名を取得
		/// </summary>
		private string GetArrowAssetName(Arrow type) {
			switch (type) {
			case Arrow.RED:
			default:
				return ResourcesPath.LINK_ARROW_RED;
			case Arrow.GREEN:
				return ResourcesPath.LINK_ARROW_GREEN;
			case Arrow.BLUE:
				return ResourcesPath.LINK_ARROW_BLUE;


			}
		}

	}
}
