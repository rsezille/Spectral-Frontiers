// Inspired by https://www.shadertoy.com/view/XsXSWS
// By Remi Sezille

Shader "SF/Fire" {
	Properties {
		_MainTex("Texture", 2D) = "black" {}
		_Intensity("Intensity", Range(0, 10)) = 4.0
		_Blur("Blur", Range(0, 10)) = 1.0
		_Speed("Speed", Range(0, 10)) = 1.0
		_Opacity("Opacity", Range(0, 1)) = 1.0
		_Test("Test", Range(0, 100)) = 1.0
	}

	SubShader {
		//Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
		//Blend SrcAlpha One
		//Blend One Zero
			//Blend SrcAlpha OneMinusSrcAlpha
		//Blend One SrcAlpha
		LOD 200

			//Blend SrcAlpha OneMinusSrcColor why not
			//Blend SrcAlpha One why not

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"



			float2 hash(float2 p) {
				p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));

				return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
			}
			
			float noise(float2 p) {
				static const float K1 = 0.366025404; // (sqrt(3)-1)/2;
				static const float K2 = 0.211324865; // (3-sqrt(3))/6;

				float2 i = floor(p + (p.x + p.y) * K1);

				float2 a = p - i + (i.x + i.y) * K2;
				float2 o = (a.x > a.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
				float2 b = a - o + K2;
				float2 c = a - 1.0 + 2.0 * K2;

				float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);

				float3 n = h * h * h * h * float3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));

				return dot(n, float3(70.0, 70.0, 70.0));
			}

			float fbm(float2 uv) {
				float f;

				float2x2 m = float2x2(1.6, 1.2, -1.2, 1.6);

				f = 0.5000 * noise(uv);
				uv = mul(m, uv);
				f += 0.2500 * noise(uv);
				uv = mul(m, uv);
				f += 0.1250 * noise(uv);
				uv = mul(m, uv);
				f += 0.0625 * noise(uv);
				uv = mul(m, uv);
				f = 0.5 + 0.5 * f;

				return f;
			}

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Intensity;
			float _Blur;
			float _Speed;
			float _Opacity;
			float _Test;

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			// no defines, standard redish flames
			//#define BLUE_FLAME
			//#define GREEN_FLAME

			fixed4 frag(v2f i) : SV_TARGET {
				//float2 uv = i.position.xy / _ScreenParams.xy;
				float2 uv = i.uv;
				float2 q = uv;
				//q.x = mul(q.x, 5.0);
				q.y = mul(q.y, 2.0);
				//float strength = floor(q.x + 1.0);
				float strength = _Blur;
				float T3 = max(3.0, 1.25 * strength) * _Time.y * _Speed;
				q.x = fmod(q.x, 1.0) - 0.5;
				q.y = q.y - 0.25;
				float n = fbm(strength * q - float2(0, T3));
				float c = 1.0 - 16.0 * pow(max(0., length(q * float2(1.8 + q.y * 1.5, 0.75)) - n * max(0.0, q.y + 0.25)), 1.2);

				//float c1 = n * c * (1.5 - pow(2.50 * uv.y, _Intensity));
				float c1 = n * c * (1.5 - pow(uv.y, _Intensity));
				//float c1 = n * c * (1.5 - 1.0);
				//float c1 = n * c * (0.5);
				c1 = clamp(c1, 0.0, 1.0);

				float3 col = float3(1.5 * c1, 1.5 * c1 * c1 * c1, c1 * c1 * c1 * c1 * c1 * c1) * _Opacity;
				//float3 col = float3(0.0, 0.0, 0.0);


				#ifdef BLUE_FLAME
				col = col.zyx;
				#endif
				#ifdef GREEN_FLAME
				col = _Test*col.yxz;
				#endif
				 
				float a = c * (1. - pow(uv.y, 3.0));
				//float a = _Test * pow(uv.y, 3.0);
				/*if (col.x == 0.0 && col.y == 0.0 && col.z == 0.0) {
					return half4(0.0, 0.0, 0.0, 0.0);
				}*/
				//return half4(lerp(float3(0.0, 0.0, 0.0), col, a), 1.0);
				//return half4(lerp(float3(0.0, 0.0, 0.0), col, a), _Test);
				float blackComponent = (col.r + col.g + col.b) / 3.0;
				col.r = col.r * (1 + blackComponent) * (1 + blackComponent) * (1 + blackComponent);

				return half4(lerp(float3(0.0, 0.0, 0.0), col, a), blackComponent);
				//return half4(lerp(float3(uv.x, uv.y, 0.0), col, a), 1.0);
			}

			ENDCG
		}
	}
}
