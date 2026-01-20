using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class NewScriptableObjectPath
    {
        public const string TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY = "TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY";

        public const string defaultDirectory = "Packages/CaptureTool/";
        public static string DirectoryFolder(string folderName)
        {
            if (!PlayerPrefs.HasKey(TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY))
                PlayerPrefs.SetString(TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY, defaultDirectory);
            string saveObjectDirectory = PlayerPrefs.GetString(TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY);

            string separatorString = System.IO.Path.DirectorySeparatorChar.ToString();
            if (saveObjectDirectory.Equals(separatorString))
                saveObjectDirectory = "";

            if (saveObjectDirectory.StartsWith(separatorString) && saveObjectDirectory.Length > separatorString.Length)
                saveObjectDirectory = saveObjectDirectory.Substring(separatorString.Length);

            if (!saveObjectDirectory.EndsWith(separatorString))
                saveObjectDirectory += separatorString;

            string saveObjectFolderInNativeFormat = Application.dataPath + separatorString + string.Join(separatorString, saveObjectDirectory.Split('/'));
            try
            {
                System.IO.Directory.CreateDirectory(saveObjectFolderInNativeFormat);
            }
            catch (System.Exception e)
            {
                PlayerPrefs.SetString(TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY, defaultDirectory);
                Debug.LogError("Invalid save object directory. Resetting to default. Change in ScreenshotScript or GifScript > Settings > Object Folder\nError details: " + e);
            }

            string specificSaveDirectory = saveObjectFolderInNativeFormat + separatorString + folderName;
            System.IO.Directory.CreateDirectory(specificSaveDirectory);

            return saveObjectDirectory + "/" + folderName;
        }

        public static string AssetDatabaseDirectory(string folderName)
        {
            return "Assets/" + DirectoryFolder(folderName);
        }
    }
}