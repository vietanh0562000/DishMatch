using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Settings;
using UnityEngine;
using UnityEngine.UI;

public class BtnSettings : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick_Show_UIPopup_Setting);
    }

    public void OnClick_Show_UIPopup_Setting()
    {
        //UIManager.Instance.OpenPopup(SettingsAssetPaths.GetPath(AssetIDs.SETTINGS_ASSETS));
        WindowManager.Instance.OpenWindow<UI_Setting>();
    }
}