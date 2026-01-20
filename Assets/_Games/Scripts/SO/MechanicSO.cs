namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = "MechanicData", menuName = "MechanicData")]
    public class MechanicSO : ScriptableObject
    {
        [TableList] public List<MechanicData> Mechanics;

        public MechanicData GetData(int id)
        {
            if (id < 0 || id >= Mechanics.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Mechanic id is out of range");
            }

            return Mechanics[id];
        }

        public bool IsLevelUnlockMechanic(int level, out int mechanicID)
        {
            mechanicID = -1;

            for (int i = 0; i < Mechanics.Count; i++)
            {
                if (Mechanics[i].LevelUnlock == level)
                {
                    mechanicID = i;
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public struct MechanicData
    {
        [HorizontalGroup("Row", 100)] [HideLabel] [PreviewField(Height = 100)]
        public Sprite Icon;

        [VerticalGroup("Row/Details")] [BoxGroup("Row/Details/Info")]
        public string Name;

        [BoxGroup("Row/Details/Info")] public string Detail;
        [BoxGroup("Row/Details/Info")] public uint   LevelUnlock;
    }
}