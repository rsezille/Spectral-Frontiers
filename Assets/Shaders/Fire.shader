// Inspired by https://www.shadertoy.com/view/XsXSWS
// By Remi Sezille

Shader "SF/Fire" {
	Properties {
		//_MainTex("Texture", 2D) = "white" {}
		_Intensity("Intensity", Range(0, 2)) = 1.0
		_IntensityStrong("Intensity Strong", Range(0, 5)) = 1.0
		_Luminosity("Luminosity", Range(1, 2)) = 1.5
		_Detailed("Detailed", Range(0, 10)) = 1.0
		_Speed("Speed", Range(0, 10)) = 1.0
		_Opacity("Opacity", Range(0, 1)) = 1.0
		_Stretching("Stretching", Range(0, 3)) = 2.0
		_Shape("Shape", Range(0, 100)) = 1.5
		_Width("Width", Range(0, 2)) = 0.25
		_Height("Height", Range(0, 10)) = 0.75
		_Border("Border", Range(0.01, 4)) = 1.2
		_Size("Size", Range(0, 10)) = 1.0
		//_Black("Black", Range(0, 10)) = 5.0
		//_BlackBorder("BlackBorder", Range(0, 2)) = 0.0
		_Test("Test", Range(0, 10)) = 1.0
		_Random("Random", Range(0, 10)) = 1.0
	}

	SubShader {
		Tags{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"DisableBatching" = "True"
		}

		Pass {
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

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
				half4 color : COLOR;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			//sampler2D _MainTex;
			//float4 _MainTex_ST;
			float _Intensity;
			float _Luminosity;
			float _IntensityStrong;
			float _Detailed;
			float _Speed;
			float _Opacity;
			float _Stretching;
			float _Shape;
			float _Width;
			float _Height;
			float _Border;
			float _Size;
			//float _Black;
			//float _BlackBorder;
			float _Test;
			float _Random;

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
				o.color = v.color;

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {
				float2 q = i.uv;
				q.y = q.y * _Stretching;

				float T3 = max(3.0, 1.25 * _Detailed) * _Time.y * _Speed;
				q.x = fmod(q.x, 1.0) - 0.5;
				q.y = q.y - 0.25;
				float n = fbm(_Detailed * q - float2(0, T3 - _Random)) * _IntensityStrong;
				float c = 1.0 - 16.0 * pow(max(0.0, length(q * float2(_Test + q.y * _Shape, _Height) * _Size) - n * max(0.0, q.y + _Width)), _Border);

				float c1 = n * c * (_Luminosity - pow(i.uv.y, _Intensity));
				c1 = clamp(c1, 0.0, 1.0);
				
				float3 col = float3(1.5 * c1, 1.5 * c1 * c1 * c1, c1 * c1 * c1 * c1 * c1 * c1) * _Opacity;
				 
				//float a = c * (1.0 - pow(i.uv.y, 3.0)) * _Luminosity * i.color;


				//float a = _Luminosity;
				float blackComponent = (col.r + col.g + col.b) / 3.0;

				//col.r = col.r + ((blackComponent) * _Black);
				//col = lerp(col, pow(float3(1.0 - clamp(c1, -1.0, 0.0), 1.0 - clamp(c1, -1.0, 0.0), 1.0 - clamp(c1, -1.0, 0.0)) * pow(fbm(_Detailed * q * 1.25 - float2(0, T3)), 2.0), float3(2.0, 2.0, 2.0)),
				//	(0.75 - (col.x + col.y + col.z) / 3.0) * _BlackBorder);
				col.r = col.r * (1.25 / blackComponent);

				col = col * i.color; // Let the user to tune the color by changing the material's one

				//return tex2D(_MainTex, float2(col.x, col.y));
				return half4(col.x, col.y, col.z, blackComponent);
				//return half4(lerp(float3(0.0, 0.0, 0.0), col, a), blackComponent);
			}

			ENDCG
		}
	}
}
