using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

// using UnityEngine.SceneManagement;

public static class UILoadPopupUltis
{
    public static Dictionary<string, Action<UIPopup>> dicActionOnOpenPopup = new Dictionary<string, Action<UIPopup>>();

    public static void LoadPopup(string path, string name, Action<UIPopup> action)
    {
        dicActionOnOpenPopup[name] = action;
        // SceneManager.LoadScene(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));
        var go = Addressables.LoadAssetAsync<GameObject>(path);
        go.Completed += OnLoadComplete;
    }

    private static void OnLoadComplete(AsyncOperationHandle<GameObject> obj)
    {
        Object.Instantiate(obj.Result);
    }
}