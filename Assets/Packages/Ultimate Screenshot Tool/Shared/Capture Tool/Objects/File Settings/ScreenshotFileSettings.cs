using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ScreenshotFileSettings : CaptureFileSettings
    {
        public bool saveInBackground = true;
        public System.Threading.ThreadPriority threadPriority = System.Threading.ThreadPriority.BelowNormal;

        public FileType fileType = FileType.PNG;
        public enum FileType { EXR, PNG, JPG, TGA, } // RAW,
        public static readonly string[] ValidFileExtensions = { ".exr", ".jpg", ".jpeg", ".png", ".tga", }; // ".raw",
        public static FileType FileTypeForExtension(string extension)
        {
            switch (extension)
            {
                case ".exr":
                    return FileType.EXR;
                case ".png":
                    return FileType.PNG;
                case ".jpg":
                case ".jpeg":
                    return FileType.JPG;
                case ".tga":
                    return FileType.TGA;
                //case ".raw":
                //    return FileType.RAW;
                default:
                    throw new UnityException("Unhandled extension: " + extension + " in FileTypeForExtension");
            }
        }

        public static string EncodingForFileType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.EXR:
                    return "exr";
                case FileType.PNG:
                    return "png";
                case FileType.JPG:
                    return "jpg";
                case FileType.TGA:
                    return "tga";
                //case FileType.RAW:
                //    return "raw";
                default:
                    throw new UnityException("Unhandled fileType: " + fileType + " in EncodingForFileType");
            }
        }

        public override string encoding { get { return EncodingForFileType(fileType); } }

        public bool allowTransparency;

        public TextureEncodingParameters encodingParameters = new TextureEncodingParameters();

        public override string fullPrefix {
            get {
                if(string.IsNullOrEmpty(frameDescription)) return prefix;
                string fullFrameDescription = frameDescriptionPrefix + frameDescription + frameDescriptionSuffix;
                if (includeFrameDescriptionBeforePrefix) return fullFrameDescription + prefix;
                return prefix + fullFrameDescription;
            }
        }
        public bool includeFrameDescriptionBeforePrefix = false;
        public string frameDescriptionPrefix = "In";
        public string frameDescriptionSuffix = "";
        public string frameDescription;

        public bool includeLanguageInPath = false;
        public string languageIdentifier
        {
            get
            {
#if UNITY_LOCALIZATION
                if (LocalizationSettings.SelectedLocale == null) return "";
                return LocalizationSettings.SelectedLocale.Identifier.Code;
#else
                return systemLanguage.ToString();
#endif
            }
        }

        public UnityEngine.SystemLanguage systemLanguage { get { return shouldOverrideSystemLanguage ? overrideSystemLanguage : Application.systemLanguage; } }
        public bool shouldOverrideSystemLanguage = false;
        public static UnityEngine.SystemLanguage overrideSystemLanguage;

        public ScreenshotFileSettings(ScreenshotFileSettings original) : base(original)
        {
            saveInBackground = original.saveInBackground;
            threadPriority = original.threadPriority;

            fileType = original.fileType;

            allowTransparency = original.allowTransparency;
            encodingParameters = original.encodingParameters;

            includeLanguageInPath = original.includeLanguageInPath;
            shouldOverrideSystemLanguage = original.shouldOverrideSystemLanguage;
        }

        public ScreenshotFileSettings()
        {
            fileType = FileType.PNG;
        }

        public override void SetUp(int uniqueId, string saveType = "Screenshots")
        {
            base.SetUp(uniqueId, saveType);

            if (setup)
                return;
        }

        public override string FullFilePathWithCaptureDetails(string cameraName, string resolutionName, string resolutionString)
        {
            if (includeLanguageInPath)
            {
                string fullResolutionString = includeResolutionSizeInPath ? ConnectParts(resolutionName, resolutionString) : resolutionName;
                string folderName = FolderName(languageIdentifier, fullResolutionString);
                string fileName = FileNameWithCaptureDetails(cameraName, resolutionString);
                return FullFilePath(folderName, fileName);
            }

            return base.FullFilePathWithCaptureDetails(cameraName, resolutionName, resolutionString);
        }

        public static void OverrideLanguage(UnityEngine.SystemLanguage language)
        {
            overrideSystemLanguage = language;
        }

        public void ValidateFilePath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath);

            bool isValidExtension = false;
            for(int i = 0; i < ValidFileExtensions.Length; ++i)
            {
                if(ValidFileExtensions[i].Equals(extension)) {
                    isValidExtension = true;
                    break;
                }
            }
            if (!isValidExtension)
                Debug.LogError("Invalid file extension: " + extension + " for screenshot");

            FileType fileTypeForExtension = FileTypeForExtension(extension);
            if (fileType != fileTypeForExtension)
                Debug.LogWarning("File settings specify saving as a " + EncodingForFileType(fileType) + ", but the extension in the specified file path is: " + extension);
        }
    }
}