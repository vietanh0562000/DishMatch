using System.Collections.Generic;
using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.Analytics;
#if UNITY_ANALYTICS

#endif

namespace SweetSugar.Scripts.Integrations
{
    /// <summary>
    /// Game events listener.
    /// </summary>
    public class EventsListener : MonoBehaviour {

        void OnEnable() {
            MLevelManager.OnMapState += OnMapState;
            MLevelManager.OnEnterGame += OnEnterGame;
            MLevelManager.OnLevelLoaded += OnLevelLoaded;
            MLevelManager.OnMenuPlay += OnMenuPlay;
            MLevelManager.OnMenuComplete += OnMenuComplete;
            MLevelManager.OnStartPlay += OnStartPlay;
            MLevelManager.OnWin += OnWin;
            MLevelManager.OnLose += OnLose;

        }

        void OnDisable() {
            MLevelManager.OnMapState -= OnMapState;
            MLevelManager.OnEnterGame -= OnEnterGame;
            MLevelManager.OnLevelLoaded -= OnLevelLoaded;
            MLevelManager.OnMenuPlay -= OnMenuPlay;
            MLevelManager.OnMenuComplete -= OnMenuComplete;
            MLevelManager.OnStartPlay -= OnStartPlay;
            MLevelManager.OnWin -= OnWin;
            MLevelManager.OnLose -= OnLose;

        }

        #region GAME_EVENTS
        void OnMapState() {
        }
        void OnEnterGame() {
            AnalyticsEvent("OnEnterGame", MLevelManager.THIS.currentLevel);
        }
        void OnLevelLoaded() {
        }
        void OnMenuPlay() {
        }
        void OnMenuComplete() {
        }
        void OnStartPlay() {
            Debug.Log("OnStartPlay");
        }
        void OnWin() {
            AnalyticsEvent("OnWin", MLevelManager.THIS.currentLevel);
        }
        void OnLose() {
            AnalyticsEvent("OnLose", MLevelManager.THIS.currentLevel);
        }

        #endregion

        void AnalyticsEvent(string _event, int level) {
#if UNITY_ANALYTICS
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(_event, level);
            Analytics.CustomEvent(_event, dic);

#endif
        }


    }
}
