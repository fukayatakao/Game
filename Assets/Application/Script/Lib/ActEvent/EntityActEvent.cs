using System.Collections.Generic;
using UnityEngine;


namespace Project.Lib {

    /// <summary>
    /// ActEventのインタフェース定義
    /// </summary>
    /// <remarks>
    /// 主にエディタで使う
    /// </remarks>
    public interface IEntityActEvent {
        void Add(string name, ActEventData data);

        bool Play(string name, float speed);
        void Stop();

        bool IsPlay();
        bool IsPlay(string name);
#if UNITY_EDITOR
        /// <summary>
        /// 指定の時間までのイベントをスキップして途中から開始できるようにする
        /// </summary>
        void Seek(float seekTime);
#endif
    }

    /// <summary>
    /// 特定のタイミングでイベントを実行する
    /// </summary>
    /// <remarks>
    /// MotionEventのアニメーションから解放されたバージョン
    /// </remarks>
    public class EntityActEvent<T> : IEntityActEvent {
		//関数デリゲートのキャッシュクラス
		static public ActEventCache<T> Cache = new ActEventCache<T>();
		//イベントリスト
		Dictionary<string, ActEventCache<T>.DeployParam[]> events_ = new Dictionary<string, ActEventCache<T>.DeployParam[]>();
		Dictionary<string, ActEventData> eventData_ = new Dictionary<string, ActEventData>();
		//現在実行中のイベント
		ActEventCache<T>.DeployParam[] currentEvents_;
		ActEventData currentData_;
		public List<AnimationCurve> CurrentCurveList { get { return currentData_.curveList; } }

        string name_;
		//イベントをどこまで実行したか
		int seeker_ = 0;
		//イベントの最大数
		int max_ = 0;
		//現在時間
		float time_;
		//再生スピード
		float speed_;
		//一時停止フラグ
		bool pause_;
		/// <summary>
		/// 初期化
		/// </summary>
		public EntityActEvent() {
			events_.Clear();
			eventData_.Clear();
			currentEvents_ = null;
		}

		/// <summary>
		/// イベントデータを追加
		/// </summary>
		public void Add(string name, ActEventData data) {
			events_[name] = Cache.Deploy(data);
			eventData_[name] = data;
		}

		/// <summary>
		/// イベント再生開始
		/// </summary>
		public bool Play(string name, float speed=1f) {
			Debug.Assert(events_ != null, "error:" + name);
			Debug.Assert(name != null, "error:" + name);
            if (!events_.ContainsKey(name)){
				Debug.LogError("not found action:" + name);
                name_ = "";
                return false;
            }
			currentEvents_ = events_[name];
			currentData_ = eventData_[name];
            name_ = name;

            time_ = 0f;
			seeker_ = 0;
			speed_ = speed;
			max_ = currentEvents_.Length;
			pause_ = false;

			return true;
		}
        /// <summary>
        /// イベント停止
        /// </summary>
        public void Stop() {
            seeker_ = max_;
        }

		/// <summary>
		/// 一時停止
		/// </summary>
		public void Pause() {
			pause_ = true;
		}
		/// <summary>
		/// 再開
		/// </summary>
		public void Resume() {
			pause_ = false;
		}

		/// <summary>
		/// イベント再生中か
		/// </summary>
		public bool IsPlay() {
			if (seeker_ >= max_) {
				return false;
			} else {
				return true;
			}
		}
        /// <summary>
        /// イベント再生中か
        /// </summary>
        public bool IsPlay(string name) {
            if (name_ != name)
                return false;
            if (seeker_ >= max_) {
                return false;
            } else {
                return true;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 指定の時間までのイベントをスキップして途中から開始できるようにする
        /// </summary>
        public void Seek(float seekTime) {
            if (currentEvents_ == null)
                return;
			time_ = seekTime;
			//時間が経過していたものは実行
			for (int i = 0; i < currentEvents_.Length; i++) {
				if (currentEvents_[i].timeStamp >= time_) {
					seeker_ = i;
					return;
				}
			}
			seeker_ = currentEvents_.Length;
		}
#endif
	    /// <summary>
	    /// 現在時間からスキップしてアクションを終了する
	    /// </summary>
	    public void Skip(T entity)
	    {
		    if (seeker_ >= max_)
			    return;
		    //未実行のイベントのうちスキップしても実行するものを実行
		    for (int i = seeker_; i < max_; i++) {
			    if(currentEvents_[i].executeSkip)
				    currentEvents_[i].method(entity, currentEvents_[i].args);
		    }
		    seeker_ = max_;
	    }

	    /// <summary>
		/// イベントの呼び出し
		/// </summary>
		public void Execute(T entity) {
			if (seeker_ >= max_)
				return;
			if (pause_)
				return;
			//@note アニメーション途中で再生速度が変化するケースに注意
			time_ += Time.deltaTime * speed_;
			//時間が経過していたものは実行
			for (int i = seeker_; i < max_; i++) {
				if (currentEvents_[i].timeStamp > time_) {
					seeker_ = i;
					return;
				}
				currentEvents_[i].method(entity, currentEvents_[i].args);
			}
			seeker_ = max_;
		}

		public float NormalizeTime() {
			if (!IsPlay())
				return 0f;
			return time_ / currentEvents_[max_ - 1].timeStamp;
		}

	}
}
