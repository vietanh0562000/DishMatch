using UnityEngine;

namespace PuzzleGames
{
    using System.Collections.Generic;
    using TMPro;

    public class NavigatorTabNameHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text     _selectedTab;
        [SerializeField] private List<string> _name;

        public void OnTabChanged(int oldIndex, int newIndex)
        {
            _selectedTab.SetText(_name[newIndex]);
        }
    }
}
