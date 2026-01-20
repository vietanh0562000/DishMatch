using System;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

namespace PuzzleGames
{
    public class PopupFx : MonoBehaviour
    {
        public float Delay;
        private CanvasGroup cg;

        private void OnEnable()
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
            if (cg == null)
                cg = transform.AddComponent<CanvasGroup>();

            cg.alpha = 0;
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true).From(new Vector3(0.7f, 0.7f))
                .OnStart(() => { cg.alpha = 1; }).SetDelay(Delay);
        }
    }
}