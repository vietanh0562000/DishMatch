using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    [ExecuteAlways]
    public class PreviewScript : MonoBehaviour
    {
        public PreviewSettings previewSettings;

        // Cache sorted names list.
        public string[] names = new string[0];
        public Dictionary<string, Texture2D> previewsByName = new Dictionary<string, Texture2D>();
        public Dictionary<string, PreviewDevice> devicesByName = new Dictionary<string, PreviewDevice>();

        float lastAutoUpdate;

        #region Editor variables
#pragma warning disable 0414
        public int previewEditorRefreshHack;
        public bool previewUpdateInProgress;
#pragma warning restore 0414
        #endregion

        void Update()
        {
            if (!Application.isPlaying) return;

            if (previewSettings != null && previewSettings.autoUpdate)
            {
                if (Time.unscaledTime - lastAutoUpdate >= previewSettings.autoUpdateDelay)
                {
                    lastAutoUpdate = Time.unscaledTime;
                    UpdatePreviews();
                }
            }
        }

        public void UpdatePreviews(ScreenshotScript screenshotScript = null)
        {
            StartCoroutine(UpdatePreviewsCoroutine(screenshotScript));
        }

        public IEnumerator UpdatePreviewsCoroutine(ScreenshotScript screenshotScript = null)
        {
            previewUpdateInProgress = true;

            previewsByName = new Dictionary<string, Texture2D>();
#if UNITY_EDITOR
            int originalSelectedSizeIndex = GameView.GetSelectedSizeIndex();
#else
            Resolution originalResolution = ScreenExtensions.CurrentResolution();
#endif

            foreach (PreviewDevice device in previewSettings.previewDevices)
            {
                if (!device.active) continue;

                if (previewSettings.rotation != PreviewSettings.Rotation.Both)
                {
                    if (previewSettings.rotation == PreviewSettings.Rotation.Portrait)
                    {
                        if (device.width > device.height) continue;
                    }
                    else
                    {
                        if (device.height > device.width) continue;
                    }
                }

                Texture2D preview = null;
#if UNITY_EDITOR
                if (!GameView.SizeExists(device.size))
                    GameView.AddTempCustomSize(GameView.GameViewSizeType.FixedResolution, device.size);
#endif
                Resolution currentResolution = ScreenExtensions.CurrentResolution();
                bool resolutionIsDifferent = !device.size.IsSameSizeAs(currentResolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(device.size);
                    yield return new WaitForResolutionUpdates();
                }

                if (previewSettings.useScreenshotScript && screenshotScript != null)
                {
                    yield return screenshotScript.TakeSingleScreenshotCoroutine(false);
                    preview = screenshotScript.lastScreenshotTexture.EditableCopy();
                    if (device.sizingType == PreviewDevice.SizingType.SetScreenshotToSize && (preview.width != device.size.width || preview.height != device.size.height))
                    {
                        Debug.LogWarning("ScreenshotScript settings have adjusted size from captured rect. Scaling to compensate.\nExpected: " + device.size.width + "x" + device.size.height + " Received: " + preview.width + "x" + preview.height);
                        preview.Resize(device.size, false);
                    }
                }
                else
                {
                    yield return new WaitForEndOfFrame();

                    Rect fullRect = new Rect(0, 0, device.width, device.height);
                    preview = CaptureRawFrame.ScreenCapture(fullRect, ResolutionExtensions.EmptyResolution(), true).Process();
                }

                if (previewSettings.safeAreaEnabled)
                    preview = preview.OverlaySafeArea(previewSettings.safeAreaColor, device.safeAreaTopOffset, device.safeAreaBottomOffset, device.safeAreaLeftOffset, device.safeAreaRightOffset);

                if (device.type != PreviewDevice.Type.Screenshot)
                {
                    Texture2D frame = device.frame;
                    if (device.type == PreviewDevice.Type.TextureFrame)
                    {
                        switch (device.frameRotation)
                        {
                            case RotationType.Clockwise:
                                frame = frame.Rotate90Degrees(true, false, false);
                                break;
                            case RotationType.CounterClockwise:
                                frame = frame.Rotate90Degrees(false, false, false);
                                break;
                            case RotationType.Flip:
                                frame = frame.Rotate180Degrees(false, false);
                                break;
                        }
                    }
                    else if (device.type == PreviewDevice.Type.DeviceFrame)
                    {
                        // Device frames often require reloading. Likely due to importing them from outside Assets.
                        device.ReloadDeviceFrame();
                        frame = device.frame;

                        switch (device.orientation)
                        {
                            case ScreenOrientation.Portrait:
                                break;
                            case ScreenOrientation.PortraitUpsideDown:
                                frame = frame.Rotate180Degrees(false, false);
                                break;
                            case ScreenOrientation.LandscapeLeft:
                                frame = frame.Rotate90Degrees(false, false, false);
                                break;
                            case ScreenOrientation.LandscapeRight:
                                frame = frame.Rotate90Degrees(true, false, false);
                                break;
                            default:
                                Debug.LogError("Invalid orientation: " + device.orientation);
                                break;
                        }
                    }

                    FrameResizeMethod frameResizeMethod = device.sizingType == PreviewDevice.SizingType.SetScreenshotToSize ? FrameResizeMethod.ResizeFrameToFitTexture : FrameResizeMethod.ResizeBothToFitOriginalResolution;
                    preview = preview.AddFrame(frame, frameResizeMethod, Color.clear,  false);
                }
                preview.Apply(false);

                previewsByName[device.deviceName] = preview;
                devicesByName[device.deviceName] = device;
            }

#if UNITY_EDITOR
            if (GameView.GetSelectedSizeIndex() != originalSelectedSizeIndex)
                GameView.SetSelecedSizeIndex(originalSelectedSizeIndex);
#else
                Resolution finalResolution = ScreenExtensions.CurrentResolution();
                if (!finalResolution.IsSameSizeAs(originalResolution))
                    ScreenExtensions.UpdateResolution(originalResolution);
#endif
            // Cache sorted names list.
            names = new string[previewsByName.Keys.Count];
            previewsByName.Keys.CopyTo(names, 0);
            //System.Array.Sort(names);

            previewUpdateInProgress = false;
        }

        public void Save()
        {
            if(string.IsNullOrEmpty(previewSettings.saveDirectory))
            {
                Debug.LogError("No save directory set in preview settings");
                return;
            }

            foreach (string name in names)
            {
                string fileName = name;
                Texture2D preview = previewsByName[name];
                if (!(name.EndsWith(".png") || name.EndsWith(".jpg") || name.EndsWith(".jpeg")))
                    fileName += ".png";
                string filePath = System.IO.Path.Combine(previewSettings.saveDirectory, fileName);
                if (previewSettings.saveAsynchronously && Application.isPlaying)
                    preview.AsyncSaveToFilePath(filePath);
                else
                    preview.SyncSaveToFilePath(filePath);
                Debug.Log("Saved Preview to: " + filePath);
            }
        }
    }
}