namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class AResource : MonoBehaviour, IResource
    {
        public abstract ResourceType Type { get; }

        [SerializeField] protected VisualResource visual;

        public virtual  Sprite     GetIcon()      => visual._icon;
        public virtual  Sprite     GetIconWithAmount(int amount)      => GetIcon();
        public virtual  AFlyObject GetFlyObject() => visual._uiPrefab;
        public abstract int        GetAmount();
        public abstract void       Add(int amount, string where = "", string itemId = "");
        public abstract void       Subtract(int amount, string where = "", string itemId = "");
        public abstract string     GetString();
        public abstract bool       IsInFreeMode   { get; }
        public abstract TimeSpan   FreeModeRemain { get; }
        public abstract void       ActivateFreeMode(int minutes);

        public IResourceUI UI => uiStacks.Count > 0 ? uiStacks.Peek() : null;

        private readonly Stack<IResourceUI> uiStacks = new Stack<IResourceUI>();

        public void PushUI(IResourceUI ui)
        {
            if (uiStacks.Count > 0 && uiStacks.Peek() == ui)
            {
                return;
            }

            uiStacks.Push(ui);
        }
        public void PopUI()
        {
            uiStacks.Pop();
            if (uiStacks.Count <= 0) return;
            var ui = uiStacks.Peek();
            ui.UpdateUI();
        }
        public void ReleaseUI() { uiStacks.Clear(); }
    }
}