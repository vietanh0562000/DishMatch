using UnityEngine;
using System.Collections;

namespace TRS.CaptureTool
{
    public class DemoCubeSpawnerScript : MonoBehaviour
    {
        public GameObject cubePrefab;
        public float interval = 0.5f;

        void OnEnable()
        {
            if (cubePrefab == null)
                return;

            StartCoroutine(SpawnCube());
        }

        IEnumerator SpawnCube()
        {
            while (true)
            {
                Instantiate(cubePrefab, transform.position, Random.rotationUniform, transform);

                yield return new WaitForSeconds(interval);
            }
        }
    }
}