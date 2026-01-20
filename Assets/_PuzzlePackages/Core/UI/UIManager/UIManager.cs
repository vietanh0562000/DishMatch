using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine.SceneManagement;
using Path = System.IO.Path;

public class UIManager : Singleton<UIManager>
{
    #region Menus

    [Tooltip("All the menus in the scene.")]
    public List<UIMenu> AllMenus = new List<UIMenu>();

    [HideInInspector] public UIMenu CurActiveMenu;

    #endregion

    #region Pop-ups

    [Tooltip("All the popups in the scene.")]
    public List<UIPopup> AllPopups = new List<UIPopup>();

    [HideInInspector] public UIPopup CurActivePopup;

    #endregion

    [Header("Store - UIPopup")] public RectTransform rootStorePopup;

    private bool canEsc = true;
    private List<UIPopup> stack = new List<UIPopup>();

    protected override void Awake()
    {
        //If there's no default menu, find one
        if (!CurActiveMenu && AllMenus.Count > 0)
        {
            //Find the first menu that is not null
            for (int i = 0; i < AllMenus.Count; i++)
            {
                if (AllMenus[i] != null)
                {
                    CurActiveMenu = AllMenus[i];
                    break;
                }
            }

            foreach (UIMenu m in AllMenus)
            {
                if (m == null) continue;
                if (m == CurActiveMenu)
                {
                    m.gameObject.SetActive(true);
                    m.ChangeVisibility(true);
                    break;
                }
            }
        }

        base.Awake();
    }

    protected override void OnDestroy()
    {
        UnLoadAsset();
        base.OnDestroy();
    }

    private void UnLoadAsset()
    {
        AllMenus = null;
        AllPopups = null;
    }

    //private void LateUpdate()
    //{
    //    if (Input.GetKeyUp(KeyCode.Escape) && canEsc)
    //    {
    //        if (CurActivePopup)
    //        {
    //            ClosePopup();
    //        }
    //        else
    //        {
    //            if (CurActiveMenu.isOverrideBack)
    //            {
    //                CurActiveMenu.onOverrideBack?.Invoke();
    //                return;
    //            }

    //            if (CurActiveMenu.PreviousMenu)
    //            {
    //                OpenMenu(CurActiveMenu.PreviousMenu);
    //            }
    //        }
    //    }
    //}


#if UNITY_EDITOR
    [Header("Editor")] public List<UIPopup> popupGenerate = new List<UIPopup>();

    [Sirenix.OdinInspector.Button]
    public void GenerateSceneContainPopup()
    {
        List<string> popupName = new List<string>();
        for (int i = 0; i < popupGenerate.Count; i++)
        {
            Debug.LogError($"i " + i);
            var obj = popupGenerate[i];
            obj.gameObject.SetActive(true);
            if (popupName.Contains(obj.name))
            {
                Debug.LogWarning("2 popup samename");
                Debug.LogWarning(obj.name);
                continue;
            }

            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Additive);

            scene.name = obj.name;

            popupName.Add(obj.name);

            var gameObj = UnityEditor.PrefabUtility.InstantiatePrefab(obj, scene);
            Debug.LogError(gameObj.name);
            var prefab = scene.GetRootGameObjects()[0];
            // if (prefab.GetComponent<SetupPopup>() == null)
            // {
            //     prefab.gameObject.AddComponent<SetupPopup>();
            // }
            //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
            //UnityEngine.Debug.Log(go.name, go);
            string path = $"Assets/_Games/Scenes/Popup/{scene.name}.unity";
            //SceneManager.MoveGameObjectToScene(go, scene);
            bool r = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, path);
            UnityEngine.Debug.LogError(r);
            //UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync(scene);
        }
    }
#endif

    [Sirenix.OdinInspector.Button]
    public void LoadSceneAdditive(string name)
    {
        SceneController.instance.LoadScene(name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    #region Menu Functions

    private UIMenu GetCurrentMenu()
    {
        if (CurActiveMenu != null)
        {
            foreach (UIMenu m in AllMenus)
            {
                if (m.name == CurActiveMenu.name)
                {
                    return m;
                }
            }
        }

        return null;
    }

    public void OpenMenu_Stack(string menuName)
    {
        var currentMenu = GetCurrentMenu();

        if (currentMenu.name == menuName)
        {
            return;
        }

        OpenMenu(menuName);
        CurActiveMenu.PreviousMenu = currentMenu;
    }

    /// <summary>
    /// Open menu by name, and close the current active one.
    /// </summary>
    /// <param name="menuName"></param>
    public void OpenMenu(string menuName)
    {
        UIMenu lastM = CurActiveMenu;
        bool foundIt = false;

        foreach (UIMenu m in AllMenus)
        {
            if (m == null)
            {
                Debug.LogError(
                    "Found an empty element in the menu's list, please make sure there's no empty elements",
                    gameObject);
                continue;
            }

            if (m.name == menuName)
            {
                if (!m.gameObject.activeSelf) m.gameObject.SetActive(true);
                m.ChangeVisibility(true);

                CurActiveMenu = m;
                foundIt = true;
                break;
            }
        }

        if (lastM != CurActiveMenu)
            lastM.ChangeVisibility(false);

        if (!foundIt)
            Debug.LogError("There's no menu named \"" + menuName + "\" inside Menu Manager's \"All Menus\"  list.",
                gameObject);
    }

    /// <summary>
    /// Open menu by reference, and close the current active one.
    /// </summary>
    /// <param name="menu"></param>
    public void OpenMenu(UIMenu menu)
    {
        UIMenu lastM = CurActiveMenu;
        bool foundIt = false;

        foreach (UIMenu m in AllMenus)
        {
            if (m == null)
            {
                Debug.LogError(
                    "Found an empty element in the menu's list, please make sure there's no empty elements",
                    gameObject);
                continue;
            }

            if (m == menu)
            {
                if (!m.gameObject.activeSelf) m.gameObject.SetActive(true);
                m.ChangeVisibility(true);

                CurActiveMenu = m;
                foundIt = true;
                break;
            }
        }

        if (lastM != CurActiveMenu)
            lastM.ChangeVisibility(false);

        if (!foundIt)
            Debug.LogError("There's no menu named \"" + menu.name + "\" inside Menu Manager's \"All Menus\"  list.",
                gameObject);
    }

    /// <summary>
    /// Open current menu's NextMenu.
    /// </summary>
    public void NextMenu()
    {
        if (CurActiveMenu.NextMenu)
            OpenMenu(CurActiveMenu.NextMenu);
    }

    #endregion

    #region Pop-up Functions

    public Dictionary<string, Action<UIPopup>> dicActionOnOpenPopup = new Dictionary<string, Action<UIPopup>>();

    /// <summary>
    /// Open a popup by name.
    /// </summary>
    /// <param name="popupPath">The path of the popup gameObject.</param>
    public void OpenPopup(string popupPath, Action<UIPopup> callback = null)
    {
        var popupName = Path.GetFileNameWithoutExtension(popupPath);
        foreach (UIPopup p in AllPopups)
        {
            if (p == null) continue;

            if (p.name == popupName)
            {
                OpenPopup(p, callback);
                return;
            }
        }

        // Debug.LogError(
        //    "Opening Pop-up faild. Couldn't find a pop-up with the name " + popupName +
        //    ". Please make sure its listed in the Pop-ups list.", gameObject);

        if (dicActionOnOpenPopup.ContainsKey(popupName))
        {
            return;
        }

        dicActionOnOpenPopup.Add(popupName, callback);
        UILoadPopupUltis.LoadPopup(popupPath, popupName, (p) =>
        {
            AllPopups.Add(p);
            OpenPopup(p, dicActionOnOpenPopup[popupName]);
        });
        //SceneManager.LoadScene(popupName, new LoadSceneParameters(LoadSceneMode.Additive));
    }


    public static Action OnUIPopupChange;

    /// <summary>
    /// Open a popup by reference.
    /// </summary>
    /// <param name="popup"></param>
    public void OpenPopup(UIPopup popup, Action<UIPopup> callback = null)
    {
        popup.ChangeVisibility(true);

        CurActivePopup = popup;

        //To Stack Popup
        RegisterPopupStack(popup);

        StateActivatePopupStack(true);
        OnUIPopupChange?.Invoke();

        callback?.Invoke(popup);
    }

    public void OpenPopupIE(UIPopup popup, Action onShow)
    {
        StartCoroutine(IEOpenPopup());

        IEnumerator IEOpenPopup()
        {
            yield return new WaitForEndOfFrame();

            if (popup.gameObject.activeInHierarchy == false)
            {
                popup.ChangeVisibility(true);
            }

            CurActivePopup = popup;

            //To Stack Popup
            RegisterPopupStack(popup);

            StateActivatePopupStack(true);

            onShow?.Invoke();
        }
    }

    /// <summary>
    /// Close the current opened popup
    /// </summary>
    public void ClosePopup()
    {
        if (CurActivePopup)
        {
            CurActivePopup.ChangeVisibility(false);
            stack.Remove(CurActivePopup);
        }

        CurActivePopup = GetTopMostPopupStack();

        StateActivatePopupStack(false);
    }

    /// <summary>
    /// Close popup by name, only if its opened.
    /// </summary>
    public void ClosePopup(string popupName)
    {
        foreach (UIPopup p in AllPopups)
        {
            if (p.name == popupName)
            {
                ClosePopup(p);
                return;
            }
        }

        Debug.LogError(
            "Closing Pop-up faild. Couldn't find a pop-up with the name " + popupName +
            ". Please make sure its listed in the Pop-ups list.", gameObject);
    }

    /// <summary>
    /// Close popup by reference, only if its opened.
    /// </summary>
    public void ClosePopup(UIPopup popup)
    {
        popup.ChangeVisibility(false);
        stack.Remove(popup);

        CurActivePopup = GetTopMostPopupStack();

        StateActivatePopupStack(false);
        OnUIPopupChange?.Invoke();
    }

    public void CloseAllPopup()
    {
        if (stack != null && stack.Count > 0)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                ClosePopup(stack[i]);
            }
        }
    }

    private void RegisterPopupStack(UIPopup popup)
    {
        stack.Remove(popup);
        stack.Add(popup);
    }

    private UIPopup GetTopMostPopupStack()
    {
        UIPopup nextPopup = null;
        for (var i = stack.Count - 1; i >= 0; i--)
        {
            var popup = stack[i];
            if (popup != null && popup.Visible)
            {
                nextPopup = popup;
                break;
            }
        }

        return nextPopup;
    }

    private void StateActivatePopupStack(bool isOpenPopup)
    {
        if (CurActivePopup)
        {
            CurActivePopup.transform.SetParent(rootStorePopup);

            var rect = CurActivePopup.GetComponent<RectTransform>();
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.anchoredPosition3D = Vector3.zero;

            if (isOpenPopup)
            {
                CurActivePopup.transform.SetAsLastSibling();
            }
        }
    }

    public List<UIPopup> GetAllPopupStack()
    {
        return stack;
    }

    public int GetNumPopupStack()
    {
        return stack.Count;
    }

    public bool IsFreeUI_PopupFullScreen()
    {
        var isFree = true;

        if (stack != null && stack.Count > 0)
        {
            foreach (var item in stack)
            {
                if (item.isFullScreen)
                {
                    isFree = false;
                    break;
                }
            }
        }

        return isFree;
    }

    public bool IsFreeUI_PopupHalfFullScreen()
    {
        var isFree = true;

        if (stack != null && stack.Count > 0)
        {
            foreach (var item in stack)
            {
                if (item.isHalfFullScreen)
                {
                    isFree = false;
                    break;
                }
            }
        }

        return isFree;
    }

    #endregion

    /// <summary>
    /// Open current menu's "PreviousMenu" or closes the opened popup or sidemenu.
    /// </summary>
    public void OpenUI(string nameUI)
    {
        foreach (var menu in AllMenus)
        {
            if (menu.name == nameUI)
            {
                Instance.OpenMenu(nameUI);
                return;
            }
        }

        foreach (var popup in AllPopups)
        {
            if (popup.name == nameUI)
            {
                OpenPopup(nameUI);
                return;
            }
        }

        Debug.LogError($"Can't find UI : {gameObject.name}");
    }

    /// <summary>
    /// Turning back functionality on/off.
    /// </summary>
    /// <param name="on"></param>
    public void StateEsc(bool isOn)
    {
        canEsc = isOn;
    }
}