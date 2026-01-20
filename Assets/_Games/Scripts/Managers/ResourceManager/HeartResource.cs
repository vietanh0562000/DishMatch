namespace PuzzleGames
{
    using System;
    using UnityEngine;

    public class HeartResource : AResource
    {
        public override ResourceType Type => ResourceType.Heart;

        [SerializeField] protected VisualResource infiHeart;

        public override Sprite     GetIcon()      => IsInFreeMode ? infiHeart._icon : visual._icon;
        public override AFlyObject GetFlyObject() => IsInFreeMode ? infiHeart._uiPrefab : visual._uiPrefab;

        public override int    GetAmount() => UserResourceController.instance.UserResource.heart;
        public override string GetString() => IsInFreeMode ? FreeModeRemain.ToString() : GetAmount().ToString();

        public override void Add(int amount, string where = "", string itemId = "")
        {
            AudioController.PlaySound(SoundKind.UICashCheck);
            UserResourceController.instance.AddHeart(amount);
        }
        public override void Subtract(int amount, string where = "", string itemId = "") { UserResourceController.instance.MinusHeart(amount); }

        public override bool IsInFreeMode
            => UserResourceController.instance.IsInfiHeart();

        public override TimeSpan FreeModeRemain                => UserResourceController.instance.GetTargetTimeSpanInfiHearth();
        public override void     ActivateFreeMode(int minutes) { UserResourceController.instance.AddInfiHeart(minutes); }
    }
}