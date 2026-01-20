using System.Collections.Generic;
using DG.Tweening;
using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;
using UnityEngine.Events;

namespace BasePuzzle.PuzzlePackages.Navigator
{
    using BasePuzzle.PuzzlePackages.Core;
    using ChuongCustom;
    using PuzzleGames;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class Navigator : BaseScreen
    {
        [SerializeField] private RectTransform _selectedTabBG;
        [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
        [SerializeField] private int _selectedTabIndex;
        [Space(10)] [SerializeField] private UnityEvent<int, int> _onTabChanged;
        
        [SerializeField, HideInInspector] private float _iconOffsetY = 60;
        [SerializeField, HideInInspector] private Vector2 _iconNormalScale = Vector2.one; 
        [SerializeField, HideInInspector] private Vector2 _iconUpScale = new(1.4f, 1.4f); 
        [SerializeField] private List<NavigatorTab> _tabs = new List<NavigatorTab>();
        [SerializeField] private List<RectTransform> _icons = new List<RectTransform>();
        
        [Space(5)]
        [SerializeField] private HorizontalScrollSnap _horizontalScrollSnap;
        
        private float _normalTabWidth;

        public static Navigator Instance => SingletonHandler.Resolve<Navigator>();
        public UnityEvent<int, int> OnTabChanged => _onTabChanged;
        
        public override void Init() { }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _normalTabWidth = _tabs[0].GetComponent<RectTransform>().sizeDelta.x;
            
            GameEvent<NavigatorTab>.Register(GameEventID.FPN_NAVIGATOR_TAB_CLICKED, OnTabClicked, this);
            SingletonHandler.Register(this);

            var delta = _selectedTabBG.sizeDelta;
            delta.x                  = _normalTabWidth;
            _selectedTabBG.sizeDelta = delta;

            for (int i = 0; i < _icons.Count; i++)
            {
                _icons[i].DOAnchorPosX(_normalTabWidth / 2 + _normalTabWidth * i + _horizontalLayoutGroup.spacing * i, 0);
            }
            
            _selectedTabBG.DOAnchorPosX(_normalTabWidth * 2 + _horizontalLayoutGroup.spacing, 0);
            
            DOVirtual.DelayedCall(0.5f,() =>
            {
                TutorialManager.Instance.CheckTutorials();
                PopupFlowController.Instance.TryShowPopupFlow();
            });
        }

        private void OnDestroy()
        {
            GameEvent<NavigatorTab>.Unregister(GameEventID.FPN_NAVIGATOR_TAB_CLICKED, OnTabClicked, this);
            SingletonHandler.Unregister(this);
        }

        public void ChangeNavigatorIcon(int tabIndex)
        {
            if (_selectedTabIndex == tabIndex) return;
            _onTabChanged?.Invoke(_selectedTabIndex, tabIndex);
            
            int indexHorizontalSnap = tabIndex;
            MoveIcons(_selectedTabIndex, indexHorizontalSnap);
            
            //Set selected background position
            var targetX = _normalTabWidth * (indexHorizontalSnap + 1) + _horizontalLayoutGroup.spacing * indexHorizontalSnap;
            _selectedTabBG.DOAnchorPosX(targetX, 0.36f).SetEase(Ease.OutQuart);

            _selectedTabIndex = indexHorizontalSnap;
        }

        private void OnTabClicked(NavigatorTab tab)
        {
            SelectTab(tab);
        }

        private void SelectTab(NavigatorTab newTab)
        {
            if (_selectedTabIndex == newTab.Index) return;
            _onTabChanged?.Invoke(_selectedTabIndex, newTab.Index);

            /*//Update tab index
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (i <= newTab.Index)
                {
                    _tabs[i].SetIndex(i);
                    continue;
                }

                _tabs[i].SetIndex(i - 1);
            }*/

            MoveIcons(_selectedTabIndex, newTab.Index);
            
            _selectedTabIndex = newTab.Index;
            _horizontalScrollSnap.GoToScreen(newTab.Index);

            //Set selected background position
            var targetX = _normalTabWidth * (newTab.Index + 1) + _horizontalLayoutGroup.spacing * newTab.Index;
            _selectedTabBG.DOAnchorPosX(targetX, 0.36f).SetEase(Ease.OutQuart);
        }

        private void MoveIcons(int oldIndex, int newIndex)
        {
            var direction = newIndex < oldIndex ? 1 : -1;
            var min = Mathf.Min(newIndex, oldIndex);
            var max = Mathf.Max(newIndex, oldIndex);

            for (int i = min; i <= max; i++)
            {
                var distance = (i == min || i == max)
                    ? direction * _normalTabWidth / 2
                    : direction * _normalTabWidth;

                _icons[i].DOComplete(true);
                //_icons[i].DOAnchorPosX(_icons[i].anchoredPosition.x + distance, 0.125f);
            }

            var seq = DOTween.Sequence();
            seq.Join(_icons[oldIndex].DOAnchorPosY(0, 0.125f))
                .Join(_icons[newIndex].DOAnchorPosY(_iconOffsetY, 0.125f))
                .Join(_icons[oldIndex].DOScale(_iconNormalScale, 0.15f).SetEase(Ease.OutBack))
                .Join(_icons[newIndex].DOScale(_iconUpScale, 0.15f).SetEase(Ease.OutQuad))
                .Play();
        }

        public void MoveToTab(int index)
        {
            _tabs[index].Select();
        }

#if UNITY_EDITOR

        public int SelectedTabIndex
        {
            set => _selectedTabIndex = value;
        }
        
        public List<NavigatorTab> Tabs => _tabs;
        public List<RectTransform> Icons => _icons;

        public float SelectedIconOffsetY
        {
            set => _iconOffsetY = value;
        }
        
        public Vector2 IconNormalScale
        {
            set => _iconNormalScale = value;
        }
        
        public Vector2 IconUpScale
        {
            set => _iconUpScale = value;
        }
        
        public RectTransform SelectedBG
        {
            set => _selectedTabBG = value;
        }

#endif
    }
}