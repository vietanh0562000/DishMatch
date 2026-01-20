namespace PuzzleGames
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public abstract class PowerUpResource : AResource
    {
        public          string Detail;
        public abstract int    Price    { get; }
        public abstract int    Amount   { get; }
        public          string LogWhere => $"buy_{Kind.ToString()}";

        public PowerupKind Kind => Type.ToPowerUp();

        public abstract override ResourceType Type { get; }

        public override int    GetAmount() => PowerUpDataController.instance.GetNumPowerup(Kind);
        public override string GetString() => GetAmount().ToString();

        public override void Add(int amount, string where = "", string itemId = "")
        {
            AudioController.PlaySound(SoundKind.UICashCheck);
            PowerUpDataController.instance.AddPowerup(Kind, amount);
        }
        public override void     Subtract(int amount, string where = "", string itemId = "") { PowerUpDataController.instance.UserPowerup(Kind, amount, where); }
        public override bool     IsInFreeMode                                                => false;
        public override TimeSpan FreeModeRemain                                              => TimeSpan.Zero;

        public bool EnoughResourceToBuy
            => UserResourceController.instance.UserResource.gold >= Price;

        public override void ActivateFreeMode(int minutes) { }
        public void ExchangeResource()
        {
            UserResourceController.instance.MinusGold(Price, LogWhere, LogWhere);
            Add(Amount, LogWhere, LogWhere);
        }
    }
}