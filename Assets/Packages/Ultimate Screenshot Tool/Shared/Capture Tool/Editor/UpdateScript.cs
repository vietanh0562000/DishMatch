using UnityEngine;
using UnityEditor;
#if UNITY_5_4_OR_NEWER
using UnityEditor.SceneManagement;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [InitializeOnLoad]
    public static class UpdateScript
    {
        const string TOOL_VERSION = "TRS_TOOL_VERSION_KEY";

        static UpdateScript()
        {
            UpdateIfNecessary();
        }

        public static void UpdateIfNecessary()
        {
            double savedToolVersion = System.Convert.ToDouble(SavedToolVersion());
            double currentToolVersion = System.Convert.ToDouble(ToolInfo.ToolVersion());
            if (savedToolVersion < currentToolVersion)
            {
                if (savedToolVersion < 3.52)
                {
                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
                    for (int i = 0; i < filePaths.Length; ++i)
                    {
                        string filePath = filePaths[i];
                        string filename = System.IO.Path.GetFileName(filePath);
                        string directory = System.IO.Path.GetDirectoryName(filePath);

                        // Delete original copies of files moved into folders.
                        if (filename == "PreviewDevice.cs" || filename == "PreviewScript.cs" || filename == "PreviewSettings.cs")
                        {
                            if (!directory.Contains("Preview"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                    }
                }
                if (savedToolVersion < 3.5)
                {
                    DeleteScriptWithName("DemoCanPreviewButtonScript.cs");
                    DeleteScriptWithName("DemoCanResetButtonScript.cs");
                    DeleteScriptWithName("DisableButtonWhileProcessingOrSavingScript.cs");
                    DeleteScriptWithName("SaveLoopDemoScript.cs");
                    DeleteScriptWithName("ToggleCaptureModeButtonScript.cs");
                    DeleteScriptWithName("ToggleRecordingButtonScript.cs");
                }
                if (savedToolVersion < 3.38)
                {
                    // Perform twice for combined tool.
                    DeleteScriptWithName("MigrationGuide.pdf");
                    DeleteScriptWithName("MigrationGuide.pdf");

                    DeleteScriptWithName("CaptureAction.cs");
                    DeleteScriptWithName("OnRenderImageScript.cs");
                    DeleteScriptWithName("RenderTextureRawFrameData.cs");

                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
                    for (int i = 0; i < filePaths.Length; ++i)
                    {
                        string filePath = filePaths[i];
                        string filename = System.IO.Path.GetFileName(filePath);

                        // Delete original copies of files moved into folders.
                        if (filename == "CutoutScreenshotSetScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "GameObjectScreenshotEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "MultiCameraGameObjectScreenshotScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "MultiCameraScreenshotScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "MultiLangScreenshotScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "ScreenshotBurstScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "ScreenshotSeriesScriptEditor.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                        else if (filename == "DemoLocalizationScript.cs")
                        {
                            if (!filePath.Contains("ScreenshotSubComponents"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }

                        // Delete old copy of file that was moved out of the folder.
                        if (filePath.Contains("ScreenshotSubComponents"))
                        {
                            if (filename == "ScreenshotSubComponentScript.cs")
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }

                        if (filePath.Contains("Capture Tool"))
                        {
                            // Delete files from deleted folder (moved into CaptureTool).
                            if (filePath.Contains("Helpful Extras"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }

                            // Delete old copy of file that was moved in to Helpful Extras.
                            if (filename == "TimeActivateScript.cs")
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }

                            // Delete old copy of file that was moved in to Helpful Extras.
                            if (filename == "AutoResetInputFieldScript.cs")
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }

                        // Delete old copy of file that was moved out of shared.
                        if (filename == "ScreenshotResolutionTransformation.cs")
                        {
                            if (filePath.Contains("Shared"))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                    }
                }

                if (savedToolVersion < 3.37)
                {
                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
                    foreach (string filePath in filePaths)
                    {
                        string filename = System.IO.Path.GetFileName(filePath);

                        if (!filePath.Contains("ScreenshotSubComponents"))
                        {
                            if (filename == "CutoutScreenshotSetScript.cs" || filename == "GameObjectScreenshotScript.cs" || filename == "MultiCameraScreenshotScript.cs" || filename == "MultiLangScreenshotScript.cs" || filename == "ScreenshotBurstScript.cs" || filename == "ScreenshotSeriesScript.cs")
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                    }
                }

                if (savedToolVersion < 3.36)
                {
                    DeleteScriptWithName("TransparentNeuQuant.cs");
                    DeleteScriptWithName("LoadGifFromScreenshotsScript.cs");
                    DeleteScriptWithName("AsyncCaptureScript.cs");
                    DeleteScriptWithName("Empty.cs");
                    DeleteScriptWithName("CutoutGraphicScript.cs");
                    DeleteScriptWithName("AbstractGifFrame.cs");
                    DeleteScriptWithName("FrameTransformationReorderableList.cs");
                }
                if (savedToolVersion < 3.34)
                {
                    DeleteScriptWithName("TransparentNeuQuant.cs");

                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
                    for(int i = 0; i < filePaths.Length; ++i)
                    {
                        string filePath = filePaths[i];
                        string filename = System.IO.Path.GetFileName(filePath);

                        // Delete old version of moved files.
                        if (filePath.Contains("Demo") && filename == "LastTakenScreenshotScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Demo") && filename == "ScreenshotDisplayScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Demo") && filename == "InstantScreenshotShareButtonScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                    }
                }
                if (savedToolVersion < 3.33)
                {
                    DeleteScriptWithName("LoadGifFromScreenshotsScript.cs");
                }
                if (savedToolVersion < 3.32)
                {
                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.unity", System.IO.SearchOption.AllDirectories);
                    for (int i = 0; i < filePaths.Length; ++i)
                    {
                        string filePath = filePaths[i];
                        string filename = System.IO.Path.GetFileName(filePath);

                        // Delete old version of moved files.
                        if (filePath.Contains("OtherDemos") && filename == "Demo.unity")
                            System.IO.File.Delete(filePath);
                    }
                }
                if (savedToolVersion < 3.28)
                {
                    DeleteScriptWithName("AsyncCaptureScript.cs");
                    DeleteScriptWithName("Empty.cs");

                    string[] filePaths = System.IO.Directory.GetFiles("Assets/", "*.cs", System.IO.SearchOption.AllDirectories);
                    foreach (string filePath in filePaths)
                    {
                        string filename = System.IO.Path.GetFileName(filePath);

                        // Delete old version of moved files.
                        if (filePath.Contains("Demo") && filename == "InstantScreenshotShareButtonScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Shared") && filename == "MultiLangScreenshotScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Shared") && filename == "MultiLangScreenshotScriptEditor.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Shared") && filename == "LegacyMultiLangScreenshotScriptEditor.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }
                        else if (filePath.Contains("Demo") && filename == "ScreenshotDisplayScript.cs")
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.Delete(filePath + ".meta");
                        }

                        if (!filePath.Contains("ScreenshotSubComponents"))
                        {
                            if (filename == "CutoutScreenshotSetScript.cs" || filename == "GameObjectScreenshotScript.cs" || filename == "MultiCameraScreenshotScript.cs" || filename == "MultiLangScreenshotScript.cs" || filename == "ScreenshotBurstScript.cs" || filename == "ScreenshotSeriesScript.cs")
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.Delete(filePath + ".meta");
                            }
                        }
                    }
                }

                if (savedToolVersion < 3.27)
                    DeleteScriptWithName("CutoutGraphicScript.cs");

                if (savedToolVersion < 3.26)
                    DeleteScriptWithName("AbstractGifFrame.cs");

                SaveToolVersion(ToolInfo.ToolVersion());
            }
        }

        public static void DeleteScriptWithName(string scriptName)
        {
            string filePath = PathExtensions.GetFilePath("Assets/", "*.cs", scriptName);
            if (string.IsNullOrEmpty(filePath)) return;

            System.IO.File.Delete(filePath);
            System.IO.File.Delete(filePath + ".meta");
        }

        #region Track Tool Version
        public static string SavedToolVersion()
        {
            if (PlayerPrefs.HasKey(TOOL_VERSION))
                return PlayerPrefs.GetString(TOOL_VERSION);

            return null;
        }

        public static void SaveToolVersion(string toolVersion)
        {
            PlayerPrefs.SetString(TOOL_VERSION, toolVersion);
            PlayerPrefs.Save();
        }
        #endregion
    }
}