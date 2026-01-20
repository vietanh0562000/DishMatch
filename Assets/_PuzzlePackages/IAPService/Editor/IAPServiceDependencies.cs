using BasePuzzle.PuzzlePackages.Core;
using UnityEditor;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    using BasePuzzle.PuzzlePackages.Core;

    public static class IAPServiceDependencies
    {
        [InitializeOnLoadMethod]
        private static void CheckAndImportRequiredPackage()
        {
            PackageDependencyChecker.CheckPackageInstall("com.unity.purchasing");
        }
    }
}
