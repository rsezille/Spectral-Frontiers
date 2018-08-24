Shader "SF/SpriteDiffuseSpecular" {
	Properties {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Dump("Dump", 2D) = "bump" {}
		_Color("Tint", Color) = (1,1,1,1)
			_Test("Test", Range(0, 1)) = 1.0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader {
		Tags {
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#include "UnitySprites.cginc"

		sampler2D _Dump;
		float _Test;

		struct Input {
			float2 uv_MainTex;
			fixed4 color;
			float2 uv_Dump;
		};

		void vert(inout appdata_full v, out Input o) {
			v.vertex = UnityFlipSprite(v.vertex, _Flip);

			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap(v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color * _RendererColor;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_Dump, IN.uv_Dump));
			//o.Emission= UnpackNormal(tex2D(_Dump, IN.uv_Dump));
			
			o.Gloss = _Test;
		}
		ENDCG
	}

	Fallback "Sprites/Diffuse"
}
