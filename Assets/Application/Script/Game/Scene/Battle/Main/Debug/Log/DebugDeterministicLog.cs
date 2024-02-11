using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Project.Game {
#if DEVELOP_BUILD
	/// <summary>
	/// 決定論的動作の検証用ログを出力
	/// </summary>
	public static class DebugDeterministicLog {
		static int logCount_ = 0;

		public static void Init() {
			logCount_ = 0;
		}

		public static void Execute() {
			if (logCount_ % 10 == 0) {
				SameCheck();
				WriteLog();
			}

			logCount_++;
		}

		private static string CreateLog(CharacterEntity entity) {
			Encoding utf8 = Encoding.UTF8;
			string hp = GetHexString(System.BitConverter.GetBytes(entity.HaveUnitParam.Fight.Hp));
			string lp = GetHexString(System.BitConverter.GetBytes(entity.HaveUnitParam.Fight.Lp));
			Vector3 p = entity.GetPosition();
			string pos = GetHexString(System.BitConverter.GetBytes(p.x)) + "," + GetHexString(System.BitConverter.GetBytes(p.y)) + "," + GetHexString(System.BitConverter.GetBytes(p.z));
			Quaternion r = entity.GetRotation();
			string rot = GetHexString(System.BitConverter.GetBytes(r.x)) + "," + GetHexString(System.BitConverter.GetBytes(r.y)) + "," + GetHexString(System.BitConverter.GetBytes(r.z)) + "," + GetHexString(System.BitConverter.GetBytes(r.w));


			string state = entity.HaveState.CurrentState.ToString();

			return hp + "," + lp + "," + pos + ","  + rot;
		}
		private static string GetHexString(byte[] bytes) {
			string str = "";
			for (int i = 0, max = bytes.Length; i < max; i++) {
				str += bytes[i].ToString("X2");
			}
			return str;
		}

		private static void WriteLog() {
			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.dataPath.Replace("/Assets", "/Logs/frame_" + logCount_ + ".txt"))){
				for (int i = 0; i < CharacterAssembly.I.Count; i++) {

					sw.WriteLine("キャラクター" + i + "番目" + ":" + CreateLog(CharacterAssembly.I.Current[i]));
				}
			}
			logCount_++;
		}


		private static void SameCheck() {
			string path = Application.dataPath.Replace("/Assets", "/Logs/frame_" + logCount_ + ".txt");
			//ファイルが存在しないときは無視
			if (!System.IO.File.Exists(path))
				return;


			List<string> log = new List<string>();
			using (System.IO.StreamReader sr = new System.IO.StreamReader(path)) {
				string line;
				while ((line = sr.ReadLine()) != null) {
					string[] term = line.Split(':');
					log.Add(term[1]);
				}
			}

			Debug.Assert(log.Count == CharacterAssembly.I.Count, "record count error");

			for (int i = 0; i < CharacterAssembly.I.Count; i++) {
				string current = CreateLog(CharacterAssembly.I.Current[i]);

				if(current != log[i]){
					Debug.LogError("same check error:" + i + ":" + logCount_);
					Debug.LogError(current);
					Debug.LogError(log[i]);
				}
			}

		}
	}
#endif
}
