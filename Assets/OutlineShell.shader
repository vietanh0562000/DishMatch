Shader "Universal Render Pipeline/Unlit/OutlineShell X"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.1)) = 0.01
        _ThicknessScale ("Thickness Scale", Range(0.1, 5.0)) = 1.0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4 // Default to LessEqual (corresponds to index 4 in Unity's CompareFunction enum)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" } // Important: This must match a ShaderTagId used in OutlinePass.cs

            Cull Front     // Draw back-faces of the extruded shell
            ZWrite On      // Write to depth buffer
            ZTest [_ZTest] // Use the ZTest property for configurable depth testing via material/OutlineSettings

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Core URP library
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // GPU Instancing setup
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON // Added for BatchRendererGroup support
            // If you are using instanced properties that affect scaling, uncomment the next line
            // #pragma instancing_options assumeuniformscaling

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID // For GPU Instancing
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID // For GPU Instancing
                UNITY_VERTEX_OUTPUT_STEREO // For Stereo Rendering (VR/XR)
            };

            // Material properties
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
                float _ThicknessScale;
            CBUFFER_END

            // If you were using per-instance properties (e.g., different outline color per object),
            // you would define them here. For example:
            // UNITY_INSTANCING_BUFFER_START(PerInstanceProperties)
            //     UNITY_DEFINE_INSTANCED_PROP(float4, _InstanceOutlineColor)
            // UNITY_INSTANCING_BUFFER_END(PerInstanceProperties)

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN); // Setup for GPU Instancing
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT); // Transfer instance ID to fragment shader
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT); // Setup for Stereo Rendering

                // Extrude vertex along its normal in Object Space
                float3 extrudedPositionOS = IN.positionOS.xyz + IN.normalOS * _OutlineThickness * _ThicknessScale;
                
                // Transform position from Object Space to Homogeneous Clip Space
                OUT.positionCS = TransformObjectToHClip(extrudedPositionOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN); // Setup for GPU Instancing (needed if using instanced properties)
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN); // Setup for Stereo Rendering

                // Accessing per-instance properties would look like:
                // half4 instancedColor = UNITY_ACCESS_INSTANCED_PROP(PerInstanceProperties, _InstanceOutlineColor);
                // return instancedColor;

                return _OutlineColor; // Return the uniform outline color
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError" // Fallback for unsupported GPUs/situations
} 