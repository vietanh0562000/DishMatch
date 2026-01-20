Shader "Custom/Mask"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
        }
        Pass
        {
            Blend Zero One
        }
    }
}