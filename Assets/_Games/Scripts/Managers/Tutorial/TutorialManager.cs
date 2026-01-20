namespace PuzzleGames
{
    using System.Collections;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class TutorialManager : PersistentSingleton<TutorialManager>
    {
        [SerializeField] private TutorialInfo[] baseTutorials;

        private List<TutorialInfo> tutorials = new List<TutorialInfo>();

        private bool isTutorialRunning = false;

        protected override void Awake()
        {
            base.Awake();
            RegisterTutorials();
        }

        private void RegisterTutorials() { tutorials = new List<TutorialInfo>(baseTutorials); }

        [Button]
        public void CheckTutorials()
        {
            if (PopupFlowController.Instance.IsShowingPopup) return;

            if (isTutorialRunning) return;

            foreach (var tut in tutorials)
            {
                if (tut.tutorialPrefab.CanStart())
                {
                    StartCoroutine(RunTutorial(tut));
                    break;
                }
            }
        }

        private IEnumerator RunTutorial(TutorialInfo info)
        {
            isTutorialRunning = true;
            var tutorial = info.tutorialPrefab;
            tutorial.StartTutorial();
            yield return new WaitUntil(() => tutorial.IsEnd);

            tutorial.CompleteTutorial();

            tutorials.Remove(info);
            
            isTutorialRunning = false;
        }

        // Gọi hàm này từ bất cứ đâu để thêm tutorial vào hệ thống
        public void RegisterTutorial(Tutorial prefab)
        {
            tutorials.Add(new TutorialInfo
            {
                tutorialPrefab = prefab,
            });
        }
        public bool IsTutorialActive() { return isTutorialRunning; }
    }

    [System.Serializable]
    public class TutorialInfo
    {
        [SerializeReference] public Tutorial tutorialPrefab;
    }

    public interface ICondition
    {
        bool CanStart();
    }
}