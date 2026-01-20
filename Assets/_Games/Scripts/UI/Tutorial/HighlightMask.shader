Shader "UI/UIHolePunch"
{
    Properties
    {
        _OverlayColor("Overlay Color", Color) = (0,0,0,0.8)
        _HoleCenter("Hole Center (X,Y)", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize("Hole Size (Width,Height)", Vector) = (0.2, 0.2, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
            };

            float4 _HoleCenter;   // xy = center in normalized UV (0..1)
            float4 _HoleSize;     // xy = w,h in normalized UV
            fixed4 _OverlayColor;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // IN.uv là tọa độ (0..1) trên full-screen Image
                float2 uv = IN.uv;

                // Tính biên của lỗ:
                float2 halfSize = _HoleSize.xy * 0.5;
                float2 min = _HoleCenter.xy - halfSize;
                float2 max = _HoleCenter.xy + halfSize;

                // Nếu uv nằm trong vùng hole, trả về alpha = 0 (trong suốt)
                if (uv.x >= min.x && uv.x <= max.x &&
                    uv.y >= min.y && uv.y <= max.y)
                {
                    // trả về (0,0,0,0) ⇒ transparent
                    return fixed4(0,0,0,0);
                }

                // Ngoài vùng hole ⇒ vẽ overlayColor
                return _OverlayColor;
            }
            ENDCG
        }
    }
}
