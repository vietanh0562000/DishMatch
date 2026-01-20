namespace PuzzleGames
{
    using System;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using BasePuzzle.PuzzlePackages;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public enum GameState
    {
        Prepare,
        Playing,
        Won,
        Lost,
        Stopped
    }

    public class GameManager : Singleton<GameManager>
    {
        private bool isPlayed;
        private bool firstLose;

        private GameState _currentState = GameState.Prepare;

        public static event Action OnGameStart;
        public static event Action OnGamePlaying;
        public static event Action OnGameWon;
        public static event Action OnGameLost;
        public static event Action OnGameStop;
        public static event Action OnGameRevive;
        public static event Action OnQuitGame;
        

        public Action OnStickmanMoveHole;
        public Action OnClickHole;

        public GameState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                HandleStateChange(_currentState);
            }
        }

        public bool IsPlayed => isPlayed;

        private void Start()
        {
            _currentState = GameState.Prepare;
        }

        public void SetPlayed()
        {
            if (isPlayed)
            {
                return;
            }

            StartGame();
            isPlayed = true;
        }
        
        [Button]
        private void StartGame()
        {
            if (CurrentState == GameState.Prepare)
            {
                firstLose    = false;
                isPlayed     = true;
                CurrentState = GameState.Playing;
                OnGameStart?.Invoke();
            }
        }

        [Button]
        public void WinGame()
        {
            if (_currentState == GameState.Playing)
            {
                CurrentState = GameState.Won;
            }
        }

        [Button]
        public void LoseGame()
        {
            if (_currentState == GameState.Playing)
            {
                firstLose    = true;
                CurrentState = GameState.Lost;
            }
        }

        public void ReviveGame()
        {
            if (CurrentState == GameState.Lost)
            {
                OnGameRevive?.Invoke();
                CurrentState = GameState.Playing;
                //AdsManager.Instance.ShowBanner();
            }
        }

        private void HandleStateChange(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    OnGamePlaying?.Invoke();
                    break;
                case GameState.Won:
                    OnGameWon?.Invoke();
                    break;
                case GameState.Lost:
                    OnGameLost?.Invoke();
                    break;
                case GameState.Stopped:
                    OnGameStop?.Invoke();
                    break;
                default:
                    break;
            }
        }
        public void Stop()
        {
            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Stopped;
            }
        }
        public void Continue()
        {
            if (CurrentState != GameState.Lost)
            {
                CurrentState = isPlayed ? GameState.Playing : GameState.Prepare;
                //AdsManager.Instance.ShowBanner();
            }
        }
        public void QuitGame()
        {
            OnQuitGame?.Invoke();
            //AdsManager.Instance.HideBanner();

            Action loadHome = () =>
            {
                LoadSceneManager.Instance.LoadScene("Home");
            };

            if (ServerConfig.Instance<ValueRemoteConfig>().showInterstitialWhenGoHome)
            {
                ShowInterstitials(loadHome, loadHome, "quit_go_home",
                    LevelManager.Instance.currentLevelToLog);
            }
            else
            {
                loadHome.Invoke();
            }
        }

        public void EndGame(bool watchedAds = false)
        {
            OnQuitGame?.Invoke();
            if (LevelDataController.instance.GetLevelJustPassed() >=
                ServerConfig.Instance<ValueRemoteConfig>().numLevelToShowRewarded && !watchedAds)
            {
                ShowInterstitials(LoadNextScene, LoadNextScene, "complete_level",
                    LevelDataController.instance.GetLevelJustPassed());
            }
            else
            {
                LoadNextScene();
            }
        }

        public void ShowInterstitials(Action onSuccess, Action onFail, string where, int levelLog)
        {
            if (CanShowInterstitial)
            {
                WindowManager.Instance.OpenWindow<PreShowAdsPanel>(onLoaded: panel =>
                {
                    #if UNITY_EDITOR
                    panel.Setup(() => OnShowInterstitialAds(onSuccess));
                    return;
                    #endif
                    
                    panel.Setup(() =>
                    {
                        // AdsManager.Instance.ShowInterstitials(() => { OnShowInterstitialAds(onSuccess); },
                        //     onFail, where, levelLog);
                    });
                });
            }
            else
            {
                onSuccess?.Invoke();
            }
        }

        private void OnShowInterstitialAds(Action onEnd)
        {
            GameController.Instance.CountShowInterstitial++;
            if (GameController.Instance.CountShowInterstitial >=
                ServerConfig.Instance<ValueRemoteConfig>().numInterstitialToShowRemoveAds)
            {
                GameController.Instance.CountShowInterstitial = 0;
                WindowManager.Instance.OpenWindow<NoAdsPanel>(onLoaded: adsPanel =>
                {
                    adsPanel.SetInGame();
                    adsPanel.SetActionOnClose(onEnd);
                });
            }
            else
            {
                onEnd?.Invoke();
            }
        }

        private void LoadNextScene()
        {
            if (LevelDataController.instance.GetLevelJustPassed() >=
                ServerConfig.Instance<ValueRemoteConfig>().numLevelToGoHomeAfterPassLevel)
            {
                LoadSceneManager.Instance.LoadScene("Home");
            }
            else
            { 
                LevelLoader.LoadLevel(LevelDataController.instance.Level);
            }
        }

        private DateTime _targetTime = DateTime.UtcNow;

        public bool CanShowInterstitial =>  false; // DateTimeUtils.UtcNow >= _targetTime &&AdsManager.Instance.CanShowInter;

        public void ShowAdsUpdateTargetTime() { _targetTime = DateTimeUtils.UtcNow.AddSeconds(ServerConfig.Instance<ValueRemoteConfig>().numSecondsToReShowAds); }
    }
}