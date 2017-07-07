// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/Distortion/ParticlesDistort" {
Properties {
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Distort ("Distort (RGB)", 2D) = "black" {}
		_BumpAmt ("Distortion", Float) = 10
}

Category {

	Tags { "Queue"="Transparent"  "IgnoreProjector"="True"  "RenderType"="Opaque" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Mode Off}

	SubShader {

		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
	float2 uvcutout : TEXCOORD3;
	fixed4 color : COLOR;

};

sampler2D _MainTex;
sampler2D _CutOut;
sampler2D _Distort;

float _BumpAmt;
sampler2D _GrabTextureMobile;
float4 _GrabTextureMobile_TexelSize;
fixed4 _TintColor;


float4 _Distort_ST;
float4 _MainTex_ST;
float4 _CutOut_ST;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uvgrab = ComputeScreenPos(o.vertex);
	o.color = v.color;
	o.uvbump = TRANSFORM_TEX( v.texcoord, _Distort);
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.uvcutout = TRANSFORM_TEX( v.texcoord, _CutOut );
	
	return o;
}

sampler2D _CameraDepthTexture;

half4 frag( v2f i ) : COLOR
{
	half4 bump = tex2D( _Distort, i.uvbump );
	float2 offset = bump.xy * _BumpAmt * _GrabTextureMobile_TexelSize.xy;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	
	half4 col = tex2Dproj( _GrabTextureMobile, UNITY_PROJ_COORD(i.uvgrab));

	fixed4 emission = col * i.color ;
    emission.a = _TintColor.a * i.color.a ;
	return emission;
}
ENDCG
		}
	}


}

}

