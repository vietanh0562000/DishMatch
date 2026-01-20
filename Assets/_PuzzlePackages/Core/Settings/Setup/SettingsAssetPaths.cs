using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Settings
{
    using BasePuzzle.PuzzlePackages.Core;

    public abstract class SettingsAssetPaths
    {
        public const string SETTINGS_PATH_CONTAINER_PATH =
            "Assets/FalconPuzzlePackages/Core/Settings/Resources/SettingsPathContainer.asset";

        public const string SETTINGS_PATH_CONTAINER_FOLDER = "Assets/FalconPuzzlePackages/Core/Settings/Resources";

        private static PathsContainer _pathsContainer;

        private static PathsContainer Container
        {
            get
            {
                if (_pathsContainer == null)
                {
                    _pathsContainer = Resources.Load<PathsContainer>("SettingsPathContainer");
                }

                return _pathsContainer;
            }
        }

        public static string GetPath(string assetID)
        {
            if (Container == null)
            {
                Debug.LogError(
                    $"Chưa tạo scriptable object với type {typeof(PathsContainer)} tại đường dẫn {SETTINGS_PATH_CONTAINER_PATH}");
                return string.Empty;
            }

            if (Container.Paths.TryGetValue(assetID, out var path)) return path;

            Debug.LogError(
                $"PathContainer tại đường dẫn {SETTINGS_PATH_CONTAINER_PATH} không chứa asset có ID {assetID}");
            return string.Empty;
        }
    }
}