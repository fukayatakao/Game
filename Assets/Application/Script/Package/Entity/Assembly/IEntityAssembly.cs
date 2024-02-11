using System.Threading;
using UnityEngine;

namespace Project.Lib {
    /// <summary>
    /// Entity管理インターフェース
    /// </summary>
    public interface IEntityAssembly {
		//トークンセット
		void Init(CancellationToken token);
        // セットアップ
        void Setup(Transform root, string name);
        // クリーンアップ
        void Cleanup();
		// 実行可能状態に移す
		void Flush();
        // 実行処理
        void Execute();
		// Playableの更新
		void Evaluate();
		// 実行後処理
		void LateExecute();
       // インスタンス破棄
        void DestroyInstance();
    }

}
