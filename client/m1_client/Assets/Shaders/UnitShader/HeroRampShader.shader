// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/HeroRampShader"
{
	Properties{
		_Color("Color Tint",Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _Specular("Specular", Range(0, 1)) = 0.1
		_Shininess("Shininess", Range(0.01, 1)) = 0.5
	    _RimColor("RimColor",Color) = (0,1,1,1)
		_RimPower("Rim Power", Range(0.1,8.0)) = 1.0
		[Toggle(UNITY_GRAY)] _UseGray("Use Gray", Float) = 0

	}
	SubShader{
		Tags{ "Queue" = "Geometry+20" "RenderType" = "Opaque" }
		LOD 200
	Pass
	  {
		Blend SrcAlpha One
		ZWrite off
		Lighting off
		ztest greater
		CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"  

		float4 _RimColor;
	    float _RimPower;

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float4 color:COLOR;
		float4 normal:NORMAL;
	};

	struct v2f {
		float4  pos : SV_POSITION;
		float4  color:COLOR;
	};
	v2f vert(appdata_t v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		float rim = 1 - saturate(dot(viewDir,v.normal));
		o.color = _RimColor*pow(rim,_RimPower);
		return o;
	}
	float4 frag(v2f i) : COLOR
	{
		return i.color;
	}
		ENDCG
	}
	Pass
	{
		//Tags{ "Queue" = "Geometry+20" "RenderType" = "Opaque" }
		Tags{ "LightMode" = "ForwardBase" }
		Lighting On
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

     #pragma vertex vert
     #pragma fragment frag
     
     #pragma fragmentoption ARB_precision_hint_fastest
     #pragma multi_compile __ UNITY_GRAY
     #include "UnityCG.cginc"
     #include "Lighting.cginc"
     #include "AutoLight.cginc" 
     #include "UnityShaderVariables.cginc"
	fixed4 _Color;
	sampler2D _MainTex;
	fixed _Specular;
	fixed _Shininess;

	float4 _MainTex_ST;


	struct a2v
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		float4 tangent : TANGENT;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float3 worldNormal : TEXCOORD1;
		float3 lightDirection : TEXCOORD2;
		float3 worldPos : TEXCOORD3;
		LIGHTING_COORDS(4,5)
	};

	v2f vert(a2v v)
	{
		v2f o;

		TANGENT_SPACE_ROTATION;

		o.lightDirection = mul(rotation, ObjSpaceLightDir(v.vertex));

		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		TRANSFER_VERTEX_TO_FRAGMENT(o);
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		 fixed3 worldNormal = normalize(i.worldNormal);
	     fixed4 c = tex2D(_MainTex, i.uv);
        
        //Based on the ambient light
		fixed3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
        
		fixed atten = LIGHT_ATTENUATION(i);
        
		fixed diff = saturate(dot(worldNormal, normalize(i.lightDirection)));
        
        fixed hLambert = diff * 0.5 + 0.5;
        
        fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
        fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
        fixed3 halfDir = normalize(worldLightDir + viewDir);
        fixed3 specular = pow(max(0, dot(worldNormal, halfDir)), _Shininess)*_Specular;
        fixed3 diffuse = _LightColor0 * atten;

        c.a = _Color.a;
        c.rgb = _Color.rgb * diffuse * c.rgb + specular*c.a;
        #ifdef UNITY_GRAY
		c.rgb  = Luminance(c.rgb );
        #endif 
	    return c;

	}
		ENDCG
	}
	}

}