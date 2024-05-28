Shader "PolygonR/PBRMetalRough_Overlay" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,2)) = 1.0
		
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicGlossMap("Metallic (RGB) Gloss (A)", 2D) = "black" {}
	
		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
		//_OcclusionMap("Occlusion Map", 2D) = "white" {}

		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionMap("Emissive", 2D) = "black" {}
		_EmissionFactor("Emissive Factor", Float) = 1.0

		_BumpScale("Normal Factor", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		//Colormask Values
		_ColorMask("Color Mask  (RGB)", 2D) = "black" {}
		_MaskedColorA ("Masked Color 1 (R)", Color) = (1,0,0,1)
		_MaskedColorB ("Masked Color 2 (G)", Color) = (0,1,0,1)
		_MaskedColorC ("Masked Color 3 (B)", Color) = (0,0,1,1)

		_DamageFX("Damage FX", Float) = 0.0
		//_UVScale("UV Scale", Vector) =  (1,1,1,1)
		//_UVWorldScale("UV World Scale", float) = 1.0
		
	}

	

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _MetallicGlossMap,  _EmissionMap, _BumpMap, _Rough; //_OcclusionMap
	
		struct Input {
			float2 uv_MainTex;
			//float3 worldPos;
		};

		half _Glossiness, _Metallic, _EmissionFactor, _BumpScale, _OcclusionStrength, _RoughLerp, _DamageFX;
	
		fixed4 _Color, _EmissionColor;

		//UV Scaler
		//half _UVWorldScale;
		//fixed4 _UVScale;
		
		//Color Mask 
		sampler2D _ColorMask;
		fixed4 _MaskedColorA, _MaskedColorB, _MaskedColorC;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			half2 RescaledUV = IN.uv_MainTex;
			//RescaledUV.x *= _UVScale.x * _UVWorldScale;
			//RescaledUV.y *= _UVScale.y * _UVWorldScale;
			
			fixed4 c = tex2D (_MainTex, RescaledUV) * _Color;
			
			half gray = (c.r + c.g + c.b) / 3;

			fixed4 colorMask = tex2D(_ColorMask, RescaledUV);
			fixed3 FinalMaskedColors = (_MaskedColorA * colorMask.r) + (_MaskedColorB * colorMask.g) + (c.rgb *(_MaskedColorC * colorMask.b));
			//NORMAL
			//c.rgb = lerp(c.rgb, FinalMaskedColors, colorMask.r + colorMask.g +  colorMask.b);
			//MULTIPLY
			//c.rgb = lerp(c.rgb, c.rgb * FinalMaskedColors, colorMask.r + colorMask.g + colorMask.b);
			float3 c2 = lerp(c.rgb, lerp(1 - (2 * (1 - c)) * (1 - FinalMaskedColors), (2 * c) * FinalMaskedColors, step(gray, 0.5f)), colorMask.r + colorMask.g + colorMask.b);

			o.Albedo = c2.rgb;

			float MetallicMap = tex2D(_MetallicGlossMap, RescaledUV).r;
			float GlossMap = tex2D(_MetallicGlossMap, RescaledUV).a;
			//half occPainter = tex2D(_MetallicGlossMap, RescaledUV).b;
						
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic * MetallicMap;

			o.Smoothness = _Glossiness * GlossMap;

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, RescaledUV), _BumpScale);
			
			//half occ = tex2D(_OcclusionMap, RescaledUV).r;
			half occ = tex2D(_MetallicGlossMap, RescaledUV).b;
			o.Occlusion = LerpOneTo(occ , _OcclusionStrength );
			//o.Occlusion = occ * _OcclusionStrength;
			
			o.Alpha = c.a;

			o.Emission = tex2D(_EmissionMap, RescaledUV) * _EmissionColor * _EmissionFactor + _DamageFX;
		}
		ENDCG
	} 

	

	FallBack "Diffuse"
}
