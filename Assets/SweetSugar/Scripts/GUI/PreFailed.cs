using System;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Pre failed menu
    /// </summary>
    public class PreFailed : MonoBehaviour
    {
        public Sprite spriteTime;
        public GameObject[] objects;

        /// <summary>
        /// Initialization
        /// </summary>
        public void SetFailed()
        {
            objects[0].SetActive(true);
            if(MLevelManager.THIS.mLevelData.limitType == LIMIT.TIME)
                objects[0].GetComponent<Image>().sprite = spriteTime;
            objects[1].SetActive(false);
        }

        public void SetBombFailed()
        {
            objects[1].SetActive(true);
            objects[0].SetActive(false);
        }
        
        /// <summary>
        /// Continue the game after choose a variant
        /// </summary>
        public void Continue()
        {
            if(IsFail())
            {
                ContinueFailed();
                ContinueBomb();
            }
            else ContinueBomb();
            AnimAction(() => MLevelManager.THIS.gameStatus = GameState.Playing);
        }

        /// <summary>
        /// Further animation and game over
        /// </summary>
        public void Close()
        {  
            var timeBombs = FindObjectsOfType<MItemTimeBomb>().Where(i=>i.timer<=0);
            if(timeBombs.Count() > 0){
            timeBombs.NextRandom().OnExlodeAnimationFinished += () => MLevelManager.THIS.gameStatus = GameState.GameOver;
            AnimAction(() =>
            {
                for (var index = 0; index < timeBombs.Count(); index++)
                {
                    var i = timeBombs.ToList()[index];
                    i.ExlodeAnimation(index != 0,null);
                }
            });}
            else AnimAction(()=>MLevelManager.THIS.gameStatus = GameState.GameOver);
        }

        void AnimAction( Action call)
        {
            Animation anim = GetComponent<Animation>();
            var animationState = anim["bannerFailed"];
            animationState.speed = 1;
            anim.Play();
            LeanTween.delayedCall(anim.GetClip("bannerFailed").length - animationState.time, call);
        }

        private bool IsFail() => objects[0].activeSelf;

        void ContinueBomb()
        {
            FindObjectsOfType<MItemTimeBomb>().ForEachY(i =>
            {
                i.timer += 5;
                i.InitItem();
            });
        }

        /// <summary>
        /// Continue the game
        /// </summary>
        private void ContinueFailed()
        {
            if (MLevelManager.THIS.mLevelData.limitType == LIMIT.MOVES)
                MLevelManager.THIS.mLevelData.limit += MLevelManager.THIS.ExtraFailedMoves;
            else
                MLevelManager.THIS.mLevelData.limit += MLevelManager.THIS.ExtraFailedSecs;
        }
    }
}