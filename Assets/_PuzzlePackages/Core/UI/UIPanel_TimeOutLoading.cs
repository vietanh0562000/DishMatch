using System;
using System.Collections;
using UnityEngine;

public class UIPanel_TimeOutLoading : MonoBehaviour
{
    public Action onRectNotifyClosed;

    public RectTransform rectLoading;
    // public RectTransform rectNotify;

    private Coroutine _coroutineLoading;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(7f);
    private WaitForSeconds waitForSecondClose = new WaitForSeconds(2.5f);

    private void OnEnable()
    {
        if (_coroutineLoading != null)
        {
            StopCoroutine(_coroutineLoading);
        }

        _coroutineLoading = StartCoroutine(CoroutineActiveLoading());
    }

    private void OnDisable()
    {
        rectLoading.gameObject.SetActive(false);
        // rectNotify.gameObject.SetActive(false);
    }

    private IEnumerator CoroutineActiveLoading()
    {
        rectLoading.gameObject.SetActive(true);
        // rectNotify.gameObject.SetActive(false);
        yield return waitForSeconds;
        rectLoading.gameObject.SetActive(false);
        // rectNotify.gameObject.SetActive(true);
        yield return waitForSecondClose;
        // onRectNotifyClosed?.Invoke();
        // onRectNotifyClosed = null;

        //Close This UIPanel
        gameObject.SetActive(false);
    }
}