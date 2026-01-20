namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public struct SpriteInRange
    {
        [HorizontalGroup("Row", 150)]
        public int    Min;
        [HorizontalGroup("Row", 150)]
        public int    Max;
        [VerticalGroup("Row/Icon")] [HideLabel] [PreviewField(50)]
        public Sprite Icon;
    }
    
    public class GoldResource : AResource
    {
        [SerializeField] private List<SpriteInRange> _goldIcons;
        public override          ResourceType        Type => ResourceType.Gold;

        public override Sprite GetIconWithAmount(int amount)
        {
            foreach (var goldIcon in _goldIcons)
            {
                if (goldIcon.Min < amount && goldIcon.Max >= amount)
                {
                    return goldIcon.Icon;
                }
            }

            return visual._icon;
        }

        public override int    GetAmount() => UserResourceController.instance.UserResource.gold;
        public override string GetString() => GetAmount().ToString();

        public override void Add(int amount, string where = "", string itemId = "")
        {
            AudioController.PlaySound(SoundKind.UICashCheck);
            UserResourceController.instance.AddGold(amount);
        }
        public override void     Subtract(int amount, string where = "", string itemId = "") { UserResourceController.instance.MinusGold(amount, where, itemId); }
        public override bool     IsInFreeMode                                                   => false;
        public override TimeSpan FreeModeRemain                                                 => TimeSpan.Zero;
        public override void     ActivateFreeMode(int minutes)                                  { }
    }
}