using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Level scriptable file. Resources/Levels/
    /// </summary>
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "LevelContainer", order = 1)]
    public class LevelContainer : ScriptableObject
    {
        [FormerlySerializedAs("levelData")] public MLevelData mLevelData;

        public void SetData(MLevelData mLevelData)
        {
            this.mLevelData = mLevelData;
        }
    }
}