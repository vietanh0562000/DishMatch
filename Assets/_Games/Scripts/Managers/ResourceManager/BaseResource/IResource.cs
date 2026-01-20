namespace PuzzleGames
{
    using System;
    using UnityEngine;

    public interface IResource
    {
        ResourceType Type { get; }
        Sprite       GetIcon();
        Sprite       GetIconWithAmount(int amount);
        AFlyObject   GetFlyObject();
        int          GetAmount();
        void         Add(int amount, string where = "", string itemId = "");
        void         Subtract(int amount, string where = "", string itemId = "");
        string       GetString();
        bool         IsInFreeMode   { get; }
        TimeSpan     FreeModeRemain { get; }
        void         ActivateFreeMode(int minutes);
        IResourceUI  UI { get; }
        void         PushUI(IResourceUI ui);
        void         PopUI();
        void         ReleaseUI();
    }
}