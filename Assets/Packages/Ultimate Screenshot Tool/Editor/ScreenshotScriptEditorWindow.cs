using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Share;
namespace TRS.CaptureTool
{
    public class ScreenshotScriptEditorWindow : EditorWindow
    {
        ScreenshotScript screenshotScript;
        Editor screenshotScriptEditor;

        ShareScript shareScript;
        UpdateShareWithScreenshotScript updateShareWithScreenshotScript;

        GameObject temp;
        Vector2 scrollPos;

        GUIStyle labelGUIStyle;

        [MenuItem("Tools/Ultimate Screenshot Tool/Editor Window", false, 12)]
        static void Init()
        {
            ScreenshotScriptEditorWindow editorWindow = (ScreenshotScriptEditorWindow)GetWindow(typeof(ScreenshotScriptEditorWindow));
            GUIContent titleContent = new GUIContent("Screenshot");
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent = titleContent;
            editorWindow.Show();
        }

        void OnEnable()
        {
            if (temp == null)
                temp = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            if (screenshotScript == null)
                screenshotScript = temp.AddComponent<ScreenshotScript>();
            if (screenshotScriptEditor == null)
                screenshotScriptEditor = Editor.CreateEditor(screenshotScript);

            if (shareScript == null)
                shareScript = temp.AddComponent<ShareScript>();
            if (updateShareWithScreenshotScript == null)
                updateShareWithScreenshotScript = temp.AddComponent<UpdateShareWithScreenshotScript>();

            screenshotScript.editorWindowMode = true;
        }

        void OnDestroy()
        {
            if (temp != null)
                DestroyImmediate(temp);
        }

        void OnGUI()
        {
            if (labelGUIStyle == null)
            {
                labelGUIStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                labelGUIStyle.fontStyle = FontStyle.Bold;
                labelGUIStyle.fontSize = 12;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            screenshotScriptEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
    }
}