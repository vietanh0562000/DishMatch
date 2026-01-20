using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Demo
{
    public class DemoAngleTextScript : MonoBehaviour
    {
        public GameObject objectToMeasure;

        Text textComponent;

        void Awake()
        {
            textComponent = GetComponent<Text>();
        }

        void Update()
        {
            textComponent.text = "Angle: " + Mathf.RoundToInt(objectToMeasure.transform.eulerAngles.y);
        }
    }
}
