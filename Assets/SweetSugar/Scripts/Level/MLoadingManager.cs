using System.IO;
using SweetSugar.Scripts.System;
using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Loading level 
    /// </summary>
    public static class MLoadingManager
    {
        private static MLevelData _mLevelData;
        static string levelPath = "Assets/SweetSugar/Resources/Levels/";

        public static MLevelData LoadForPlay(int currentLevel, MLevelData mLevelData)
        {
            mLevelData = new MLevelData(Application.isPlaying, currentLevel);
            mLevelData = LoadlLevel(currentLevel, mLevelData).DeepCopyForPlay(currentLevel);
            mLevelData.LoadTargetObject();
            mLevelData.InitTargetObjects(true);
            return mLevelData;
        }

        public static MLevelData LoadlLevel(int currentLevel, MLevelData mLevelData)
        {
            mLevelData = ScriptableLevelManager.LoadLevel(currentLevel);
            mLevelData.CheckLayers();
            // LevelData.THIS = levelData;
            mLevelData.LoadTargetObject();
            // levelData.InitTargetObjects();

            return mLevelData;
        }


        public static int GetLastLevelNum()
        {
            return Resources.LoadAll<LevelContainer>("Levels").Length;
        }
    }
}

