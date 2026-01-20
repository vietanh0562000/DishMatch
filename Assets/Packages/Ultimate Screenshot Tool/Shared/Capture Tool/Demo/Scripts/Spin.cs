using UnityEngine;

namespace TRS.CaptureTool
{
    public class Spin : MonoBehaviour
    {
        public Vector3 axisOfRotation = Vector3.up;
        public float speed = 10f;

        void Update()
        {
            transform.Rotate(axisOfRotation, speed * Time.deltaTime);
        }
    }
}