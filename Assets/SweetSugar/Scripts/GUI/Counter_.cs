using System.Collections;
using System.Linq;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// various GUi counters
    /// </summary>
    public class Counter_ : MonoBehaviour
    {
        TextMeshProUGUI txt;
        private float lastTime;
        bool alert;

        private MLevelData _thisMLevelData;

        public MLevelData ThisMLevelData
        {
            get
            {
                if (_thisMLevelData == null) _thisMLevelData = MLevelData.THIS;
                return _thisMLevelData;
            }
            set => _thisMLevelData = value;
        }

        // Use this for initialization
        void Awake()
        {
 

            // txt = GetComponent<Text>();
            txt = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            ThisMLevelData = MLevelManager.THIS.mLevelData;
        }

        void OnEnable()
        {
            lastTime = 0;
            UpdateText();
            alert = false; StartCoroutine(UpdateRare());
            if (name == "Limit") StartCoroutine(TimeTick());
        }

        // Update is called once per frame
        IEnumerator UpdateRare()
        {
            while (true)
            {
                if (txt == null) continue;

                UpdateText();
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void UpdateText()
        {
            if (name == "Score")
            {
                txt.text = "" + MLevelManager.Score;
            }

            if (name == "BestScore")
            {
                txt.text = "Best score:" + PlayerPrefs.GetInt("Score" + PlayerPrefs.GetInt("OpenLevel"));
            }

            if (name == "Limit" && ThisMLevelData != null)
            {
                if (ThisMLevelData.limitType == LIMIT.MOVES)
                {
                    txt.text = "" + Mathf.Clamp(ThisMLevelData.limit, 0, ThisMLevelData.limit);
                    txt.transform.localScale = Vector3.one;
                    if (ThisMLevelData.limit <= 5)
                    {
                        txt.color = new Color(225f / 255f, 32f / 255, 30f / 255);
                        txt.outlineColor = Color.white;
                        if (!alert)
                        {
                            alert = true;
//                            SoundBase.Instance.PlayOneShot(SoundBase.Instance.alert);
                        }
                    }
                    else
                    {
                        alert = false;
                        txt.color = Color.white;
                        // txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
                    }
                }
                else
                {
                    var minutes = Mathf.FloorToInt(ThisMLevelData.limit / 60F);
                    var seconds = Mathf.FloorToInt(ThisMLevelData.limit - minutes * 60);
                    txt.text = "" + $"{minutes:00}:{seconds:00}";
                    txt.transform.localScale = Vector3.one * 0.68f;
                    txt.fontSize = 80;
                    if (ThisMLevelData.limit <= 5 && MLevelManager.THIS.gameStatus == GameState.Playing)
                    {
                        // txt.color = new Color(216f / 255f, 0, 0);
                        // txt.outlineColor = Color.white;
                        if (lastTime + 5 < Time.time)
                        {
                            lastTime = Time.time;
                            SoundBase.Instance.PlayOneShot(SoundBase.Instance.timeOut);
                        }
                    }
                    else
                    {
                        txt.color = Color.white;
                        txt.outlineColor = new Color(225f / 255f, 32f / 255f, 30f / 255f);
                    }
                }
            }

            if (name == "Lifes")
            {
                txt.text = "" + InitScript.Instance?.GetLife();
            }

            if (name == "FailedCount")
            {
                if (ThisMLevelData.limitType == LIMIT.MOVES)
                    txt.text = "+" + MLevelManager.THIS.ExtraFailedMoves;
                else
                    txt.text = "+" + MLevelManager.THIS.ExtraFailedSecs;
            }

            if (name == "FailedPrice")
            {
                txt.text = "" + MLevelManager.THIS.FailedCost;
            }

            if (name == "FailedDescription")
            {
                txt.text = "" + MLevelData.THIS.GetTargetCounters().First(i => !i.IsTotalTargetReached()).targetLevel.GetFailedDescription();
            }


            if (name == "Gems")
            {
                txt.text = "" + InitScript.Gems;
            }

            if (name == "TargetScore")
            {
                txt.text = "" + ThisMLevelData.star1;
            }

            if (name == "Level")
            {
                txt.text = "" + PlayerPrefs.GetInt("OpenLevel");
            }

            // if (name == "TargetDescription1")
            // {
            //     txt.text = "" + LevelData.THIS.GetTargetContainersForUI().First().targetLevel.GetDescription();
            // }
        }

        IEnumerator TimeTick()
        {
            while (true)
            {
                if (MLevelManager.THIS.gameStatus == GameState.Playing)
                {
                    if (_thisMLevelData.limitType == LIMIT.TIME)
                    {
                        _thisMLevelData.limit--;
                        if (!MLevelManager.THIS.DragBlocked)
                            MLevelManager.THIS.CheckWinLose();
                    }
                }
                if (MLevelManager.THIS.gameStatus == GameState.Map)
                    yield break;
                yield return new WaitForSeconds(1);
            }
        }
    }
}
