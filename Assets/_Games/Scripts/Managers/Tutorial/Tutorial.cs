namespace PuzzleGames
{
    using System.Collections.Generic;
    using FalconPuzzlePackages;
    using Sirenix.OdinInspector;

    public abstract class Tutorial : ICondition
    {
        public TutorialCanvas Canvas => TutorialCanvas.Instance;

        public bool IsEnd;

        private TutorialStepData currentStep;

        private List<TutorialStepData> steps;

        private int stepIndex = 0;

        [Button]
        public void StartTutorial()
        {
            stepIndex = 0;
            IsEnd     = false;
            Canvas.RaycastPanel.SetActive(true);
            steps = GetSteps();
            ShowStep();
        }

        protected abstract List<TutorialStepData> GetSteps();

        void ShowStep()
        {
            currentStep?.UnHightlight();
            currentStep = steps[stepIndex];

            Canvas.BlackBackground.SetActive(currentStep.blackBackground);

            currentStep.HighlightTut(Canvas);

            currentStep.AddActionForNextStep(NextStep);
        }

        protected void NextStep()
        {
            stepIndex++;

            if (stepIndex >= steps.Count)
            {
                currentStep?.UnHightlight();
                Canvas.RaycastPanel.SetActive(false);
                Canvas.BlackBackground.SetActive(false);
                IsEnd = true;
            }
            else
            {
                ShowStep();
            }
        }
        protected abstract bool CheckCondition();
        protected abstract int  TutID { get; }

        public virtual bool CanStart() { return CheckCondition() && !TutorialDataController.instance.IsTutIDExist(TutID); }

        public virtual void CompleteTutorial() { TutorialDataController.instance.AddTutID(TutID); }
    }
}