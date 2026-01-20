namespace PuzzleGames
{
    using UnityEngine;

    // ReSharper disable InconsistentNaming
    public enum PurchaseID
    {
        shop_list_item_gold_1   = 1,
        shop_list_item_gold_2   = 2,
        shop_list_item_gold_3   = 3,
        shop_list_item_gold_4   = 4,
        shop_list_item_gold_5   = 5,
        shop_list_item_gold_6   = 6,
        shop_small_pack         = 7,
        shop_medium_pack        = 8,
        shop_large_pack         = 9,
        in_game_fail_offer_pack = 10,
        no_ads_pack             = 11,
        no_ads_pack_bundle      = 12
    }

    public static class PurchaseIDExtension
    {
        public static string GetPurchasePKG(this PurchaseID purchaseID)
        {
            var pkg = Application.identifier.ToLower();
            return string.Join('.', pkg, purchaseID.ToString());
        }
    }
}