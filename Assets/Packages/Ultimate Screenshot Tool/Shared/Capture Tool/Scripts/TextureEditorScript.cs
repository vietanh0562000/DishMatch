using UnityEngine;

namespace TRS.CaptureTool
{
    public class TextureEditorScript : MonoBehaviour
    {
        public Texture2D texture;
        public bool copyBeforeEdit = true;

        public TextureTransformation[] frameTransformations;

        public ScreenshotFileSettings fileSettings;

        public string lastSaveFilePath;

        #region Editor variables
#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField]
        bool showSaveSettings = true;
#pragma warning restore 0414
#endif
        #endregion

        public Texture2D Texture()
        {
            Texture2D textureToTransform = texture;
            if (copyBeforeEdit)
                textureToTransform = texture.EditableTexture(true);
            return textureToTransform.ApplyTransformations(frameTransformations, true);
        }

        public void Save()
        {
            Texture2D resultTexture = Texture();
            string fullFilePath = fileSettings.FullFilePath("", fileSettings.FileNameWithCaptureDetails("", resultTexture.width + "x" + resultTexture.height));
            SaveAccordingToFileSettings(resultTexture, fileSettings, fullFilePath);
        }

        public void SaveAccordingToFileSettings(Texture2D textureToSave, ScreenshotFileSettings fileSettings, string filePath)
        {
            System.Action<string, bool> completionBlock = (savedFilePath, savedSuccessfully) =>
            {
#if UNITY_EDITOR
                if (savedSuccessfully)
                    Debug.Log("Saved combined texture to: " + savedFilePath);
#endif
                lastSaveFilePath = savedFilePath;
            };

            textureToSave.SaveAccordingToFileSettings(fileSettings, filePath, completionBlock);
            fileSettings.IncrementCount();
            fileSettings.SaveCount();
        }
    }
}