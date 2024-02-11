using Project.Lib;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Game {
    /// <summary>
    /// マテリアル処理クラス
    /// </summary>
    public class RenderMaterial : MonoPretender {
		//private Color currentMaterialMultiColor_ = Color.white;	// 頂点乗算カラー
		//private Color currentMaterialAddColor_ = Color.clear;	// 頂点加算カラー

		//private IEnumerator changeMultiColorCoroutine_ = null;
		//private IEnumerator changeAddColorCoroutine_ = null;
		[SerializeField]
		private List<Renderer> rendererList_ = new List<Renderer>();

		//private static readonly string ADD_COLOR_NAME = "_AddColor";
		//private static readonly string MULTI_COLOR_NAME = "_MultiColor";

		private const string TRANSPARENCY_NAME = "_Tweak_transparency";
		/// <summary>
		/// 生成処理
		/// </summary>
		/// <remarks>
		/// rendererを収集する
		/// </remarks>
		protected override void Create(GameObject obj)
		{

			rendererList_.Clear ();

			//InitColor ();
			// 子オブジェクトを含めた全Renderer取得
			rendererList_.AddRange (obj.transform.GetComponentsInChildren<Renderer> ());
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Init(List<Renderer> list) {
			//InitColor ();
			rendererList_ = list;
		}

		/// <summary>
		/// アルファを設定
		/// </summary>
		public void SetAlpha(float alpha) {
			SetMaterialColor(TRANSPARENCY_NAME, alpha);
		}
		/// <summary>
		/// 表示設定
		/// </summary>
		public void SetActive (bool active) {
			for (int i = 0, max = rendererList_.Count; i < max; i++ ) {
				rendererList_[i].enabled = active;
			}
		}
		/// <summary>
		/// シェーダに送るカラーの情報を設定
		/// </summary>
		private void SetMaterialColor(string propatyName, float value) {
			for (int i = rendererList_.Count - 1; i >= 0; i--) {
				//@todo materialsにアクセスするとoutlineの色が白くなる。。。なんでだ。。。？
				//Renderがnullになるケースがあるようなので対策
				if (!ReferenceEquals(rendererList_[i], null) && !ReferenceEquals(rendererList_[i].material, null)) {
					rendererList_[i].material.SetFloat(propatyName, value);
				}
			}
		}




		/*
		/// <summary>
		/// カラーの初期化
		/// </summary>
		public void InitColor() {
			SetMaterialColor(Color.white, Color.clear);
		}
		/// <summary>
		/// カラーをまとめて設定
		/// </summary>
		public void SetMaterialColor(Color multiCol, Color addCol) {
			StopChangeMultiColor ();
			StopChangeAddColor ();

			SetMultiColor(multiCol);
			SetAddColor(addCol);


			isHitFlashing_ = false;
		}

		/// <summary>
		/// 乗算カラーを設定
		/// </summary>
		public void SetMultiColor(Color color) {
			currentMaterialMultiColor_ = color;

			SetMaterialColor (MULTI_COLOR_NAME, color);
		}
		/// <summary>
		/// 加算カラーを設定
		/// </summary>
		public void SetAddColor(Color color) {
			currentMaterialAddColor_ = color;

			SetMaterialColor (ADD_COLOR_NAME, color);
		}
		/// <summary>
		/// シェーダに送るカラーの情報を設定
		/// </summary>
		private void SetMaterialColor(string propatyName, Color color)
		{
			for (int i = rendererList_.Count - 1; i >= 0; i --) {
				//Renderがnullになるケースがあるようなので対策
				//複数体ヒットフラッシュでnullチェック回数多くなりそうなので高速化対策する
				if (!ReferenceEquals(rendererList_[i], null) && !ReferenceEquals(rendererList_[i].material, null)) {
					rendererList_[i].material.SetColor(propatyName, color);
				}
			}
		}
		/// <summary>
		/// 乗算カラー変化開始
		/// </summary>
		public void StartChangeMultiColor(Color color, float changeTime) {
			changeMultiColorCoroutine_ = ChangeMultiColorCoroutine(color, changeTime);
		}

		/// <summary>
		/// 乗算カラー変化停止
		/// </summary>
		public void StopChangeMultiColor()
		{
			changeMultiColorCoroutine_ = null;
		}
		/// <summary>
		/// 乗算カラー変化処理
		/// </summary>
		private IEnumerator ChangeMultiColorCoroutine(Color color, float changeTime) {

			float currentTime = 0f;
			while(currentTime < changeTime) {
				currentTime += Time.deltaTime;
				float t = currentTime / changeTime;
				Color col = color * t;
				SetMultiColor (col);

				yield return null;
			}
			//色遷移しきっていない部分の直し
			SetMultiColor(color);
			yield break;
		}

		/// <summary>
		/// 加算カラー変化開始
		/// </summary>
		public void StartChangeAddColor(Color color, float changeTime) {
			changeAddColorCoroutine_ = ChangeAddColorCoroutine(color, changeTime);
		}
		/// <summary>
		/// 加算カラー変化停止
		/// </summary>
		public void StopChangeAddColor()
		{
			changeAddColorCoroutine_ = null;
		}
		/// <summary>
		/// 加算カラー線形変化処理
		/// </summary>
		private IEnumerator ChangeAddColorCoroutine(Color color, float changeTime) {
			float currentTime = 0f;
			while(currentTime < changeTime) {
				currentTime += Time.deltaTime;
				float t = currentTime / changeTime;
				Color col = color * t;
				SetAddColor (col);

				yield return null;
			}
			//色遷移しきっていない部分の直し
			SetAddColor(color);
			yield break;
		}

		public void LateExecute() {

			if (changeMultiColorCoroutine_ != null) {
				bool ret = changeMultiColorCoroutine_.MoveNext();
				if (!ret) {
					changeMultiColorCoroutine_ = null;
				}
			}

			if (changeAddColorCoroutine_ != null) {
				bool ret = changeAddColorCoroutine_.MoveNext ();
				if (!ret) {
					changeAddColorCoroutine_ = null;
				}
			}

			if (isHitFlashing_ && Time.time > hitFlashTimer_) {
				isHitFlashing_ = false;
				SetAddColor(Color.clear);
			}
		}
		*/

		// --------------------------------------------------
		// シェーダの切り替え
		// --------------------------------------------------
		//@note シルエットシェーダは排除。使ってないのとカラーの定義名が他と違うので面倒くさいから


		//public const string ShaderName_Default = "Custom/Mobile_Unlit_Color";
		//public const string ShaderName_Custom_Alpha = "Custom/Unlit_Alpha";
		//private static Shader m_defaultShader = null;

		/*public void SetShader(ShaderCache.ShaderType shaderType = ShaderCache.ShaderType.SHADER_TYPE_DEFAULT) {
			SetShader (ShaderCache.GetShader(shaderType));
		}

		private void SetShader(Shader shader) {
			for (int i = 0, max = rendererList_.Count; i < max; i++ ) {
				if (rendererList_[i].material.shader.name == shader.name) continue;
				rendererList_[i].material.shader = shader;
			}
		}

		// ヒットフラッシュ --------------------------------------------------------------------------------------

		private bool isHitFlashing_ = false;
		private float hitFlashTimer_ = 0;

		//　ヒットフラッシュ
		public void HitFlash() {
			SetAddColor(new Color(0.4f,0.4f,0.4f));
			isHitFlashing_ = true;
			hitFlashTimer_ = Time.time + 0.2f;
		}*/

	}
}
