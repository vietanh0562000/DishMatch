namespace PuzzleGames
{
    using System;

    public class ConstantValue
    {
        public const int LOST_TIMES_TO_SHOW_SUPPORT_PACK = 10;
    }

    public static class EnumUtils
    {
        public static PowerupKind  ToPowerUp(this ResourceType resource) => (PowerupKind)(resource - 3);
        public static ResourceType ToResourceType(this PowerupKind kind) => (ResourceType)(kind + 3);
                
        public static int GetEnumCount<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Length;
        }
    }
}