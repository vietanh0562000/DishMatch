using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class LiveFrame
    {
        public bool active = true;
        public string name;
        public RawImage textureDestination;

        public bool activeOnlyIfGameObjectActive;
        public GameObject gameObjectRequiredToBeActive;

        public bool overrideResolution;
        public Resolution resolution { get { return new Resolution { width = resolutionWidth, height = resolutionHeight }; } }
        public int resolutionWidth;
        public int resolutionHeight;

        public Resolution originalResolution;

        public GameObject[] tempEnabledObjects = new GameObject[0];
        public GameObject[] tempDisabledObjects = new GameObject[0];
        public TextureTransformation[] transformations = new TextureTransformation[0];
    }
}