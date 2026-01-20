using UnityEngine;

namespace PuzzleGames
{
    using TMPro;
    using UnityEngine.UI;

    public class RewardInfo : MonoBehaviour
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private Image        _icon;
        [SerializeField] private Image        _bg;
        [SerializeField] private TMP_Text     _txtValue;

        public ResourceType Type => _type;

        public ResourceValue ResourceValue
        {
            get;
            private set;
        }

        public void SetType(ResourceType type) { _type = type; }

        public void SetValue(ResourceValue resourceValue)
        {
            ResourceValue = resourceValue;
            var value = "";

            switch (resourceValue.type)
            {
                case ResourceType.Gold:
                    value = resourceValue.value.ToString();
                    break;
                case ResourceType.Heart:
                    value = resourceValue.value.DisplayTimeWithMinutes();
                    break;
                default:
                    value = $"x{resourceValue.value}";
                    break;
            }

            _txtValue.SetText(value);
        }

        public void SetIcon(Sprite icon) { _icon.sprite = icon; }

        public void DisableBackground() { _bg.enabled = false; }
    }
    
    public class RewardFlyInfo
    {
        public ResourceValue Value;
        public RectTransform Rect;
        
        public RewardFlyInfo(ResourceValue value, RectTransform rect)
        {
            Value = value;
            Rect  = rect;
        }
    }
}