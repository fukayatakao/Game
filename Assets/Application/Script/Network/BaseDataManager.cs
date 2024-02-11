using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Project.Lib;
using Project.Network;
using System.Threading.Tasks;

namespace Project.Mst {
	public static partial class BaseDataManager {
		//部分クラスのどこかでDictionary生成関数を定義すると実行される
		static partial void PostCacheProcess();
		/// <summary>
		/// 毎回キャストしなくてもよくなるようにキャッシュを作る
		/// </summary>
		private static class Cache<T> {
			//マスターのキャッシュ
			private static List<T> cache_;

			/// <summary>
			/// リストを取得
			/// </summary>
			public static List<T> GetList() {
				//キャッシュがまだ出来ていない場合は作成
				if(cache_ == null) {
					//大元のキャッシュにデータが存在しない場合はあさーとで止める。新規でマスターを追加したときにMstVersionに追記し忘れるとここで止まる。
					Debug.Assert(BaseDataManager.mstCache_.ContainsKey(typeof(T).Name), "master cache not found:" + typeof(T).Name);

					//大元のキャッシュから必要な型をキャストした状態でキャッシュしておく
					cache_ = BaseDataManager.mstCache_[typeof(T).Name] as List<T>;
					//インスタンスを破棄できるように破棄関数を登録する
					BaseDataManager.destroyFunc_.Add(()=>{ cache_ = null; });
				}
				return cache_;
			}
		}

		/// <summary>
		/// Dictionaryのキャッシュを作る
		/// </summary>
		private static class Cache<S, T> {

			//マスターのDictionary
			private static Dictionary<S, T> cache_;

			/// <summary>
			/// Dictionaryキャッシュの生成
			/// </summary>
			public static void Create(System.Func<List<T>, Dictionary<S, T>> func) {
				cache_ = func(BaseDataManager.GetList<T>());
				//インスタンスを破棄できるように破棄関数を登録する
				BaseDataManager.destroyFunc_.Add(() => { cache_ = null; });
			}
			/// <summary>
			/// 元データを加工してキャッシュの生成
			/// </summary>
			public static void Create<U>(System.Func<List<U>, Dictionary<S, T>> func) {
				cache_ = func(BaseDataManager.GetList<U>());
				//インスタンスを破棄できるように破棄関数を登録する
				BaseDataManager.destroyFunc_.Add(() => { cache_ = null; });
			}
			/// <summary>
			/// Dictionary取得
			/// </summary>
			public static Dictionary<S, T> GetDictionary() {
				return cache_;
			}

		}



		//キャスト済のデータを保持するインスタンスの後始末をする関数デリゲート
		private static List<System.Action> destroyFunc_ = new List<System.Action>();
		//キャストしてない大元のデータ
		private static Dictionary<string, object> mstCache_ = new Dictionary<string, object>();
		private const string MST_PREFS_PREFIX = "product_mst_version_";
		private const string MST_TYPE_LIST_KEY = "product_mst_type_list";

		//マスターのクラス定義のある名前空間
		private const string MST_NAMESPACE_PREFIX = "Project.Mst.";
		//ローカルキャッシュの拡張子
		private const string MST_CACHE_EXTEND = ".d";

		//これを使ってリストを取得する。中でキャッシュを作ってるので高速でアクセス可能。
		public static List<T> GetList<T>() { return Cache<T>.GetList(); }
		public static Dictionary<S, T> GetDictionary<S, T>() { return Cache<S, T>.GetDictionary(); }

		//有効になっているか
		private static bool isValid_;
		public static bool IsVaild(){ return isValid_; }

		/// <summary>
		/// マスターの更新を行う
		/// </summary>
		public static async void Refresh(System.Action callback) {
			//前回のインスタンスが残っている場合は破棄
			Clear();
			string mstVersionName = typeof(MstVersion).Name;
			Task<string> task = MstDataRequest(mstVersionName, "", false);
			await task;


			//なんかエラーだった
			if (task.Result == null) {
				Debug.LogError("MstVersion Request Error");
				return;
			}
			List<MstVersion> list = JsonList.FromJsonList(task.Result, MST_NAMESPACE_PREFIX + mstVersionName) as List<MstVersion>;

			//現存するマスターとローカルにキャッシュしているマスターの突き合わせ
			CheckExistMst(list);

			List<Task<bool>> arrayTask = new List<Task<bool>>();
			for (int i = 0, max = list.Count; i < max; i++) {
				arrayTask.Add(MstDataStart(list[i]));
			}

			await Task.WhenAll(arrayTask);
			for (int i = 0, max = list.Count; i < max; i++) {
				if (!arrayTask[i].Result) {
					Debug.LogError("Master Request Error:" + list[i].Name);
					return;
				}
			}
			PostCacheProcess();

			//全部終わったらコールバックを呼ぶ
			callback();

			isValid_ = true;
		}
		/// <summary>
		/// マスターの更新(コルーチンモード)
		/// </summary>
		public static IEnumerator Refresh() {
			bool continuous = true;
			Refresh(() => { continuous = false; });
			while (continuous) {
				yield return null;
			}
		}
		/// <summary>
		/// ローカルキャッシュを全削除
		/// </summary>
		public static void ClearCache() {
			if (PlayerPrefs.HasKey(MST_TYPE_LIST_KEY)) {
				string[] mstNames = PlayerPrefs.GetString(MST_TYPE_LIST_KEY, "").Split(',');

				for(int i = 0, max = mstNames.Length; i < max; i++) {
					RemoveMst(mstNames[i]);
				}
				// ベースデータタイプ名のリストもクリアする。
				PlayerPrefs.DeleteKey(MST_TYPE_LIST_KEY);
			}
		}

		/// <summary>
		/// 消えたマスターのローカルキャッシュを削除
		/// </summary>
		private static void RemoveMst(string mstName) {
			PlayerPrefs.DeleteKey(GetKey(mstName));
			FileIOUtil.Delete(GetPath(mstName));
		}

		/// <summary>
		/// マスター名から保存場所を取得
		/// </summary>
		private static string GetPath(string mstName) {
			return Application.temporaryCachePath + "/" + mstName + MST_CACHE_EXTEND;
		}

		/// <summary>
		/// ハッシュを保存してあるキーを取得
		/// </summary>
		private static string GetKey(string mstName) {
			return MST_PREFS_PREFIX + mstName;
		}


		/// <summary>
		/// 現存しないマスターデータがキャッシュされていたら削除する
		/// </summary>
		private static void CheckExistMst(List<MstVersion> list) {
			//ローカルに存在しているマスターデータのリストを取得
			if (PlayerPrefs.HasKey(MST_TYPE_LIST_KEY)) {
				// カンマ区切りで保存されているので分割する
				string[] types = PlayerPrefs.GetString(MST_TYPE_LIST_KEY, "").Split(',');
				int len = list.Count;

				//サーバから取得したマスター一覧と突き合わせてローカルに存在するがサーバから消えているマスターをあぶりだす
				for (int i = 0; i < types.Length; i++) {
					string mstName = types[i];
					bool isRemove = true;
					for (int j = 0; j < len; j++) {
						//まだ存在するので消去リストからはずす
						if (mstName == list[j].Name) {
							isRemove = false;
							break;
						}
					}
					//サーバから消えているのでローカルのキャッシュも消す
					if (isRemove) {
						RemoveMst(mstName);
					}
				}
			}

			//現存するマスター名のリストをカンマ区切りで保存
			string value = "";
			for (int i = 0, max = list.Count; i < max; i++) {
				value += list[i].Name + ",";
			}
			//リストを保存
			PlayerPrefs.SetString(MST_TYPE_LIST_KEY, value);
		}
		/// <summary>
		/// 非同期でサーバーまたはローカルからベースデータを取得する
		/// </summary>
		private static async Task<bool> MstDataStart(MstVersion mstVer) {
			string mst_file_path = GetPath(mstVer.Name);

			Debug.Log("[Mst] mst_file_path -> " + mst_file_path);

			// ベースデータのバージョンをロードする
			string local_mst_version = PlayerPrefs.GetString(GetKey(mstVer.Name), "");


			string value = "";
			// 初回なのでローカルにベースデータが保存されていない、
			// またはベースデータのバージョンが更新されているので、サーバーからベースデータを取得して更新する。
			if (local_mst_version != mstVer.Value
				|| !FileIOUtil.Exists(mst_file_path)) {

				Task<string> task = MstDataRequest(mstVer.Name, mstVer.Value);
				await task;
				value = task.Result;

			// ベースデータのバージョンが更新されていないので、ローカルファイルからベースデータを取得する。
			} else {
				byte[] json_binary_compress = FileIOUtil.Load(mst_file_path);

				// ローカルに保存したベースデータバイナリの改竄チェック。
				string local_basedata_hash = HashUtil.ComputeMd5Hash(json_binary_compress);

				Debug.Log("[Mst] local binary hash compare -> " + mstVer.Name + " : hash -> local[" + local_basedata_hash + "] : server[" + mstVer.Value + "]");

				//改竄若しくはデータ破損している->サーバから取得しなおし
				if (local_basedata_hash != mstVer.Value) {
//オフラインのときは破損チェックしない
#if !USE_OFFLINE
					// ローカルに保存したベースデータのハッシュ値とサーバ側から送られたベースデータのハッシュ値が違う
					Debug.LogError("[BaseData] local data error -> " + mstVer.Name + " : " + local_basedata_hash + " : " + mstVer.Value);
#endif
					Task<string> task = MstDataRequest(mstVer.Name, mstVer.Value);
					await task;
					value = task.Result;
				} else {
					value = GZipUtil.decompress(json_binary_compress);
				}
			}


			//通信エラーなどでマスターのJsonが空になっていたらエラー出して抜ける
			if (string.IsNullOrEmpty(value)) {
				Debug.LogError("mst json error:" + mstVer.Name);
				return false;
			}
			mstCache_[mstVer.Name] = JsonList.FromJsonList(value, MST_NAMESPACE_PREFIX + mstVer.Name);
			return true;
		}

		/// <summary>
		/// サーバにマスターデータをリクエスト
		/// </summary>
		private static async Task<string> MstDataRequest(string mstName, string version, bool isLocalCache=true) {
			Task<Project.Network.Command.Result> task = BaseDataCmd.CreateTask(new BaseDataRequest(mstName));
			await task;
			//なんかエラーだった
			if (!task.Result.IsSuccess()) {
				return null;
			}


			//サーバからはzip圧縮されたバイナリデータのJsonが落ちてくる
			// Jsonテキストの圧縮結果のBase64文字列をデコードし、byte配列として保存する
			byte[] json_binary_compress = System.Convert.FromBase64String((task.Result.Response as BaseDataResponse).data);

			//ローカルに保存
			if (isLocalCache){
				FileIOUtil.Save(GetPath(mstName), json_binary_compress);
				//
				PlayerPrefs.SetString(GetKey(mstName), version);
			}
			return GZipUtil.decompress(json_binary_compress);

		}

		/// <summary>
		/// メンバのインスタンスを破棄する
		/// </summary>
		private static void Clear() {
			isValid_ = false;
			for (int i = 0, max = destroyFunc_.Count; i < max; i++) {
				destroyFunc_[i]();
			}
			destroyFunc_.Clear();
			mstCache_.Clear();
		}


#if USE_OFFLINE
		/// <summary>
		/// BaseData取得のオフライン用関数
		/// </summary>
		public static Response BaseDataOffline(Request request) {
			string mstName = (request as BaseDataRequest).type;
			TextAsset asset = (Resources.Load(mstName + "_json") as TextAsset);
			Debug.Assert(asset != null, "not found master json:" + mstName);
			string json = asset.text;
			//jsonを圧縮してBase64変換した結果をレスポンスで返す
			byte[] array = GZipUtil.compress(json);
			return new BaseDataResponse() { data = System.Convert.ToBase64String(array) };
		}
#endif


	}
}



















