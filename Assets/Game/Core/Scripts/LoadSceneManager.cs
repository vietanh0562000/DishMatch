using System.Threading.Tasks;
using DG.Tweening;
using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleGames
{
    using System;
    using System.Collections;
    using BasePuzzle.Modules.UI.Transition.Runtime;

    public class LoadSceneManager : PersistentSingleton<LoadSceneManager>
    {
        [SerializeField] private UITransitionManager transitionManager;

        private string _previousScene;
        private bool   _isInProgress;
        private bool   _isFirstLoad;

        protected override void Awake()
        {
            _previousScene = SceneManager.GetActiveScene().name;

            base.Awake();
        }

        public void LoadScene(string sceneName)
        {
            ResourceManager.Instance.ReleaseUI();
            StartCoroutine(CoLoadSceneAsync(sceneName));
        }

        private IEnumerator CoLoadSceneAsync(string sceneName)
        {
            if (_isInProgress) yield break;
            
            Time.timeScale = 1;
            _isInProgress  = true;

            yield return null;

            if (!_isFirstLoad)
            {
                _isFirstLoad = true;
                transitionManager.ProgressBarIn(() =>
                {
                    transitionManager.ProgressBarOut(null);
                });
                
                var task = transitionManager.ProgressBarInAsync;

                // Wait until the task completes
                yield return new WaitUntil(() => task.IsCompleted);
            }
            
            var wipeInAsync = transitionManager.IrisWipeInAsync;
            
            yield return new WaitUntil(() => wipeInAsync.IsCompleted);
            
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            while (loadOperation is { isDone: false })
            {
                yield return null;
            }

            if (_previousScene != sceneName)
            {
                var unloadOperation = SceneManager.UnloadSceneAsync(_previousScene);
                while (unloadOperation is { isDone: false })
                {
                    yield return null;
                }
            }
            else
            {
                var first           = SceneManager.GetActiveScene();
                var unloadOperation = SceneManager.UnloadSceneAsync(first);
                while (unloadOperation is { isDone: false })
                {
                    yield return null;
                }
            }
            
            yield return null;

            transitionManager.IrisWipeOut(null);

            _previousScene = sceneName;
            _isInProgress  = false;
        }
    }
}