using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace BasePuzzle.PuzzlePackages.Core
{
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using PuzzleGames;

    public class LoadLevelManager : NMSingleton<LoadLevelManager>
    {
        private const string SERVER_LEVEL_PATH = "/server_levels";


        private Dictionary<string, string> Md52LevelData = new Dictionary<string, string>();

        protected override void Init()
        {
        }

        public void LoadLevelFromServer(int level)
        {
        
        }

        public void LoadLevelsFromServer(int startLevel, int numberLevel)
        {
        }

        public void OnLevelComplete(int level, string levelData, bool win, int time, int num_booster, double inapp)
        {
            if (win)
            {
                LoadLevelFromServer(level + ServerConfig.Instance<ValueRemoteConfig>().number_of_load_levels_after_login);
            }
                
            string md5Level = Md5Utils.GetMd5First5Char(levelData);
            Md52LevelData[md5Level] = levelData;
        }

        public string GetLevelDataByMd5(string md5LevelData)
        {
            return Md52LevelData[md5LevelData];
        }

        public bool IsValidTxtLevel(string levelData)
        {
            try
            {
                TxtLevelData data = JsonConvert.DeserializeObject<TxtLevelData>(levelData);

                if (data == null)
                    throw new Exception("LevelData is null");

                var decompressMapData = JsonCompressing.Decompressing(data.mapData);
                if (string.IsNullOrEmpty(decompressMapData))
                    throw new Exception($"Cannot decompress MapData: {data.mapData}");

                var mapJson = JsonConvert.DeserializeObject<LevelData>(decompressMapData);
                if (mapJson == null)
                    throw new Exception($"Cannot deserialize decompressMapData: {decompressMapData}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        public byte[] GetImageFromTxtLevel(string levelData)
        {
            if (IsValidTxtLevel(levelData))
            {
                return ScreenShotManager.Instance.ScreenShot(levelData);
            }

            return null;
        }

        private string GetMd5Level(int level)
        {
            string level_data = ReadLevelData(level);
            return "" + level + "_" + Md5Utils.GetMd5First5Char(level_data);
        }

        private List<string> GetMd5Levels(int startLevel, int numberLevel)
        {
//            int curLevel = LevelDataController.instance.Level;
            List<string> list_md5_level = new List<string>();
            for (int i = startLevel; i < startLevel + numberLevel; i++)
            {
                string md5_level = GetMd5Level(i);
                if (md5_level.Length > 0)
                {
                    list_md5_level.Add(md5_level);
                }
            }

            return list_md5_level;
        }

        private void SaveLevel(LevelItem item)
        {
            string folderPath = Application.persistentDataPath + SERVER_LEVEL_PATH;
            string fileName = item.level + ".txt";
            string fullPath = Path.Combine(folderPath, fileName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Ghi nội dung vào file
            File.WriteAllText(fullPath, item.levelData);
        }

        public void SaveAllLevel(LevelItem[] items)
        {
            foreach (LevelItem item in items)
            {
                SaveLevel(item);
            }
        }
        
        private string LoadLevelTxt(int level)
        {
            TextAsset levelData = Resources.Load<TextAsset>(level.ToString());
            if (levelData == null)
            {
                //Debug.LogError($"LevelLoader > Không tồn tại {level}");
                return string.Empty;
            }

            return levelData.text;
        }

        public string ReadLevelData(int level)
        {
            string folderPath = Application.persistentDataPath + SERVER_LEVEL_PATH;
            string fileName = level + ".txt";
            string fullPath = Path.Combine(folderPath, fileName);

            if (File.Exists(fullPath))
            {
                var txtLevel = File.ReadAllText(fullPath);
                if (IsValidTxtLevel(txtLevel)) 
                    return txtLevel;
            }
            else
            {
                var txtLevel = LoadLevelTxt(level);
                if (!string.IsNullOrEmpty(txtLevel) && IsValidTxtLevel(txtLevel))
                    return txtLevel;
            }

            return LoadRandomLevel(folderPath);
        }

        private string LoadRandomLevel(string folderPath)
        {             
            var minLvl = ServerConfig.Instance<ValueRemoteConfig>().minLevelForRandomLoad;
            var maxLvl = ServerConfig.Instance<ValueRemoteConfig>().maxLevelForRandomLoad;
            for (int i = 0; i < 10; i++)
            {
                var randLvl = Random.Range(minLvl, maxLvl);
       
                   string fileName = randLvl + ".txt";
                   string fullPath = Path.Combine(folderPath, fileName);
       
                   if (File.Exists(fullPath))
                   {
                       var txtLevel = File.ReadAllText(fullPath);
                       if (IsValidTxtLevel(txtLevel)) 
                           return txtLevel;
                   }
                   else
                   {
                       var txtLevel = LoadLevelTxt(randLvl);
                       if (!string.IsNullOrEmpty(txtLevel) && IsValidTxtLevel(txtLevel))
                           return txtLevel;
                   }         
            }

            return LoadLevelTxt(100);
        }
    }
}