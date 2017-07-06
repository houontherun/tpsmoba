// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/MobileWater"
{
	Properties
	{
		_WaterTex("Normal Map (RGB), Foam (A)", 2D) = "white" {}
	     _ReflectionTex("Reflection", 2D) = "white" {}
	     _Color0("Shallow Color", Color) = (1,1,1,1)
		_Color1("Deep Color", Color) = (0,0,0,0)
		_Specular("Specular", Color) = (0,0,0,0)
		_FoamColor("FoamColor", Color) = (0,0,0,0)
		_Shininess("Shininess", Range(0.01, 1.0)) = 1.0
		_Tiling("Tiling", Range(0.025, 0.25)) = 0.25
		_ReflectionTint("Reflection Tint", Range(0.0, 1.0)) = 0.8
		_Speedx("SpeedX", Range(0, 5)) = 2
		_Speedy("SpeedY", Range(0, 5)) = 2
		_InvRanges("Inverse Alpha, Depth and Color ranges", Vector) = (1.0, 0.17, 0.17, 0.0)
	}


		CGINCLUDE
		//#pragma target 3.0
#include "UnityCG.cginc"

	half4 _Color0;
	half4 _Color1;
	half4 _FoamColor;
	half4 _Specular;
	float _Shininess;
	float _Tiling;
	float _ReflectionTint;
	float _Speedx;
	float _Speedy;
	half4 _InvRanges;
	sampler2D _ReflectionTex;

	sampler2D_float _CameraDepthTexture;
	sampler2D_float _WaterTex;

	half4 LightingPPL(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
	{
		half3 nNormal = normalize(s.Normal);
		half shininess = s.Gloss * 250.0 + 4.0;

#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
#endif
		// Phong shading model
		half reflectiveFactor = max(0.0, dot(-viewDir, reflect(lightDir, nNormal)));

		// Blinn-Phong shading model
		//half reflectiveFactor = max(0.0, dot(nNormal, normalize(lightDir + viewDir)));

		half diffuseFactor = max(0.0, dot(nNormal, lightDir));
		half specularFactor = pow(reflectiveFactor, shininess) * s.Specular;

		half4 c;
		c.rgb = (s.Albedo * diffuseFactor + _Specular.rgb * specularFactor) * _LightColor0.rgb;
		c.rgb *= (atten * 2.0);
		c.a = s.Alpha;
		return c;
	}
	ENDCG

		SubShader
	{
		Lod 200
		Tags{ "Queue" = "Transparent-10" }
		//	Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off

		CGPROGRAM
#pragma surface surf PPL alpha vertex:vert nolightmap noforwardadd halfasview 

	struct Input
	{
		float4 position  : POSITION;
		float2 uv_WaterTex : TEXCOORD0;
		float3 worldPos  : TEXCOORD2;	// Used to calculate the texture UVs and world view vector
		float4 proj0   	 : TEXCOORD3;	// Used for depth and reflection textures
	};

	void vert(inout appdata_full v, out Input o)
	{
	    UNITY_INITIALIZE_OUTPUT(Input,o);
		o.worldPos = v.vertex.xyz;
		o.position = UnityObjectToClipPos(v.vertex);
		o.proj0 = ComputeScreenPos(o.position);
		COMPUTE_EYEDEPTH(o.proj0.z);

	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		float3 worldView = (IN.worldPos - _WorldSpaceCameraPos);

		half2 offset = half2(_Time.x * _Speedx*0.1, _Time.y * _Speedy*0.1);
		half2 tiling = IN.uv_WaterTex * _Tiling*100;
		half4 nmap = (tex2D(_WaterTex, tiling + offset) + tex2D(_WaterTex, half2(-tiling.y, tiling.x) - offset)) * 0.5;
		o.Normal = nmap.xyz * 2.0 - 1.0;

		// World space normal (Y-up)
		half3 worldNormal = o.Normal.xzy;
		worldNormal.z = -worldNormal.z;

		half4 projTC = UNITY_PROJ_COORD(IN.proj0);
		// Calculate the depth difference at the current pixel
		float depth = tex2Dproj(_CameraDepthTexture, projTC).r;
		depth = LinearEyeDepth(depth);
		depth -= IN.proj0.z;

		// Calculate the depth ranges (X = Alpha, Y = Color Depth)
		half3 ranges = saturate(_InvRanges.xyz * depth);
		ranges.y = 1.0 - ranges.y;
		ranges.y = lerp(ranges.y, ranges.y * ranges.y * ranges.y, 0.5);

		// Calculate the color tint
		half4 col;
		col.rgb = lerp(_Color1.rgb, _Color0.rgb, ranges.y);
		col.a = ranges.x;

		// Initial material properties
		o.Specular = col.a;
		o.Gloss = _Shininess;

		// Dot product for fresnel effect
		half fresnel = sqrt(1.0 - dot(-normalize(worldView), worldNormal));

		IN.proj0.xy += o.Normal.xy * 0.5;
		half3 reflection = tex2Dproj(_ReflectionTex, projTC).rgb;
		reflection = lerp(reflection * col.rgb, reflection, fresnel * _ReflectionTint);


		half3 refraction = lerp(col.rgb, col.rgb * col.rgb, ranges.y);
		o.Alpha = 1.0 - ranges.y * ranges.y;

		// Color the refraction based on depth
		refraction = lerp(lerp(col.rgb, col.rgb * refraction, ranges.y), refraction, ranges.y);

		// The amount of foam added (35% of intensity so it's subtle)
		half foam = nmap.a * (1.0 - abs(col.a * 2.0 - 1.0)) * 0.35;

		// Always assume 20% reflection right off the bat, and make the fresnel fade out slower so there is more refraction overall
		fresnel *= fresnel * fresnel;
		fresnel = (0.8 * fresnel + 0.2) * col.a;

		// Calculate the initial material color
		o.Albedo = lerp(refraction, reflection, fresnel) + foam*ranges.z*_FoamColor;

		// Calculate the amount of illumination that the pixel has received already
		// Foam is counted at 50% to make it more visible at night
		fresnel = min(1.0, fresnel + foam * 0.5);
		o.Emission = o.Albedo * (1.0 - fresnel);

		// Set the final color
#ifdef USING_DIRECTIONAL_LIGHT
		o.Albedo *= fresnel;
#else
		// Setting it directly using the equals operator causes the shader to be "optimized" and break
		o.Albedo = lerp(o.Albedo.r, 1.0, 1.0);
#endif
	}
	ENDCG
	}
		Fallback Off

}
