using UnityEngine.UI;
using TRS.CaptureTool.Extras;

namespace TRS.CaptureTool
{
    public class UIMouseFollowScript : MouseFollowScript
    {
        RawImage mouseImage;

        void Awake()
        {
            mouseImage = GetComponent<RawImage>();
        }

        void Update()
        {
            mouseImage.rectTransform.anchoredPosition = FlexibleInput.MousePosition();
        }
    }
}