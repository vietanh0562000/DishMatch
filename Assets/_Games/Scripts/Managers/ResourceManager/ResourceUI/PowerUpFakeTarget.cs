namespace PuzzleGames
{
    using System;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages.Core;
    using UnityEngine;

    public class PowerUpFakeTarget : MonoBehaviour, IResourceUI
    {
        [SerializeField] private EndPointInfo   endPoint;
        [SerializeField] private ParticleSystem fx;

        public EndPointInfo EndPoint               => endPoint;
        public void OnReachUI(bool isLast)
        {
            fx.Stop();
            fx.Play();
            
            AudioController.PlaySound(SoundKind.UIRecivedItem);
        }
        public void         UpdateUI()             { }

        private void OnEnable()
        {
            Push();

            transform.SetParent(endPoint.EndPoint);
            transform.GetComponent<RectTransform>().anchoredPosition  = Vector2.zero;
        }

        public virtual void EnableCanvas() { }

        public void Push()
        {
            ResourceType.Powerup_AddSlot.Manager().PushUI(this);
            ResourceType.Powerup_RainbowHole.Manager().PushUI(this);
            ResourceType.Powerup_Helidrop.Manager().PushUI(this);
        }
        public void Pop()
        {
            ResourceType.Powerup_AddSlot.Manager().PopUI();
            ResourceType.Powerup_RainbowHole.Manager().PopUI();
            ResourceType.Powerup_Helidrop.Manager().PopUI();
        }
    }
}