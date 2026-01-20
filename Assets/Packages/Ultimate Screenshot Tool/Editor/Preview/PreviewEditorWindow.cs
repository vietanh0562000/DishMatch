using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace TRS.CaptureTool
{
    [ExecuteAlways]
    public class PreviewEditorWindow : EditorWindow
    {
        const string TRS_PREVIEW_SETTINGS_LOCATION_KEY = "TRS_PREVIEW_SETTINGS_LOCATION_KEY";
        const float BUTTON_WIDTH = 120f;

        ScreenshotScript screenshotScript;
        PreviewScript previewScript;
        PreviewSettings previewSettings;

        GameObject temp;
        Vector2 scrollPos;
        GUIStyle labelGUIStyle;
        GUIStyle gridCellGUIStyle;

        [MenuItem("Assets/Create/Preview Settings")]
        public static PreviewSettings Create()
        {
            return PreviewSettings.CreateWithFileName("NewPreviewSettings.asset");
        }

        void OnInspectorUpdate()
        {
            if (previewScript.previewUpdateInProgress)
                EditorApplication.QueuePlayerLoopUpdate();
        }

        [MenuItem("Tools/Ultimate Screenshot Tool/Preview Window", false, 13)]
        [MenuItem("Window/Ultimate Screenshot Tool Preview", false, 13)]
        static void Init()
        {
            PreviewEditorWindow editorWindow = (PreviewEditorWindow)GetWindow(typeof(PreviewEditorWindow));
            GUIContent titleContent = new GUIContent("Preview");
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent = titleContent;
            editorWindow.Show();
        }

        void OnEnable()
        {
            if (previewSettings == null) {
                if(EditorPrefs.HasKey(TRS_PREVIEW_SETTINGS_LOCATION_KEY))
                    previewSettings = AssetDatabase.LoadAssetAtPath<PreviewSettings>(EditorPrefs.GetString(TRS_PREVIEW_SETTINGS_LOCATION_KEY));

                if(previewSettings ==  null)
                {
                    EditorPrefs.DeleteKey(TRS_PREVIEW_SETTINGS_LOCATION_KEY);
                    previewSettings = PreviewSettings.Default();
                }
            }

            if (temp == null)
                temp = new GameObject { hideFlags = HideFlags.HideAndDontSave };
            if (previewScript == null)
                previewScript = temp.AddComponent<PreviewScript>();
            previewScript.previewSettings = previewSettings;

            UpdateScreenshotScript();

#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }

        void OnDisable()
        {
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }

        void OnDestroy()
        {
            if (temp != null)
                DestroyImmediate(temp);
        }

        void OnGUI()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !previewScript.previewUpdateInProgress && !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (previewScript.previewUpdateInProgress)
                ForceUpdates();

            if (labelGUIStyle == null)
            {
                labelGUIStyle = new GUIStyle(GUI.skin.label);
                labelGUIStyle.fontStyle = FontStyle.Bold;
                labelGUIStyle.fontSize = 16;
                labelGUIStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (gridCellGUIStyle == null)
            {
                gridCellGUIStyle = new GUIStyle(GUI.skin.label);
                gridCellGUIStyle.fontStyle = FontStyle.Bold;
                gridCellGUIStyle.fontSize = 16;
                gridCellGUIStyle.imagePosition = ImagePosition.ImageAbove;
                gridCellGUIStyle.alignment = TextAnchor.MiddleCenter;
            }

            float width = position.width;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            StartNewLineIfNecessary(BUTTON_WIDTH, ref width);
            if (GUILayout.Button("Update", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewScript.UpdatePreviews(screenshotScript);

            StartNewLineIfNecessary(BUTTON_WIDTH, ref width);
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewScript.Save();

            StartNewLineIfNecessary(BUTTON_WIDTH, ref width);
            if (GUILayout.Button("Safe Area " + StringForEnabledStatus(previewSettings != null ? previewSettings.safeAreaEnabled : false), EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewSettings.safeAreaEnabled = !previewSettings.safeAreaEnabled;

            StartNewLineIfNecessary(BUTTON_WIDTH, ref width);
            if (GUILayout.Button(PreviewSettings.NameForRotation(previewSettings != null ? previewSettings.rotation : PreviewSettings.Rotation.Both), EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewSettings.rotation = NextRotation(previewSettings.rotation);

            StartNewLineIfNecessary(BUTTON_WIDTH, ref width);
            string gridButtonName = previewSettings != null ? (previewSettings.displayInGrid ? "Grid" : "Condensed") : "Condensed";
            if (GUILayout.Button(gridButtonName, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewSettings.displayInGrid = !previewSettings.displayInGrid;

            StartNewLineIfNecessary(115f * 3, ref width);
            if(previewSettings != null && previewSettings.useScreenshotScript) {
                string screenshotScriptTooltip = "Optional. Used to capture screenshots with additional settings like capture list, transformations, etc.";
                EditorGUILayout.LabelField(new GUIContent("ScreenshotScript", screenshotScriptTooltip), GUILayout.MaxWidth(115f));
                screenshotScript = EditorGUILayout.ObjectField(screenshotScript, typeof(ScreenshotScript), true) as ScreenshotScript;
            }

            EditorGUILayout.LabelField("Devices & Settings", GUILayout.MaxWidth(115f));
            EditorGUI.BeginChangeCheck();
            previewSettings = EditorGUILayout.ObjectField(previewSettings, typeof(PreviewSettings), false) as PreviewSettings;
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(TRS_PREVIEW_SETTINGS_LOCATION_KEY, AssetDatabase.GetAssetPath(previewSettings));
                previewScript.previewSettings = previewSettings;
                EditorUtility.SetDirty(previewScript);

                Selection.activeObject = previewSettings;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            width = position.width;

            StartNewLineIfNecessary(BUTTON_WIDTH + 105f * 3, ref width);
            string autoUpdateButtonText = "Auto Update " + StringForEnabledStatus(Application.isPlaying ? previewSettings != null && previewSettings.autoUpdate : false);
            bool nestedOriginalGUIEnabled = GUI.enabled;
            GUI.enabled &= Application.isPlaying;
            if (GUILayout.Button(autoUpdateButtonText, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewSettings.autoUpdate = !previewSettings.autoUpdate;
            GUI.enabled = nestedOriginalGUIEnabled;

            nestedOriginalGUIEnabled = GUI.enabled;
            GUI.enabled &= previewSettings != null;
            EditorGUILayout.LabelField("Update Delay (s)", GUILayout.MaxWidth(105f));
            if(previewSettings != null)
                previewSettings.autoUpdateDelay = EditorGUILayout.Slider(previewSettings.autoUpdateDelay, 0.5f, 30f);
            else
                EditorGUILayout.Slider(1.0f, 0.5f, 30f);
            GUILayout.Space(20f);

            StartNewLineIfNecessary(45f * 3, ref width);
            EditorGUILayout.LabelField("Scale", GUILayout.MaxWidth(45f));
            if (previewSettings != null)
                previewSettings.scale = EditorGUILayout.Slider(previewSettings.scale, 0.01f, 2f);
            else
                EditorGUILayout.Slider(1.0f, 0.01f, 2f);

            if (GUILayout.Button("1:1", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                previewSettings.scale = 1f;

            GUI.enabled = nestedOriginalGUIEnabled;
            EditorGUILayout.EndHorizontal();

            List<GUIContent> previews = new List<GUIContent>(previewScript.names.Length);
            List<Rect> textureRects = new List<Rect>(previewScript.names.Length);
            List<Rect> labelRects = new List<Rect>(previewScript.names.Length);

            // Subtract the scroll bar width.
            float previewWidth = position.width - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (previewScript != null)// && Event.current.type.Equals(EventType.Repaint))
            {
                bool hasPreviews = previewScript.previewsByName != null && previewScript.previewsByName.Count > 0;
                if (hasPreviews)
                {
                    float totalHeight = 0;
                    float maxRowHeight = 0;
                    const float padding = 50;
                    int itemCountInRow = 0;
                    int minItemsPerRow = previewScript.names.Length;
                    Rect previewPosition = new Rect(0, 0, 0, 0);
                    for (int i = 0; i < previewScript.names.Length; ++i)
                    {
                        string name = previewScript.names[i];
                        if (!previewScript.previewsByName.ContainsKey(name)) continue;
                        Texture2D preview = previewScript.previewsByName[name];
                        if (preview == null) continue;

                        PreviewDevice device = previewScript.devicesByName[name];
                        string displayName = name;
                        if (previewSettings != null && previewSettings.excludeFolderPathFromDeviceDisplayName)
                        {
                            int slashIndex = displayName.LastIndexOf("/") + 1;
                            displayName = displayName.Substring(slashIndex, displayName.Length - slashIndex);
                        }
                        displayName += "\n" + device.width + "x" + device.height;
                        previews.Add(new GUIContent(displayName, preview));

                        float previewScale = previewSettings != null ? previewSettings.scale : 1.0f;
                        if (previewPosition.x == 0)
                        {
                            previewPosition.x = padding;
                            previewPosition.width -= padding;
                        }
                        previewPosition.width = preview.width * previewScale;

                        float textureHeight = preview.height * previewScale;
                        float previewHeight = textureHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 2;
                        if (previewPosition.x + previewPosition.width >= previewWidth)
                        {
                            previewPosition.x = 0;
                            previewPosition.y += maxRowHeight + padding;
                            maxRowHeight = 0;
                            if (itemCountInRow < minItemsPerRow)
                                minItemsPerRow = itemCountInRow;
                        }
                        else
                            ++itemCountInRow;

                        if (previewHeight > maxRowHeight) maxRowHeight = previewHeight;

                        float originalY = previewPosition.y;
                        previewPosition.height = textureHeight;
                        textureRects.Add(previewPosition);
                        previewPosition.y += previewPosition.height + EditorGUIUtility.standardVerticalSpacing;
                        previewPosition.height = EditorGUIUtility.singleLineHeight * 2;
                        previewPosition.x -= padding;
                        previewPosition.width += padding * 2;
                        labelRects.Add(previewPosition);

                        // Padding is included in the label padding shift.
                        previewPosition.x += previewPosition.width;
                        float possibleHeight = previewPosition.y + previewPosition.height + EditorGUIUtility.singleLineHeight;
                        if (possibleHeight > totalHeight)
                            totalHeight = possibleHeight;

                        previewPosition.y = originalY;
                    }

                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float currentYPosition = lastRect.y + lastRect.height;
                    float additionalOffset = (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * 2;
                    Rect controlRect = EditorGUILayout.GetControlRect(false, position.height - currentYPosition - additionalOffset);

                    Rect viewRect = new Rect(0, 0, previewWidth, totalHeight);
                    scrollPos = GUI.BeginScrollView(controlRect, scrollPos, viewRect);
                    if (previewSettings != null && previewSettings.displayInGrid)
                    {
                        if (minItemsPerRow <= 0) minItemsPerRow = 1;
                        GUI.SelectionGrid(viewRect, -1, previews.ToArray(), minItemsPerRow, gridCellGUIStyle);
                    }
                    else
                    {
                        for (int i = 0; i < textureRects.Count; ++i)
                        {
                            GUI.DrawTexture(textureRects[i], previews[i].image, ScaleMode.ScaleToFit);
                            GUI.Label(labelRects[i], previews[i].text, labelGUIStyle);
                        }
                    }
                    GUI.EndScrollView();
                } else
                {
                    GUILayout.BeginVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Preview", GUILayout.Width(150), GUILayout.Height(40)))
                        previewScript.UpdatePreviews(screenshotScript);

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();

                    GUILayout.EndVertical();
                }
            }

            GUI.enabled = originalGUIEnabled;
        }

        // RequiresConstantRepaint doesn't avert getting paused within a Take All Screenshot Resolutions loop.
        void ForceUpdates()
        {
            if (previewScript.previewUpdateInProgress)
                previewScript.previewEditorRefreshHack += 1;
            else if (previewScript.previewEditorRefreshHack > 0)
                previewScript.previewEditorRefreshHack -= 1;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateScreenshotScript();
        }

        void UpdateScreenshotScript()
        {
            if (screenshotScript == null)
                screenshotScript = FindObjectOfType<ScreenshotScript>();
        }

        void StartNewLineIfNecessary(float itemWidth, ref float width)
        {
            if (itemWidth > width)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

                width = position.width;
            }
            width -= itemWidth;
        }

        string StringForEnabledStatus(bool enabled)
        {
            return enabled ? "(ON)" : "(OFF)";
        }

        PreviewSettings.Rotation NextRotation(PreviewSettings.Rotation rotation)
        {
            switch (rotation)
            {
                case PreviewSettings.Rotation.Portrait:
                    return PreviewSettings.Rotation.Landscape;
                case PreviewSettings.Rotation.Landscape:
                    return PreviewSettings.Rotation.Both;
                case PreviewSettings.Rotation.Both:
                    return PreviewSettings.Rotation.Portrait;
            }

            throw new UnityException("Unhandled rotation in NextRotation switch statement");
        }
    }
}