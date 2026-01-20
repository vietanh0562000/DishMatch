using BasePuzzle.PuzzlePackages.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Resource_FreeLives_Item : MonoBehaviour
{
    public Text txtName;
    public Button btnAdd;

    public void SetItemData(UIPopup_Resource_FreeLives refManager, FreeLiveRowData data)
    {
        txtName.text = data.player_name;

        btnAdd.onClick.RemoveAllListeners();
        btnAdd.onClick.AddListener(() =>
        {
            if (UserResourceController.instance.IsInfiHeart() || UserResourceController.instance.IsMaxHeart())
            {
                UIToastManager.Instance.Show("Lives is full");
            }
            else
            {
                //Show Overlay Spawn
                refManager.rectLoading.gameObject.SetActive(true);
                UserResourceController.instance.ReceiveFreeLive(data.player_code); 
            }
        });
    }
}
