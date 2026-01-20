using System.Collections;
using System.Collections.Generic;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;

public class LoadLevelMonobehaviour : MonoBehaviour
{
    //Start is called before the first frame update
    void Start() { LoadLevelManager.instance.LoadLevelsFromServer(LevelDataController.instance.Level, ServerConfig.Instance<ValueRemoteConfig>().number_of_load_levels_after_login); }
}