// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PolygonR/Particles Smoke" {
Properties {
	_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_SecondLayerColor ("Second Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	_Factor ("Add Factor", Float) = 1.0
	_Speed ("Laser Speed", Float) = -0.5
	_Speed2 ("Laser Speed 2", Float) = 0.1
	
	_Distort ("Distort Texture", 2D) = "white" {}
	_DistortSpeed ("Distort Speed", Float) = -0.5
	_DistortFactor ("Distort Factor", Range(-4,4)) = 1.0
	
	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex, _Distort;
			fixed4 _Color, _SecondLayerColor;
			half _Factor, _Speed, _Speed2, _DistortSpeed, _DistortFactor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				
				float2 texcoord2 : TEXCOORD1;
				float2 texcoord3 : TEXCOORD2;
				float2 texcoord4 : TEXCOORD3;
				float2 texcoord5 : TEXCOORD4;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD5;
				#endif
			};
			
			float4 _MainTex_ST;
			float4 _Distort_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o); 

				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				float AnimUV = (v.texcoord.x + _Time.x * _Speed);
				float AnimUV2 = ( v.texcoord.x  + _Time.y * ( _Speed * _Speed2));
				
				//float AnimUV3 = (v.texcoord.x + ( _Time * _DistortSpeed));
				//float AnimUV4 = -( v.texcoord.x  + (_Time.y *  _DistortSpeed));
				
				float AnimUV3 = (v.texcoord.x + ( _Time.y * _DistortSpeed));
				float AnimUV4 = -( v.texcoord.x  + (_Time.y *  _DistortSpeed));
				
				o.texcoord = TRANSFORM_TEX(float2(AnimUV ,v.texcoord.y ) ,_MainTex);
				o.texcoord2 = TRANSFORM_TEX(float2(AnimUV2 ,v.texcoord.y ) ,_MainTex);
				
				o.texcoord3 = TRANSFORM_TEX(float2(AnimUV3 ,v.texcoord.y ) ,_MainTex);
				o.texcoord4 = TRANSFORM_TEX(float2(AnimUV4 ,v.texcoord.y ) ,_MainTex);
				o.texcoord5 = TRANSFORM_TEX(v.texcoord ,_MainTex);
				
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * (sceneZ-partZ) * 2);
					i.color.a *= fade;
				#endif
				
				float4 DistortMask1 = tex2D(_Distort, i.texcoord3);
				float4 DistortMask2 = tex2D(_Distort, i.texcoord4);
				
				float4 DistortMask = ((DistortMask2 * DistortMask1) * 0.2);
				
				half2 UV1 = i.texcoord + ((DistortMask - 0.1 ) * _DistortFactor);
				half2 UV2 = i.texcoord2 + ((DistortMask - 0.1 ) * _DistortFactor);
				
				UV1.y = clamp(UV1.y + (_DistortFactor * 0.05), 0.0, 1.0) ;
				UV2.y = clamp(UV2.y + (_DistortFactor * 0.05), 0.0, 1.0) ;
				
				//UV1.x = clamp(UV1.x + (_DistortFactor ), 0.0, 1.0) ;
				//UV2.x = clamp(UV2.x + (_DistortFactor ), 0.0, 1.0) ;;
				
				UV2.y *= -1;
				
				float Mask = tex2D(_MainTex, i.texcoord5).a;
				
				float4 ColorTexture = tex2D(_MainTex, UV1  )  ;
				float4 ColorTexture2 = tex2D(_MainTex, i.texcoord5 );
				
				//float4 FinalColor = ((i.color * _Color * ColorTexture) + (ColorTexture2 * (_SecondLayerColor * 2)) + (ColorTexture * _Color)) * _Factor;
				
				float4 FinalColor = ((i.color * _Color * ( (ColorTexture2 + ColorTexture) * 0.5)) );
				
				FinalColor.a = ((i.color.a * _Color.a * ColorTexture.a) + (ColorTexture2.a * (_SecondLayerColor.a * 2))) * _Factor;
				
				//FinalColor.a = ((i.color.a * _Color.a * Mask)) ;

				FinalColor.a *= Mask * i.color.a;
				
				FinalColor = clamp(FinalColor, 0, 1);

				return FinalColor;
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_Color]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
