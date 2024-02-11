using UnityEngine;
using Project.Lib;


namespace Project.Game {
#if DEVELOP_BUILD
    public class CharacterEnable : DebugWindow {
        static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
        static readonly Vector2 size = new Vector2(0.2f, 0.6f);
        static readonly FitCommon.Alignment align = FitCommon.Alignment.UpperLeft;


        Vector2 scroll_ = new Vector2();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterEnable() {
            base.Init(FitCommon.CalcRect(mergin, size, align));
        }

        /// <summary>
        /// Window内部のUI表示
        /// </summary>
        protected override void Draw(int windowID) {
            CloseButton.Draw(this);
            CaptionLabel.Draw("キャラ停止");
            scroll_ = FitGUILayout.BeginScrollView(scroll_);

            if (FitGUILayout.Button("全停止")) {
                for(int i = 0; i < CharacterAssembly.I.Count; i++) {
                    CharacterAssembly.I.Current[i].ExecuteEnable = false;
                }
            }

            if (FitGUILayout.Button("全再生")) {
                for (int i = 0; i < CharacterAssembly.I.Count; i++) {
                    CharacterAssembly.I.Current[i].ExecuteEnable = true;
                }
            }


#if UNITY_EDITOR
            if (FitGUILayout.Button("選択以外停止")) {
                //UnityEditor.Selection.gameObjects.
                for (int i = 0; i < CharacterAssembly.I.Count; i++) {



                    if(CharacterAssembly.I.Current[i].CacheTrans.gameObject == UnityEditor.Selection.activeGameObject){
                        CharacterAssembly.I.Current[i].ExecuteEnable = true;
                    } else {
                        CharacterAssembly.I.Current[i].ExecuteEnable = false;
                    }
                }
            }
#endif


            FitGUILayout.EndScrollView();
        }

    }
#endif
        }
