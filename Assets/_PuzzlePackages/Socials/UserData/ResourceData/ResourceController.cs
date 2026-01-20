using Sirenix.OdinInspector;

public enum ResourceType
{
    Heart                       = 0,
    Gold                        = 1,
    Star                        = 2,
    Powerup_AddSlot             = 3,
    Powerup_Helidrop                 = 4,
    Powerup_RainbowHole         = 5,
}


[System.Serializable]
public struct ResourceValue
{
    [HorizontalGroup("ResourceRow", Width = 200)] [LabelText("Type")] [HideLabel]
    public ResourceType type;

    [HorizontalGroup("ResourceRow")] [LabelText("Value")] [HideLabel]
    public int value;

    public ResourceType GetTypeEnum() { return type; }

    public ResourceValue(int type, int value)
    {
        this.type  = (ResourceType)type;
        this.value = value;
    }

    public ResourceValue(ResourceType type, int value)
    {
        this.type  = type;
        this.value = value;
    }
}

public enum ChestType
{
    Chest_0,
    Chest_1,
    Chest_2,
    Chest_3,
}