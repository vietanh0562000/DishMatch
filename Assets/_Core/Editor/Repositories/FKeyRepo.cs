using UnityEditor;

namespace BasePuzzle.Core.Editor.Repositories
{
    public static class FKeyRepo
    {
        private const string FalconKeyPrefName = "FALCON_FKEY";

        public static string GetFalconKey()
        {
            return EditorPrefs.GetString(FalconKeyPrefName, "");
        }

        public static void SaveFalconKey(string fKey)
        {
            EditorPrefs.SetString(FalconKeyPrefName, fKey);
        }

        public static void DeleteFalconKey()
        {
            EditorPrefs.DeleteKey(FalconKeyPrefName);
        }

        public static bool HasFalconKey()
        {
            return EditorPrefs.HasKey(FalconKeyPrefName);
        }
    }
}