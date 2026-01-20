using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BasePuzzle.PuzzlePackages.Core
{
    using Cysharp.Threading.Tasks;
    using PuzzleGames;
    using Sirenix.OdinInspector;

    [Serializable]
    public class EndPointInfo
    {
        [SerializeField] private RectTransform _endPoint;
        [SerializeField] private RectTransform _scaleTarget;

        public RectTransform EndPoint    => _endPoint;
        public RectTransform ScaleTarget => _scaleTarget;
    }

    [Serializable]
    public class CollectItemEffectInfo
    {
        private AFlyObject    _protype;
        private Action<bool>  _onReachTarget;
        private EndPointInfo  _end;
        private RectTransform _startPoint;
        private int           _numFlyObjects = 1;
        private int           _quantity      = 1;

        [SerializeField] private float _startScale       = 1f;
        [SerializeField] private float _endScale         = 1f;
        [SerializeField] private float _targetScaleExtra = 0.1f;

        public int           Quantity         => _quantity;
        public AFlyObject    Protype          => _protype;
        public int           NoFlyObjects     => Mathf.Min(_numFlyObjects, 13);
        public RectTransform StartPoint       => _startPoint;
        public float         StartScale       => _startScale;
        public RectTransform EndPoint         => _end.EndPoint;
        public float         EndScale         => _endScale;
        public RectTransform ScaleTarget      => _end.ScaleTarget;
        public float         TargetScaleExtra => _targetScaleExtra;
        public Action<bool>  OnReachTarget    => _onReachTarget;

        private int _usedItems = 0;

        public void IncreaseUsedItem()                    { _usedItems++; }
        public void ResetUsedItem()                       { _usedItems  = 0; }
        public void SetStartPoint(RectTransform startPos) { _startPoint = startPos; }
        public void SetEndScale(float endScale) { _endScale = endScale; }
        public bool IsLastItem                  => _usedItems == NoFlyObjects;

        public CollectItemEffectInfo Clone(AFlyObject prototype, Action<bool> onReachTarget, EndPointInfo endPoint)
        {
            return new CollectItemEffectInfo
            {
                _end              = endPoint,
                _protype          = prototype,
                _numFlyObjects    = prototype.NumberToFly,
                _quantity         = prototype.Number,
                _startPoint       = this._startPoint,
                _startScale       = this._startScale,
                _endScale         = this._endScale,
                _targetScaleExtra = this._targetScaleExtra,
                _onReachTarget    = onReachTarget
            };
        }
    }

    public class CollectingItemEffect : MonoBehaviour
    {
        [FoldoutGroup("Custom")] [SerializeField]
        private float _startScaleDuration = 0.08f;

        [FoldoutGroup("Custom")] [SerializeField]
        private float _moveTargetDuration = 0.66f;

        [FoldoutGroup("Custom")] [SerializeField]
        private Ease _moveCurve = Ease.InBack;

        [SerializeField] private float _offsetX, _offsetY;

        [SerializeField, Tooltip("Thời gian giữa mỗi lần spawn item (miliseconds)")]
        private int _intervalTime;

        private void Awake() { _dictListItem.Clear(); }

        private readonly Dictionary<AFlyObject, List<AFlyObject>> _dictListItem = new();

        public async UniTask StartEffect(CollectItemEffectInfo[] infos)
        {
            try
            {
                List<int> listIndex = new List<int>();

                for (int i = 0; i < infos.Length; i++)
                {
                    for (int j = 0; j < infos[i].NoFlyObjects; j++)
                    {
                        listIndex.Add(i);
                    }

                    infos[i].ResetUsedItem();

                    if (!_dictListItem.ContainsKey(infos[i].Protype))
                        _dictListItem.Add(infos[i].Protype, new List<AFlyObject>());
                }

                listIndex.Shuffle();

                for (int i = 0; i < listIndex.Count; i++)
                {
                    if (i < listIndex.Count - 1)
                    {
                        _ = DoEffectByInfo(infos[listIndex[i]]);
                        await UniTask.Delay(_intervalTime, ignoreTimeScale: true);
                    }
                    else
                    {
                        await DoEffectByInfo(infos[listIndex[i]]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async UniTask DoEffectByInfo(CollectItemEffectInfo info)
        {
            info.IncreaseUsedItem();
            var item = GetItem(info);

            item.transform.SetParent(info.StartPoint);
            item.ShowVisual(info.Quantity);
            item.rectTrans.anchoredPosition = Vector2.zero;
            item.gameObject.SetActive(true);
            item.transform.SetParent(transform);
            await DoEffect(info, item.rectTrans, info.IsLastItem);
        }

        private AFlyObject GetItem(CollectItemEffectInfo info)
        {
            var listItems = _dictListItem[info.Protype];

            foreach (var item in listItems)
            {
                if (!item.gameObject.activeInHierarchy) return item;
            }

            return CreateItem(info);
        }

        private AFlyObject CreateItem(CollectItemEffectInfo info)
        {
            var item = Instantiate(info.Protype, transform);
            _dictListItem[info.Protype].Add(item);
            item.gameObject.SetActive(false);
            return item;
        }
        
        public async Task StartExistEffect(CollectItemEffectInfo[] infos)
        {
            try
            {
                List<int> listIndex = new List<int>();

                for (int i = 0; i < infos.Length; i++)
                {
                    for (int j = 0; j < infos[i].NoFlyObjects; j++)
                    {
                        listIndex.Add(i);
                    }

                    infos[i].ResetUsedItem();

                    if (!_dictListItem.ContainsKey(infos[i].Protype))
                        _dictListItem.Add(infos[i].Protype, new List<AFlyObject>());
                }

                listIndex.Shuffle();

                for (int i = 0; i < listIndex.Count; i++)
                {
                    var info = infos[listIndex[i]];
                    if (i < listIndex.Count - 1)
                    {
                        _ = DoExistEffectByInfo(info);
                        await UniTask.Delay(_intervalTime, ignoreTimeScale: true);
                    }
                    else
                    {
                        await DoExistEffectByInfo(info);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async Task DoExistEffectByInfo(CollectItemEffectInfo info)
        {
            info.IncreaseUsedItem();
            var item = info.Protype;
            item.transform.SetParent(info.StartPoint);
            item.ShowVisual(info.Quantity);
            item.transform.SetParent(transform);
            await DoEffect(info, item.rectTrans, info.IsLastItem, true);
            Destroy(item.gameObject);
        }
        
        [SerializeField] private float _scatterDistance = 100f;
        private async Task DoEffect(CollectItemEffectInfo info, RectTransform item, bool isLast, bool hasExist = false)
        {
            // Điểm khởi đầu, control point và đích
            Vector3 p0         = item.position + (isLast ? Vector3.zero : new Vector3(Random.Range(-_offsetX,_offsetX), 0, 0));
            Vector3 p2         = info.EndPoint.position;
            Vector3 dir        = (p2 - p0).normalized;
            Vector3 scatterDir = -dir;
            Vector3 p1 = p0 + scatterDir * _scatterDistance + new Vector3(
                Random.Range(-_offsetX, _offsetX),
                Random.Range(-_offsetY, _offsetY),
                0f
            );

            // Tạo sequence Bezier
            var seq = DOTween.Sequence();
            if(!hasExist)
                seq.Append(item.DOScale(new Vector2(info.StartScale, info.StartScale),
                _startScaleDuration).From(0.1f)).SetUpdate(true);

            // Animating t from 0->1 và tính vị trí trên Bezier
            float t = 0f;
            seq.Join(DOTween.To(
                () => 0f,
                x =>
                {
                    t             = x;
                    item.position = CalculateQuadraticBezierPoint(t, p0, p1, info.EndPoint.position);
                }, 1f,
                _moveTargetDuration
            ).SetEase(_moveCurve).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                info.OnReachTarget?.Invoke(isLast);
            }).SetUpdate(true));

            // Scale đến EndScale nếu có
            if (Math.Abs(info.EndScale - info.StartScale) > 0.01f)
            {
                seq.Join(item.DOScale(
                    new Vector2(info.EndScale, info.EndScale),
                    _moveTargetDuration
                ).SetUpdate(true));
            }

            // Punch scale cho ScaleTarget
            seq.AppendCallback(() =>
            {
                info.ScaleTarget.DOComplete(true);
                info.ScaleTarget.DOPunchScale(
                    new Vector3(info.TargetScaleExtra, info.TargetScaleExtra, info.TargetScaleExtra
                    ), 0.1f, 1, 0f
                ).SetUpdate(true);
            });

            seq.Play().SetUpdate(true);
            await seq.AsyncWaitForCompletion();
        }

        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            return u * u * p0 + 2 * u * t * p1 + t * t * p2;
        }
    }
}