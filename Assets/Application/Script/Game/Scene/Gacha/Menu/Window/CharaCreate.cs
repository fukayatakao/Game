using Project.Lib;
using Project.StableDiffusion;
using System;
using UnityEngine;
using Project.Network;
using Project.Http.Mst;

namespace Project.Game {
#if DEVELOP_BUILD
	public class CharaCreate : DebugWindow {
		static readonly Vector2 mergin = new Vector2(0.01f, 0.01f);
		static readonly Vector2 size = new Vector2(0.3f, 0.2f);
		static readonly FitCommon.Alignment align = FitCommon.Alignment.LowerRight;

		public UnityEngine.UI.Image image_;
		public UnityEngine.UI.Text text_;
		MessageSystem.Receptor receptor_;

		int select_ = 0;
		enum State : int {
			MAIN,
			GENERATE,
		}
		private State state_ = State.MAIN;


		bool stableRequest_ = false;
		//StableDiffusionに流すプロンプト(男用、女用）
		string[] prompt_ = new string[] {
			"a boy, medieval soldier ware armor,  animation style, show upper body ",
			"a girl, medieval soldier ware armor,  animation style, show upper body " };
		string negative_ = "easy-negative";
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CharaCreate() {
			receptor_ = MessageSystem.CreateReceptor(this, MessageGroup.UserEvent);

			base.Init(FitCommon.CalcRect(mergin, size, align));
			SetAutoResize();
		}

		public void Init(GachaMain owner) {
			image_ = owner.Image;
			text_ = owner.Text;
		}

		/// <summary>
		/// Window内部のUI表示
		/// </summary>
		protected override void Draw(int windowID) {
			switch (state_) {
			case State.MAIN:
				DrawMainGUI();
				break;
			case State.GENERATE:
				DrawGenerateGUI();
				break;
			}
		}
		/// <summary>
		/// 生成するときのGUI表示
		/// </summary>
		private void DrawMainGUI() {
			if (FitGUILayout.Button("リーダー追加")) {
				LotteryLeaderCmd.CreateAsync(new LotteryLeaderRequest(), (res) => {
					LotteryLeaderResponse response = (LotteryLeaderResponse)res;
					GachaSituationData.I.Leaders.Add(response.leaderData);
					/*Texture2D texture = CharacterPortraitUtil.LoadTexture(false, response.characterData.portrait);

						image_.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
						GachaMessage.ShowResult.Broadcast(response.characterData);*/
				});

			}
			if (FitGUILayout.Button("キャラクター追加")) {
				LotteryCharacterCmd.CreateAsync(new LotteryCharacterRequest((int)SPECIES_TYPE.HUMAN), (res) => {
					LotteryCharacterResponse response = (LotteryCharacterResponse)res;
					GachaMessage.ShowCharacter.Broadcast(response.characterData);
					GachaSituationData.I.Characters.Add(response.characterData);
				});

			}
			if (FitGUILayout.Button("AIでキャラ生成")) {
				state_ = State.GENERATE;
			}
			if (FitGUILayout.Button("タウンへ戻る")) {
				SceneTransition.ChangeTown();
			}
		}


		/// <summary>
		/// 生成するときのGUI表示
		/// </summary>
		private void DrawGenerateGUI() {
			//男性・女性の切り替えに連動して画像とテキストを変える
			select_ = FitGUILayout.SelectionGrid(select_, new string[] { "男", "女" }, 2);
			if (FitGUILayout.Button("生成")) {
				CreateImage();
			}

			//折り畳み時は表示しない
			if (stableRequest_) {
				float progress = WebUIConnection.UpdateGenerationProgress();
				FitGUILayout.Label((int)(progress * 100) + "%");
			} else {
				if (FitGUILayout.Button("戻る")) {
					state_ = State.MAIN;
				}
			}
		}
		/// <summary>
		/// 画像生成
		/// </summary>
		private void CreateImage() {
			stableRequest_ = true;
			WebUIConnection.TextToImangeAsync(new Text2ImageRequest() {
				prompt = prompt_[select_],
				negative_prompt = negative_,
			}, (res) => {
				Text2ImageResponse response = (Text2ImageResponse)res;
				// Decode the image from Base64 string into an array of bytes
				byte[] imageData = Convert.FromBase64String(response.images[0]);
				var name = CharacterPortraitUtil.SaveTexture(imageData);
				LotteryOriginalCmd.CreateAsync(new LotteryOriginalRequest((int)SPECIES_TYPE.HUMAN, name), (res) => {
					LotteryOriginalResponse response = (LotteryOriginalResponse)res;
					GachaMessage.ShowCharacter.Broadcast(response.characterData);
					GachaSituationData.I.Characters.Add(response.characterData);
				});

				stableRequest_ = false;

			}, (status) => {
				Debug.LogError(status);
				stableRequest_ = false;

			});
		}


	}
#endif
}
