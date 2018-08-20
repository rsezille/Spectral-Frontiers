// Original GLSL script: https://www.shadertoy.com/view/XsXSWS
// Converted from GLSL by Remi Sezille

Shader "Shadertoy/Fire" {
	Properties{}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float2 hash(float2 p) {
				p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));

				return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
			}

			float noise(in float2 p) {
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
				//uv = mul(m * uv);
				f += 0.2500 * noise(uv);
				//uv = m * uv;
				f += 0.1250 * noise(uv);
				//uv = m * uv;
				f += 0.0625 * noise(uv);
				//uv = m * uv;
				f = 0.5 + 0.5 * f;

				return f;
			}

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 position : SV_POSITION;
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);

				return o;
			}

			// no defines, standard redish flames
			//#define BLUE_FLAME
			//#define GREEN_FLAME

			fixed4 frag(v2f i) : SV_TARGET{
				float2 uv = i.position.xy / _ScreenParams.xy;
				float2 q = uv;
				q.x = mul(q.x, 5.0);
				q.y = mul(q.y, 2.0);
				float strength = floor(q.x + 1.0);
				float T3 = max(3.0, 1.25 * strength) * _Time.y;
				q.x = fmod(q.x, 1.0) - 0.5;
				q.y = q.y - 0.25;
				float n = fbm(strength * q - float2(0, T3));
				float c = 1. - 16. * pow(max(0., length(q * float2(1.8 + q.y * 1.5, 0.75)) - n * max(0.0, q.y + 0.25)), 1.2);

				float c1 = n * c * (1.5 - pow(2.50 * uv.y, 4.0));
				c1 = clamp(c1, 0.0, 1.0);

				float3 col = float3(1.5 * c1, 1.5 * c1 * c1 * c1, c1 * c1 * c1 * c1 * c1 * c1);

				/*#ifdef BLUE_FLAME
				col = col.zyx;
				#endif
				#ifdef GREEN_FLAME
				col = 0.85*col.yxz;
				#endif*/

				float a = c * (1. - pow(uv.y, 3.0));

				return half4(lerp(float3(0.0, 0.0, 0.0), col, a), 1.0);
			}

			ENDCG
		}
	}
}
