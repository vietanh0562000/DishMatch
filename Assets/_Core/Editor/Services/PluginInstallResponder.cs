using System;
using System.Collections;

namespace BasePuzzle.Core.Editor.FPlugins
{
    public abstract class PluginInstallResponder
    {
        public abstract String GetPackageName();
        public abstract IEnumerator OnPluginInstalled(String installLocation);
    }
}