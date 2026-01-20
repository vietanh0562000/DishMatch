using System.Collections;
using BasePuzzle.Core.Editor.FPlugins;
using BasePuzzle.Core.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;

namespace BasePuzzle.Core.Editor.Services
{
    using BasePuzzle.Core.Editor.FPlugins;
    using BasePuzzle.Core.Editor.Utils;

    public class FalconCoreInstallResponder : PluginInstallResponder, IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            DefineSymbols.Add("FALCON_AB_TESTING");
            DefineSymbols.Add("FALCON_REMOTE_CONFIG");
        }

        public override string GetPackageName()
        {
            return "FalconCore";
        }

        public override IEnumerator OnPluginInstalled(string installLocation)
        {
            DefineSymbols.Add("FALCON_AB_TESTING");
            DefineSymbols.Add("FALCON_REMOTE_CONFIG");
            yield return null;
        }
    }
}