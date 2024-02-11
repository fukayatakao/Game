using System;
using System.Collections.Generic;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// ActEventで使用する関数キャッシュ
	/// </summary>
	public class ActEventCache<T> {
		//デリゲート関数の定義
		public delegate void MethodFunc(T entity, object[] args);
		//関数名で取得できるようにキャッシュ
		Dictionary<string, MethodFunc> methodCache_;
		//スキップしたときも実行するフラグ
		HashSet<string> methodExecuteSkip_;

		/// <summary>
		/// 使えるように展開したパラメータ
		/// </summary>
		public struct DeployParam {
			public float timeStamp;
			public MethodFunc method;
			public object[] args;
			public bool executeSkip;
		}

		/// <summary>
		/// クラス内にある関数のキャッシュを作る
		/// </summary>
		public void Initiate(Type EventClass) {
			Debug.Assert(methodCache_ == null, "already alloc instance: ActEventCache<" + typeof(T) + ">");

			methodCache_ = new Dictionary<string, MethodFunc>();
			methodExecuteSkip_ = new HashSet<string>();
			System.Reflection.MethodInfo[] list = EventClass.GetMethods ();
			for (int i = 0, max = list.Length; i < max; i++) {
				System.Reflection.ParameterInfo[] param = list[i].GetParameters();
				//引数が２つで1つ目がEntityのクラス、2つ目がobject配列のもののみを抜き出す（EventClassの中に扱えない関数があっても無視する
				if(param.Length == 2 && param[0].ParameterType == typeof(T) && param[1].ParameterType == typeof(object[])){
					//delegate関数を作ってDictionaryにキャッシュする
					MethodFunc func = (MethodFunc)Delegate.CreateDelegate (typeof(MethodFunc), list[i]);
					methodCache_.Add (list[i].Name, func);
				}
				//スキップ時も実行属性があるか
				if (Attribute.IsDefined(list[i], typeof(ExecuteSkipAttribute))) {
					methodExecuteSkip_.Add(list[i].Name);
				}
			}
			methodCache_.Add (((MethodFunc)ActEventTermination.Termination).Method.Name, ActEventTermination.Termination);
			//終端関数名の文字列が一致しない場合は変更ミスっているので直す
			Debug.Assert(((MethodFunc)ActEventTermination.Termination).Method.Name == ActEventTermination.TerminationMethod, "Termination method error");
		}
		/// <summary>
		/// キャッシュを破棄する
		/// </summary>
		public void Terminate() {
			Debug.Assert(methodCache_ != null, "already free instance: ActEventCache<" + typeof(T) + ">");
			methodCache_ = null;
		}

		/// <summary>
		/// プログラムで使える形へ展開
		/// </summary>
		public DeployParam[] Deploy(ActEventData param) {
			int max = param.eventDataList.Count;
			DeployParam[] deployArray = new DeployParam[max];
			for(int i = 0; i < max; i++) {
				ActEventData.EventData data = param.eventDataList[i];
				DeployParam deploy = new DeployParam();
				//関数名からキャッシュ済の関数デリゲートを取得
				deploy.method = methodCache_[data.method];
				//引数を文字列からプログラムで使える形へ変換
				deploy.args = MethodArgs.DeserializeArgs(data.args, data.types, data.argsBinary);
				//実行時間をセット
				deploy.timeStamp = data.execTime;
				deploy.executeSkip = methodExecuteSkip_.Contains(data.method);
				deployArray[i] = deploy;
			}
			return deployArray;
		}

	}
	/// <summary>
	/// 終端関数管理用クラス
	/// </summary>
	public static class ActEventTermination {
		//終端判定用の空関数名
		public static readonly string TerminationMethod = "Termination";
		//終端判定用の空関数
		public static void Termination<T>(T entity, object[] args) {
		}

	}
}
