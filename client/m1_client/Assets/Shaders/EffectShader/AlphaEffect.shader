// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/AlphaEffect" 
{
	Properties
	{
	    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Alpha (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		[Enum(additive,1,blend,10)] _DestBlend ("Dest Blend Mode", Float) = 10
		[Toggle(UNITYX2)] _UseX2("X2", Float) = 0
		[Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 0
	    Inten("强度", Range(1, 5)) = 1
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue"="Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off Lighting Off 
			ZWrite [_ZWrite]
			Fog { Mode Off }
			Blend SrcAlpha [_DestBlend]
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITYX2
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			fixed Inten;
			fixed4 _TintColor;
			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	    		return o;
			}

			half4 frag (v2f i) : COLOR
			{
				fixed4 col;  
				
				col.rgb = tex2D(_MainTex, i.texcoord).rgb*Inten;
				col.a = tex2D(_AlphaTex, i.texcoord).r;
				#ifdef UNITYX2
				 fixed alpha = col.a;
				 col = col * i.color.a + col * (1.0 - i.color.a);
				 col += i.color *alpha;
				#endif
				col = col*2.0f * i.color * _TintColor;  
				clip(col.a - _Cutoff);
                return col;  
			}
			ENDCG
		}
	}
	Fallback Off
}
