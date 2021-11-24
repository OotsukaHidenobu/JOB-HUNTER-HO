Shader "Unlit/UnlitTransprentRGB"
{
    
	Properties {
		_Color      ("Color"       , Color      ) = (1, 1, 1, 1)
		_MainTex    ("Albedo (RGB)", 2D         ) = "white" {}
		_Cutoff     ("Cutoff"      , Range(0, 1)) = 0.5
	}

	SubShader {
		Tags {
			"Queue"      = "AlphaTest"
			"RenderType" = "TransparentCutout"
		}

		LOD 200
		
		Cull Off
		
		CGPROGRAM
			#pragma target 3.0
			#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

			fixed4 _Color;
			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				o.Albedo     = c.rgb* _Color;
				o.Alpha      = c.a;
			}
		ENDCG
	}

	FallBack "Transparent/Cutout/Diffuse"
}
