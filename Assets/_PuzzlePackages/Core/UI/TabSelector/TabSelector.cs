using UnityEngine;
using UnityEngine.Events;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class TabSelector : MonoBehaviour
    {
        [SerializeField] private TabElement[] _tabElements;

        [SerializeField, Tooltip("Index của tab được chọn mặc định (bắt đầu từ 0.")]
        private int _defaultIndex;

        [SerializeField] private UnityEvent<int, int> _onTabChanged;

        private TabElement _selectedTab;

        private void Awake()
        {
            for (int i = 0; i < _tabElements.Length; i++)
            {
                _tabElements[i].Create(i, this);
                _tabElements[i].SetActive(_defaultIndex == i);

                if (_defaultIndex == i)
                    _selectedTab = _tabElements[i];
            }
        }

        public void SelectTab(int index)
        {
            var oldIndex = _selectedTab.Index;
            if (index == oldIndex) return;
            
            _selectedTab.SetActive(false);
            _selectedTab = _tabElements[index];
            _selectedTab.SetActive(true);
            
            _onTabChanged?.Invoke(oldIndex, index);
        }
    }
}