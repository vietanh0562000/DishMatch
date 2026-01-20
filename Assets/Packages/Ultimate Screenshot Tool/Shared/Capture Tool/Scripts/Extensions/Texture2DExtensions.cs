using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public class TextureEncodingParameters
    {
        public int jpgQuality = 100;
        public Texture2D.EXRFlags exrFlags = Texture2D.EXRFlags.None;
    }

    public static class Texture2DExtensions
    {
        #region Transformations
        public static Texture2D ApplyTransformation(this Texture2D texture, TextureTransformation textureTransformation, bool apply = true, bool destroyOriginal = true)
        {
            return textureTransformation.ApplyTransformation(texture, apply, destroyOriginal);
        }

        public static Texture2D ApplyTransformations(this Texture2D texture, TextureTransformation[] textureTransformations, bool apply = true, bool destroyOriginals = true)
        {
            Texture2D editableTexture = texture.EditableTexture(!destroyOriginals);
            for (int i = 0; i < textureTransformations.Length; ++i)
            {
                TextureTransformation frameTransformation = textureTransformations[i];
                editableTexture = frameTransformation.ApplyTransformation(editableTexture, false, destroyOriginals);
            }

            if (apply)
                editableTexture.Apply(false);
            return editableTexture;
        }

        public static Texture2D AddFrame(this Texture2D texture, Texture2D frame, FrameResizeMethod frameResizeMethod, Color backgroundFillColor, bool apply = true, bool destroyOriginal = true)
        {
            if (frame == null)
            {
                Debug.LogError("Frame must be set to apply add frame transformation.");
                return texture;
            }

            if (frameResizeMethod == FrameResizeMethod.ResizeBothToFitOriginalResolution && (frame.width != texture.width || frame.height != texture.height))
            {
                Resolution resolution = new Resolution { width = texture.width, height = texture.height };
                frame = frame.EditableCopy().Resize(resolution, true);
            }

            Rect rectWithinFrame = RectWithinFrame(frame);
            if (frameResizeMethod == FrameResizeMethod.ResizeTextureToFitFrame || frameResizeMethod == FrameResizeMethod.ResizeBothToFitOriginalResolution)
            {
                Resolution resolution = new Resolution { width = (int)rectWithinFrame.width, height = (int)rectWithinFrame.height };
                if (!destroyOriginal)
                    texture = texture.EditableCopy();
                texture = texture.Resize(resolution, true);
            }
            else if (frameResizeMethod == FrameResizeMethod.ResizeFrameToFitTexture && (rectWithinFrame.width != texture.width || rectWithinFrame.height != texture.height))
            {
                float scaleX = (float)texture.width / (float)rectWithinFrame.width;
                float scaleY = (float)texture.height / (float)rectWithinFrame.height;
                if (scaleX != scaleY)
                    Debug.LogWarning("Aspect ratio of frame is being altered to fit the texture. Scaling X: " + scaleX + " Scaling Y: " + scaleY);

                float scaledWidth = frame.width * scaleX;
                float scaledHeight = frame.height * scaleY;
                Resolution resolution = new Resolution { width = (int)scaledWidth, height = (int)scaledHeight };
                frame = frame.EditableCopy().Resize(resolution, true);
                rectWithinFrame = RectWithinFrame(frame);
            }

            Texture2D result = texture.Blend(frame, new Vector2Int((int)-rectWithinFrame.x, (int)-rectWithinFrame.y), false, backgroundFillColor, true, apply, false);
            if (destroyOriginal) texture.DestroyIfPossible();
            return result;
        }

        public static Texture2D ApplyShader(this Texture2D texture, Shader shader, bool apply = true, bool destroyOriginal = true)
        {
            if (shader == null)
            {
                Debug.LogError("Shader is null and cannot be applied.");
                return texture;
            }

            return texture.ApplyMaterial(new Material(shader), apply, destroyOriginal);
        }

        public static Texture2D ApplyMaterial(this Texture2D texture, Material material, bool apply = true, bool destroyOriginal = true)
        {
            if (material == null)
            {
                Debug.LogError("Material is null and cannot be applied.");
                return texture;
            }

            return texture.ApplyMaterials(new Material[] { material }, apply, destroyOriginal);
        }

        public static Texture2D ApplyShaders(this Texture2D texture, Shader[] shaders, bool apply = true, bool destroyOriginal = true)
        {
            if (shaders == null || shaders.Length <= 0)
            {
                Debug.LogError("Shaders array is null or empty and cannot be applied.");
                return texture;
            }

            Material[] materials = new Material[shaders.Length];
            for (int i = 0; i < shaders.Length; ++i)
                materials[i] = new Material(shaders[i]);

            return texture.ApplyMaterials(materials, apply, destroyOriginal);
        }

        public static Texture2D ApplyMaterials(this Texture2D texture, Material[] materials, bool apply = true, bool destroyOriginal = true)
        {
            if(materials == null || materials.Length <= 0)
            {
                Debug.LogError("Materials array is null or empty and cannot be applied.");
                return texture;
            }

            // Texture must be applied prior to Blit.
            texture.Apply(false);

            // Shaders only apply on the GPU, so render material to a render texture and read pixel back to the original texture.
            RenderTexture tempRenderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, tempRenderTexture, materials[0]);

            if(materials.Length > 1)
            {
                RenderTexture tempRenderTextureAlt = RenderTexture.GetTemporary(texture.width, texture.height, 0);

                for (int i = 1; i < materials.Length; ++i)
                    Graphics.Blit(i % 2 == 0 ? tempRenderTextureAlt : tempRenderTexture, i % 2 == 1 ? tempRenderTextureAlt : tempRenderTexture, materials[i]);

                // If we last transferred to the alt, use that.
                if ((materials.Length - 1) % 2 == 1)
                {
                    RenderTexture.ReleaseTemporary(tempRenderTexture);
                    tempRenderTexture = tempRenderTextureAlt;
                } else
                    RenderTexture.ReleaseTemporary(tempRenderTextureAlt);
            }

            Texture2D resultTexture = texture;
            if (!destroyOriginal)
                resultTexture = new Texture2D((int)texture.width, (int)texture.height, texture.format, false);
            resultTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, false);

            RenderTexture.ReleaseTemporary(tempRenderTexture);
            if (apply)
                resultTexture.Apply(false);

            return resultTexture;
        }

        public static Texture2D Cutout(this Texture2D original, Rect cutoutRect, bool apply = true, bool destroyOriginal = true)
        {
            if (cutoutRect == Rect.zero || cutoutRect == new Rect(0, 0, original.width, original.height))
                return original;

            bool editableOriginal = original.format == original.EditableTextureFormat();
            bool useMipMaps = original.mipmapCount > 1;
            Texture2D resultTexture = new Texture2D((int)cutoutRect.width, (int)cutoutRect.height, original.EditableTextureFormat(), useMipMaps);
            Graphics.CopyTexture(original, 0, useMipMaps ? original.mipmapCount : 0, (int)cutoutRect.x, (int)cutoutRect.y, (int)cutoutRect.width, (int)cutoutRect.height, resultTexture, 0, useMipMaps ? resultTexture.mipmapCount : 0, 0, 0);

            if (editableOriginal && destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(original);

            return resultTexture;
        }

        public static Texture2D SafeAreaOverlayTexture(Vector2Int size, Color32 overlayColor, int top, int bottom, int left, int right, bool apply = true)
        {
            Color32[] resultPixels = new Color32[size.x * size.y];

            int maxClearRow = size.y - top - 1;
            int maxClearCol = size.x - right - 1;
            for (int row = 0; row < size.y; ++row)
            {
                bool rowTriggersOverlay = row < bottom || row > maxClearRow;
                for (int col = 0; col < size.x; ++col)
                {
                    int resultPixelIndex = row * size.x + col;
                    bool showOverlay = rowTriggersOverlay || col < left || col > maxClearCol;
                    if (showOverlay)
                        resultPixels[resultPixelIndex] = overlayColor;
                    else
                        resultPixels[resultPixelIndex] = Color.clear;
                }
            }

            Texture2D resultTexture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);
            resultTexture.SetPixels32(resultPixels);
            if (apply)
                resultTexture.Apply(false);

            return resultTexture;
        }

        public static Texture2D OverlaySafeArea(this Texture2D original, Color32 overlayColor, int top, int bottom, int left, int right, bool apply = true, bool destroyOriginal = true)
        {
            if (top == 0 && bottom == 0 && left == 0 && right == 0)
                return original;

            Texture2D overlayTexture = SafeAreaOverlayTexture(new Vector2Int(original.width, original.height), overlayColor, top, bottom, left, right, false);
            if (!destroyOriginal)
                overlayTexture.DestroyIfPossible();

            Texture2D resultTexture = original.Blend(overlayTexture, true, true, apply, destroyOriginal);
            return resultTexture;
        }

        public static Texture2D Solidify(this Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            if (!TextureFormatHasAlpha(texture.format))
                return texture;

            Texture2D resultTexture = texture;
            if (texture.format != texture.EditableTextureFormat())
                resultTexture = new Texture2D(texture.width, texture.height, texture.EditableTextureFormat(), texture.mipmapCount > 1);

            // Color32 is faster if available.
            if(TextureFormatHas8BitPrecision(texture.format))
            {
                Color32[] resultPixels = texture.GetPixels32();
                for (var i = 0; i < resultPixels.Length; ++i)
                    resultPixels[i].a = 255;
                resultTexture.SetPixels32(resultPixels);
            } else
            {
                Color[] resultPixels = texture.GetPixels();
                for (var i = 0; i < resultPixels.Length; ++i)
                    resultPixels[i].a = 1f;
                resultTexture.SetPixels(resultPixels);
            }

            if (apply)
                resultTexture.Apply(false);

            if (resultTexture != texture && destroyOriginal) MonoBehaviourExtended.FlexibleDestroy(texture);
            return resultTexture;
        }

        public static Texture2D Resize(this Texture2D texture, Resolution resizeResolution, bool apply = true)
        {
            if ((texture.width == resizeResolution.width && texture.height == resizeResolution.height) || !resizeResolution.HasSize())
                return texture;

            Texture2D editableTexture = texture.EditableTexture();

            if (TextureFormatHas8BitPrecision(texture.format))
                FastTextureScale.Bilinear(texture, resizeResolution.width, resizeResolution.height, apply);
            else
                TextureScale.Bilinear(texture, resizeResolution.width, resizeResolution.height, apply);

            return editableTexture;
        }

        public static Texture2D Rotate90Degrees(this Texture2D texture, bool clockwise, bool apply = true, bool destroyOriginal = true)
        {
            // Color32 is faster if available.
            if (TextureFormatHas8BitPrecision(texture.format))
                return texture.Rotate90Degrees32(clockwise, apply, destroyOriginal);

            return texture.Rotate90DegreesFull(clockwise, apply, destroyOriginal);
        }

        public static Texture2D Rotate90Degrees32(this Texture2D texture, bool clockwise, bool apply = true, bool destroyOriginal = true)
        {
            Color32[] original = texture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];

            for (int row = 0; row < texture.height; ++row)
            {
                for (int col = 0; col < texture.width; ++col)
                {
                    int rotatedIndex = (col + 1) * texture.height - row - 1;
                    int originalIndex = clockwise ? original.Length - 1 - (row * texture.width + col) : row * texture.width + col;
                    rotated[rotatedIndex] = original[originalIndex];
                }
            }

            Texture2D rotatedTexture = new Texture2D(texture.height, texture.width);
            rotatedTexture.SetPixels32(rotated);
            if(apply)
                rotatedTexture.Apply();
            if (destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(texture);
            return rotatedTexture;
        }

        public static Texture2D Rotate90DegreesFull(this Texture2D texture, bool clockwise, bool apply = true, bool destroyOriginal = true)
        {
            Color[] original = texture.GetPixels();
            Color[] rotated = new Color[original.Length];

            for (int row = 0; row < texture.height; ++row)
            {
                for (int col = 0; col < texture.width; ++col)
                {
                    int rotatedIndex = (col + 1) * texture.height - row - 1;
                    int originalIndex = clockwise ? original.Length - 1 - (row * texture.width + col) : row * texture.width + col;
                    rotated[rotatedIndex] = original[originalIndex];
                }
            }

            Texture2D rotatedTexture = new Texture2D(texture.height, texture.width);
            rotatedTexture.SetPixels(rotated);
            if (apply)
                rotatedTexture.Apply();
            if (destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(texture);
            return rotatedTexture;
        }

        public static Texture2D Rotate180Degrees(this Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            // Color32 is faster if available.
            if (TextureFormatHas8BitPrecision(texture.format))
                return texture.Rotate180Degrees32(apply, destroyOriginal);

            return texture.Rotate180DegreesFull(apply, destroyOriginal);
        }

        public static Texture2D Rotate180Degrees32(this Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            Color32[] original = texture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];

            for (int i = 0; i < rotated.Length; ++i)
                rotated[i] = original[original.Length - i - 1];

            Texture2D rotatedTexture = new Texture2D(texture.width, texture.height);
            rotatedTexture.SetPixels32(rotated);
            if (apply)
                rotatedTexture.Apply();
            if (destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(texture);
            return rotatedTexture;
        }

        public static Texture2D Rotate180DegreesFull(this Texture2D texture, bool apply = true, bool destroyOriginal = true)
        {
            Color[] original = texture.GetPixels();
            Color[] rotated = new Color[original.Length];

            for (int i = 0; i < rotated.Length; ++i)
                rotated[i] = original[original.Length - i - 1];


            Texture2D rotatedTexture = new Texture2D(texture.width, texture.height);
            rotatedTexture.SetPixels(rotated);
            if (apply)
                rotatedTexture.Apply();
            if (destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(texture);
            return rotatedTexture;
        }

        public static Texture2D Blend(this Texture2D background, Texture2D foreground, bool overlapOnly, bool alphaBlend, bool apply = true, bool destroyOriginals = true)
        {
            return background.Blend(foreground, new Vector2Int(0, 0), overlapOnly, Color.clear, alphaBlend, apply, destroyOriginals);
        }

        public static Texture2D AlphaBlend(this Texture2D background, Texture2D foreground, bool overlapOnly, bool apply = true, bool destroyOriginals = true)
        {
            return background.Blend(foreground, new Vector2Int(0, 0), overlapOnly, Color.clear, true, apply, destroyOriginals);
        }

        public static Texture2D SLBlend(this Texture2D background, Texture2D foreground, bool overlapOnly, bool apply = true, bool destroyOriginals = true)
        {
            return background.Blend(foreground, new Vector2Int(0, 0), overlapOnly, Color.clear, false, apply, destroyOriginals);
        }

        public static Texture2D AlphaBlend(this Texture2D background, Texture2D foreground, Vector2Int position, bool overlapOnly, Color emptySpaceFillColor, bool apply = true, bool destroyOriginals = true)
        {
            return background.Blend(foreground, position, overlapOnly, emptySpaceFillColor, true, apply, destroyOriginals);
        }

        public static Texture2D SLBlend(this Texture2D background, Texture2D foreground, Vector2Int position, bool overlapOnly, Color emptySpaceFillColor, bool apply = true, bool destroyOriginals = true)
        {
            return background.Blend(foreground, position, overlapOnly, emptySpaceFillColor, false, apply, destroyOriginals);
        }

        public static Vector2Int FinalSizeForBlend(Vector2Int backgroundSize, Vector2Int foregroundSize, Vector2Int position, bool overlapOnly)
        {
            int resultWidth;
            int resultHeight;
            if (overlapOnly)
            {
                if (position.x < 0)
                    resultWidth = Mathf.Min(foregroundSize.x + position.x, backgroundSize.x);
                else
                    resultWidth = Mathf.Min(backgroundSize.x - position.x, foregroundSize.x);

                if (position.y < 0)
                    resultHeight = Mathf.Min(foregroundSize.y + position.y, backgroundSize.y);
                else
                    resultHeight = Mathf.Min(backgroundSize.y - position.y, foregroundSize.y);

                if (resultWidth < 0 || resultHeight < 0)
                {
                    Debug.LogError("Textures do no overlap as layered. Size would be: " + resultWidth + "x" + resultHeight);
                    return new Vector2Int(0, 0);
                }
            } else {
                if (position.x < 0)
                    resultWidth = Mathf.Abs(position.x) + Mathf.Max(foregroundSize.x + position.x, backgroundSize.x);
                else
                    resultWidth = Mathf.Max(position.x + foregroundSize.x, backgroundSize.x);

                if (position.y < 0)
                    resultHeight = Mathf.Abs(position.y) + Mathf.Max(foregroundSize.y + position.y, backgroundSize.y);
                else
                    resultHeight = Mathf.Max(position.y + foregroundSize.y, backgroundSize.y);
            }

            return new Vector2Int(resultWidth, resultHeight);
        }

        // Inspired by this method: https://answers.unity.com/questions/1008802/merge-multiple-png-images-one-on-top-of-the-other.html
        // Modified to allow different sized textures.
        // Position is from top left. Negative offsets are supported.
        public static Texture2D Blend(this Texture2D background, Texture2D foreground, Vector2Int position, bool overlapOnly, Color32 emptySpaceFillColor, bool alphaBlend = true, bool apply = true, bool destroyOriginals = true)
        {
            // Color32 is faster if available.
            if (TextureFormatHas8BitPrecision(background.format) && TextureFormatHas8BitPrecision(foreground.format))
                return background.Blend32(foreground, position, overlapOnly, emptySpaceFillColor, alphaBlend, apply, destroyOriginals);

            return background.BlendFull(foreground, position, overlapOnly, emptySpaceFillColor, alphaBlend, apply, destroyOriginals);
        }

        public static void SetColor(this Texture2D texture, Color color, bool apply = true)
        {
            Color[] resultPixels = texture.GetPixels();
            for (int i = 0; i < resultPixels.Length; ++i)
                resultPixels[i] = color;

            texture.SetPixels(resultPixels);
            if (apply)
                texture.Apply(false);
        }

        public static void SetColor32(this Texture2D texture, Color32 color, bool apply = true)
        {
            Color32[] resultPixels = texture.GetPixels32();
            for (int i = 0; i < resultPixels.Length; ++i)
                resultPixels[i] = color;

            texture.SetPixels32(resultPixels);
            if (apply)
                texture.Apply(false);
        }
        #endregion

        #region Save and Load Textures
        // Load from any texture. Texture size does not matter, since LoadImage will replace with with incoming image size.
        // Example: Texture2D fileTexture = (new Texture2D(0, 0)).LoadFromFilePath(filePath);
        public static Texture2D LoadFromFilePath(this Texture2D texture, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError("No texture exists at: " + filePath);
                return null;
            }
            byte[] textureBytes = System.IO.File.ReadAllBytes(filePath);
            texture.LoadImage(textureBytes);
            return texture;
        }

        [System.Obsolete("SaveToFilePath is deprecated. Please explicitly use SyncSaveToFilePath or AsyncSaveToFilePath.")]
        public static bool SaveToFilePath(this Texture2D texture, string filePath, TextureEncodingParameters texture2DEncodingDetails = null)
        {
            return texture.SyncSaveToFilePath(filePath, texture2DEncodingDetails);
        }

        /* Save texture to file path asynchronously. Return value bool indicates if successful. */
        public static void AsyncSaveToFilePath(this Texture2D texture, string filePath, TextureEncodingParameters texture2DEncodingDetails = null, System.Action<string, bool> completionBlock = null, System.Threading.ThreadPriority threadPriority = System.Threading.ThreadPriority.BelowNormal)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            AsyncSaveToFilePath(texture.ToBytes(extension, texture2DEncodingDetails), filePath, completionBlock, threadPriority);
        }

        /* Save texture to file path. Return value bool indicates if successful. */
        public static bool SyncSaveToFilePath(this Texture2D texture, string filePath, TextureEncodingParameters texture2DEncodingDetails = null)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            return SaveToFilePath(texture.ToBytes(extension, texture2DEncodingDetails), filePath);
        }

        // Automatically requests gallery permissions if necessary.
        // To request permissions prior to attempting to save use:
        // NativeGallery.Permission permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
        public static void SaveAccordingToFileSettings(this Texture2D texture, ScreenshotFileSettings fileSettings, string overrideFilePath = "", System.Action<string, bool> completionBlock = null)
        {
            if (fileSettings.saveInBackground)
                AsyncSaveAccordingToFileSettings(texture, fileSettings, overrideFilePath, completionBlock);
            else
            {
                string filePath = string.IsNullOrEmpty(overrideFilePath) ? fileSettings.FullFilePath() : overrideFilePath;
                bool savedSuccessfully = SyncSaveAccordingToFileSettings(texture, fileSettings, overrideFilePath);
                completionBlock(filePath, savedSuccessfully);
            }
        }

        // Automatically requests gallery permissions if necessary.
        // To request permissions prior to attempting to save use:
        // NativeGallery.Permission permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
        public static void AsyncSaveAccordingToFileSettings(this Texture2D texture, ScreenshotFileSettings fileSettings, string overrideFilePath = "", System.Action<string, bool> completionBlock = null)
        {
            string filePath = string.IsNullOrEmpty(overrideFilePath) ? fileSettings.FullFilePath() : overrideFilePath;
            fileSettings.ValidateFilePath(filePath);

            string extension = System.IO.Path.GetExtension(filePath);
            byte[] textureBytes = texture.ToBytes(extension, fileSettings.encodingParameters);

            bool saveToGallery = false;
#if UNITY_IOS || UNITY_ANDROID
            saveToGallery = fileSettings.saveToGallery;
#endif

            if (fileSettings.persistLocally || saveToGallery)
            {
                System.Action<string, bool> completionBlockToUse = completionBlock;
                if (saveToGallery)
                {
                    completionBlockToUse = (string savedFilePath, bool savedSuccessfully) =>
                    {
                        if (string.IsNullOrEmpty(fileSettings.album))
                            Debug.LogError("Album name must be set to save to gallery. Set on the Edit & Save Tab > Save Settings > Mobile Save Settings");

                        // Use savedFilePath as NativeGallery returns null to Editor callbacks.
                        NativeGallery.SaveImageToGallery(savedFilePath, fileSettings.album, System.IO.Path.GetFileName(savedFilePath),
                            (success, path) => {
                                if (!fileSettings.persistLocally) { System.IO.File.Delete(savedFilePath); }
                                if (!success) { Debug.LogError("Save to NativeGallery.SaveImageToGallery Failed"); }
                                completionBlock(savedFilePath, success);
                            });
                    };
                }
                AsyncSaveToFilePath(textureBytes, filePath, completionBlockToUse, fileSettings.threadPriority);
            }

#if UNITY_WEBGL
            AsyncSaveAccordingToFileSettingsWeb(textureBytes, fileSettings, System.IO.Path.GetFileName(filePath));
#endif
        }

        public static bool SyncSaveAccordingToFileSettings(this Texture2D texture, ScreenshotFileSettings fileSettings, string overrideFilePath = "")
        {
            string filePath = string.IsNullOrEmpty(overrideFilePath) ? fileSettings.FullFilePath() : overrideFilePath;
            fileSettings.ValidateFilePath(filePath);

            string extension = System.IO.Path.GetExtension(filePath);
            byte[] textureBytes = texture.ToBytes(extension, fileSettings.encodingParameters);

            bool savedSuccessfully = false;
            if (fileSettings.persistLocally)
                savedSuccessfully = SaveToFilePath(textureBytes, filePath);

#if UNITY_IOS || UNITY_ANDROID
            if (fileSettings.saveToGallery)
            {
                if (string.IsNullOrEmpty(fileSettings.album))
                    Debug.LogError("Album name must be set to save to gallery. Set on the Edit & Save Tab > Save Settings > Mobile Save Settings");

                string fileName = System.IO.Path.GetFileName(filePath);
                if (fileSettings.persistLocally)
                    NativeGallery.SaveImageToGallery(filePath, fileSettings.album, fileName, (success, path) => { if (!success) { Debug.LogError("Save to NativeGallery.SaveImageToGallery Failed"); } });
                else
                    NativeGallery.SaveImageToGallery(textureBytes, fileSettings.album, fileName, (success, path) => { if (!success) { Debug.LogError("Save to NativeGallery.SaveImageToGallery Failed"); } });
            }
#elif UNITY_WEBGL
            SyncSaveAccordingToFileSettingsWeb(textureBytes, fileSettings, System.IO.Path.GetFileName(filePath));
#endif
            return savedSuccessfully;
        }

#if UNITY_WEBGL
        public static void SyncSaveAccordingToFileSettingsWeb(byte[] bytes, CaptureFileSettings fileSettings, string overrideFileName = "")
        {
            string fileName = string.IsNullOrEmpty(overrideFileName) ? fileSettings.fullWebFileName : overrideFileName;
            CoroutineBehaviour.StaticWaitForCoroutine(SaveToWebCoroutine(bytes, fileName, fileSettings.encoding, fileSettings.openInNewTab, fileSettings.download));
        }

        public static void AsyncSaveAccordingToFileSettingsWeb(byte[] bytes, CaptureFileSettings fileSettings, string overrideFileName = "")
        {
            string fileName = string.IsNullOrEmpty(overrideFileName) ? fileSettings.fullWebFileName : overrideFileName;
            CoroutineBehaviour.StaticStartCoroutine(SaveToWebCoroutine(bytes, fileName, fileSettings.encoding, fileSettings.openInNewTab, fileSettings.download));
        }

        public static void SaveToWeb(byte[] bytes, string fileName, string encoding, bool openInNewTab, bool download)
        {
            CoroutineBehaviour.StaticWaitForCoroutine(SaveToWebCoroutine(bytes, fileName, encoding, openInNewTab, download));
        }

        public static System.Collections.IEnumerator SaveToWebCoroutine(byte[] bytes, string fileName, string encoding, bool openInNewTab, bool download)
        {
            if (!openInNewTab && !download) yield break;

            string encodedText = System.Convert.ToBase64String(bytes);
#if UNITY_EDITOR
            Debug.Log("SaveToWeb for: " + fileName + " called successfully in the Editor");
#else
            processImage(encodedText, fileName, encoding, openInNewTab, download);
#endif
        }

        [System.Obsolete("Process is deprecated to use a more descriptive name. Please use SaveAccordingToFileSettingsWeb")]
        public static void Process(byte[] bytes, CaptureFileSettings fileSettings, string overrideFileName = "")
        {
            SyncSaveAccordingToFileSettingsWeb(bytes, fileSettings, overrideFileName);
        }
#endif

#if !UNITY_EDITOR && UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void processImage(string url, string fileName, string type, bool display, bool download);
#endif

#endregion

#region Internal
        static void AsyncSaveToFilePath(byte[] textureBytes, string filePath, System.Action<string, bool> completionBlock = null, System.Threading.ThreadPriority threadPriority = System.Threading.ThreadPriority.BelowNormal)
        {
            ByteArraySaveWorker worker = new ByteArraySaveWorker(threadPriority)
            {
                bytes = textureBytes,
                filePath = filePath,
                OnFileSaved = (int workerId, string savedFilePath, bool savedSuccessfully) => {
                    if(completionBlock != null)
                        completionBlock(savedFilePath, savedSuccessfully);
                },
            };

#if UNITY_EDITOR || !UNITY_WEBGL
            worker.Start();
#else
            CoroutineBehaviour.StaticStartCoroutine(worker.SaveBytes());
#endif
        }

        static bool SaveToFilePath(byte[] textureBytes, string filePath)
        {
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.IO.File.WriteAllBytes(filePath, textureBytes);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception attempting to save texture: " + e);
                return false;
            }

            return true;
        }
        #endregion

        #region Blend Implementation
        public static Texture2D Blend32(this Texture2D background, Texture2D foreground, Vector2Int position, bool overlapOnly, Color32 emptySpaceFillColor, bool alphaBlend = true, bool apply = true, bool destroyOriginals = true)
        {
            Color32[] backgroundPixels = background.GetPixels32();
            Color32[] foregroundPixels = foreground.GetPixels32();

            Vector2Int resultSize = FinalSizeForBlend(new Vector2Int(background.width, background.height), new Vector2Int(foreground.width, foreground.height), position, overlapOnly);
            int resultWidth = resultSize.x;
            int resultHeight = resultSize.y;

            bool negativePositionX = position.x < 0;
            bool negativePositionY = position.y < 0;

            int startRow = overlapOnly ? Mathf.Max(position.y, 0) : 0;
            int startCol = overlapOnly ? Mathf.Max(position.x, 0) : 0;
            Color32[] resultPixels = new Color32[resultWidth * resultHeight];
            for (int row = startRow; row < resultHeight + startRow; row++)
            {
                int backgroundPixelRow = -1;
                if (row + position.y >= 0)
                {
                    backgroundPixelRow = negativePositionY ? row + position.y : row;
                    if (backgroundPixelRow >= background.height) backgroundPixelRow = -1;
                }

                int foregroundPixelRow = -1;
                if (position.y <= row)
                {
                    foregroundPixelRow = negativePositionY ? row : row - position.y;
                    if (foregroundPixelRow >= foreground.height) foregroundPixelRow = -1;
                }

                bool validBackgroundPixelRow = backgroundPixelRow >= 0;
                bool validForegroundPixelRow = foregroundPixelRow >= 0;
                for (int col = startCol; col < resultWidth + startCol; col++)
                {
                    int backgroundPixelIndex = -1;
                    if (validBackgroundPixelRow && col + position.x >= 0)
                    {
                        int backgroundPixelCol = negativePositionX ? col + position.x : col;
                        if (backgroundPixelCol >= background.width) backgroundPixelCol = -1;
                        if (backgroundPixelCol >= 0) backgroundPixelIndex = backgroundPixelRow * background.width + backgroundPixelCol;
                    }

                    int foregroundPixelIndex = -1;
                    if (validForegroundPixelRow && position.x <= col)
                    {
                        int foregroundPixelCol = negativePositionX ? col : col - position.x;
                        if (foregroundPixelCol >= foreground.width) foregroundPixelCol = -1;
                        if (foregroundPixelCol >= 0) foregroundPixelIndex = foregroundPixelRow * foreground.width + foregroundPixelCol;
                    }

                    int resultPixelIndex = (row - startRow) * resultWidth + (col - startCol);
                    if (backgroundPixelIndex >= 0 && foregroundPixelIndex >= 0)
                    {
                        Color32 backgroundColor = backgroundPixels[backgroundPixelIndex];
                        Color32 foregroundColor = foregroundPixels[foregroundPixelIndex];
                        if (alphaBlend)
                            resultPixels[resultPixelIndex] = backgroundColor.AlphaBlend(foregroundColor);
                        else
                            resultPixels[resultPixelIndex] = backgroundColor.SLBlend(foregroundColor);
                    }
                    else if (foregroundPixelIndex >= 0)
                        resultPixels[resultPixelIndex] = foregroundPixels[foregroundPixelIndex];
                    else if (backgroundPixelIndex >= 0)
                        resultPixels[resultPixelIndex] = backgroundPixels[backgroundPixelIndex];
                    else
                        resultPixels[resultPixelIndex] = emptySpaceFillColor;

                    if (resultPixels[resultPixelIndex].a <= 0)
                        resultPixels[resultPixelIndex] = emptySpaceFillColor;
                }
            }

            Texture2D resultTexture = new Texture2D(resultWidth, resultHeight, TextureFormat.ARGB32, background.mipmapCount > 1 && foreground.mipmapCount > 1);
            resultTexture.SetPixels32(resultPixels);
            if (apply)
                resultTexture.Apply(false);

            if (destroyOriginals)
            {
                background.DestroyIfPossible();
                foreground.DestroyIfPossible();
            }
            return resultTexture;
        }

        // Inspired by this method: https://answers.unity.com/questions/1008802/merge-multiple-png-images-one-on-top-of-the-other.html
        // Modified to allow different sized textures.
        // Position is from top left. Negative offsets are supported.
        public static Texture2D BlendFull(this Texture2D background, Texture2D foreground, Vector2Int position, bool overlapOnly, Color emptySpaceFillColor, bool alphaBlend = true, bool apply = true, bool destroyOriginals = true)
        {
            if (emptySpaceFillColor == null) emptySpaceFillColor = Color.clear;

            Color[] backgroundPixels = background.GetPixels();
            Color[] foregroundPixels = foreground.GetPixels();

            Vector2Int resultSize = FinalSizeForBlend(new Vector2Int(background.width, background.height), new Vector2Int(foreground.width, foreground.height), position, overlapOnly);
            int resultWidth = resultSize.x;
            int resultHeight = resultSize.y;

            bool negativePositionX = position.x < 0;
            bool negativePositionY = position.y < 0;

            int startRow = overlapOnly ? Mathf.Max(position.y, 0) : 0;
            int startCol = overlapOnly ? Mathf.Max(position.x, 0) : 0;
            Color[] resultPixels = new Color[resultWidth * resultHeight];
            for (int row = startRow; row < resultHeight + startRow; row++)
            {
                int backgroundPixelRow = -1;
                if (row + position.y >= 0)
                {
                    backgroundPixelRow = negativePositionY ? row + position.y : row;
                    if (backgroundPixelRow >= background.height) backgroundPixelRow = -1;
                }

                int foregroundPixelRow = -1;
                if (position.y <= row)
                {
                    foregroundPixelRow = negativePositionY ? row : row - position.y;
                    if (foregroundPixelRow >= foreground.height) foregroundPixelRow = -1;
                }

                bool validBackgroundPixelRow = backgroundPixelRow >= 0;
                bool validForegroundPixelRow = foregroundPixelRow >= 0;
                for (int col = startCol; col < resultWidth + startCol; col++)
                {
                    int backgroundPixelIndex = -1;
                    if (validBackgroundPixelRow && col + position.x >= 0)
                    {
                        int backgroundPixelCol = negativePositionX ? col + position.x : col;
                        if (backgroundPixelCol >= background.width) backgroundPixelCol = -1;
                        if (backgroundPixelCol >= 0) backgroundPixelIndex = backgroundPixelRow * background.width + backgroundPixelCol;
                    }

                    int foregroundPixelIndex = -1;
                    if (validForegroundPixelRow && position.x <= col)
                    {
                        int foregroundPixelCol = negativePositionX ? col : col - position.x;
                        if (foregroundPixelCol >= foreground.width) foregroundPixelCol = -1;
                        if (foregroundPixelCol >= 0) foregroundPixelIndex = foregroundPixelRow * foreground.width + foregroundPixelCol;
                    }

                    int resultPixelIndex = (row - startRow) * resultWidth + (col - startCol);
                    if (backgroundPixelIndex >= 0 && foregroundPixelIndex >= 0)
                    {
                        Color backgroundColor = backgroundPixels[backgroundPixelIndex];
                        Color foregroundColor = foregroundPixels[foregroundPixelIndex];
                        if (alphaBlend)
                            resultPixels[resultPixelIndex] = backgroundColor.AlphaBlend(foregroundColor);
                        else
                            resultPixels[resultPixelIndex] = backgroundColor.SLBlend(foregroundColor);
                    }
                    else if (foregroundPixelIndex >= 0)
                        resultPixels[resultPixelIndex] = foregroundPixels[foregroundPixelIndex];
                    else if (backgroundPixelIndex >= 0)
                        resultPixels[resultPixelIndex] = backgroundPixels[backgroundPixelIndex];
                    else
                        resultPixels[resultPixelIndex] = emptySpaceFillColor;

                    if (resultPixels[resultPixelIndex].a <= 0)
                        resultPixels[resultPixelIndex] = emptySpaceFillColor;
                }
            }

            Texture2D resultTexture = new Texture2D(resultWidth, resultHeight, TextureFormat.ARGB32, background.mipmapCount > 1 && foreground.mipmapCount > 1);
            resultTexture.SetPixels(resultPixels);

            if (apply)
                resultTexture.Apply(false);

            if (destroyOriginals)
            {
                background.DestroyIfPossible();
                foreground.DestroyIfPossible();
            }
            return resultTexture;
        }
#endregion

#region Utilities
        public static Texture2D EditableCopy(this Texture2D texture, bool apply = false)
        {
            return texture.EditableTexture(true, apply);
        }

        public static Texture2D EditableTexture(this Texture2D texture, bool forceCopy = false, bool apply = false)
        {
            if (!forceCopy && texture.format == texture.EditableTextureFormat())
                return texture;

            if (!forceCopy)
                Debug.LogWarning("Texture is not editable, so making an editable copy.");

            //to do: Try replacing with bytes
            //Unity.Collections.NativeArray<Color32> resultPixels = texture.GetRawTextureData<Color32>();
            //Texture2D resultTexture = new Texture2D(texture.width, texture.height, texture.EditableTextureFormat(), texture.mipmapCount > 1);
            //resultTexture.LoadRawTextureData<Color32>(resultPixels);
            //if (apply)
            //    resultTexture.Apply(false);

            Texture2D resultTexture = new Texture2D(texture.width, texture.height, texture.EditableTextureFormat(), texture.mipmapCount > 1);
            resultTexture.SetPixels(texture.GetPixels());
            if(apply)
                resultTexture.Apply(false);

            return resultTexture;
        }

        public static Texture2D ConvertToTextureFormat(this Texture2D texture, TextureFormat textureFormat)
        {
            //Unity.Collections.NativeArray<byte> resultPixels = texture.GetRawTextureData<byte>();
            //Texture2D resultTexture = new Texture2D(texture.width, texture.height, textureFormat, texture.mipmapCount > 1);
            //resultTexture.LoadRawTextureData<byte>(resultPixels);
            //resultTexture.Apply(false);
            //return resultTexture;


            Color[] resultPixels = texture.GetPixels();
            Texture2D resultTexture = new Texture2D(texture.width, texture.height, textureFormat, texture.mipmapCount > 1);
            resultTexture.SetPixels(resultPixels);
            resultTexture.Apply(false);
            DestroyIfPossible(texture);
            return resultTexture;
        }

        public static byte[] ToBytes(this Texture2D texture, string extension, TextureEncodingParameters texture2DEncodingDetails = null, bool hasFailed = false)
        {
            byte[] textureBytes = null;
            if (texture2DEncodingDetails == null) texture2DEncodingDetails = new TextureEncodingParameters();
            ScreenshotFileSettings.FileType fileType = ScreenshotFileSettings.FileTypeForExtension(extension);
            try
            {
                if (fileType == ScreenshotFileSettings.FileType.PNG)
                    textureBytes = texture.EncodeToPNG();
                else if (fileType == ScreenshotFileSettings.FileType.JPG)
                    textureBytes = texture.EncodeToJPG(texture2DEncodingDetails.jpgQuality);
                else if (fileType == ScreenshotFileSettings.FileType.EXR)
                    textureBytes = texture.EncodeToEXR(texture2DEncodingDetails.exrFlags);
                else if (fileType == ScreenshotFileSettings.FileType.TGA)
                    textureBytes = texture.EncodeToTGA();
                //else if (fileType == ScreenshotFileSettings.FileType.RAW)
                //    textureBytes = texture.GetRawTextureData();
                else
                    Debug.LogError("Invalid file extension: " + extension);
            }
            catch (UnityException e)
            {
                if (hasFailed)
                    return null;
                Debug.LogError("UnityException: " + e);
                return texture.EditableTexture(false, true).ToBytes(extension, texture2DEncodingDetails, true);
            }
            catch (System.ArgumentException e)
            {
                if (hasFailed)
                    return null;
                Debug.LogError("System.ArgumentException: " + e);
                return texture.EditableTexture(false, true).ToBytes(extension, texture2DEncodingDetails, true);
            }
            return textureBytes;
        }

        public static bool IsEqual(this Texture2D a, Texture2D b)
        {
            Unity.Collections.NativeArray<Color32> aPixels = a.GetRawTextureData<Color32>();
            Unity.Collections.NativeArray<Color32> bPixels = b.GetRawTextureData<Color32>();
            if (aPixels.Length != bPixels.Length)
                return false;
            for (int i = 0; i < aPixels.Length; i++)
            {
                if (aPixels[i].r != bPixels[i].r || aPixels[i].g != bPixels[i].g || aPixels[i].b != bPixels[i].b || aPixels[i].a != bPixels[i].a)
                {
                    Debug.Log("I: " + i + "\nA: " + aPixels[i] + "\nB: " + bPixels[i]);
                    return false;
                }
            }

            return true;
        }

        public static void DestroyIfPossible(this Texture2D texture)
        {
            bool editable = texture.format == texture.EditableTextureFormat();
            if (!editable) return;

            MonoBehaviourExtended.FlexibleDestroy(texture);
        }

        public static bool TextureFormatHasAlpha(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.RGBA4444:
                case TextureFormat.BGRA32:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RGBAFloat:
                case TextureFormat.RGBA64:
                case TextureFormat.DXT5:
#if !UNITY_IOS && !UNITY_TVOS && !UNITY_STANDALONE_OSX
                case TextureFormat.DXT5Crunched:
                case TextureFormat.ETC2_RGBA8Crunched:
#endif
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.ASTC_4x4:
                case TextureFormat.ASTC_5x5:
                case TextureFormat.ASTC_6x6:
                case TextureFormat.ASTC_8x8:
                case TextureFormat.ASTC_10x10:
                case TextureFormat.ASTC_12x12:
                case TextureFormat.ASTC_HDR_4x4:
                case TextureFormat.ASTC_HDR_5x5:
                case TextureFormat.ASTC_HDR_6x6:
                case TextureFormat.ASTC_HDR_8x8:
                case TextureFormat.ASTC_HDR_10x10:
                case TextureFormat.ASTC_HDR_12x12:
                    return true;
            }

            return false;
        }

        public static bool TextureFormatHas8BitPrecision(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.RGB24:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                case TextureFormat.RG16:
                case TextureFormat.R8: 
                    return true;
            }

            return false;
        }

        public static TextureFormat EditableTextureFormat(this Texture2D texture)
        {
            return UncompressedTextureFormat(texture);
        }

        public static TextureFormat UncompressedTextureFormat(this Texture2D texture)
        {
            // These are at most the edible texture formats. A few may not be.
            switch (texture.format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGB24:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.RGB565:
                case TextureFormat.R16:
                case TextureFormat.RGBA4444:
                case TextureFormat.BGRA32:
                case TextureFormat.RHalf:
                case TextureFormat.RGHalf:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RFloat:
                case TextureFormat.RGFloat:
                case TextureFormat.RGBAFloat:
                case TextureFormat.YUY2:
                case TextureFormat.RGB9e5Float:
                case TextureFormat.RG16:
                case TextureFormat.R8:
                case TextureFormat.RG32:
                case TextureFormat.RGB48:
                case TextureFormat.RGBA64:
                    return texture.format;
            }

            return TextureFormat.ARGB32;
        }

        public static TextureFormat TextureFormatForRenderTextureFormat(RenderTextureFormat renderTextureFormat)
        {
            switch (renderTextureFormat)
            {
                case RenderTextureFormat.ARGBFloat:
                    return  TextureFormat.RGBAFloat;
                case RenderTextureFormat.ARGBHalf:
                    return TextureFormat.RGBAHalf;
                case RenderTextureFormat.ARGBInt:
                    return TextureFormat.RGBA64;
                case RenderTextureFormat.BGRA32:
                case RenderTextureFormat.ARGB32:
                    return TextureFormat.ARGB32;
                default:
                    Debug.LogError("No TextureFormat Found for RenderTextureFormat: " + renderTextureFormat);
                    return TextureFormat.ARGB32;
            }
        }

       public static int BitsPerPixelForFormat(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.RGBA4444:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGB565:
                    return 16;
                case TextureFormat.RGB24:
                    return 24;
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                    return 32;
                case TextureFormat.RGB48:
                    return 48;
                case TextureFormat.RGBA64:
                    return 64;
                case TextureFormat.Alpha8:
                    return 8;
                case TextureFormat.R8:
                    return 8;
                case TextureFormat.R16:
                    return 16;
                case TextureFormat.RG16:
                    return 16;
                case TextureFormat.RG32:
                    return 32;
                case TextureFormat.RHalf:
                    return 16;
                case TextureFormat.RGHalf:
                    return 32;
                case TextureFormat.RGBAHalf:
                    return 64;
                case TextureFormat.RFloat:
                    return 32;
                case TextureFormat.RGFloat:
                    return 64;
                case TextureFormat.RGBAFloat:
                    return 128;
                case TextureFormat.RGB9e5Float:
                    return 32;
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                    return 2;
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                    return 4;
                case TextureFormat.EAC_R:
                case TextureFormat.EAC_R_SIGNED:
                    return 4;
                case TextureFormat.EAC_RG:
                case TextureFormat.EAC_RG_SIGNED:
                    return 8;
                case TextureFormat.ETC_RGB4:
                case TextureFormat.ETC2_RGB:
                case TextureFormat.ETC2_RGBA1:
                    return 4;
                case TextureFormat.ETC2_RGBA8:
                    return 8;
                default:
                    return -1;
            }
        }

        public static long SizeInBytes(this Texture2D texture)
        {
#if UNITY_EDITOR
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture);
#else
            long bytesPerPixel = BitsPerPixelForFormat(texture.format) / 8;
            long roundedBytesPerPixel = Mathf.CeilToInt(BitsPerPixelForFormat(texture.format) / 16.0f) * 2;

            long calculatedTextureSize = texture.width * texture.height * roundedBytesPerPixel;
            if (texture.isReadable)
                calculatedTextureSize += bytesPerPixel * texture.width * texture.height;
            if (texture.mipmapCount > 1)
                calculatedTextureSize = (long)((float)calculatedTextureSize * 1.333333333f);

            // Add padding as there's some extra bytes 
            calculatedTextureSize += 512;

#if UNITY_EDITOR
            // Debugging code used to test this method.
            long profilerSize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture);
            if (calculatedTextureSize < profilerSize)
            {
                long dataCalculatedTextureSize = texture.GetRawTextureData<byte>().Length;
                if (texture.isReadable)
                {
                    long bytesFromReadableCopy = bytesPerPixel * texture.width * texture.height;
                    if (texture.mipmapCount > 1)
                        bytesFromReadableCopy = (long)((float)bytesFromReadableCopy * 1.333333333f);
                    dataCalculatedTextureSize += bytesFromReadableCopy;

                    // Add padding as there's some extra bytes
                    dataCalculatedTextureSize += 512;
                }

                string debugOutput = "Texture2D with size: " + texture.width + "x" + texture.height + " format: " + texture.format + " mipmap count: " + texture.mipmapCount + " has profiler size: " + profilerSize + " (" + (profilerSize / (texture.width * texture.height)) + " bpp) vs. calculated size: " + calculatedTextureSize + " (" + (calculatedTextureSize / (texture.width * texture.height)) + " bpp) vs. data calculated size: " + dataCalculatedTextureSize + " (" + (dataCalculatedTextureSize / (texture.width * texture.height)) + " bpp)";
                Debug.LogError("Texture2D size calculation is underestimated. Please contact with debug output:  " + debugOutput);
            }

            //if (calculatedTextureSize != profilerSize)
            //    Debug.LogError("Render texture size calculation is incorrect.");
#endif

            return calculatedTextureSize;
#endif
        }

        public static long EstimatedSizeInBytes(int width, int height, bool withAlpha, bool readable)
        {
            // 512 is extra padding to match additional data within the Texture2D object.
            long bytesPerPixel = readable ? (withAlpha ? 8 : 7) : (withAlpha ? 4 : 3);
            return width * height * bytesPerPixel + 512;
        }
#endregion

#region Internal Utilities
        static Rect RectWithinFrame(Texture2D frame)
        {
            Color32[] framePixels = frame.GetPixels32();
            int center = frame.width / 2 + (frame.height / 2) * frame.width;
            if (framePixels[center].a > 0)
            {
                Debug.LogError("It's expected that the empty region of the frame be in the center of the image.");
                return Rect.zero;
            }

            int verticallyOffCenter = frame.width / 2 + (int)(frame.height * 0.175f) * frame.width;
            int leftEdgeIndex = framePixels.FindNextSolidPixelIndex(verticallyOffCenter, -1);
            int rightEdgeIndex = framePixels.FindNextSolidPixelIndex(verticallyOffCenter, 1);
            // Find the x-coordinate of the solid pixel and add 1 to find the first transparent location.
            int x = leftEdgeIndex % frame.width + 1;
            int width = (rightEdgeIndex % frame.width) - x;

            int horizontallyOffCenter = (int)(frame.width * 0.175f) + (frame.height / 2) * frame.width;
            int topEdgeRaw = framePixels.FindNextSolidPixelIndex(horizontallyOffCenter, frame.width);
            int bottomEdgeIndex = framePixels.FindNextSolidPixelIndex(horizontallyOffCenter, -frame.width);
            // Find the y-coordinate of the solid pixel and add 1 to find the first transparent location.
            int y = bottomEdgeIndex / frame.width + 1;
            int height = (topEdgeRaw / frame.width) - y;

            return new Rect(x, y, width, height);
        }
#endregion
    }
}