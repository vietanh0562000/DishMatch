using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGames
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _btnRestorePurchase;

        private void Awake()
        {
#if UNITY_IOS
            _btnRestorePurchase.SetActive(true);
#else
            _btnRestorePurchase.SetActive(false);
#endif
        }

        public void ClickBtnPrivacy()
        {
            Application.OpenURL("https://falcongames.com/policy/en/privacy-policy.html");
        }
        
        public void ClickBtnSupport() { Application.OpenURL("https://falcongames.com/policy/en/privacy-policy.html"); }
    }
}