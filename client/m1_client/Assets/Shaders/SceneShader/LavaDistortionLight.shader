Shader "TL/Transparent/LavaDistortionLight" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FlowTex ("Light Texture(A)", 2D) = "black"{}
		_MaskTex ("Mask Texture(A)",2D) = "white"{}
		_ScrollXSpeed("X Scroll Speed", Range(-5, 5)) = 2
	    _ScrollYSpeed("Y Scroll Speed", Range(-5, 5)) = 2
	    _DistortInten("扰动强度", Range(-1, 1)) = 0.4
		_Inten ("强度",Color) = (0.5,0.5,0.5,0.5)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent-10" }
		LOD 200
	    Cull Off  ZWrite Off Fog{ Color(0,0,0,0) }
		//Lighting Off 
		CGPROGRAM
       #pragma surface surf Lambert alpha
        
	    fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _FlowTex;
		sampler2D _MaskTex;
		fixed _ScrollXSpeed;
		fixed _ScrollYSpeed;
		fixed _DistortInten;
		float4 _Inten;
		struct Input {
			float2 uv_MainTex;
			float2 uv_FlowTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex)* _Color;
			half4 Noise = tex2D(_FlowTex, IN.uv_FlowTex);
			float2 scrolledUV = IN.uv_FlowTex;
			fixed xScrollValue = _ScrollXSpeed * _Time + Noise.r*_DistortInten;
			fixed yScrollValue = _ScrollYSpeed * _Time + Noise.g*_DistortInten;

			scrolledUV += fixed2(xScrollValue, yScrollValue);
			
			
			fixed flow = tex2D (_FlowTex, scrolledUV).a;
			float mask = tex2D (_MaskTex, IN.uv_MainTex).a;
			o.Albedo = c.rgb +float3(flow, flow, flow)*_Inten.rgb*mask;
			o.Alpha = c.a;
			//o.Emission = c.rgb*mask;
		}
		ENDCG
	} 
	
}
