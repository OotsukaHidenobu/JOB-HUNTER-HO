Shader "Custom/ScrollEmission"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _NormalStrength("NormalStrength",float) = 0.3
        _EmissionMap ("EmissionMap", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
        _EmissionScrollMap("_EmissionScrollMap", 2D) = "white" {}
        _ScrollX("Scroll X", float) = 0
        _ScrollY("Scroll Y", float) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex,_EmissionMap,_EmissionScrollMap,_BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        half _Glossiness;
        half _Metallic;
        float _ScrollX,_ScrollY,_NormalStrength;
        float3 _EmissionColor;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float2 scroll = float2(IN.uv_MainTex.x +_ScrollX,IN.uv_MainTex.y + _ScrollY);
            fixed4 c = tex2D (_MainTex,  IN.uv_MainTex) * _Color;

            fixed3 emissionMap = tex2D (_EmissionMap,  IN.uv_MainTex) * _EmissionColor;
            fixed3 emissionScrollMap =tex2D (_EmissionScrollMap,  scroll);

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap)) * _NormalStrength;
            o.Emission = emissionMap*emissionScrollMap;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
