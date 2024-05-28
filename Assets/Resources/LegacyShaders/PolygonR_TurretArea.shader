// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PolygonR/TurretArea" {
Properties {
	_BaseColor ("Base Color", Color) = (1.0,1.0,1.0,1.0)
	_AreaColor("Area Color", Color) = (0.5,0.5,0.5,0.5)
	_LineColor("Line Color", Color) = (0.5,0.5,0.5,0.5)

	_MainTex ("Area Display Texture", 2D) = "white" {}
	_Masks("Area Mask Texture", 2D) = "white" {}

	_Angle("Angle", Range(1.0,  179.55)) = 0.0
}

Category {
	Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Masks;
			fixed4 _BaseColor, _AreaColor, _LineColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};
			
			float4 _MainTex_ST;
			float4 _Masks_ST;
			float _Angle;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

				// Pivot
				float2 pivot = float2(0.5, 0.5);
				// Rotation Matrix
				float cosAngle = cos(_Angle / 57.15);
				float sinAngle = sin(_Angle / 57.15);

				float cosAngle2 = cos((_Angle / 57.15) * -1);
				float sinAngle2 = sin((_Angle / 57.15) * -1);

				float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
				float2x2 rot2 = float2x2(cosAngle2, -sinAngle2, sinAngle2, cosAngle2);

				// Rotation consedering pivot
				float2 uv = v.texcoord.xy - pivot;
				o.uv = mul(rot, uv);
				o.uv2 = mul(rot2, uv);
				o.uv += pivot;
				o.uv2 += pivot;

				return o;
			}
	
			fixed4 frag (v2f i) : COLOR
			{
				//Area Mask
				fixed fixedMask = tex2D(_Masks, i.texcoord).r;
				fixed fixedMask2 = tex2D(_Masks, i.texcoord).g;
				fixed mask = tex2D(_Masks, i.uv).r * fixedMask2;
				fixed mask2 = tex2D(_Masks, i.uv2).g * fixedMask;
				fixed finalMask = clamp(0.0, 1.0, mask + mask2);
				//Area
				float4 area = tex2D(_MainTex, i.texcoord).r * _AreaColor * finalMask;
				//Lines
				float4 lineR = tex2D(_MainTex, i.uv).g * _LineColor;
				float4 lineL = tex2D(_MainTex, i.uv2).b * _LineColor;
				float4 finalLines = (lineR + lineL) * (1 - (_Angle / 179.55));
				float4 finalColor = area + finalLines;

				return 2.0f * i.color * _BaseColor * finalColor ;
				//return finalMask * 2.0f;
			}
			ENDCG 
		}
	}	
}
}
