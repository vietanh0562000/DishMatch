namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ShopPack", menuName = "ShopPack")]
    public class ShopPackSO : ScriptableObject
    {
        [TableList] public List<GoldPack>           GoldPacks;
        public             List<StandardPackBundle> StandardPackBundles;

        public GoldPack GetGoldPack(PurchaseID packName)
        {
            foreach (var goldPack in GoldPacks)
            {
                if (goldPack.ID == packName)
                {
                    return goldPack;
                }
            }

            Debug.LogError($"Don't containe pack id {packName} in gold packs");
            return new GoldPack();
        }

        public StandardPackBundle GetStandardPack(PurchaseID packName)
        {
            foreach (var standardPackBundle in StandardPackBundles)
            {
                if (standardPackBundle.ID == packName)
                {
                    return standardPackBundle;
                }
            }

            Debug.LogError($"Don't containe pack id {packName} in bundle packs");
            return new StandardPackBundle();
        }

        public string GetPurchasePKG(PurchaseID purchaseID) { return purchaseID.GetPurchasePKG(); }
    }

    [Serializable]
    public struct GoldPack
    {
        [HideLabel] [VerticalGroup("Row/Details")]
        public PurchaseID ID;

        [HorizontalGroup("Row", 100)]
        [HideLabel] // Ẩn label của ô Icon
        [PreviewField(Height = 100)]
        public Sprite Icon;

        [VerticalGroup("Row/Details")] // Tạo nhóm dọc bên phải để hiển thị các giá trị còn lại.
        [BoxGroup("Row/Details/Info", ShowLabel = false)] // Tạo nhóm không có title để căn chỉnh nội dung bên trong.
        [LabelText("Price($)")]
        // Hiển thị label cho mức giá.
        public float Price;

        [BoxGroup("Row/Details/Info", ShowLabel = false)]
        public int GoldValue;

        [BoxGroup("Row/Details/Info", ShowLabel = false)]
        public int BonusValue;
    }

    [Serializable]
    public struct StandardPackBundle
    {
        [HideLabel] public PurchaseID ID;

        public PurchasePackage     Prefab;
        public string              bundleName;
        public float               Price;
        public bool                RemoveAds;
        public List<ResourceValue> Resources;
        public List<ResourceValue> InfiResources;
    }
}