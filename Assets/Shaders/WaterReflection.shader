﻿// Original by Dan Moran http://danjohnmoran.com/
// Modified by Remi Sezille

Shader "SF/WaterReflection" {
	Properties {
		_MainTex("Albedo", 2D) = "white" {}
		_OffsetTex("Refraction Map", 2D) = "white" {}
		_AmbientTex("Ambient Reflections", 2D) = "black" {}
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"DisableBatching" = "True"
		}

		Pass {
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = ((o.vertex.xy) + 1) * 0.5;

				#if UNITY_UV_STARTS_AT_TOP
				o.screenuv.y = 1 - o.screenuv.y;
				#endif

				o.color = v.color;
				return o;
			}

			float2 safemul(float4x4 M, float4 v) {
				float2 r;

				r.x = dot(M._m00_m01_m02, v);
				r.y = dot(M._m10_m11_m12, v);

				return r;
			}

			sampler2D _MainTex;
			sampler2D _OffsetTex;
			sampler2D _AmbientTex;

			uniform sampler2D _GlobalRefractionTex;
			uniform float _GlobalVisibility;
			uniform float _GlobalRefractionMag;

			float4 frag(v2f i) : SV_Target {
				float4 color = tex2D(_MainTex, i.uv) * i.color;
				float2 offset = safemul(unity_ObjectToWorld, tex2D(_OffsetTex, i.uv) * 2 - 1);
				float4 ambient = tex2D(_AmbientTex, (i.screenuv + offset * _GlobalRefractionMag * 5) * 2);
				float4 worldRefl = tex2D(_GlobalRefractionTex, i.screenuv + offset.xy * _GlobalRefractionMag);

				color.rgb = (color.rgb + ambient.rgb) * (1.0 - worldRefl.a * _GlobalVisibility) + worldRefl.rgb * worldRefl.a * _GlobalVisibility;

				return color;
			}

			ENDCG
		}
	}

	Fallback "Sprites/Default"
}
