namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class ListDataSo<T> where T : class
    {
        [SerializeField] private List<T> listData;

        public T GetData(int index)
        {
            if (index < 0 || index >= listData.Count)
            {
                return listData[0];
            }

            return listData[index];
        }
        
        public int GetCount() { return listData.Count; }
    }
}