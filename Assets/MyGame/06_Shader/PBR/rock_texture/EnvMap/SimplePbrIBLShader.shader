Shader "Unlit/SimplePbrIBLShader"
{
    Properties
    {
        _AlbedoColor("アルベドカラー", Color) = (1.0, 1.0, 1.0, 1)
        _Metallic("メタリック", Range(0.0, 1.0)) = 0
        _Smoothness("スムースネス", Range(0.0, 1.0)) = 0.0

        _IBLSpecularMap("IBL鏡面反射マップ", Cube) = "" {}
        _IBLBRDFMap("IBL_BRDFマップ", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float3 position : TEXCOORD1;
                float3 normal   : TEXCOORD2;
                float4 vertex   : SV_POSITION;
            };

            // アルベドカラー
            uniform float4 _AlbedoColor;
            // メタリック 0.0(非金属）～1.0(金属）
            uniform float _Metallic;
            // スムースネス 0.0(ザラザラ）～1.0（ツルツル)
            uniform float _Smoothness;
            // ライトのカラー
            uniform float3 _LightColor;

            // IBL鏡面反射マップ
            UNITY_DECLARE_TEXCUBE(_IBLSpecularMap);
            // 環境BRDF事前計算マップ
            uniform sampler2D _IBLBRDFMap;
            // IBL鏡面反射マップのMIPMAP数
            static const float SPECCUBE_LOD_STEPS = 9;

            // 円周率
            static const float PI = 3.1415926f;
            // ノンリニア空間からリニアに変換
            float3 ConvertToLinear(float3 color) {
                return pow(color, 2.2);
            }
            // リニア空間からノンリニア空間に変換
            float3 ConvertToNoneLinear(float3 color) {
                return pow(color, 1 / 2.2);
            }

            // スムースネスをラフネスに変換
            float SmoothnessToPerceptualRoughness(float smoothness) {
                return (1.0f - smoothness);
            }
            // リニア空間のラフネスに変換
            float PerceptualRoughnessToRoughness(float perceptualRoughness) {
                return perceptualRoughness * perceptualRoughness;
            }

            // 5乗
            float Pow5(float x) {
                return x * x * x * x * x;
            }
            // フレネル反射
            float3 F_Schlick(float cosA, float3 f0) {
                return f0 + (1.0 - f0) * Pow5(1.0 - cosA);
            }
            // マイクロファセット分布関数 Trowbridge-Reitz (GGX)
            float D_GGX(float NoH, float linearRoughness) {
                float a2 = linearRoughness * linearRoughness;
                float f = (NoH * a2 - NoH) * NoH + 1.0;
                return a2 / (PI * f * f);
            }
            // 可視性関数
            float V_SmithGGXCorrelatedFast(float NoV, float NoL, float linearRoughness) {
                float a = linearRoughness;
                float GGXV = NoL * (NoV * (1.0 - a) + a);
                float GGXL = NoV * (NoL * (1.0 - a) + a);
                return 0.5f / (GGXV + GGXL + 1e-5);
            }
            // 鏡面反射BRDF（Cook-Torrance approximation）
            float3 SpecularBRDF(float3 specularColor, float NoH, float NoV, float NoL, float LoH,
                                                                                       float linearRoughness) {
                float D = D_GGX(NoH, linearRoughness);
                float V = V_SmithGGXCorrelatedFast(NoV, NoL, linearRoughness);
                float3 F = F_Schlick(LoH, specularColor);
                return D * V * F;
            }

            // 拡散反射
            float Fd_Lambert() {
                return 1.0 / PI;
            }
            // 拡散反射BRDF
            float3 DiffuseBRDF(float3 diffuseColor) {
                return diffuseColor;//* Fd_Lambert();
            }




            // IBLの計算
            float3 ImageBasedLighting(float3 N, float3 R, float NoV,
                  float3 diffuseColor, float3 specularColor, float perceptualRoughness) {
                // 拡散反射光用のIBLキューブマップを参照（鏡面反射用のIBLキューブマップを代用して近似）
                float3 Ld = UNITY_SAMPLE_TEXCUBE_LOD(_IBLSpecularMap, N, SPECCUBE_LOD_STEPS - 0.5f).rgb
                                                                                 * diffuseColor;// * Fd_Lambert();
                // 鏡面反射用のIBLキューブマップを参照
                float3 Lld = UNITY_SAMPLE_TEXCUBE_LOD(_IBLSpecularMap, R,
                                                      perceptualRoughness * SPECCUBE_LOD_STEPS).rgb;
                // IBL用BRDF計算用テクスチャを参照
                float2 Ldfg = tex2D(_IBLBRDFMap, float2(NoV, perceptualRoughness)).xy;
                // 鏡面反射の計算
                float3 Lr = (specularColor * Ldfg.x + Ldfg.y) * Lld;
                // 鏡面反射と拡散反射を合成
                return Ld + Lr;
            }


            // 頂点シェーダー
            v2f vert(appdata v)
            {
                v2f o;
                // ワールド座標系の頂点座標を求める
                o.position = mul(unity_ObjectToWorld, v.vertex);
                // ワールド座標系の法線ベクトルを求める
                o.normal = UnityObjectToWorldNormal(v.normal);
                // テクスチャ座標の出力
                o.uv = v.uv;
                // ワールド・ビュー・プロジェクション変換
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // ピクセルシェーダー
            fixed4 frag(v2f i) : SV_Target
            {
                // リニアカラーに変換
                float3 albedoColor = ConvertToLinear(_AlbedoColor.rgb);

                // 拡散反射カラーの取得（非金属）
                float3 diffuseColor = lerp(albedoColor.rgb, 0.0f, _Metallic);
                // 鏡面反射カラーの取得（金属）
                float3 specularColor = lerp(0.04f, albedoColor.rgb, _Metallic);
                // ラフネスに変換　0.0(ツルツル）～1.0(ザラザラ）
                float perceptualRoughness = SmoothnessToPerceptualRoughness(_Smoothness);
                // リニア空間のラフネスに変換　0.0(ツルツル）～1.0(ザラザラ）
                float linearRoughness = PerceptualRoughnessToRoughness(perceptualRoughness);

                // 各種ベクトルを求める
                float3 N = normalize(i.normal);                             // 法線ベクトル
                float3 L = normalize(UnityWorldSpaceLightDir(i.position));  // ライト方向のベクトル
                float3 V = normalize(UnityWorldSpaceViewDir(i.position));   // 視点方向のベクトル
                float3 H = normalize(L + V);                                // 2等分ベクトル
                float3 R = reflect(-V, N);                                  // 反射ベクトル
                // 各種ベクトルの内積
                float NoV = abs(dot(N, V)) + 1e-5;
                float NoL = saturate(dot(N, L));
                float NoH = saturate(dot(N, H));
                float LoV = saturate(dot(L, V));
                float LoH = saturate(dot(L, H));

                // 放射照度の計算
                float3 irradiance = NoL * _LightColor0 * PI;

                // 双方向反射率分布関数(BRDF)の計算
                float3 Fd = DiffuseBRDF(diffuseColor);
                float3 Fr = SpecularBRDF(specularColor, NoH, NoV, NoL, LoH, linearRoughness);
                float3 BRDF = (Fd + Fr) * irradiance;

                // イメージベースドライティング
                float3 IBL = ImageBasedLighting(N, R, NoV, diffuseColor, specularColor, perceptualRoughness);

                // 最終的なカラーの計算
                float3 finalColor = BRDF + IBL;

                // ノンリニアカラーに変換して出力
                finalColor = finalColor;
                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }

}
