Shader "SF/Sprites/Diffuse with Outline" {
	Properties {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		[MaterialToggle] _IsOutlineEnabled("Enable Outline", float) = 0
		[HDR] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		_AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.01
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#pragma vertex ComputeVertex
			#pragma fragment ComputeFragment
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			fixed4 _OutlineColor;
			float _IsOutlineEnabled;
			float _AlphaThreshold;
			fixed4 _RendererColor;
			float _EnableExternalAlpha;

			sampler2D _MainTex, _AlphaTex;
			float4 _MainTex_TexelSize;
			fixed4 _Color;

			struct VertexInput {
				float4 Vertex : POSITION;
				float4 Color : COLOR;
				float2 TexCoord : TEXCOORD0;
				float3 Normal : NORMAL;
			};

			struct VertexOutput {
				float4 Vertex : SV_POSITION;
				fixed4 Color : COLOR;
				float2 TexCoord : TEXCOORD0;
			};

			int ShouldDrawOutlineOutside(fixed4 sampledColor, float2 texCoord, int isOutlineEnabled, float alphaThreshold) {
				if (isOutlineEnabled == 0 || sampledColor.a > alphaThreshold) return 0;

				float2 texDdx = ddx(texCoord);
				float2 texDdy = ddy(texCoord);

				float2 pixelUpTexCoord = texCoord + float2(0, _MainTex_TexelSize.y);
				fixed pixelUpAlpha = tex2Dgrad(_MainTex, pixelUpTexCoord, texDdx, texDdy).a;
				if (pixelUpAlpha > alphaThreshold) return 1;

				float2 pixelDownTexCoord = texCoord - float2(0, _MainTex_TexelSize.y);
				fixed pixelDownAlpha = tex2Dgrad(_MainTex, pixelDownTexCoord, texDdx, texDdy).a;
				if (pixelDownAlpha > alphaThreshold) return 1;

				float2 pixelRightTexCoord = texCoord + float2(_MainTex_TexelSize.x, 0);
				fixed pixelRightAlpha = tex2Dgrad(_MainTex, pixelRightTexCoord, texDdx, texDdy).a;
				if (pixelRightAlpha > alphaThreshold) return 1;

				float2 pixelLeftTexCoord = texCoord - float2(_MainTex_TexelSize.x, 0);
				fixed pixelLeftAlpha = tex2Dgrad(_MainTex, pixelLeftTexCoord, texDdx, texDdy).a;
				if (pixelLeftAlpha > alphaThreshold) return 1;

				return 0;
			}

			fixed4 SampleSpriteTexture(float2 uv) {
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				fixed4 alpha = tex2D(_AlphaTex, uv);
				color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
				#endif

				return color;
			}

			VertexOutput ComputeVertex(VertexInput vertexInput) {
				VertexOutput vertexOutput;

				vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
				vertexOutput.TexCoord = vertexInput.TexCoord;
				vertexOutput.Color = vertexInput.Color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
				vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
				#endif

				return vertexOutput;
			}

			fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target {
				fixed4 color = SampleSpriteTexture(vertexOutput.TexCoord) * vertexOutput.Color;
				color.rgb = color.rgb * color.a;

				return lerp(color, _OutlineColor * _OutlineColor.a, ShouldDrawOutlineOutside(color, vertexOutput.TexCoord, _IsOutlineEnabled, _AlphaThreshold));
			}

			ENDCG
		}

		// Pass with SpriteDuffiseSpecular shader
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#include "UnitySprites.cginc"

		struct Input {
			float2 uv_MainTex;
			fixed4 color;
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
		}

		ENDCG
	}
}
