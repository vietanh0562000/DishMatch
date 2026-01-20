using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TRS.CaptureTool
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    [System.Serializable]
    public class PreviewDevice
    {
        [System.Serializable]
        public enum Type
        {
            Screenshot,
            TextureFrame,
            DeviceFrame,
        }

        [System.Serializable]
        public enum SizingType
        {
            SetScreenshotToSize,
            SetFrameToSize
        }

        public bool active = true;
        public string deviceName;
        public Type type = Type.Screenshot;

        // Used exclusively for TextureFrame Type PreviewDevices.
        public RotationType frameRotation = RotationType.None;

        // Used exclusively for DeviceFrame Type PreviewDevices.
        public ScreenOrientation orientation = ScreenOrientation.Portrait;
        public string deviceFilePath;
        [SerializeField]
        string deviceFrameFilePath;

        public SizingType sizingType = SizingType.SetScreenshotToSize;
        public Resolution size { get { return new Resolution { width = width, height = height }; } }
        public int width;
        public int height;

        public Texture2D frame;
        public int safeAreaTopOffset;
        public int safeAreaLeftOffset;
        public int safeAreaRightOffset;
        public int safeAreaBottomOffset;

        public void ReloadDeviceFrame()
        {
            if (type != Type.DeviceFrame || string.IsNullOrEmpty(deviceFrameFilePath)) return;
#if UNITY_EDITOR
            frame = new Texture2D(1, 1);
            frame.name = deviceName + " Frame";
            frame.LoadImage(System.IO.File.ReadAllBytes(deviceFrameFilePath));
#endif
        }

        public void ReloadFromDeviceIfValid()
        {
            if (type == Type.DeviceFrame && !string.IsNullOrEmpty(deviceFilePath))
                ReloadFromDevice();
        }

        public void ReloadFromDevice()
        {
            if (type != Type.DeviceFrame)
            {
                Debug.LogError("Trying to load from device, but currently configured not to use a device. Preview is of type: " + type);
                return;
            }

            if (string.IsNullOrEmpty(deviceFilePath))
            {
                Debug.LogError("Trying to load from device, but no device is set.");
                return;
            }

            string jsonString = System.IO.File.ReadAllText(deviceFilePath);
            SimpleJSON.JSONNode resultJSON = SimpleJSON.JSON.Parse(jsonString);
            SimpleJSON.JSONNode screenJSON = resultJSON["screens"][0];
            SimpleJSON.JSONNode presentationJSON = screenJSON["presentation"];
            string texturePartialFilePath = presentationJSON["overlayPath"].ToString().Trim('"');

#if UNITY_EDITOR
            deviceFrameFilePath = System.IO.Path.GetDirectoryName(deviceFilePath) + "/" + texturePartialFilePath;
            ReloadDeviceFrame();
#endif

            width = screenJSON["width"].AsInt;
            height = screenJSON["height"].AsInt;
            if (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
            {
                height = screenJSON["width"].AsInt;
                width = screenJSON["height"].AsInt;
            }

            SimpleJSON.JSONArray orientations = screenJSON["orientations"].AsArray;
            for (int i = 0; i < orientations.Count; ++i)
            {
                SimpleJSON.JSONNode jsonOrientation = orientations[i];
                if (jsonOrientation["orientation"].AsInt == (int)orientation)
                {
                    SimpleJSON.JSONNode safeArea = jsonOrientation["safeArea"];

                    safeAreaLeftOffset = safeArea["x"].AsInt;
                    safeAreaRightOffset = width - safeArea["width"].AsInt - safeAreaLeftOffset;

                    safeAreaBottomOffset = safeArea["y"].AsInt;
                    safeAreaTopOffset = height - safeArea["height"].AsInt - safeAreaBottomOffset;
                }
            }
        }

        public override string ToString()
        {
            string description = "Device:\n\n";
            description += "active: " + active + "\n\n";
            description += "deviceName: " + deviceName + "\n\n";
            description += "type: " + type + "\n\n";
            description += "frameRotation: " + frameRotation + "\n\n";
            description += "orientation: " + orientation + "\n\n";
            description += "sizingType: " + sizingType + "\n\n";
            description += "width: " + width + "\n\n";
            description += "height: " + height + "\n\n";
            description += "frame: " + frame + "\n\n";
            description += "safeAreaTopOffset: " + safeAreaTopOffset + "\n\n";
            description += "safeAreaLeftOffset: " + safeAreaLeftOffset + "\n\n";
            description += "safeAreaRightOffset: " + safeAreaRightOffset + "\n\n";
            description += "safeAreaBottomOffset: " + safeAreaBottomOffset + "\n\n";
            description += "deviceFilePath: " + deviceFilePath + "\n\n";
            return description;
        }
    }
}