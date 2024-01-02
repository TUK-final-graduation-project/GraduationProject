Shader "Custom/NewSurfaceShader"
{
    // 속성 선언
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}    // 메인 텍스처
        _BumpMap("NormalMap", 2D) = "White" {}       // 노멀 맵 텍스처
    }

        // 서브 쉐이더 정의
        SubShader
    {
        Tags { "RenderType" = "Opaque" }   // 불투명한 물체 렌더링 태그
        Cull Back                          // 뒷면 제거

        CGPROGRAM
        #pragma surface surf Toon noambient   // Toon 라이팅 모델 사용, 주변광 미사용
        #pragma target 3.0                   // 쉐이더 대상 버전 3.0

        // 샘플러 선언
        sampler2D _MainTex;
        sampler2D _BumpMap;

        // 입력 구조체 정의
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        // 서피스 함수 정의
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Albedo = c.rgb;

            o.Alpha = c.a;
        }

        // Toon 라이팅 함수 정의
        float4 LightingToon(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            // 라이트와 표면 노멀의 내적 계산
            float ndotl = dot(s.Normal, lightDir) * 0.5 + 0.5;

            // Toon 쉐이딩을 위한 계산
            ndotl *= 3;
            ndotl = ceil(ndotl) / 3;

            // 최종 결과 계산
            float4 final;
            final.rgb = s.Albedo * ndotl * _LightColor0.rgb; // Toon 쉐이딩 부분
            final.a = s.Alpha;

            return final;
        }
        ENDCG
    }

        // 대체 쉐이더 지정
            FallBack "Diffuse"
}
