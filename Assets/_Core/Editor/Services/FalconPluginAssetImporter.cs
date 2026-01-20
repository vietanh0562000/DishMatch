using System;
using System.IO;
using System.Linq;
using BasePuzzle.Core.Editor.FPlugins;
using BasePuzzle.Core.Editor.Utils;
using BasePuzzle.Core.Scripts.Logs;
using UnityEditor;
using UnityEngine;

namespace BasePuzzle.Core.Editor.Services
{
    using BasePuzzle.Core.Editor.FPlugins;
    using BasePuzzle.Core.Editor.Utils;
    using BasePuzzle.Core.Scripts.Logs;

    [InitializeOnLoad]
    public static class FalconPluginAssetImporter
    {
        static FalconPluginAssetImporter()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            var importPackageLocation = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), packageName);
            if (Directory.Exists(Path.Combine(Application.dataPath, "Falcon", packageName)))
                try
                {
                    FileUtil.MoveFileOrDirectory(Application.dataPath + Path.DirectorySeparatorChar + "Falcon" +
                                                 Path.DirectorySeparatorChar + packageName, importPackageLocation);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Warning(e);
                }

            CleanUp();

            var responder = LookUpInstallResponders(packageName);
            if (responder != null) new EditorSequence(responder.OnPluginInstalled(importPackageLocation)).Start();

            CoreLogger.Instance.Info("ImportPackageCompleted(" + packageName + ")");
        }

        private static void OnImportPackageFailed(string packageName, string errormessage)
        {
            CleanUp();
        }


        private static void CleanUp()
        {
            if (Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "Falcon") &&
                IsDirectoryEmpty(Application.dataPath + Path.DirectorySeparatorChar + "Falcon"))
                FileUtil.DeleteFileOrDirectory(Application.dataPath + Path.DirectorySeparatorChar + "Falcon");
        }

        private static bool IsDirectoryEmpty(string directoryPath)
        {
            if (Directory.GetFiles(directoryPath).Length > 0) return false;

            foreach (var path in Directory.GetDirectories(directoryPath))
                if (!IsDirectoryEmpty(path))
                    return false;

            return true;
        }

        private static PluginInstallResponder LookUpInstallResponders(string packageName)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(PluginInstallResponder)) && !t.IsAbstract)
                .Select(t => (PluginInstallResponder)Activator.CreateInstance(t));

            foreach (var responder in types)
                if (responder.GetPackageName().Equals(packageName))
                    return responder;

            return null;
        }
    }
}