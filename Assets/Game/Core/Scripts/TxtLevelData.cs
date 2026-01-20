using System;

namespace PuzzleGames
{
    [Serializable]
    public class TxtLevelData
    {
        public LevelDifficulty difficulty = LevelDifficulty.Easy;
        public int time; //Tính bằng giây
        public string mapData;

        public TxtLevelData()
        {
        }

        public TxtLevelData(LevelDifficulty difficulty, int time, string mapData)
        {
            this.difficulty = difficulty;
            this.mapData = mapData;
            this.time = time;
        }
    }
}