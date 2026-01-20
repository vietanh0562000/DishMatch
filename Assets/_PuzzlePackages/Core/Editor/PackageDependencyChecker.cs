using System.Collections;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class PackageDependencyChecker
    {
        public static void CheckPackageInstall(string packageName)
        {
            EditorCoroutine.StartCoroutine(CheckPackageInstallation(packageName));
        }

        private static IEnumerator CheckPackageInstallation(string packageName)
        {
            var request = Client.List(true);
            yield return new WaitUntil(() => request.IsCompleted);
            
            bool isInstalled = IsPackageInstalled(packageName);
            if (isInstalled) yield break;
            
            if (EditorUtility.DisplayDialogComplex(
                    "Package Requirement",
                    $"This module requires {packageName} package. Do you want to install it?",
                    "Install",
                    "Cancel",
                    "") == 0)
            {
                Client.Add(packageName);
            }
        }

        private static bool IsPackageInstalled(string packageName)
        {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
            {
                //Waiting for request completed;
            }

            if (listRequest.Status != StatusCode.Success) return false;
            
            foreach (var package in listRequest.Result)
            {
                if (package.name == packageName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}