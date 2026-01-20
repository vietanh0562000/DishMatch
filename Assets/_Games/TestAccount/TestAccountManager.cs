using System;
using com.ootii.Messages;
using BasePuzzle.PuzzlePackages;
using PuzzleGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _iptLevel;
    [SerializeField] private Toggle         _cbTopUI, _cbBoosterUI;
    [SerializeField] private GameObject     _topUI,   _boosterUI, _panelTestAccount;

    private void Awake()
    {
        gameObject.SetActive(false);
#if ACCOUNT_TEST
        gameObject.SetActive(true);
#endif

        if (!gameObject.activeInHierarchy) return;

        _cbTopUI.onValueChanged.AddListener(OnTopUIChanged);
        _cbBoosterUI.onValueChanged.AddListener(OnBoosterUIChanged);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _panelTestAccount.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            LevelManager.Instance.CompleteLevel();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.LoseGame();
        }
        
    }
#endif

    private void OnTopUIChanged(bool value) { _topUI.gameObject.SetActive(value); }

    private void OnBoosterUIChanged(bool value) { _boosterUI.gameObject.SetActive(value); }

    public void ClickBtnPlay()
    {
        if (!int.TryParse(_iptLevel.text, out int level)) return;
        LevelDataController.instance.LevelData.level = level;
        LevelLoader.LoadLevel(level);
    }

    public void ClickBtnAddGold(int amount)
    {
        ResourceType.Gold.Manager()?.Add(amount);
        ResourceType.Gold.Manager().UI.UpdateUI();
    }

    public void ClickBtnAddUFO(int amount)
    {
        ResourceType.Powerup_Helidrop.Manager()?.Add(amount);
        ResourceType.Powerup_Helidrop.Manager().UI.UpdateUI();
    }

    public void ClickBtnAddSlot(int amount)
    {
        ResourceType.Powerup_AddSlot.Manager()?.Add(amount);
        ResourceType.Powerup_AddSlot.Manager().UI.UpdateUI();
    }

    public void ClickBtnAddRainbowHole(int amount)
    {
        ResourceType.Powerup_RainbowHole.Manager()?.Add(amount);
        ResourceType.Powerup_RainbowHole.Manager().UI.UpdateUI();
    }

    public void ClearGold()
    {
        UserResourceController.instance.SetGold(0);
        ResourceType.Gold.Manager().UI.UpdateUI();
    }

    public void ClickBtnClose() { _panelTestAccount.SetActive(false); }

    public void ClickBtnWin() { LevelManager.Instance.CompleteLevel(); }

    public void ClickBtnLose() { GameManager.Instance.LoseGame(); }
}