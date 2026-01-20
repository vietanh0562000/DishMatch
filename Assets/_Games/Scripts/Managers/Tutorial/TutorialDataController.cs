namespace FalconPuzzlePackages
{
    using System.Collections.Generic;

    public class TutorialDataController : NMSingleton<TutorialDataController>
    {
        private const string data_key = "tut_data";

        private TutData tutData;

        protected override void Init() { InitData(); }

        private void InitData()
        {
            if (SaveLoadHandler.Exist(data_key))
            {
                tutData = SaveLoadHandler.Load<TutData>(data_key);
            }
            else
            {
                tutData = new TutData()
                {
                    tutIDs = new List<int>()
                };
            }

            Save();
        }

        public void AddTutID(int id)
        {
            if (!tutData.tutIDs.Contains(id))
            {
                tutData.tutIDs.Add(id);
            }

            Save();
        }
        
        public bool IsTutIDExist(int id)
        {
            return tutData.tutIDs.Contains(id);
        }
        
        public void Save() { SaveLoadHandler.Save(data_key, tutData); }
    }

    public class TutData
    {
        public List<int> tutIDs;
    }
}