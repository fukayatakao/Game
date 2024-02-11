using UnityEngine;
using Project.Lib;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Game {

	/// <summary>
	/// 小隊管理
	/// </summary>
	public class PlatoonAssembly : EntityAssembly<PlatoonEntity, PlatoonAssembly> {
		/// <summary>
		/// セットアップ
		/// </summary>
		public override void Setup(Transform root, string name) {
			base.Setup(root, name);
			BehaviorTree<PlatoonEntity>.Initiate(typeof(PlatoonEvaluate), typeof(PlatoonOrder));
		}
		/// <summary>
		/// クリーンアップ
		/// </summary>
		public override void Cleanup() {
			BehaviorTree<PlatoonEntity>.Terminate();
			base.Cleanup();
		}

		/// <summary>
		/// Entity生成
		/// </summary>
		public async Task<PlatoonEntity> CreateAsync(string resName, string aiName, PlatoonData data, bool isImmediate = false, bool isReserve = false) {
			//本体生成
			PlatoonEntity entity = await base.CreateImplAsync(resName, isImmediate, isReserve);
			//下にいる分隊とリーダーを生成
			for (int i = 0, max = data.squads.Count; i < max; i++) {
				await entity.CreateSquadAsync(data.squads[i], isImmediate);
			}

			//ヒエラルキー上で分かりやすいようにgameObject名を変える
			for (int i = 0, max = entity.Squads.Count; i < max; i++) {
				entity.Squads[i].gameObject.name += GameConst.Phonetic[i];
			}
			await entity.LoadAI(aiName);
			//パラメータ初期化
			entity.SetData(data);

			return entity;
		}
	}
}
