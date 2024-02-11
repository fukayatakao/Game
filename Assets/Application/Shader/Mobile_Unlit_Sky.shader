Shader "Custom/FieldMap/Unlit/Mobile_Unlit_Sky" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		//SkyBoxなのでキューをBackgroundにして最初に描画させる。Zテスト不要で常に描く。
		Tags {"Queue"="Background" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha 
		Pass {
			Lighting Off
			SetTexture [_MainTex] 
			{ 
                constantColor [_Color]
				combine texture * constant
			} 
		}
	}
}