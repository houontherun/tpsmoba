// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/Transparent/LavaDistortion"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	    _FlowTex("FlowTex", 2D) = "black"{}
	    _MaskTex ("Mask Texture(A)",2D) = "white"{}
		_ScrollXSpeed("X Scroll Speed", Range(-5, 5)) = 2
	    _ScrollYSpeed("Y Scroll Speed", Range(-5, 5)) = 2
	    _DistortInten("扰动强度", Range(-1, 1)) = 0.4
		_Inten ("强度",Color) = (0.5,0.5,0.5,0.5)
		 [Toggle(UNITY_Alpha)] _fAlphaTex("透明效果", Float) = 0
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+100" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull off ZWrite Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile __ UNITY_Alpha
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
			    float2 uv1 : TEXCOORD1;
			    float2 Maskuv : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FlowTex;
			float4 _FlowTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			fixed _ScrollXSpeed;
			fixed _ScrollYSpeed;
			fixed _DistortInten;
			float4 _Inten;
			float _fAlphaTex;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv, _FlowTex);
				o.Maskuv = TRANSFORM_TEX(v.uv, _MaskTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col ;
			    half4 c = tex2D(_MainTex, i.uv)* _Color;
			    half4 Noise = tex2D(_FlowTex, i.uv1);
			    float2 scrolledUV = i.uv1;
			    fixed xScrollValue = _ScrollXSpeed * _Time + Noise.r*_DistortInten;
			    fixed yScrollValue = _ScrollYSpeed * _Time + Noise.g*_DistortInten;
			    
			    scrolledUV += fixed2(xScrollValue, yScrollValue);
			    
			    
			    fixed flow = tex2D(_FlowTex, scrolledUV).a;
			    float mask = tex2D(_MaskTex, i.Maskuv).r;
			    col.rgb = c.rgb + float3(flow, flow, flow)*_Inten.rgb*mask;
			    col.a = c.a;
			    #ifdef UNITY_Alpha
			     	col.a = c.a*mask;
			     #endif
				return col;
			}
			ENDCG
		}
	}
}
