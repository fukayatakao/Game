using System.Collections.Generic;
using UnityEngine;
using Project.Lib;

namespace Project.Game {
	public class TownGridMap {
		private const float OFFSET = 0.5f;
		private const float OFFSET_Y = 0.02f;
		private static readonly Color OffColor = new Color(0.5f, 1f, 0.5f, 0.5f);
		private static readonly Color OnColor = new Color(1f, 0.3f, 0.3f, 0.5f);

		private Transform root_;
		private int width_;
		private int depth_;
		private bool[] occupy_;

		public TownGridMap(int w, int d, Transform parent = null) {
			root_ = new GameObject("GridMap").transform;
			root_.SetParent(parent);
			//縦横ともに2の倍数でない場合はエラー出す
			Debug.Assert(w % 2 == 0, "width size is need even:" + w);
			Debug.Assert(d % 2 == 0, "width size is need even:" + d);
			width_ = w;
			depth_ = d;

			occupy_ = new bool[w * d];
#if DEVELOP_BUILD
			//メッセージ受容体作成
			MessageSystem.CreateReceptor(this, MessageGroup.DebugEvent);
#endif
		}

		/// <summary>
		/// 空地チェック
		/// </summary>
		public bool IsVacant(int cx, int cz, int w, int d) {
			(int min_x, int max_x, int min_z, int max_z) area = CreateArea(cx, cz, w, d);
			for (int i = area.min_x; i < area.max_x; i++) {
				for (int j = area.min_z; j < area.max_z; j++) {
					//一マスでも空いてない場所があるなら空いてない
					int index = GetIndex(i, j);
					if (occupy_[index])
						return false;
				}
			}
			//空いてた
			return true;
		}
		/// <summary>
		/// 空地塗りつぶし
		/// </summary>
		public void Fill((int cx, int cz, int w, int d) collision) {
			SetOccupy(collision.cx, collision.cz, collision.w, collision.d, true);
		}

		/// <summary>
		/// 空地塗りつぶし
		/// </summary>
		public void Fill(int cx, int cz, int w, int d) {
			SetOccupy(cx, cz, w, d, true);
		}
		/// <summary>
		/// クリアして空地にする
		/// </summary>
		public void Clear((int cx, int cz, int w, int d) collision) {
			SetOccupy(collision.cx, collision.cz, collision.w, collision.d, false);
		}
		/// <summary>
		/// クリアして空地にする
		/// </summary>
		public void Clear(int cx, int cz, int w, int d) {
			SetOccupy(cx, cz, w, d, false);
		}
		/// <summary>
		/// 占有管理
		/// </summary>
		private void SetOccupy(int cx, int cz, int w, int d, bool flag) {
			(int min_x, int max_x, int min_z, int max_z) area = CreateArea(cx, cz, w, d);
			for (int i = area.min_x; i < area.max_x; i++)
			{
				for (int j = area.min_z; j < area.max_z; j++)
				{
					int index = GetIndex(i, j);
					//塗りつぶす範囲がなんらかの原因で既に塗られてたらエラー出す
					Debug.Assert(occupy_[index] == !flag, "unknown flag error:" + index);
					occupy_[index] = flag;
				}
			}
		}


		int GetIndex(int x, int z)
		{
			return x + width_ * z;
		}

		(int, int, int, int) CreateArea(int cx, int cz, int w, int d)
		{
			int half_w = width_ / 2;
			int half_d = depth_ / 2;

			int min_x = Mathf.Clamp(cx + half_w - (w / 2), 0, width_ - 1);
			int max_x = Mathf.Clamp(cx + half_w + (w / 2), 0, width_ - 1);

			int min_z = Mathf.Clamp(cz + half_d - (d / 2), 0, depth_ - 1);
			int max_z = Mathf.Clamp(cz + half_d + (d / 2), 0, depth_ - 1);

			return (min_x, max_x, min_z, max_z);
		}

		public void Execute(TownMain townMain)
		{
			if (townMain.MainCamera == null)
				return;
			Vector3 pos = townMain.MainCamera.Camera.transform.position;
			Vector3 vec = townMain.MainCamera.Camera.transform.forward;

			float len = pos.y / vec.y;
			Vector3 target = new Vector3(pos.x - vec.x * len, 0f, pos.z - vec.z * len);
			Draw((int)target.x, (int)target.z, drawWidth_, drawDepth_);
		}

#if DEVELOP_BUILD

		//デバッグ描画用
		List<PrimitiveGrid> tiles_;
		private int maxTile_;
		private int drawWidth_;
		private int drawDepth_;

		/// <summary>
		/// 描画用インスタンスを作成する
		/// </summary>
		public void CreateDraw(int maxTile, int drawWidth, int drawDepth) {
			//表示タイルが表示範囲分用意されない場合は生成しない
			if (maxTile < drawWidth * drawDepth) {
				Debug.LogError("grid map draw setting error:" + maxTile);
				return;
			}

			//インスタンスが既にある場合
			if (tiles_ != null) {
				//設定値に変化がないなら無視してそのまま使う
				if (maxTile_ == maxTile && drawWidth_ == drawWidth && drawDepth_ == drawDepth)
					return;
				//設定値に変化があるようなので作り直す
				DestroyDraw();
			}


			maxTile_ = maxTile;
			drawWidth_ = drawWidth;
			drawDepth_ = drawDepth;

			tiles_ = new List<PrimitiveGrid>();
			for (int i = 0; i < maxTile; i++) {
				var tile = new PrimitiveGrid();
				tile.Create("tile" + i);
				tile.SetRotation(Quaternion.Euler(90f, 0f, 0f));
				tile.SetColor(OffColor);
				tile.SetParent(root_);
				tile.SetActive(false);
				tiles_.Add(tile);
			}
		}
		/// <summary>
		/// 描画用インスタンス破棄
		/// </summary>
		public void DestroyDraw() {
			if (tiles_ == null)
				return;
			for (int i = 0, max = tiles_.Count; i < max; i++) {
				GameObject.Destroy(tiles_[i].gameObject);
			}

			tiles_ = null;
		}

		/// <summary>
		/// 描画する
		/// </summary>
		private void Draw(int cx, int cz, int w, int d) {
			if (tiles_ == null)
				return;

			int half_w = width_ / 2;
			int half_d = depth_ / 2;


			(int min_x, int max_x, int min_z, int max_z) area = CreateArea(cx, cz, w, d);
			//一旦表示をすべてoffにする
			for (int i = 0, max = tiles_.Count; i < max; i++) {
				tiles_[i].SetActive(false);
			}
			//表示範囲のタイルを表示
			int count = 0;
			for (int i = area.min_x; i < area.max_x; i++) {
				for (int j = area.min_z; j < area.max_z; j++) {
					int index = GetIndex(i, j);
					tiles_[count].SetPosition(new Vector3(i - half_w + OFFSET, OFFSET_Y, j - half_d + OFFSET));
					tiles_[count].SetActive(true);
					tiles_[count].SetColor(occupy_[index] ? OnColor : OffColor);

					count++;
				}
			}
		}
#endif
	}
}
