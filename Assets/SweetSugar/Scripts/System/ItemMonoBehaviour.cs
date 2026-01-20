using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.System.Pool;
using UnityEngine;

namespace SweetSugar.Scripts.System
{
    public class ItemMonoBehaviour : MonoBehaviour
    {
        private bool quit;

        protected virtual void Start()
        {
            // Debug.Log(gameObject.name + " " + gameObject.GetInstanceID() + " created ");
        }

        public void DestroyBehaviour()
        {
            var item = GetComponent<MItem>();
            if (item == null || !gameObject.activeSelf) return;
            StartCoroutine(DestroyDelay(item));
        }

        public static int finishBonusCounter;

        IEnumerator DestroyDelay(MItem mItem)
        {
            bool changeTypeFinished=false;
            if (mItem.NextType != ItemsTypes.NONE)
            {
                if(MLevelManager.THIS.gameStatus == GameState.PreWinAnimations)
                {
                    finishBonusCounter++;
                    if (finishBonusCounter >= 5) mItem.NextType = ItemsTypes.NONE;
                }
                if(MLevelManager.THIS.gameStatus != GameState.PreWinAnimations || (MLevelManager.THIS.gameStatus == GameState.PreWinAnimations && finishBonusCounter <5))
                {
                    mItem.ChangeType((x) => { changeTypeFinished = true; });
                    yield return new WaitUntil(() => changeTypeFinished);
                }
            }

            if (MLevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log(name + " dontDestroyOnThisMove " + mItem.dontDestroyOnThisMove + " dontDestroyForThisCombine " + gameObject.GetComponent<MItem>()
                                       .dontDestroyForThisCombine,  DebugLogKeeper.LogType.Destroying);
            if (mItem.dontDestroyOnThisMove || gameObject.GetComponent<MItem>().dontDestroyForThisCombine)
            {
                GetComponent<MItem>().StopDestroy();
                yield break;
            }
            if (MLevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log(gameObject.GetInstanceID() + " destroyed " + mItem.name + " " + mItem.GetInstanceID(), DebugLogKeeper.LogType.Destroying);
            OnDestroyItem(mItem);
            ObjectPooler.Instance.PutBack(gameObject);
            yield return new WaitForSeconds(0);
        }

        private void OnDestroyItem(MItem mItem)
        {
//        if (item.square && item == item.square.Item)
//            item.square.Item = null;
            mItem.square = null;
            mItem.field.squaresArray.Where(i => i.MItem == mItem).ForEachY(i => i.MItem = null);
            mItem.previousSquare = null;
            mItem.tutorialItem = false;
            mItem.NextType = ItemsTypes.NONE;
            if(transform.childCount>0)
            {
                transform.GetChild(0).transform.localScale = Vector3.one;
                transform.GetChild(0).transform.localPosition = Vector3.zero;
            }
        }

        void OnApplicationQuit()
        {
            quit = true;
        }

        void OnDestroy()
        {
            // if (!quit) Debug.Log(gameObject.name + " " + gameObject.GetInstanceID() + " OnDestroyed ");
        }
    }
}