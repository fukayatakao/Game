using Project.Lib;
using Project.Mst;
using Project.Network;
using UnityEngine;

namespace Project.Game {
#if DEVELOP_BUILD
	public class DebugRootMenu : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.2f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;

		//シングルトンインスタンス
		static DebugRootMenu instance_;
		public static DebugRootMenu I { get { return instance_; } }

		DebugMenuDrawer drawer = new DebugMenuDrawer(
			DebugMenu.Item("root",
				DebugMenu.Item("システム",
					DebugMenu.Item("シーン",
						DebugMenu.Item("バトル", () => {
							SceneTransition.ChangeBattle();
						}),
						DebugMenu.Item("タウン", () => {
							DebugWindowManager.SetActive(false);

							BaseDataManager.Refresh(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("town"); });
							TownMainCmd.CreateAsync(new TownMainRequest(), (response) => { TownMain.TransitionData = (response as TownMainResponse); });
						}),
						DebugMenu.Item("編成", () => {
							SceneTransition.ChangeOrganize(SceneTransition.SceneType.Town);
						})
					),
					DebugMenu.Item("エフェクト", typeof(EffectPlayTest)),
					DebugMenu.Item("操作情報", typeof(OperationInfo)),
					DebugMenu.Item("パフォーマンス", typeof(PerformanceWindow)),
					DebugMenu.Item("ビルド情報", typeof(BuildInfoWindow)),
					DebugMenu.Item("キャッシュ管理",
						DebugMenu.Item("マスター更新", () => { BaseDataManager.Refresh(() => { Debug.Log("update finish"); }); }),
						DebugMenu.Item("マスター削除", () => { BaseDataManager.ClearCache(); }),
						DebugMenu.Item("アセット削除", () => { Caching.ClearCache(); })
					)
				)
			)
		);

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DebugRootMenu() {
			Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();

			instance_ = this;
		}
		/// <summary>
		/// メニュー描画本体
		/// </summary>
		protected override void Draw(int id) {
			drawer.Draw();
		}

		/// <summary>
		/// サブメニューを追加
		/// </summary>
		public void Add(DebugMenu menu) {
			drawer.Add(menu);
		}

		/// <summary>
		/// サブメニューを削除
		/// </summary>
		public void Remove(DebugMenu menu) {
			drawer.Remove(menu);
		}

	}
#endif
}
