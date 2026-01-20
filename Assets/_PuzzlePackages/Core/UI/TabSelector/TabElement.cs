using UnityEngine;
using UnityEngine.EventSystems;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class TabElement : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject _activeUI, _normalUI;
        [SerializeField] private GameObject _content;

        public int Index { get; private set; }
        private TabSelector _tabSelector;

        public void Create(int index, TabSelector tabSelector)
        {
            Index = index;
            _tabSelector = tabSelector;
        }

        public void SetActive(bool active)
        {
            _content.SetActive(active);
            _activeUI.SetActive(active);
            _normalUI.SetActive(!active);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tabSelector.SelectTab(Index);
        }
    }
}