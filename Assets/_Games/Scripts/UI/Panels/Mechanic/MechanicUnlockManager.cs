namespace PuzzleGames
{
    using BasePuzzle.PuzzlePackages;
    using BasePuzzle.PuzzlePackages.Core;
    using UnityEngine;

    public class MechanicUnlockManager : MonoBehaviour
    {
        [SerializeField] private MechanicSO _mechanicSo;

        private void Start()
        {
            var isFirstLevel = LevelManager.Instance.currentLevelToLog <= 1;
            
#if ACCOUNT_TEST
            if (_mechanicSo.IsLevelUnlockMechanic(LevelDataController.instance.Level, out var i) && !isFirstLevel)
            {
                AdsManager.Instance.HideBanner();
                WindowManager.Instance.OpenWindow<MechanicUnlockPanel>(onLoaded: panel =>
                {
                    panel.SetUpWithMechanicID(i);
                });
            }
#else
            if (_mechanicSo.IsLevelUnlockMechanic(LevelDataController.instance.Level, out var idNewMechanic) &&
                LevelDataController.instance.IsFirstTryLevel && !isFirstLevel)
            {
                WindowManager.Instance.OpenWindow<MechanicUnlockPanel>(onLoaded: panel =>
                {
                    panel.SetUpWithMechanicID(idNewMechanic);
                });
            }
#endif
        }
    }
}