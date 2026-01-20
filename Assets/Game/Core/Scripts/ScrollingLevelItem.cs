using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGames
{
    public class ScrollingLevelItem : MonoBehaviour
    {
        [SerializeField] private Image      _background;
        [SerializeField] private TMP_Text   _txtLevel;
        [SerializeField] private GameObject _lockIcon;

        public void SetLevel(Sprite bgSprite, int level, Color color)
        {
            //_background.sprite = bgSprite;
            //_background.SetNativeSize();
            _txtLevel.text = level.ToString();
            _txtLevel.color = color;
            _background.gameObject.SetActive(true);
        }

        public void ShowLockIcon(bool value)
        {
            //_lockIcon.SetActive(value);
            //_background.gameObject.SetActive(!value);
        }
    }
}