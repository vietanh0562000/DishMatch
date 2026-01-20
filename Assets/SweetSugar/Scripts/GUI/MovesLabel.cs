using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Localization;
using TMPro;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Moves / Time label in the game
    /// </summary>
    public class MovesLabel : MonoBehaviour
    {
        // Use this for initialization
        void OnEnable()
        {
            if(MLevelManager.THIS?.mLevelData == null || !MLevelManager.THIS.levelLoaded)
                MLevelManager.OnLevelLoaded += Reset;
            else 
                Reset();
        }

        void OnDisable()
        {
            MLevelManager.OnLevelLoaded -= Reset;
        }


    void Reset()
    {
        if (MLevelManager.THIS != null && MLevelManager.THIS.levelLoaded)
        {
            if (MLevelManager.THIS.mLevelData.limitType == LIMIT.MOVES)
                GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetText(41, GetComponent<TextMeshProUGUI>().text);
            else
                GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetText(77, GetComponent<TextMeshProUGUI>().text);
        }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
