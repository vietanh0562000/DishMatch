namespace TRS.CaptureTool
{
    public static class ToolInfo
    {
        public static bool isScreenshotTool { get { return screenshotAssembly != null; } }
        public static bool isGifTool { get { return gifAssembly != null; } }

        static System.Reflection.Assembly screenshotAssembly;
        static System.Reflection.Assembly gifAssembly;

        static ToolInfo()
        {
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name.Equals("TRS.ScreenshotTool"))
                    screenshotAssembly = assembly;

                if (assembly.GetName().Name.Equals("TRS.GifTool"))
                    gifAssembly = assembly;
            }
        }

        public static string ScriptName()
        {
            if(isScreenshotTool && isGifTool)
                return "ScreenshotScript or GifScript";
            else if (isScreenshotTool)
                return "ScreenshotScript";
            else if (isGifTool)
                return "GifScript";

            UnityEngine.Debug.LogError("Unrecognized asset");
            return "";
        }

        public static string ScreenshotVersion()
        {
            if (!isScreenshotTool) return "";
            return VersionForType(screenshotAssembly, "TRS.CaptureTool.ScreenshotScript");
        }

        public static string GifVersion()
        {
            if (!isGifTool) return "";
            return VersionForType(gifAssembly, "TRS.CaptureTool.GifScript");
        }

        static string VersionForType(System.Reflection.Assembly assembly, string type)
        {
            System.Type versionClass = assembly.GetType(type);
            if (versionClass == null) return "";
            System.Reflection.FieldInfo versionField = versionClass.GetField("version");
            if (versionField == null) return "";
            return versionField.GetValue(null) as string;
        }

        public static string ToolVersion()
        {
            float screenshotVersion = float.MaxValue;
            float gifVersion = float.MaxValue;

            string screenshotVersionString = ScreenshotVersion();
            string gifVersionString = GifVersion();

            try {
                if (!string.IsNullOrEmpty(screenshotVersionString)) {
                    screenshotVersion = (float)System.Convert.ToDouble(screenshotVersionString);
            }
            } catch (System.Exception e) {
                UnityEngine.Debug.LogError("Unable to parse screenshot version: '" + screenshotVersionString + "' due to error:\n" + e);
            }

            try {
                if (!string.IsNullOrEmpty(gifVersionString))
                    gifVersion = (float)System.Convert.ToDouble(gifVersionString);
            } catch (System.Exception e) {
                UnityEngine.Debug.LogError("Unable to parse gif version: '" + gifVersionString + "' due to error:\n" + e);
            }

            float minVersion = UnityEngine.Mathf.Min(screenshotVersion, gifVersion);
            if(minVersion == float.MaxValue) minVersion = 0;
            return minVersion.ToString("0.00");
        }
    }
}