// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/DoubleSidedTexure" {
	Properties
	{
		_MainColor("Main Color",Color) = (1,1,1,1)
		_MainTexture("Main Texture",2D) = "Write"{}
		[Toggle(UNITY_GRAY)] _UseGray("Use Gray", Float) = 0
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent-60" "RenderType" = "Opaque"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull off

	pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
	#pragma multi_compile __ UNITY_GRAY
		sampler2D _MainTexture;
		float4 _MainColor;
		struct v2f
		{
			float4 pos:POSITION;
			float4 uv:TEXCOORD;
		};
		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}
		half4 frag(v2f i) :COLOR
		{
			half4 c = tex2D(_MainTexture,i.uv)*_MainColor;
			#ifdef UNITY_GRAY
		    c.rgb  = Luminance(c.rgb);
            #endif 
			return c;
		}
		ENDCG
	}

	}
	//CustomEditor "DoubleTextureShaderGUI"
}