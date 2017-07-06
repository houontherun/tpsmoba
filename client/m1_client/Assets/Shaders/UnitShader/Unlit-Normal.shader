// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "TL/Unlit_Texture" {
Properties {
    _Color("Color Tint",Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_DissolveSrc ("DissolveSrc", 2D) = "white" {}
	_DissolveAmount("DissolveAmount", Range (0, 1)) = 0.5
	_DissColor ("DissColor", Color) = (1,1,1,1)
	[Toggle(UNITY_GRAY)] _UseGray("Use Gray", Float) = 0
}

SubShader {
Tags{ "Queue" = "Geometry+20" "IgnoreProjector" = "True"  "RenderType" = "Opaque" }
	Pass {  
	  LOD 200
	  Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_compile __ UNITY_GRAY
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _DissolveSrc;
			half4 _DissColor;
			float4 _MainTex_ST;
			half _DissolveAmount;
			float4 _Color;
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 col = tex2D(_MainTex, i.texcoord).rgb*_Color.rgb;
				#ifdef UNITY_GRAY
		        col  = Luminance(col);
                #endif 
				fixed alpha = _Color.a;
				if (_DissolveAmount > 0)
                 {
				    half3 Color = float3(1,1,1);
		            float ClipTex = tex2D (_DissolveSrc, i.texcoord).r ;
				    float ClipAmount = ClipTex - _DissolveAmount;
				    if (ClipAmount <0)
	                {
		              clip(-0.1);
		            }
					 else
					 { 
					    if (ClipAmount < 0.10)
		                {
			             	Color.r = ClipAmount/0.10;
							Color.g = _DissColor.y;
							Color.b = _DissColor.z;
							col = col * Color*(Color.r+Color.g+Color.b)*(Color.r+Color.g+Color.b);
						}
					   
					 }
				 }
			return fixed4(col,alpha);
			}
		ENDCG
	}
}

}
