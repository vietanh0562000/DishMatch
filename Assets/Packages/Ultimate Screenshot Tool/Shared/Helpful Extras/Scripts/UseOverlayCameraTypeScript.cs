using UnityEngine;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace TRS.CaptureTool.Extras
{
    [ExecuteInEditMode]
    public class UseOverlayCameraTypeScript : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_URP
            Camera camera = GetComponent<Camera>();
            if(camera != null)
            {
                var cameraData = camera.GetUniversalAdditionalCameraData();
                cameraData.renderType = CameraRenderType.Overlay;
            }
#endif
        }
    }
}