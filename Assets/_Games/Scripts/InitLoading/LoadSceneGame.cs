using UnityEngine;

namespace PuzzleGames
{
    using System.Collections;
    using BasePuzzle.PuzzlePackages.Core;

    public class LoadSceneGame : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "Home";

        private IEnumerator Start()
        {
            yield return null;

            var dataController = LevelDataController.instance;
            if (dataController.Level <= 1)
            {
                Debug.LogError("Load level 1");
                dataController.Play();
                LoadLevel(1);
            }
            else
            {
                Debug.LogError("Load Menu");
                LoadSceneManager.Instance.LoadScene(nextSceneName);
            }
        }

        private void LoadLevel(int level)
        {
            var levelJson = LoadLevelManager.instance.ReadLevelData(level);
            TempDataHandler.Set(TempDataKeys.CURRENT_LEVEL_JSON_DATA, levelJson);
            TempDataHandler.Set(TempDataKeys.CURRENT_LEVEL_FROM_HOME, level);
            LoadSceneManager.Instance.LoadScene("GamePlay");
        }
    }
}