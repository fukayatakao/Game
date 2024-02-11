Shader "Custom/Mobile_Unlit_Color" {
	Properties {
		_MultiColor ("MultiColor", Color) = (1,1,1,1)
		_AddColor ("AddColor", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	Category {
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		ZWrite On
		Cull Back
		SubShader {
			Pass {
				SetTexture[_MainTex] {
					constantColor[_MultiColor]
					Combine texture * constant , texture * constant
				}
				SetTexture[_MainTex] {
					constantColor[_AddColor]
					Combine previous + constant , previous + constant
				}
			}
		}
	}
}