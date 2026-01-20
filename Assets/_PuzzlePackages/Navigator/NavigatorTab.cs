using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BasePuzzle.PuzzlePackages.Navigator
{
    using BasePuzzle.PuzzlePackages.Core;

    static partial class GameEventID
    {
        public static string M_ON_SHOW_COLLECTING_HEART_EFFECT = "show_heart_effect";
        public static string M_ON_SHOW_COLLECTING_ITEM_EFFECT  = "show_item_effect";
        public static string M_ON_SHOW_COLLECTING_GOLD_EFFECT  = "show_gold_effect";
        public const  string FPN_NAVIGATOR_TAB_CLICKED         = "fpn_navigator_tab_clicked";
    }

    [HideComponentField]
    public class NavigatorTab : MonoCustomInspector, IPointerDownHandler
    {
        [SerializeField] private int _index;
        public                   int Index => _index;

        public void SetIndex(int index)
        {
            _index = index;

#if UNITY_EDITOR
            gameObject.name = "Tab_" + index;
#endif
        }

        public void Select() { GameEvent<NavigatorTab>.Emit(GameEventID.FPN_NAVIGATOR_TAB_CLICKED, this); }

        public void OnPointerDown(PointerEventData eventData) { Select(); }
    }
}