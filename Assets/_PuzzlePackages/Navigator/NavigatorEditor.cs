#if UNITY_EDITOR
using BasePuzzle.PuzzlePackages.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BasePuzzle.PuzzlePackages.Navigator
{
    using BasePuzzle.PuzzlePackages.Core;

    [HideComponentField]
    public class NavigatorEditor : MonoCustomInspector
    {
        [SerializeField] private Sprite _normalTabBG, _selectedTabBG;

        [Space(6), Range(3, 5), SerializeField]
        private int _numberOfTabs = 5;

        [Tooltip("The first tab selected upon opening the scene."), SerializeField, Range(0, 4)]
        private int _defaultTab = 2;

        [Space(6), Tooltip("How far to move up an icon when its tab is selected"), SerializeField]
        private float _tabHeight = 210;

        [Tooltip("How far to move up an icon when its tab is selected"), SerializeField]
        private float _iconOffsetY = 60;

        [Tooltip("The scale of an icon when its tab is not selected"), SerializeField]
        private Vector2 _iconNormalScale = Vector2.one;

        [Tooltip("How big to scale up an icon when its tab selected"), SerializeField]
        private Vector2 _iconUpScale = new Vector2(1.4f, 1.4f);

        private Canvas _canvas;
        private Navigator _navigator;
        private RectTransform _pagesHolder, _tabsHolder, _selectedBGContainer, _iconsHolder;
        private float _normalWidth;

        private readonly Color[] _pageColors = new[]
        {
            new Color(0.68f, 0.59f, 0.72f), new Color(0.93f, 0.86f, 0.65f), new Color(0.58f, 0.73f, 0.46f),
            new Color(0.77f, 0.62f, 0.53f), new Color(0.63f, 0.68f, 0.8f)
        };

        private void OnValidate()
        {
            _defaultTab = Mathf.Clamp(_defaultTab, 0, _numberOfTabs - 1);
        }

        [InspectorButton("Setup", 100, 7, 2)]
        private void Setup()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (!_canvas)
            {
                Debug.LogError("You need to place the Navigator object as a child within a Canvas object.");
                return;
            }
            
            _normalWidth = 1080f / (_numberOfTabs);

            //SetupNavigator();
            //SetupPages();
            //SetupTabs();
            //SetupSelectedTab();
            //SetupIcons();
        }

        private RectTransform GetOrCreateChild(string childName, int siblingIndex)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).GetComponent<RectTransform>();
                if (!child) continue;

                if (child.name == childName)
                    return child;
            }

            var rect = new GameObject().AddComponent<RectTransform>();
            rect.name = childName;
            rect.SetParent(transform);
            rect.SetSiblingIndex(siblingIndex);
            return rect;
        }

        private void SetupNavigator()
        {
            var root = GetComponent<RectTransform>();
            root.name = "Navigator";
            root.anchoredPosition3D = Vector3.zero;
            root.sizeDelta = root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.pivot = new Vector2(0.5f, 0.5f);
            root.localScale = Vector3.one;
            
            _navigator = GetComponent<Navigator>();
            if (!_navigator)
                _navigator = root.AddComponent<Navigator>();
        }

        private void SetupPages()
        {
            var width = _canvas.renderingDisplaySize.x / _canvas.scaleFactor;

            _pagesHolder = GetOrCreateChild("PagesHolder", 0);
            _pagesHolder.anchoredPosition3D = new Vector3(-_defaultTab * width, 0, 0);
            _pagesHolder.sizeDelta = new Vector2(width * _numberOfTabs, 0);
            _pagesHolder.anchorMin = Vector2.zero;
            _pagesHolder.anchorMax = new Vector2(0, 1);
            _pagesHolder.pivot = new Vector2(0, 0.5f);
            _pagesHolder.localScale = Vector3.one;

            var horLayout = _pagesHolder.GetComponent<HorizontalLayoutGroup>();
            if (!horLayout)
                horLayout = _pagesHolder.AddComponent<HorizontalLayoutGroup>();

            horLayout.childControlWidth = horLayout.childControlHeight = true;
            horLayout.childForceExpandWidth = horLayout.childForceExpandHeight = true;
            horLayout.childAlignment = TextAnchor.MiddleLeft;

            for (int i = 0; i < _numberOfTabs; i++)
            {
                if (i < _pagesHolder.childCount) continue;
                CreatePage(i);
            }

            if (_pagesHolder.childCount <= _numberOfTabs) return;

            for (int i = _numberOfTabs; i < _pagesHolder.childCount; i++)
            {
                _pagesHolder.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void CreatePage(int index)
        {
            var page = new GameObject().AddComponent<RectTransform>();
            page.name = $"Page_{index}";
            page.SetParent(_pagesHolder);
            page.anchoredPosition3D = Vector3.zero;
            page.localScale = Vector3.one;
            page.pivot = new Vector2(0.5f, 0.5f);
            page.AddComponent<RectMask2D>();

            var safeArea = new GameObject().AddComponent<RectTransform>();
            safeArea.name = "SafeArea";
            safeArea.SetParent(page);
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.pivot = new Vector2(0.5f, 0.5f);
            safeArea.anchoredPosition3D = Vector3.zero;
            safeArea.sizeDelta = Vector2.zero;
            safeArea.localScale = Vector3.one;

            var image = new GameObject().AddComponent<Image>();
            image.name = "Background";
            image.transform.SetParent(page);
            image.color = index < _pageColors.Length ? _pageColors[index] : Color.white;

            var rect = image.GetComponent<RectTransform>();
            rect.anchoredPosition3D = Vector3.zero;
            rect.sizeDelta = rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
        }

        private void SetupTabs()
        {
            _tabsHolder = GetOrCreateChild("TabsHolder", 1);
            _tabsHolder.anchoredPosition3D = new Vector3(0, _tabHeight / 2, 0);
            _tabsHolder.sizeDelta = new Vector2(0, _tabHeight);
            _tabsHolder.anchorMin = Vector2.zero;
            _tabsHolder.anchorMax = new Vector2(1f, 0);
            _tabsHolder.pivot = new Vector2(0.5f, 0.5f);
            _tabsHolder.localScale = Vector3.one;

            var horLayout = _tabsHolder.GetComponent<HorizontalLayoutGroup>();
            if (!horLayout)
                horLayout = _tabsHolder.AddComponent<HorizontalLayoutGroup>();

            horLayout.childControlWidth = horLayout.childControlHeight = true;
            horLayout.childForceExpandWidth = false;
            horLayout.childForceExpandHeight = true;
            horLayout.childAlignment = TextAnchor.MiddleLeft;

            _navigator.Tabs.Clear();

            for (int i = _tabsHolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_tabsHolder.GetChild(i).gameObject);
            }

            for (int i = 0; i < _numberOfTabs; i++)
            {
                CreateTab(i, i == 0 || (i == _numberOfTabs - 1 && _defaultTab != _numberOfTabs - 1), _normalWidth);

                if (i != _defaultTab) continue;
                CreateTab(i, _defaultTab == _numberOfTabs - 1, _normalWidth);
            }
        }

        private void CreateTab(int index, bool addFlexibleWidth, float normalWidth)
        {
            var tab = new GameObject()
                .AddComponent<NavigatorTab>();

            tab.name = "Tab_" + index;
            tab.SetIndex(index);
            tab.transform.SetParent(_tabsHolder);

            var image = tab.AddComponent<Image>();
            image.type = Image.Type.Sliced;
            image.sprite = _normalTabBG;
            image.color = _normalTabBG ? Color.white : new Color(0f, 0.46f, 0.9f);

            tab.GetComponent<RectTransform>().localScale = Vector3.one;
            tab.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            _navigator.Tabs.Add(tab);

            var layout = tab.AddComponent<LayoutElement>();
            layout.preferredWidth = normalWidth;

            if (!addFlexibleWidth) return;
            layout.flexibleWidth = 1;
        }

        private void SetupSelectedTab()
        {
            _selectedBGContainer = GetOrCreateChild("SelectedBGContainer", 2);
            _selectedBGContainer.anchoredPosition3D = new Vector3(0, _tabHeight / 2, 0);
            _selectedBGContainer.sizeDelta = new Vector2(1080, _tabHeight);
            _selectedBGContainer.localScale = Vector3.one;
            _selectedBGContainer.anchorMin = _selectedBGContainer.anchorMax = new Vector2(0.5f, 0);
            _selectedBGContainer.pivot = new Vector2(0.5f, 0.5f);

            for (int i = _selectedBGContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_selectedBGContainer.GetChild(i).gameObject);
            }

            var rect = new GameObject().AddComponent<RectTransform>();
            rect.name = "SelectedBG";
            rect.SetParent(_selectedBGContainer);
            rect.anchoredPosition3D = new Vector3(_normalWidth * (_defaultTab + 1), _tabHeight / 2, 0);
            rect.sizeDelta = new Vector2(_normalWidth * 2, _tabHeight);
            rect.localScale = Vector3.one;
            rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var image = rect.AddComponent<Image>();
            image.sprite = _selectedTabBG;
            image.color = _selectedTabBG ? Color.white : new Color(0.09f, 0.69f, 1f);
            image.type = Image.Type.Sliced;

            _navigator.SelectedBG = rect;
            _navigator.SelectedTabIndex = _defaultTab;
        }

        private void SetupIcons()
        {
            _iconsHolder = GetOrCreateChild("IconsHolder", 3);
            _iconsHolder.anchoredPosition3D = new Vector3(0, _tabHeight / 2, 0);
            _iconsHolder.sizeDelta = new Vector2(1080, _tabHeight);
            _iconsHolder.anchorMin = _iconsHolder.anchorMax = new Vector2(0.5f, 0);
            _iconsHolder.pivot = new Vector2(0.5f, 0.5f);
            _iconsHolder.localScale = Vector3.one;

            _navigator.SelectedIconOffsetY = _iconOffsetY;
            _navigator.IconNormalScale = _iconNormalScale;
            _navigator.IconUpScale = _iconUpScale;

            for (int i = 0; i < _numberOfTabs; i++)
            {
                if (i < _iconsHolder.childCount) continue;
                
                var icon = new GameObject().AddComponent<RectTransform>();
                icon.SetParent(_iconsHolder);
                icon.sizeDelta = new Vector2(110, 110);
                icon.AddComponent<Image>();
                
            }
            
            _navigator.Icons.Clear();
            var icons = _iconsHolder.GetComponentsInChildren<RectTransform>();

            for (int i = 1; i < icons.Length; i++)
            {
                if (i > _numberOfTabs)
                {
                    icons[i].gameObject.SetActive(false);
                    break;
                }

                icons[i].name = $"Icon_{i - 1}";

                if (i <= _defaultTab)
                {
                    SetupIcon(icons[i], new Vector3(_normalWidth * (i - 0.5f), 0, 0), _iconNormalScale);
                    continue;
                }

                if (i == _defaultTab + 1)
                {
                    SetupIcon(icons[i], new Vector3(_normalWidth * (_defaultTab + 1), _iconOffsetY, 0), _iconUpScale);
                    continue;
                }

                SetupIcon(icons[i], new Vector3(_normalWidth * (i + 0.5f), 0, 0), _iconNormalScale);
            }
            
            if (_iconsHolder.childCount <= _numberOfTabs) return;
            
            for (int i = _numberOfTabs; i < _iconsHolder.childCount; i++)
            {
                _iconsHolder.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void SetupIcon(RectTransform icon, Vector3 pos, Vector2 scale)
        {
            _navigator.Icons.Add(icon);
            icon.anchoredPosition3D = pos;
            icon.localScale = scale;

            icon.anchorMin = icon.anchorMax = new Vector2(0, 0.5f);
            icon.pivot = new Vector2(0.5f, 0.5f);

            //Turn off raycast for all icons so they're not clickable
            var graphics = icon.GetComponentsInChildren<Graphic>();
            foreach (var g in graphics)
            {
                g.raycastTarget = false;
            }
        }

        private void Awake()
        {
            DestroyImmediate(this);
        }
    }
}
#endif