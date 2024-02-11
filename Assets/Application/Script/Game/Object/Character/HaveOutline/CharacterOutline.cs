using Project.Lib;
using UnityEngine;

namespace Project.Game {
	/// <summary>
	/// キャラクターのアウトライン表示
	/// </summary>
	public class CharacterOutline : MonoPretender {
		private Outline[] outline_;
		private Color color_;
		/// <summary>
		/// インスタンス生成
		/// </summary>
		protected override void Create(GameObject obj) {
			base.Create(obj);
			outline_ = obj.GetComponentsInChildren<Outline>();
			color_ = Color.red;
			Debug.Assert(outline_ != null, "not found outline:" + obj.name);
		}


		public void SetTeamColor(Power power) {
			if(power == Power.Player) {
				color_ = new Color(137 / 255f, 137 / 255f, 255 / 255f);
			} else {
				color_ = new Color(255 / 255f, 132 / 255f, 132 / 255f);
			}

			for (int i = 0, max = outline_.Length; i < max; i++) {
				outline_[i].OutlineColor = color_;
			}
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		protected override void Destroy() {
			base.Destroy();
		}

		public void Select() {
			for (int i = 0, max = outline_.Length; i < max; i++) {
				outline_[i].OutlineWidth = 8;
				outline_[i].OutlineColor = Color.red;
			}
		}

		public void UnSelect() {
			for (int i = 0, max = outline_.Length; i < max; i++) {
				outline_[i].OutlineWidth = 2;
				outline_[i].OutlineColor = color_;
			}
		}

		public void SelectGroup() {
			for (int i = 0, max = outline_.Length; i < max; i++) {
				outline_[i].OutlineWidth = 4;
				outline_[i].OutlineColor = Color.blue;
			}
		}

	}
}
