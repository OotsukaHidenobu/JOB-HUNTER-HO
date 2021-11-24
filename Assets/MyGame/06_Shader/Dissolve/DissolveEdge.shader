// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/DissolveEdge"
{
    Properties {
		_Color ("Color", Color) = (1,1,1,1)
        [HDR] _EdgeEmissionColor ("Emission Color", Color) = (0,0,0)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Progress("Threshold", Range(0,1))= 0.0
		_NoiseStrength("NoiseStrength",float)= 50
		_Edge("Edge",float)=0.2

		[Header(Scroll Color)]
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_EmissionMap ("EmissionMap", 2D) = "white" {}
		[HDR] _ScrollEmissionColor ("Emission Color", Color) = (0,0,0)
		_EmissionScrollMap("_EmissionScrollMap", 2D) = "white" {}
        _ScrollX("Scroll X", float) = 0
        _ScrollY("Scroll Y", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard addshadow
		#pragma target 3.0

		sampler2D _MainTex,_BumpMap,_EmissionMap,_EmissionScrollMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		half _Progress,_Edge,_NoiseStrength,_ScrollX,_ScrollY;
		fixed4 _Color,_EdgeEmissionColor,_ScrollEmissionColor;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		float unity_noise_randomValue (float2 uv)
		{
    		return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
		}

		float unity_noise_interpolate (float a, float b, float t)
		{
    		return (1.0-t)*a + (t*b);
		}

		float unity_valueNoise (float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac(uv);
			f = f * f * (3.0 - 2.0 * f);

			uv = abs(frac(uv) - 0.5);
			float2 c0 = i + float2(0.0, 0.0);
			float2 c1 = i + float2(1.0, 0.0);
			float2 c2 = i + float2(0.0, 1.0);
			float2 c3 = i + float2(1.0, 1.0);
			float r0 = unity_noise_randomValue(c0);
			float r1 = unity_noise_randomValue(c1);
			float r2 = unity_noise_randomValue(c2);
			float r3 = unity_noise_randomValue(c3);

			float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
			float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
			float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
			return t;
		}

		float Unity_SimpleNoise_float(float2 UV, float Scale)
		{
			float t = 0.0;

			float freq = pow(2.0, float(0));
			float amp = pow(0.5, float(3-0));
			t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3-1));
			t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3-2));
			t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

			return t;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float2 scroll = float2(IN.uv_BumpMap.x +_ScrollX,IN.uv_BumpMap.y + _ScrollY);
			fixed4 emissionScrollMap =tex2D (_EmissionScrollMap,  scroll);
			fixed4 emissionMap = tex2D (_EmissionMap,  IN.uv_BumpMap) * _ScrollEmissionColor;
			fixed cutoff = (1-_Progress) + _Edge;
			fixed noise =Unity_SimpleNoise_float(IN.uv_MainTex,_NoiseStrength);
			fixed m_step = step(cutoff,noise);

			fixed invertColor = 1 - m_step;
			fixed4 eggColor = mul(_EdgeEmissionColor,invertColor);

			clip(noise - (1-_Progress));
			// if( noise < _Progress ){
			// 	discard;
			// } 

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Emission = eggColor+emissionMap*emissionScrollMap;
			o.Albedo = c;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = noise;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
