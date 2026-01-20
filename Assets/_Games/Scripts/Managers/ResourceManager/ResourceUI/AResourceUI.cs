namespace PuzzleGames
{
    using System;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages.Core;
    using UnityEngine;

    public abstract class AResourceUI : MonoBehaviour, IResourceUI
    {
        [SerializeField] private EndPointInfo endPoint;

        [SerializeField] private bool pushOnStart = true;

        public abstract ResourceType Type { get; }

        public          EndPointInfo EndPoint       => endPoint;
        public virtual  void         EnableCanvas() { }
        public abstract void         OnReachUI(bool isLast);
        public abstract void         UpdateUI();

        protected virtual void Start()
        {
            if (pushOnStart)
                Push();

            DOVirtual.DelayedCall(0.05f, UpdateUI);
        }
        public void Push() { Type.Manager().PushUI(this); }
        public void Pop()  { Type.Manager().PopUI(); }
    }
}