using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Project.Lib {
    /// <summary>
    /// EntityAssemly管理
    /// </summary>
    public class AssemlyManager {
        protected List<IEntityAssembly> assemblys_ = new List<IEntityAssembly>();

        private List<System.Func<Transform, CancellationToken, IEntityAssembly>> CreateAssembly;

		//Taskキャンセル用のトークン
		protected CancellationTokenSource cancelTokenSource_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssemlyManager(List<System.Func<Transform, CancellationToken, IEntityAssembly>> createFunc) {
			cancelTokenSource_ = new CancellationTokenSource();
			CreateAssembly = createFunc;
        }

        /// <summary>
        /// 明示的にシングルトンのインスタンスを作る
        /// </summary>
        public void Create(Transform root) {
            for(int i = 0, max = CreateAssembly.Count; i < max; i++) {
                assemblys_.Add(CreateAssembly[i](root, cancelTokenSource_.Token));
            }

        }

        /// <summary>
        /// シングルトンのインスタンスを破棄する
        /// </summary>
        public void Destroy() {
            for (int i = 0, max = assemblys_.Count; i < max; i++) {
                assemblys_[i].DestroyInstance();
            }
            assemblys_.Clear();
			cancelTokenSource_.Cancel();
			cancelTokenSource_.Dispose();
		}

		/// <summary>
		/// 実行可能状態に移す
		/// </summary>
		public void Flush() {
			for (int i = 0, max = assemblys_.Count; i < max; i++) {
				assemblys_[i].Flush();
			}
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute() {
            for (int i = 0, max = assemblys_.Count; i < max; i++) {
                assemblys_[i].Execute();
            }
        }
		/// <summary>
		/// Playable更新
		/// </summary>
		public void Evaluate() {
			for (int i = 0, max = assemblys_.Count; i < max; i++) {
				assemblys_[i].Evaluate();
			}
		}
		/// <summary>
		/// 実行後処理
		/// </summary>
		public void LateExecute() {
            for (int i = 0, max = assemblys_.Count; i < max; i++) {
                assemblys_[i].LateExecute();
            }
        }
    }
}
