using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceClassify
{
    public static readonly List<ResourceType> ResourcePowerup = new List<ResourceType>()
    {
        ResourceType.Powerup_AddSlot, ResourceType.Powerup_Helidrop, ResourceType.Powerup_RainbowHole,
    };
/*
    public static readonly Dictionary<PowerupKind, ResourceType> DicResourcePowerup = new Dictionary<PowerupKind, ResourceType>(){
        { PowerupKind.BreakItem, ResourceType.Powerup_BreakItem }, { PowerupKind.Replace, ResourceType.Powerup_Replace},
        { PowerupKind.Freeze, ResourceType.Powerup_Freeze}, { PowerupKind.Swap, ResourceType.Powerup_Swap}
    };

    public static readonly List<ResourceType> ResourceBooster = new List<ResourceType>(){
        ResourceType.Booster_BreakItem, ResourceType.Booster_X2_Star, ResourceType.Booster_Time
    };

    public static readonly Dictionary<BoosterKind, ResourceType> DicResourceBooster = new Dictionary<BoosterKind, ResourceType>(){
        { BoosterKind.BreakItem, ResourceType.Booster_BreakItem }, { BoosterKind.IncreaseTime, ResourceType.Booster_Time},
        { BoosterKind.X2_Star, ResourceType.Booster_X2_Star}
    };

    public static ResourceType GetRandomPowerup()
    {
        var powerups = CollectionUtils.GetListEnum<PowerupKind>();
        var powerup = CollectionUtils.GetRandomElementInList(powerups, false);
        if (!PowerUpDataController.instance.IsUnLockPowerup(powerup))
        {
            powerup = PowerupKind.BreakItem;
        }

        return DicResourcePowerup[powerup];
    }

    public static ResourceType GetRandomBooster()
    {
        var boosters = CollectionUtils.GetListEnum<BoosterKind>();
        var booster = CollectionUtils.GetRandomElementInList(boosters, false);
        if (!BoosterDataController.instance.IsUnLockBooster(booster))
        {
            booster = BoosterKind.BreakItem;
        }

        return DicResourceBooster[booster];
    }

    public static readonly Dictionary<ResourceType, CollectionPack> DicCollectionPack = new Dictionary<ResourceType, CollectionPack>()
    {
        {ResourceType.Collection_Pack_0_Common, CollectionPack.Common},
        {ResourceType.Collection_Pack_1_Uncommon, CollectionPack.Uncommon},
        {ResourceType.Collection_Pack_2_Rare, CollectionPack.Rare},
        {ResourceType.Collection_Pack_3_Epic, CollectionPack.Epic},
        {ResourceType.Collection_Pack_4_Legendary, CollectionPack.Legendary},
    };
    */
}