// Inspired by: https://danielilett.com/2019-05-01-tut1-1-smo-greyscale/

Shader "TRS_Demo/TRS_Grayscale"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float lum = col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
                float4 grayCol = float4(lum, lum, lum, col.a);
                return grayCol;
            }
            ENDCG
        }
    }
}
