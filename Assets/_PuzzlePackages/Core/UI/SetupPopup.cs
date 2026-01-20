using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SetupPopup : MonoBehaviour
{
    string nameCallback;
    UIPopup uIPopup;

    private void Awake()
    {
        //Create UI
        name = name.Replace("(Clone)", "").Trim();
        uIPopup = GetComponent<UIPopup>();
        nameCallback = uIPopup.name;
    }

    private void Start()
    {
        UILoadPopupUltis.dicActionOnOpenPopup[nameCallback]?.Invoke(uIPopup);
    }
}