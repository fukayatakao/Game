using Project.Lib;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Geopolitics {
	[System.Serializable]
	public class Data {
		/// <summary>
		/// json->圧縮->base64テキスト変換
		/// </summary>
		public static string ToBase64String(Data data) {
			//jsonを圧縮してBase64変換した結果をレスポンスで返す
			string json = JsonUtility.ToJson(data);
			byte[] array = GZipUtil.compress(json);
			return System.Convert.ToBase64String(array);
		}
		/// <summary>
		/// json->圧縮->base64テキスト変換
		/// </summary>
		public static Data FromBase64String(string text) {
			byte[] array = System.Convert.FromBase64String(text);
			string json = GZipUtil.decompress(array);
			return JsonUtility.FromJson<Data>(json);
		}



		public List<Way> ways = new List<Way>();
		public List<Node> starts = new List<Node>();
		public Node goal;
		public List<Node> nodes = new List<Node>();
	}
	[System.Serializable]
	public class Node {
		public int id;
		public List<int> next = new List<int>();
		public Vector3 position;
	}


	[System.Serializable]
	public class Way {
		[System.Serializable]
		public class Point {
			public Vector3 position;
			public Vector3 vector;              //傾き
			public float current;               //現ポイントまでの距離（総距離を1とした場合の値）
		}

		//@todo 名前変えるか隠す
		public Point start { get { return points[0]; } }                 //開始ノード
		public Point goal { get { return points[points.Count - 1]; } }   //終端ノード

		public int startNodeId;
		public int goalNodeId;

		public List<Point> points;                   //すべてのノード
		public float length;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Way(Node start, Node goal) {
			startNodeId = start.id;
			goalNodeId = goal.id;
			// スタートとゴールを直線で結ぶ
			points = new List<Point>();
			Vector3 vec = goal.position - start.position;
			points.Add(new Point() { position = start.position, vector = vec, current = 0f });
			points.Add(new Point() { position = goal.position, vector = vec, current = 1f });
			length = vec.magnitude;
		}

		/// <summary>
		/// 評価関数
		/// </summary>
		public Vector3 Evaluate(float t) {
			//tは0～1の範囲内
			Debug.Assert(t >= 0 && t <= 1, "value limit error:" + t);
			for (int i = 0, max = points.Count - 1; i < max; i++) {
				if (points[i].current > t || points[i + 1].current < t)
					continue;
				float tt = t - points[i].current;
				tt = tt / (points[i + 1].current - points[i].current);
				return MathUtil.CalcHermite(points[i].position, points[i].vector, points[i + 1].position, points[i + 1].vector, tt);
			}
			//データがおかしい
			Debug.LogError("curve section error");
			return Vector3.zero;
		}
		/// <summary>
		/// 区間の距離を計算して全体の距離で正規化された開始点を計算して入れる
		/// </summary>
		public void CalculateLength() {
			//積分して距離を求めるときの分割数
			const int div = 100;
			float l = 0f;
			float[] len = new float[points.Count];
			//区間距離を計算
			for (int i = 0, max = points.Count - 1; i < max; i++) {
				Vector3 p1 = points[i].position;
				//曲線を小さく分割して直線距離で近似積分して全体の距離を計算する
				for (int j = 1; j <= div; j++) {
					float t = (float)j / div;
					Vector3 p2 = MathUtil.CalcHermite(points[i].position, points[i].vector, points[i + 1].position, points[i + 1].vector, t);
					l += (p2 - p1).magnitude;
					p1 = p2;
				}
				len[i + 1] = l;
			}

			for (int i = 0, max = points.Count; i < max; i++) {
				points[i].current = len[i] / l;
			}

			length = l;
		}
	}

}
