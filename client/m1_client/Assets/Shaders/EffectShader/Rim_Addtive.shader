// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "TL/Rim_Addtive"
{
	Properties
	{
		_RimColor("Rim Color", Color) = (0.5,0.5,0.5,0.5)
		_InnerColor("Inner Color", Color) = (0.5,0.5,0.5,0.5)
		_InnerColorPower("Inner Color Power", Range(0.0,1.0)) = 0.5
		_RimPower("Rim Power", Range(0.0,5.0)) = 2.5
		_AlphaPower("Alpha Rim Power", Range(0.0,8.0)) = 4.0
		_AllPower("All Power", Range(0.0, 10.0)) = 1.0
	}

	SubShader
	{
		Tags
	{
		"Queue" = "Transparent+100"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
	Pass
	{
		ZWrite Off
		Blend SrcAlpha One 
		ColorMask RGB
		CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"

		uniform float4 _LightColor0;
	    float4 _RimColor;
	    float _RimPower;
	    float _AlphaPower;
	    float _AlphaMin;
	    float _InnerColorPower;
	    float _AllPower;
	    float4 _InnerColor;

	struct VertexInput
	{
		float4 vertex : POSITION;
		half4 color : COLOR;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
	};

	struct VertexOutput
	{
		float4 pos : SV_POSITION;
		half4 color : COLOR;
		float4 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
		float4 posWorld : TEXCOORD1;
	};

	VertexOutput vert(VertexInput v)
	{
		VertexOutput o;

		o.texcoord = v.texcoord;
		o.normal = mul(float4(v.normal,0), unity_WorldToObject).xyz;
		o.color = v.color;
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		o.pos = UnityObjectToClipPos(v.vertex);

		return o;
	}

	fixed4 frag(VertexOutput i) : COLOR
	{

		fixed3 ViewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	
		half rim = 1.0 - saturate(dot(normalize(ViewDirection), i.normal));
		fixed3 finalColor = _RimColor.rgb * pow(rim, _RimPower)*_AllPower + (_InnerColor.rgb * 2 * _InnerColorPower);


		fixed alpha = (pow(rim, _AlphaPower))*_AllPower;

		return fixed4(finalColor, alpha)* i.color;
	}

		ENDCG
	}
	}
		Fallback "VertexLit"
}
