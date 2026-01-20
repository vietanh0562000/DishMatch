using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Socials.FreeLives
{
    using BasePuzzle.PuzzlePackages.Core;

    public abstract class FreeLivesAssetPaths
    {
        public const string FREELIVES_PATH_CONTAINER_PATH =
            "Assets/FalconPuzzlePackages/Socials/FreeLives/Resources/FreeLivesPathContainer.asset";

        public const string FREELIVES_PATH_CONTAINER_FOLDER = "Assets/FalconPuzzlePackages/Socials/FreeLives/Resources";

        private static PathsContainer _pathsContainer;

        private static PathsContainer Container
        {
            get
            {
                if (_pathsContainer == null)
                {
                    _pathsContainer = Resources.Load<PathsContainer>("FreeLivesPathContainer");
                }

                return _pathsContainer;
            }
        }

        public static string GetPath(string assetID)
        {
            if (Container == null)
            {
                Debug.LogError(
                    $"Chưa tạo scriptable object với type {typeof(PathsContainer)} tại đường dẫn {FREELIVES_PATH_CONTAINER_PATH}");
                return string.Empty;
            }

            if (Container.Paths.TryGetValue(assetID, out var path)) return path;

            Debug.LogError(
                $"PathContainer tại đường dẫn {FREELIVES_PATH_CONTAINER_PATH} không chứa asset có ID {assetID}");
            return string.Empty;
        }
    }
}