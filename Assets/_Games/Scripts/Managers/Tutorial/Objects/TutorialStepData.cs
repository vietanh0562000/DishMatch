namespace PuzzleGames
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class TutorialStepData
    {
        public List<ITutorialObject> objects         = new();
        public bool                  blackBackground = true;
        public Action                onCreate;
        public Action                onNextStep;

        public void HighlightTut(TutorialCanvas canvas)
        {
            onCreate?.Invoke();

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i]?.CreateUI(canvas);
            }
        }

        public void AddActionForNextStep(Action nextStep)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is INextStep tutorialObject)
                {
                    tutorialObject.AddAction(nextStep);
                }
            }
            
            onNextStep = nextStep;       
        }

        public void UnHightlight()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Hide();
            }
        }
    }
}