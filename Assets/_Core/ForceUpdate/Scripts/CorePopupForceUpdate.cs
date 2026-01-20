using BasePuzzle.Core.Scripts;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using System;
using System.Collections;
using BasePuzzle.Core.Scripts.Logs;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using UnityEngine;

public class CorePopupForceUpdate : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        GameMain.OnInitComplete += OnInitComplete;
        CoreLogger.Instance.Info("CorePopupForceUpdate init complete");
    }

    private void Awake()
    {
    }
    

    private static void OnInitComplete(object sender, EventArgs e)
    {
        FGameObj.Instance.AddIfNotExist<CorePopupForceUpdate>();
    }

    static int CompareVersion(string v1, string v2)
    {
        string[] arr1 = v1.Split('.');
        string[] arr2 = v2.Split('.');
        int target = arr1.Length > arr2.Length ? arr2.Length : arr1.Length;
        for (int i = 0; i < target; i++)
        {
            bool a = int.TryParse(arr1[i], out int rs1);
            bool b = int.TryParse(arr2[i], out int rs2);
            if (!a || !b) return -1;
            if (rs1 == rs2) continue;
            return rs1 > rs2 ? 1 : -1;
        }
        return 0;
    }
}
