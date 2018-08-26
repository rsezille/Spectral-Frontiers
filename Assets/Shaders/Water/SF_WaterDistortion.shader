// By Remi Sezille

Shader "SF/Water/Water Distortion" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
		_DistortionTex("Normalmap", 2D) = "bump" {}
		_Magnitude("Magnitude", Range(0,1)) = 0.05
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uvdisto : TEXCOORD1;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float2 uvdisto : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DistortionTex;
			float4 _DistortionTex_ST;
			float _Magnitude;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float speed = 0.5;

				o.uvdisto = v.uv.xy * _DistortionTex_ST.xy + float2(- _Time.x * speed, - _Time.x * speed);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				half4 bump = tex2D(_DistortionTex, i.uvdisto);

				half2 distortion = UnpackNormal(bump).rg;
				i.uv.xy += distortion * _Magnitude;

				half4 col = tex2D(_MainTex, i.uv);

				return col;
			}

			ENDCG
		}
	}

	Fallback "Sprites/Default"
}
