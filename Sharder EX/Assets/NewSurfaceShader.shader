Shader "Custom/NewSurfaceShader"
{
    // �Ӽ� ����
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}    // ���� �ؽ�ó
        _BumpMap("NormalMap", 2D) = "White" {}       // ��� �� �ؽ�ó
    }

        // ���� ���̴� ����
        SubShader
    {
        Tags { "RenderType" = "Opaque" }   // �������� ��ü ������ �±�
        Cull Back                          // �޸� ����

        CGPROGRAM
        #pragma surface surf Toon noambient   // Toon ������ �� ���, �ֺ��� �̻��
        #pragma target 3.0                   // ���̴� ��� ���� 3.0

        // ���÷� ����
        sampler2D _MainTex;
        sampler2D _BumpMap;

        // �Է� ����ü ����
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        // ���ǽ� �Լ� ����
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Albedo = c.rgb;

            o.Alpha = c.a;
        }

        // Toon ������ �Լ� ����
        float4 LightingToon(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            // ����Ʈ�� ǥ�� ����� ���� ���
            float ndotl = dot(s.Normal, lightDir) * 0.5 + 0.5;

            // Toon ���̵��� ���� ���
            ndotl *= 3;
            ndotl = ceil(ndotl) / 3;

            // ���� ��� ���
            float4 final;
            final.rgb = s.Albedo * ndotl * _LightColor0.rgb; // Toon ���̵� �κ�
            final.a = s.Alpha;

            return final;
        }
        ENDCG
    }

        // ��ü ���̴� ����
            FallBack "Diffuse"
}
