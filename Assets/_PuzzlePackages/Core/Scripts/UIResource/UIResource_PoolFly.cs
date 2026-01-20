using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIResource_PoolFly : MonoBehaviour
{
    public static Action<ResourceType, int> onFlyItemToTargetComplete;

    public AnimationCurve curve;

    [SerializeField] 
    private Image _itemPrefab;

    [SerializeField] 
    private Image _itemPrefab_Gold;

    [SerializeField] 
    private Image _itemPrefab_Hearth;

    private Stack<Image> _pools = new Stack<Image>();
    private Stack<Image> _pool_Golds = new Stack<Image>();
    private Stack<Image> _pool_Hearths = new Stack<Image>();
    private int num_pre_spawn = 10;

    private WaitForSeconds waitSpawnGold = new WaitForSeconds(0.1f);
    private WaitForSeconds waitSpawn = new WaitForSeconds(0.2f);
    private Vector3[] paths = new Vector3[3];

    private void Awake()
    {
        InitSpawn(_itemPrefab, _pools);
        InitSpawn(_itemPrefab_Gold, _pool_Golds);
        InitSpawn(_itemPrefab_Hearth, _pool_Hearths);

        void InitSpawn(Image itemPrefab, Stack<Image> pool)
        {
            for (int i = 0; i < num_pre_spawn; i++)
            {
                var item = Instantiate(itemPrefab, transform);
                item.gameObject.SetActive(false);
                pool.Push(item);
            }
        }
    }

    private void OnDestroy()
    {
        onFlyItemToTargetComplete = null;
    }

    private Image GetItem(ResourceType kind)
    {
        switch (kind)
        {
            case ResourceType.Gold:
                if (_pool_Golds.Count > 0)
                {
                    return _pool_Golds.Pop();
                }
                break;
            case ResourceType.Heart:
                if (_pool_Hearths.Count > 0)
                {
                    return _pool_Hearths.Pop();
                }
                break;
            default:
                if (_pools.Count > 0)
                {
                    return _pools.Pop();
                }
                break;
        }

        switch (kind)
        {
            case ResourceType.Gold:
                return Instantiate(_itemPrefab_Gold, transform);
            case ResourceType.Heart:
                return Instantiate(_itemPrefab_Hearth, transform);
        }

        return Instantiate(_itemPrefab, transform);
    }

    public void Spawn(ResourceType kindFly, Vector2 position, Vector3 target, int number, int totalResources = 0)
    {
        //Limit
        if (number > 10)
        {
            number = 10;
        }

        SpawnItem(position, target, kindFly, number, totalResources > 0 ? CreateListWithTotal(totalResources, number) : null);
    }

    private void SpawnItem(Vector3 postion, Vector3 target, ResourceType kindFly, int number, List<int> valueItems)
    {
        StartCoroutine(IESpawn());
        IEnumerator IESpawn()
        {
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < number; i++)
            {
                var a = i;
                var item = GetItem(kindFly);
                if (item != null)
                {
                    item.gameObject.SetActive(true);

                    //Thay đổi hình ảnh
                    switch (kindFly)
                    {
                        case ResourceType.Heart:
                        case ResourceType.Gold:
                            //Ko xử lý gì vì có animation riêng
                            break;
                        default:
                            // UISpriteController.Instance.SetImageResource(kindFly, item, UISpriteController.Instance.GetScale(kindFly));
                            break;
                    }

                    //Di chuyển
                    switch (kindFly)
                    {
                        case ResourceType.Gold:
                        case ResourceType.Star:
                            {
                                var rdX = Random.Range(-2.5f, 0f);
                                var rdY = Random.Range(-2f, 0f);

                                if(kindFly == ResourceType.Gold) AudioController.PlaySound(SoundKind.UICashCheck);

                                item.transform.DOScale(1, 0.2f).From(0f);
                                item.transform.position = new Vector3(postion.x + rdX, postion.y + rdY, transform.position.z);
                                item.transform.DOMove(target, 1f).SetEase(Ease.InBack).OnComplete(Despawn);

                                // item.transform.DOScale(1, 0.2f).From(0f);
                                // item.transform.position = postion;

                                // paths[0] = postion;
                                // paths[1] = new Vector3(postion.x + rdX, postion.y + rdY, transform.position.z);
                                // paths[2] = target;

                                // item.transform.DOPath(paths, 1.5f, PathType.CatmullRom).SetEase(Ease.InOutSine).OnComplete(Despawn);
                            }
                            break;
                        default:
                            {
                                var rdX = Random.Range(-3f, 3f);
                                var rdY = Random.Range(-2f, 2f);

                                item.transform.DOScale(1, 0.2f).From(0f);
                                item.transform.position = new Vector3(postion.x + rdX, postion.y + rdY, transform.position.z);

                                item.transform.DOMove(target, 1f).SetEase(Ease.InBack).SetDelay(0.15f).OnComplete(Despawn);
                            }
                            break;
                    }

                    void Despawn()
                    {
                        onFlyItemToTargetComplete?.Invoke(kindFly, valueItems != null ? valueItems[a] : 0);

                        item.gameObject.SetActive(false);
                        switch (kindFly)
                        {
                            case ResourceType.Heart:
                                _pool_Hearths.Push(item);
                                break;
                            case ResourceType.Gold:
                                _pool_Golds.Push(item);
                                break;
                            default:
                                _pools.Push(item);
                                break;
                        }
                    }
                }

                switch (kindFly)
                {
                    case ResourceType.Gold:
                        yield return waitSpawnGold;
                        break;
                    default:
                        yield return waitSpawn;
                        break;
                }
            }
        }
    }

    private List<int> CreateListWithTotal(int total, int n)
    {
        List<int> result = new List<int>();
        if (n <= 0)
        {
            LogUtils.LogError("n must be greater than 0");
            return null;
        }

        // Tính giá trị cơ sở làm tròn xuống cho mỗi phần tử
        int baseValue = total / n;
        int offset = (int)(baseValue / 2);

        // Tính tổng dư để phân phối đều cho các phần tử
        int remainder = total % n;

        // Thêm giá trị cơ sở vào danh sách
        for (int i = 0; i < n; i++)
        {
            // Phân phối dư cho các phần tử đầu tiên
            result.Add(baseValue + (i < remainder ? 1 : 0));
        }

        System.Random random = new System.Random();
        int t = 0;
        for (int i = 0; i < n - 1; i++)
        {
            int randomValue;
            if (t >= offset || t <= -offset)
            {
                randomValue = -t;
                t = 0;
            }
            else
            {
                randomValue = random.Next(-offset, offset);
                t += randomValue;
            }

            result[i] += randomValue;
        }

        result[n - 1] -= t;

        return result;
    }
}
