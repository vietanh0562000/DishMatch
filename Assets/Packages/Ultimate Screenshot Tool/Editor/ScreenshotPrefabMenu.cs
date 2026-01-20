using UnityEngine;
using UnityEditor;

// Add hotkeys:
// https://unity3d.com/learn/tutorials/topics/interface-essentials/unity-editor-extensions-menu-items

namespace TRS.CaptureTool
{
    public class ScreenshotPrefabMenu : EditorWindow
    {
        readonly static string SCREENSHOT_PREFAB_NAME = "ScreenshotTool";

        [MenuItem("Tools/Ultimate Screenshot Tool/Create Screenshot Tool", false, 1)]
        static void CreateScreenshotPrefab()
        {
            string[] screenshotToolGuids = AssetDatabase.FindAssets(SCREENSHOT_PREFAB_NAME + " t:GameObject", null);
            if (screenshotToolGuids.Length > 1)
            {
                Debug.LogError("Multiple screenshot prefabs found. Please do not name another prefab '" + SCREENSHOT_PREFAB_NAME + "' or change the constant in Ultimate Screenshot Tool/Editor/ScreenshotPrefabMenu.cs to give it a unique name, so the script can find it.");
                return;
            }
            else if (screenshotToolGuids.Length <= 0)
            {
                Debug.LogError("Screenshot prefab not found. You may have changed the prefab name. Please leave the prefab as '" + SCREENSHOT_PREFAB_NAME + "' or update the constant in Ultimate Screenshot Tool/Editor/ScreenshotPrefabMenu.cs, so the script can find it.");
                return;
            }

            GameObject screenshotToolPrefab = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(screenshotToolGuids[0]), typeof(GameObject)) as GameObject;
            GameObject screenshotTool = Instantiate(screenshotToolPrefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(screenshotTool, "Created Screenshot Tool");
        }

        [MenuItem("Tools/Ultimate Screenshot Tool/Select Screenshot Tool", false, 0)]
        static void SelectScreenshotPrefab()
        {
            GameObject screenshotTool = null;
            ScreenshotScript screenshotScript = GameObject.FindObjectOfType<ScreenshotScript>() as ScreenshotScript;
            if (screenshotScript != null)
                screenshotTool = screenshotScript.gameObject;

            if (screenshotTool != null)
                Selection.activeGameObject = screenshotTool;
            else
                Debug.LogError("No screenshot tool in scene.");
        }

        static void DestroyScreenshotPrefabs()
        {
            ScreenshotScript[] screenshotScripts = GameObject.FindObjectsOfType<ScreenshotScript>() as ScreenshotScript[];
            foreach (ScreenshotScript screenshotScript in screenshotScripts)
                Undo.DestroyObjectImmediate(screenshotScript.gameObject);
        }
    }
}