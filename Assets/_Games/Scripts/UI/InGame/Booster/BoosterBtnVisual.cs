namespace PuzzleGames
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class BoosterBtnVisual : MonoBehaviour
    {
        // The UI Button associated with this booster
        [FormerlySerializedAs("BoosterButton")]
        public Button Btn;

        public TextMeshProUGUI Count;
        public Image           IconImage;
        public GameObject      AddObj;
        public GameObject      CountObj;
        public GameObject      LockObj;
        public TextMeshProUGUI LevelUnlockTMP;

        private void OnValidate()                         { Btn        = GetComponentInChildren<Button>(); }
        public  void UpdateBoosterCount(int boosterCount) { Count.text = boosterCount.ToString(); }
        public void SetAvailable(bool isAvailable)
        {
            LockObj.SetActive(!isAvailable);
            AddObj.SetActive(isAvailable);
            IconImage.gameObject.SetActive(isAvailable);
            CountObj.SetActive(isAvailable);
        }
    }
}