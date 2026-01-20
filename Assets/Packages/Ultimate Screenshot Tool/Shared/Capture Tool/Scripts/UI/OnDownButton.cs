using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TRS.CaptureTool
{
    public class OnDownButton : Button, IPointerDownHandler
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (interactable)
                onClick.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            //base.OnPointerClick(eventData);
        }
    }
}