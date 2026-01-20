Shader "TRS_Demo/TRS_Invert"
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
                col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
