using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace TRS.CaptureTool
{
    public class GroupTextureCombinator : MonoBehaviour
    {
        public string backgroundTexturesFolder;
        public string foregroundTexturesFolder;

        public bool ignoreErrors = false;

        public Vector2 foregroundPosition;
        public TextureTransformation.LayerPositionPoint foregroundPositionPoint;
        public bool positionForegroundRelative = true;

        public bool overlapOnly = true;
        public Color emptySpaceFillColor;
        public bool useAlphaBlend = true;
        public bool solidify = true;

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

        public void Save()
        {
            if (!Directory.Exists(backgroundTexturesFolder))
                Debug.LogError("Background directory does not exist: " + backgroundTexturesFolder);
            if (!Directory.Exists(foregroundTexturesFolder))
                Debug.LogError("Foreground directory does not exist: " + foregroundTexturesFolder);

            Queue<string> relativeDirectories = new Queue<string>();
            relativeDirectories.Enqueue("");
            while (relativeDirectories.Count > 0)
            {
                string relativePath = relativeDirectories.Dequeue();
                string currentBackgroundTexturesFolder = Path.Combine(backgroundTexturesFolder, relativePath);
                string currentForegroundTexturesFolder = Path.Combine(foregroundTexturesFolder, relativePath);
                string currentResultTexturesFolder = Path.Combine(fileSettings.directory, relativePath);
                if (!Directory.Exists(currentResultTexturesFolder)) Directory.CreateDirectory(currentResultTexturesFolder);

                string[] backgroundFiles = ValidatedSortedFilesAtPath(currentBackgroundTexturesFolder, ScreenshotFileSettings.ValidFileExtensions).ToArray();
                string[] foregroundFiles = ValidatedSortedFilesAtPath(currentForegroundTexturesFolder, ScreenshotFileSettings.ValidFileExtensions).ToArray();
                bool shouldContinue = ignoreErrors || ValidateFilesMatch(relativePath, backgroundFiles, foregroundFiles);
                if (!shouldContinue) return;

                for (int i = 0; i < backgroundFiles.Length; ++i)
                {
                    string filename = Path.GetFileName(backgroundFiles[i]);
                    Texture2D backgroundTexture = (new Texture2D(0, 0)).LoadFromFilePath(backgroundFiles[i]);
                    Texture2D foregroundTexture = (new Texture2D(0, 0)).LoadFromFilePath(foregroundFiles[i]);
                    Texture2D resultTexture = CombinedTexture(backgroundTexture, foregroundTexture);
                    string textureFilePath = Path.Combine(currentResultTexturesFolder, filename);
                    SaveAccordingToFileSettings(resultTexture, fileSettings, textureFilePath);
                }

                string[] backgroundDirectories = SortedDirectoriesAtPath(currentBackgroundTexturesFolder).ToArray();
                string[] foregroundDirectories = SortedDirectoriesAtPath(currentForegroundTexturesFolder).ToArray();
                shouldContinue = ignoreErrors || ValidateDirectoriesMatch(relativePath, backgroundDirectories, foregroundDirectories);
                if (!shouldContinue) return;

                for (int i = 0; i < backgroundDirectories.Length; ++i)
                    relativeDirectories.Enqueue(Path.Combine(relativePath, new DirectoryInfo(backgroundDirectories[i]).Name));
            }
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

        Texture2D CombinedTexture(Texture2D backgroundTexture, Texture2D foregroundTexture)
        {
            TextureTransformation layerTransformation = TextureTransformation.LayerInFrontTextureTransformation(foregroundTexture, foregroundPosition, foregroundPositionPoint, positionForegroundRelative, overlapOnly, emptySpaceFillColor, useAlphaBlend);
            Texture2D resultTexture = backgroundTexture.ApplyTransformation(layerTransformation, !solidify, false);
            if (solidify && resultTexture != null)
                resultTexture = resultTexture.Solidify(true);

            return resultTexture;
        }

        bool ValidateFilesMatch(string parentFolder, string[] backgroundFiles, string[] foregroundFiles)
        {
            return ValidateMatch(parentFolder, "file", backgroundFiles, foregroundFiles);
        }

        bool ValidateDirectoriesMatch(string parentFolder, string[] backgroundDirectories, string[] foregroundDirectories)
        {
            return ValidateMatch(parentFolder, "directory", backgroundDirectories, foregroundDirectories);
        }

        bool ValidateMatch(string parentFolder, string type, string[] backgroundPaths, string[] foregroundPaths)
        {
            if (backgroundPaths.Length != foregroundPaths.Length)
            {
                Debug.LogError("Mismatched " + type + " count between background (" + backgroundPaths.Length + " " + type + "s) and foreground (" + foregroundPaths.Length + " " + type + "s) sub folder: " + parentFolder);
                return false;
            }

            for (int i = 0; i < backgroundPaths.Length; ++i)
            {
                string backgroundDirectory = new DirectoryInfo(backgroundPaths[i]).Name;
                string foregroundDirectory = new DirectoryInfo(foregroundPaths[i]).Name;
                if (backgroundDirectory != foregroundDirectory)
                {
                    Debug.LogError("Mismatched " + type + " name. Background " + type + ": " + backgroundPaths[i] + " Foreground " + type + ": " + foregroundPaths[i]);
                    return false;
                }
            }

            return true;
        }

        List<string> ValidatedSortedFilesAtPath(string path, string[] validFileTypes)
        {
            List<string> files = ConvertIEnumerableToList<string>(Directory.EnumerateFiles(path));
            files.Sort();
            files.RemoveAll(filePath =>
            {
                bool isValidFileType = false;
                foreach (string validFileType in validFileTypes)
                    isValidFileType |= filePath.EndsWith(validFileType, System.StringComparison.OrdinalIgnoreCase);
                return !isValidFileType;
            });
            return files;
        }

        List<string> SortedDirectoriesAtPath(string path)
        {
            List<string> directories = ConvertIEnumerableToList<string>(Directory.EnumerateDirectories(path));
            directories.Sort();
            return directories;
        }

        List<T> ConvertIEnumerableToList<T>(IEnumerable<T> enumerable)
        {
            List<T> list = new List<T>();
            foreach(T item in enumerable)
                list.Add(item);
            return list;
        }
    }
}