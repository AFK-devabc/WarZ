// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PolygonR/PBRMetalRough_MultiplyOccluded" {
	Properties {
		_OccludedColor("Occluded Color", Color) = (1,1,1,1)
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
		
		_TemperatureMask("Temperature Mask  (RGB)", 2D) = "white" {}
		_FrozenColor ("Fronzen Color 1 (R)", Color) = (0.5,0.6,0.7,1)
		_BurningColor ("Burning Color 1 (G)", Color) = (0.8,0.3,0.1,1)
		_FrozenMix ("Fronzen Mix", Range(0,1)) = 0.0
		_BurningMix ("Burning Mix", Range(0,1)) = 0.0
		_TemperatureVFXScale("UV Temperature Scale", float) = 4.0
	}

	

	SubShader {
		
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		LOD 200
		ZWrite On
        ZTest LEqual

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _MetallicGlossMap,  _EmissionMap, _BumpMap, _Rough; //_OcclusionMap
	
		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness, _Metallic, _EmissionFactor, _BumpScale, _OcclusionStrength, _RoughLerp, _DamageFX, _FrozenMix, _BurningMix, _TemperatureVFXScale;
	
		fixed4 _Color, _EmissionColor, _FrozenColor ,_BurningColor;

		//Color Mask 
		sampler2D _ColorMask, _TemperatureMask;
		fixed4 _MaskedColorA, _MaskedColorB, _MaskedColorC;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			half2 RescaledUV = IN.uv_MainTex;

			fixed tempMask = tex2D(_TemperatureMask, RescaledUV * _TemperatureVFXScale).r;

			half2 FireUVs = (IN.uv_MainTex * _TemperatureVFXScale) + half2(0,-_Time.x * 4) + tempMask * 0.2;

			fixed4 c = tex2D (_MainTex, RescaledUV) * _Color;
			
			fixed4 colorMask = tex2D(_ColorMask, RescaledUV);
			
			fixed fireMask = tex2D(_TemperatureMask, FireUVs * 2).g;
			fixed3 FinalMaskedColors = (_MaskedColorA * colorMask.r) + (_MaskedColorB * colorMask.g) + (c.rgb *(_MaskedColorC * colorMask.b));

			c.rgb = lerp(c.rgb, c.rgb * FinalMaskedColors, colorMask.r + colorMask.g + colorMask.b);

			fixed3 fronzenGrey = clamp((c.r + c.g + c.b),0.3,0.95) * _FrozenColor;
			fixed3 burningGrey = clamp((c.r + c.g + c.b),0.1,0.75) * _BurningColor;

			c.rgb = lerp(c.rgb, tempMask + fronzenGrey, _FrozenMix);
		
			o.Albedo = lerp(c.rgb, burningGrey, _BurningMix);

			float MetallicMap = tex2D(_MetallicGlossMap, RescaledUV).r;
			float GlossMap = tex2D(_MetallicGlossMap, RescaledUV).a;
						
			// Metallic and smoothness come from slider variables
			o.Metallic = (_Metallic * MetallicMap) - _FrozenMix;

			o.Smoothness = (_Glossiness * GlossMap) + (_FrozenMix * 0.5);

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, RescaledUV), _BumpScale);
			
			half occ = tex2D(_MetallicGlossMap, RescaledUV).b;
			o.Occlusion = LerpOneTo(occ , _OcclusionStrength );
			
			o.Alpha = c.a;
			
			o.Emission = (smoothstep(_BurningColor, half3(0,0,0), fireMask) * _BurningMix) + (tex2D(_EmissionMap, RescaledUV) * _EmissionColor * _EmissionFactor + _DamageFX);
		}
		ENDCG
	} 
	
	FallBack "Diffuse"
}
