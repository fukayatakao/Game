using UnityEngine;
using System.Collections.Generic;

namespace Project.Network {
	public class JsonList
	{
		[System.Serializable]
		class JsonListClass<T> {
			public List<T> list = null;
		}

		/// <summary>
		/// Jsonからオブジェクトに変換
		/// ルートがリストの場合
		/// T にはリストに入れる型を指定
		/// </summary>
		public static string ToJsonList<T>(List<T> list)
		{
			JsonListClass<T> jlist = new JsonListClass<T>();
			jlist.list = list;
			string json = JsonUtility.ToJson(jlist, true);

			json = json.Remove(0, "{\n    \"list\":".Length);
			json = json.Remove(json.Length - 1);
			return json;
		}

		/// <summary>
		/// Jsonからオブジェクトに変換
		/// ルートがリストの場合
		/// T にはリストに入れる型を指定
		/// </summary>
		public static List<T> FromJsonList<T>(string json) {
			string strJsonList = "{ \"list\":" + json + "}";
			JsonListClass<T> jList = JsonUtility.FromJson<JsonListClass<T>>(strJsonList);
			return jList.list;
		}
		/// <summary>
		/// Jsonからオブジェクトに変換
		/// ルートがリストの場合
		/// T にはリストに入れる型を指定
		/// </summary>
		public static object FromJsonList(string json, string typeName) {
			string strJsonList = "{ \"list\":" + json + "}";
			System.Type type = System.Type.GetType(typeName);

			Debug.Assert(type != null, "create generic type error:" + typeName);

			// List<Dummy>のTypeを作成
			System.Type genericBaseType = typeof(JsonListClass<>);
			System.Type genericType = genericBaseType.MakeGenericType(type);

			object obj = JsonUtility.FromJson(strJsonList, genericType);
			System.Reflection.FieldInfo field = genericType.GetField("list");


			return field.GetValue(obj);
		}


	}

}
