// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TL/CameraEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ChromAberrTex ("Chromatic Aberration (RGB)", 2D) = "black" {}
		_LensDirtTex ("Lens Dirt Texture", 2D) = "black" {}
		_DirtIntensity ("Lens Dirt Intensity", Float) = .1
		_ChromaticAberrationOffset("Chromatic Aberration Offset", Float) = 1

		_BloomTex("Bloom (RGBA)", 2D) = "black" {}
		_DOFTex("DOF (RGB), COC(A)", 2D) = "black" {}
		_COCTex("COC Texture (RGBA)", 2D) = "white" {}
//		_DOFStrength("DOF Strength", Float) = .5

		_SCurveIntensity ("S-Curve Intensity", Float) = .5

	}
	
	
	CGINCLUDE
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest

		#pragma multi_compile FXPRO_HDR_ON FXPRO_HDR_OFF

		//+++++++++++++++++++++++++++
		//USER-DEFINED PARAMETERS
		//Those are performance-light, and are defined here to save some keywords.
		#define S_CURVE_ON
		#define VIGNETTING_ON
		#define VIGNETTING_POWER 1		//Larger values result in vignetting being closer to the screen corners
		
		#define FOG_DENSITY 1
		//+++++++++++++++++++++++++++

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;

		#include "FxProInclude.cginc"

		struct v2f_img_aa {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv2 : TEXCOORD1;	//Flipped uv on DirectX platforms to work correctly with AA
		};

		v2f_img_aa vert_img_aa(appdata_img v)
		{
			v2f_img_aa o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.uv2 = v.texcoord;

			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv2.y = 1 - o.uv2.y;
			#endif

			return o;
		}
	ENDCG

	SubShader 
	{
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Blend Off
		
		
		Pass {	//[Pass 0] Bloom/DOF final composite
			name "bloom_dof_composite"
			CGPROGRAM
			#pragma vertex vert_img_aa
			#pragma fragment frag
 	        #pragma target 3.0
 			#pragma multi_compile LENS_DIRT_ON LENS_DIRT_OFF
			#pragma multi_compile DOF_ENABLED DOF_DISABLED
			#pragma multi_compile BLOOM_ENABLED BLOOM_DISABLED
			#pragma  multi_compile DEATH_GRAY DEATH_GRAY_DISABLED

			#ifdef DOF_ENABLED
			sampler2D _DOFTex;
			sampler2D _COCTex;
			#endif
			
			#ifdef BLOOM_ENABLED
			sampler2D _BloomTex;
			#endif

			#ifdef LENS_DIRT_ON
			sampler2D _LensDirtTex;
			half _DirtIntensity;
			#endif

			#ifdef DEATH_GRAY
			uniform fixed _luminAmount;
			#endif

			fixed4 frag ( v2f_img_aa i ) : COLOR
			{
				#ifdef COLOR_FX_ON
				fixed3 mainTex = tex2D(_MainTex, i.uv2).rgb;
				#else
				fixed3 mainTex = tex2D(_MainTex, i.uv).rgb;
				#endif

				#if defined(DOF_ENABLED) || defined(DEPTH_FX_ON)
					fixed3 cocTex = tex2D( _COCTex, i.uv2 ).rgb;
				#endif

				#ifdef DOF_ENABLED
					fixed3 dofTex = tex2D(_DOFTex, i.uv2).rgb;
					fixed3 srcTex = dofTex;

					srcTex = lerp(mainTex, srcTex, cocTex.r);
				#else
					fixed3 srcTex = mainTex;
				#endif

				#ifdef BLOOM_ENABLED
					fixed4 bloomTex = saturate( tex2D(_BloomTex, i.uv2) );

					fixed3 resColor = Screen(srcTex.rgb, bloomTex.rgb);
				#else
					fixed4 bloomTex = fixed4(0, 0, 0, 0);
					fixed3 resColor = srcTex.rgb;
				#endif

				//Convert to LDR
				resColor = saturate(resColor);
				
				#ifdef LENS_DIRT_ON
				fixed3 lensDirtTex = tex2D(_LensDirtTex, i.uv2).rgb;
				resColor = Screen(resColor, saturate(lensDirtTex * max(bloomTex.rgb, srcTex.rgb) * _DirtIntensity));
				//resColor = resColor + saturate(lensDirtTex * max(bloomTex.rgb, srcTex.rgb) * _DirtIntensity);
				#endif

				#ifdef DEATH_GRAY
				float luminosity = 0.299 * resColor.r + 0.587 * resColor.g + 0.114 * resColor.b;
				resColor = lerp(resColor, luminosity, _luminAmount);
				#endif

				return fixed4( resColor, 0 );
			} 
			ENDCG
		}
		
		Pass 	//[Pass 1] Downsample
		{ 	
			CGPROGRAM			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv[4] : TEXCOORD0;
			};
						
			v2f vert (appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				float4 uv;
				uv.xy = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
				uv.zw = 0;

				float offX = _MainTex_TexelSize.x;
				float offY = _MainTex_TexelSize.y;
				
				// Direct3D9 needs some texel offset!
				#ifdef UNITY_HALF_TEXEL_OFFSET
				uv.x += offX * 2.0f;
				uv.y += offY * 2.0f;
				#endif
				o.uv[0] = uv + float4(-offX,-offY,0,1);
				o.uv[1] = uv + float4( offX,-offY,0,1);
				o.uv[2] = uv + float4( offX, offY,0,1);
				o.uv[3] = uv + float4(-offX, offY,0,1);

				return o;
			}
			
			fixed4 frag( v2f i ) : SV_Target
			{
				fixed4 c;
				c  = tex2D( _MainTex, i.uv[0].xy );
				c += tex2D( _MainTex, i.uv[1].xy );
				c += tex2D( _MainTex, i.uv[2].xy );
				c += tex2D( _MainTex, i.uv[3].xy );
				c *= .25f;

				return c;
			}	
			ENDCG		 
		}
		Pass 	//[Pass 2] Motion Blur
		{
			CGPROGRAM
     #pragma vertex vert  
     #pragma fragment frag  
	 #pragma target 3.0
     #include "UnityCG.cginc"  
		
			uniform float SampleDist;
			uniform float Strength;

              static const float samples[6] =   
              {   
                 -0.05,  
                 -0.03,    
                 -0.01,  
                 0.01,    
                 0.03,  
                 0.05,  
              }; 
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				half2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};


			vertexOutput vert(vertexInput Input)
			{
				vertexOutput Output;

				Output.vertex = UnityObjectToClipPos(Input.vertex);

				Output.texcoord = Input.texcoord;
				Output.color = Input.color;

				return Output;
			}

			float4 frag(vertexOutput i) : COLOR
			{
			     float2 dir = float2(0.5, 0.5) - i.texcoord;
                 float2 uv = i.texcoord;
                 float dist = length(dir);  
                 dir = normalize(dir); 
                 float4 color = tex2D(_MainTex, uv);  
             
                 float4 sum = color;
                 for (int i = 0; i < 6; ++i)  
                 {  
                    sum += tex2D(_MainTex, uv+ dir * samples[i] * SampleDist);    
                 }  

                 sum /= 7.0f;  

                 //越离采样中心近的地方，越不模糊
                 float t = saturate(dist * Strength);  

                 return lerp(color, sum, t);
                            
			}
			ENDCG
		}

	}
	
	fallback off
}