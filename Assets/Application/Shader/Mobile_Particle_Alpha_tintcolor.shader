// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Effect/Mobile_Particle_Alpha_tintcolor" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}
Category {
Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ZWrite Off
	Cull Off
	BindChannels {
	Bind "color", Color
	Bind "texcoord", TexCoord
}
SubShader {
	Pass {
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile_particles
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	float4 _TintColor;
	struct appdata_t {
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		#ifdef SOFTPARTICLES_ON
		float4 projPos : TEXCOORD1;
		#endif
	};
	float4 _MainTex_ST;
	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		#ifdef SOFTPARTICLES_ON
		o.projPos = ComputeScreenPos (o.vertex);
		COMPUTE_EYEDEPTH(o.projPos.z);
		#endif
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		return o;
	}
	sampler2D _CameraDepthTexture;
	float _InvFade;
	half4 frag (v2f i) : COLOR
	{
		#ifdef SOFTPARTICLES_ON
		float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
		float partZ = i.projPos.z;
		float fade = saturate (_InvFade * (sceneZ-partZ));
		i.color.a *= fade;
		#endif
		return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
	}
	ENDCG
	}
}
SubShader {
Pass {
SetTexture [_MainTex] {constantColor [_TintColor] combine constant * primary}
SetTexture [_MainTex] {combine texture * previous DOUBLE}
}
}
}
}