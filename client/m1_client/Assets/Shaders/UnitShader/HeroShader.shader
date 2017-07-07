// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/HeroShowShader" {
	Properties{
		_Color("Color Tint",Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _Specular("Specular", Range(0, 1)) = 0.1
		_Shininess("Shininess", Range(0.01, 1)) = 0.5
		_FlowTex("FlowTex", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
		_FlowColor("FlowColor",Color) = (1,1,1,1)
		_ScrollXSpeed("XSpeed", Range(0, 10)) = 2
		_ScrollYSpeed("YSpeed", Range(0, 10)) = 0
		_ScrollDirection("Direction", Range(-1, 1)) = -1
		_Illum("Illumin (R)", 2D) = "white" { }
		_EmissionLM("Emission (Lightmapper)", Float) = 0
        _Alpha("Alpha (R)", 2D) = "white" { }
		_DissolveSrc ("DissolveSrc", 2D) = "white" {}
	    _DissolveAmount("DissolveAmount", Range (0, 1)) = 0.5
	    _DissColor ("DissColor", Color) = (1,1,1,1)
        [Toggle(UNITY_GRAY)] _UseGray("Use Gray", Float) = 0
	  }
		SubShader{
		 Tags{ "Queue" = "Geometry+20" "RenderType" = "Opaque" }
		 LOD 200
		 Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
		Tags{ "LightMode" = "ForwardBase" }
		Lighting On
		CGPROGRAM
      
      #pragma vertex vert
      #pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile __ UNITY_GRAY UNITY_ALPHA
      #include "UnityCG.cginc"
      #include "Lighting.cginc"
      #include "AutoLight.cginc" 
      #include "UnityShaderVariables.cginc"
     fixed4 _Color;
    sampler2D _MainTex;
	sampler2D _Illum;
	sampler2D _Alpha;
	fixed _Specular;
	fixed _Shininess;
	fixed _ScrollXSpeed;
	fixed _ScrollYSpeed;
	fixed _ScrollDirection;
	fixed _EmissionLM;
	float4 _MainTex_ST;
	sampler2D _FlowTex;
	float4 _FlowTex_ST;
	fixed4 _FlowColor;
	sampler2D _MaskTex;
	sampler2D _DissolveSrc;
	half4 _DissColor;
	half _DissolveAmount;
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
		float2 uv_FlowTex : TEXCOORD6;
	};

	v2f vert(a2v v)
	{
		v2f o;

		TANGENT_SPACE_ROTATION;

		o.lightDirection = mul(rotation, ObjSpaceLightDir(v.vertex));

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv_FlowTex = TRANSFORM_TEX(v.texcoord, _FlowTex);
		o.uv = v.texcoord;	
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


		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
		fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
		fixed3 halfDir = normalize(worldLightDir + viewDir);
		fixed3 specular = pow(max(0, dot(worldNormal, halfDir)), _Shininess)*_Specular;
		fixed3 diffuse = _LightColor0 * atten;

		fixed2 scrolledUV = i.uv_FlowTex;
		fixed xScrollValue = _ScrollXSpeed * _Time.y;
		fixed yScrollValue = _ScrollYSpeed * _Time.y;
		scrolledUV += fixed2(xScrollValue, yScrollValue) * _ScrollDirection;

		//fixed2 scrolledUV = worldNormal.xy;
		//scrolledUV = scrolledUV*0.5;
		//scrolledUV += _Time.xx * _ScrollXSpeed;
		fixed mask = tex2D(_MaskTex, i.uv).r;
		fixed4 flow = tex2D(_FlowTex, scrolledUV);

		c.rgb = _Color.rgb * diffuse * c.rgb + specular * c.a;
		c.rgb += flow.rgb * _FlowColor.rgb * mask;
		fixed3  Emisson = c.rgb * tex2D(_Illum, i.uv).r*_EmissionLM;
		c.rgb += Emisson;
		 #ifdef UNITY_GRAY
		c.rgb  = Luminance(c.rgb );
        #endif 

		c.a = _Color.a;
		#ifdef UNITY_ALPHA
		  c.a = tex2D(_Alpha, i.uv).r*_Color.a;
        #endif 
		if (_DissolveAmount > 0)
           {
			  half3 Color = float3(1,1,1);
		      float ClipTex = tex2D (_DissolveSrc, i.uv).r ;
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
					c.rgb  = c.rgb  * Color*(Color.r+Color.g+Color.b)*(Color.r+Color.g+Color.b);
				}
			   
			}
	       }
		return c;

	}
		ENDCG
	}
	}
	CustomEditor "AlphaHeroShaderEditor"
}
