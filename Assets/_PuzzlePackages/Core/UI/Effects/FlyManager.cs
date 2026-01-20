using UnityEngine;

namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages.Core;
    using Sirenix.OdinInspector;

    public class FlyManager : Singleton<FlyManager>
    {
        [SerializeField] private CollectingItemEffect _collectingItemEffect;

        [SerializeField] private CollectItemEffectInfo _baseEffectInfo;
        [SerializeField] private RectTransform         _startPoint;

        public void ShowFly(ResourceValue resourceValue, RectTransform startPos = null) { ShowFly(resourceValue.type, resourceValue.value, startPos); }

        [Button]
        public void ShowFly(ResourceType resourceType, int amount, RectTransform startPos = null)
        {
            var type = resourceType;

            var endpoint = GetEndPoint(type);

            if (endpoint == null)
            {
                Debug.LogError("Missing end point");
                return;
            }

            if (!type.GetFlyObject())
            {
                Debug.LogError($"Missing UI Prefab of Resource : {type}");
                return;
            }

            var ui = type.Manager().UI;
            ui.EnableCanvas();
            var flyObject = type.GetFlyObject();
            flyObject.SetData(amount);
            var effectInfo = _baseEffectInfo.Clone(flyObject, b => { ui.OnReachUI(b); }, endpoint);

            effectInfo.SetStartPoint(startPos ? startPos : _startPoint);

            _ = _collectingItemEffect.StartEffect(new[] { effectInfo });
        }

        public async Task ShowFly(List<ResourceValue> resources, RectTransform startPos = null)
        {
            var effectInfos = new List<CollectItemEffectInfo>();

            int gold = 0;

            for (int i = 0; i < resources.Count; i++)
            {
                var resourceValue = resources[i];
                var type          = resourceValue.type;

                if (type == ResourceType.Gold)
                {
                    gold += resourceValue.value;
                    continue;
                }

                var endpoint = GetEndPoint(type);
                if (endpoint == null)
                {
                    return;
                }

                var ui = type.Manager().UI;
                ui.EnableCanvas();

                var flyObject = type.GetFlyObject();
                flyObject.SetData(resourceValue.value);


                var effectInfo = _baseEffectInfo.Clone(flyObject,
                    b => { ui.OnReachUI(b); },
                    endpoint);

                effectInfo.SetStartPoint(startPos ? startPos : _startPoint);

                effectInfos.Add(effectInfo);
            }

            await _collectingItemEffect.StartEffect(effectInfos.ToArray());

            ShowFly(ResourceType.Gold, gold, startPos);
        }

        public async Task ShowFlyExist(List<RewardFlyInfo> rewardFlyInfos)
        {
            if (rewardFlyInfos == null || rewardFlyInfos.Count == 0)
                return;

            // Separate special and general resources
            RewardFlyInfo goldInfo = null;
            var generalInfos = new List<RewardFlyInfo>();

            foreach (var info in rewardFlyInfos)
            {
                if (GetEndPoint(info.Value.type) == null) continue;
                switch (info.Value.type)
                {
                    case ResourceType.Gold:
                        goldInfo = info;
                        break;
                    default:
                        generalInfos.Add(info);
                        break;
                }
                info.Rect.SetParent(transform);
            }

            transform.SetAsLastSibling();
            // Handle general resources
            foreach (var info in generalInfos)
            {
                await HandleGeneralResourceFly(info);
            }

            // Handle Gold
            if (goldInfo != null)
            {
                await HandleSpecialResourceFly(goldInfo);
            }
        }

        private async UniTask HandleGeneralResourceFly(RewardFlyInfo info)
        {
            var type = info.Value.type;
            var amount = info.Value.value;

            var endpoint = GetEndPoint(type);
            if (endpoint == null)
            {
                Debug.LogError($"Missing end point for {type}");
                return;
            }

            AFlyObject flyObject = info.Rect.GetComponent<AFlyObject>();
            var ui = type.Manager().UI;
            ui.EnableCanvas();
            flyObject.SetData(amount);
            var effectInfo = _baseEffectInfo.Clone(flyObject, b => { ui.OnReachUI(b); }, endpoint);
            effectInfo.SetStartPoint(flyObject.rectTrans);
            effectInfo.SetEndScale(0.5f);
            _ = _collectingItemEffect.StartExistEffect(new[] { effectInfo });
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), ignoreTimeScale: true);
        }

        private async UniTask HandleSpecialResourceFly(RewardFlyInfo info)
        {
            var rect = info.Rect;
            var value = info.Value;
            var endpoint = GetEndPoint(value.type);
            if (endpoint != null)
            {
                float scaleTime = 0.3f;
                await UniTask.Delay(TimeSpan.FromSeconds(scaleTime), ignoreTimeScale: true);
                float scaleValue = 0.001f;
                ShowFly(value, rect);
                rect.transform.DOScale(scaleValue, scaleTime).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1f, () => Destroy(rect.gameObject));
                }).SetUpdate(true);
            }
        }

        private EndPointInfo GetEndPoint(ResourceType type)
        {
            var ui = type.Manager().UI;

            return ui?.EndPoint;
        }
    }
}