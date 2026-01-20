using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Windows;
using ZBase.UnityScreenNavigator.Foundation;

namespace ChuongCustom.Utils
{
    public static class UINavigatorExtensions
    {
        public static TContainer UpdateSetting<TContainer>(this TContainer container,
            WindowContainerConfig layerConfig
            , IWindowContainerManager manager
            , UnityScreenNavigatorSettings settings
        )
            where TContainer : WindowContainerBase
        {
            var root          = container.gameObject;
            var rectTransform = root.GetOrAddComponent<RectTransform>();
            rectTransform.anchorMin     = Vector2.zero;
            rectTransform.anchorMax     = Vector2.one;
            rectTransform.offsetMax     = Vector2.zero;
            rectTransform.offsetMin     = Vector2.zero;
            rectTransform.pivot         = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = Vector3.zero;

            container.Initialize(layerConfig, manager, settings);

            return container;
        }
    }
}