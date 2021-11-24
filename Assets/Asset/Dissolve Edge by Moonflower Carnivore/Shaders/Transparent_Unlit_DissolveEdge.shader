Shader "MoonflowerCarnivore/Dissolve Edge"
{
	Properties
	{
		[Enum(Off,0,Front,1,Back,2)] _CullMode ("Culling Mode", int) = 0
		[Enum(Off,0,On,1)] _ZWrite("ZWrite", int) = 0
		_Progress("Progress",Range(0,1)) = 0
		_MainColor("Main Color", COLOR) = (1,1,1,1)
		[HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
		_MainTex("Main Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_Edge("Edge",Range(0.01,0.5)) = 0.01

		[Header(Edge Color)]
		[Toggle(EDGE_COLOR)] _UseEdgeColor("Edge Color?", Float) = 1
		[HideIfDisabled(EDGE_COLOR)][NoScaleOffset] _EdgeAroundRamp("Edge Ramp", 2D) = "white" {}
		[HideIfDisabled(EDGE_COLOR)]_EdgeAround("Edge Color Range",Range(0,0.5)) = 0
		[HideIfDisabled(EDGE_COLOR)]_EdgeAroundPower("Edge Color Power",Range(1,5)) = 1
		[HideIfDisabled(EDGE_COLOR)]_EdgeAroundHDR("Edge Color HDR",Range(1,3)) = 1
		[HideIfDisabled(EDGE_COLOR)]_EdgeDistortion("Edge Distortion",Range(0,1)) = 0

		[Header(Scroll Color)]
		_BumpMap ("Bumpmap", 2D) = "bump" {}
        _NormalStrength("NormalStrength",float) = 0.3
        _EmissionMap ("EmissionMap", 2D) = "white" {}
        [HDR] _ScrollEmissionColor ("Emission Color", Color) = (0,0,0)
        _EmissionScrollMap("_EmissionScrollMap", 2D) = "white" {}
        _ScrollX("Scroll X", float) = 0
        _ScrollY("Scroll Y", float) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha //Alpha Blend
			Cull[_CullMode] Lighting Off ZWrite[_ZWrite]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature EDGE_COLOR

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv3 : TEXCOORD3;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _DissolveTex;
			float4 _MainTex_ST;
			float4 _DissolveTex_ST;
			fixed _Edge;
			fixed _Progress;
			float4 _MainColor;

			sampler2D _EmissionMap,_EmissionScrollMap,_BumpMap;
			float _ScrollX,_ScrollY,_NormalStrength;
        	float4 _ScrollEmissionColor;

			#ifdef EDGE_COLOR
				sampler2D _EdgeAroundRamp;
				fixed _EdgeAround;
				float _EdgeAroundPower;
				float _EdgeAroundHDR;
				fixed _EdgeDistortion;
			#endif
			
			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv3 = TRANSFORM_TEX(v.uv, _DissolveTex);
				o.color = v.color;
				o.color.a *= _Progress;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
					float2 scroll = float2(i.uv.x +_ScrollX,i.uv.y + _ScrollY);
					fixed4 emissionMap = tex2D (_EmissionMap,  i.uv) * _ScrollEmissionColor;
            		fixed4 emissionScrollMap =tex2D (_EmissionScrollMap,  scroll);
					fixed4 col = tex2D(_DissolveTex, i.uv3);
					fixed x = col.r;
					fixed progress = i.color.a;
	
					//Edge
					fixed edge = lerp( x + _Edge, x - _Edge, progress);
					fixed alpha = smoothstep(  progress + _Edge, progress - _Edge, edge);
					
					#ifdef EDGE_COLOR
						//Edge Around Factor
						fixed edgearound = lerp( x + _EdgeAround, x - _EdgeAround, progress);
						edgearound = smoothstep( progress + _EdgeAround, progress - _EdgeAround, edgearound);
						edgearound = pow(edgearound, _EdgeAroundPower);

						//Edge Around Distortion
						fixed avoid = 0.15f;
						fixed distort = edgearound*alpha*avoid;
						float2 cuv = lerp( i.uv, i.uv + distort - avoid, progress * _EdgeDistortion);
						col = tex2D(_MainTex, cuv)* _MainColor;
						col.rgb *= i.color.rgb;

						//Edge Around Color
						fixed3 ca = tex2D(_EdgeAroundRamp, fixed2(1-edgearound, 0)).rgb;
						ca = (col.rgb + ca)*ca*_EdgeAroundHDR;
						col.rgb = lerp( ca, col.rgb, edgearound);
					#else
						col = tex2D(_MainTex, i.uv)* _MainColor +emissionMap*emissionScrollMap;
						col.rgb *= i.color.rgb;
					#endif

					col.a *= alpha;
					

					return col;
			}
			ENDCG
		}
	}
}
