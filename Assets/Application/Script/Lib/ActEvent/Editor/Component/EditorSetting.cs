using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Project.Lib {
    public class EditorSetting {
        //編集可能なEntityとイベントの組み合わせ設定
        public struct EntitySetting {
            public System.Type entityType;
            public System.Type eventType;
			public System.Func<int, object, string> createResourcePath;
        }
        //@note いずれデータ化したい
        //使える組み合わせリスト
        public static List<EntitySetting> EntitySettingList = new List<EntitySetting>() {
            new EntitySetting{ entityType = typeof(Project.Game.CharacterEntity), eventType = typeof(Project.Game.CharacterEvent), createResourcePath = Project.Game.CharacterEvent.CreateResourcePath },
            new EntitySetting{ entityType = typeof(Project.Game.FieldEntity), eventType = typeof(Project.Game.FieldEvent) },
        };
        public static string AssemblyName = typeof(Project.Game.CharacterEntity).Assembly.FullName;

		const string PickAnimationRoot = "Assets/Application/Addressable/Character/Animation";
		//AnimationClipのデータを探すディレクトリ。上から順に検索をかけて最初に見つかったものを使う。
		public static List<string> PickAnimationDirectory = new List<string>()
        {
			PickAnimationRoot,
            //全体から検索
            "Assets",
        };

		/// <summary>
		/// AnimationClipを検索して取得
		/// </summary>
		public static AnimationClip SearchClip(string name, string subdir="") {
			List<string> pickDir = new List<string>();
			if (!string.IsNullOrEmpty(subdir)){
				pickDir.Add(PickAnimationRoot + "/" + subdir);
			}
			pickDir.AddRange(PickAnimationDirectory);

			//優先して探すディレクトリから検索
			string[] guid = AssetDatabase.FindAssets("t:AnimationClip", pickDir.ToArray());
            for (int i = 0; i < guid.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(guid[i]);
                //ディレクトリ削除
                string assetName = path.Substring(path.LastIndexOf('/') + 1);
                //拡張子削除
                assetName = assetName.Remove(assetName.LastIndexOf('.'));
                if (assetName == name) {
                    return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                }
            }
            return null;
        }

    }
}
