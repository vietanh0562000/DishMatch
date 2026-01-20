using System;
using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Core.UserData;
using UnityEngine;

public class SendMessageWorker : Singleton<SendMessageWorker>
{
    // public Queue<CSMessageWaitLoginSuccess> queue;
    //
    // protected override void Awake()
    // {
    //     base.Awake();
    //     queue = new Queue<CSMessageWaitLoginSuccess>();
    //
    //     UserDataManager.onLoginStateChange += OnLogin;
    //     SceneController.instance.onChangeSceneState += OnChangeScene;
    // }
    //
    // protected override void OnDestroy()
    // {
    //     UserDataManager.onLoginStateChange -= OnLogin;
    //     SceneController.instance.onChangeSceneState -= OnChangeScene;
    //     base.OnDestroy();
    // }
    //
    // private void OnChangeScene(SceneState state)
    // {
    //     queue.Clear();
    // }
    //
    // private Coroutine _coroutine;
    //
    // private void OnLogin(bool obj)
    // {
    //     if (!obj)
    //     {
    //         return;
    //     }
    //
    //     IEnumerator CoroutineDeQueue()
    //     {
    //         while (!IsEmpty)
    //         {
    //             yield return null;
    //             CSMessageWaitLoginSuccess cs = queue.Dequeue();
    //             cs.SendNow();
    //             LogUtils.Log("SendNnow___________");
    //         }
    //     }
    //
    //     if (_coroutine != null)
    //     {
    //         StopCoroutine(_coroutine);
    //     }
    //
    //     _coroutine = StartCoroutine(CoroutineDeQueue());
    // }
    //
    // public void AddQueue(CSMessageWaitLoginSuccess cSMessage)
    // {
    //     queue.Enqueue(cSMessage);
    //     if (queue.Count > 20)
    //     {
    //         queue.Dequeue();
    //     }
    //
    //     LogUtils.Log("Add Queue___________");
    // }
    //
    // public bool IsEmpty => queue.Count == 0;
}