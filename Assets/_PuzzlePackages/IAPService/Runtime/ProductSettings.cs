using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    using System.Collections.Generic;
    using System.Linq;
    using PuzzleGames;
    using Sirenix.OdinInspector;

    [Serializable]
    public struct ProductInfo
    {
        public string      productID;
        public ProductType type;
    }

    public class ProductSettings : ScriptableObject
    {
        public const string SETTINGS_PATH = "Assets/FalconPuzzlePackages/Resources";
        public const string SETTINGS_NAME = "ProductSettings";

        [SerializeField] private ProductInfo[] _products;
        public                   ProductInfo[] Products => _products;

        [Button]
        public void CreateProductSetting() { _products = CreateProductsBasedOnEnum(); }

        public ProductInfo[] CreateProductsBasedOnEnum()
        {
            var allProducts = new List<ProductInfo>(_products); // Copy danh sách các product hiện tại

            foreach (PurchaseID purchaseId in Enum.GetValues(typeof(PurchaseID)))
            {
                if (_products.All(product => product.productID != purchaseId.GetPurchasePKG())) // Nếu productID chưa tồn tại thì thêm mới
                {
                    string productName = purchaseId.GetPurchasePKG();
                    allProducts.Add(new ProductInfo()
                    {
                        productID = productName,
                        type      = ProductType.Consumable
                    });
                }
            }

            return allProducts.ToArray(); // Trả lại danh sách đầy đủ
        }
    }
}