namespace PuzzleGames
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;

    [Serializable]
    public class PopupInfo
    {
        [SerializeReference] public APopupCondition condition;
    }

    public class PopupFlowController : Singleton<PopupFlowController>
    {
        public List<PopupInfo> popupSequence = new();

        private bool isShowingPopup;

        public bool IsShowingPopup => isShowingPopup;

        public void TryShowPopupFlow()
        {
            if (isShowingPopup || TutorialManager.Instance.IsTutorialActive())
                return;

            ShowNextPopup().Forget();
        }

        private async UniTaskVoid ShowNextPopup()
        {
            isShowingPopup = true;

            foreach (var entry in popupSequence)
            {
                if (entry.condition.CanStart())
                {
                    popupSequence.Remove(entry);
                    await ShowPopupAsync(entry.condition.ScreenType);
                    isShowingPopup = false;
                    return;
                }
            }

            isShowingPopup = false;
        }

        private UniTask ShowPopupAsync(Type popupType)
        {
            var tcs = new UniTaskCompletionSource();

            WindowManager.Instance.OpenWindow(popupType, true, (presenter) =>
            {
                presenter.OnClosed = () =>
                {
                    tcs.TrySetResult();
                    ShowNextPopup().Forget();
                };
            });
            return tcs.Task;
        }
    }
}