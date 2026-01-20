using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Socials.Profile;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_ViewProfile_Edit_Addon : MonoBehaviour
{
    public Button[] btnEdits;
    public RectTransform rectBtnAnchorName;
    public Text refTxtName;

    private void Awake()
    {
        refTxtName.GetComponent<UIContentSizeFitter>().enabled = false;
    }

    public void UpdateUI(UserInfoDetail data)
    {
        //Nếu không có dữ liệu thì xóa văn bản và ẩn nút Edit
        if (data == null)
        {
            refTxtName.text = string.Empty;

            foreach (var btnEdit in btnEdits)
            {
                btnEdit.gameObject.SetActive(false);
            }

            return;
        }

        var sizeFontDefault = 62;
        var txtName = refTxtName;
        var contentSize = txtName.GetComponent<UIContentSizeFitter>();

        //Xử Size của Text
        {
            //Tắt contentSizeFitter để tránh lỗi văn bản bị tràn
            contentSize.enabled = false;

            txtName.rectTransform.sizeDelta = data.code != AccountManager.instance.Code
                ? new Vector2(450, 75)
                : new Vector2(375, 75);
            txtName.fontSize = sizeFontDefault;
            txtName.cachedTextGenerator.Invalidate();
            txtName.cachedTextGenerator.Populate(txtName.text,
                txtName.GetGenerationSettings(txtName.rectTransform.rect.size));

            // Giảm kích thước phông chữ nếu văn bản bị tràn
            while (txtName.cachedTextGenerator.characterCountVisible < txtName.text.Length)
            {
                txtName.fontSize -= 1;
                txtName.cachedTextGenerator.Invalidate();
                txtName.cachedTextGenerator.Populate(txtName.text,
                    txtName.GetGenerationSettings(txtName.rectTransform.rect.size));
            }

            //Bật lại contentSizeFitter
            contentSize.enabled = true;
        }

        //Xử lý Edit Profile và vị trí icon Edit
        {
            var isActive = AccountManager.instance.Code == data.code;

            foreach (var btnEdit in btnEdits)
            {
                btnEdit.gameObject.SetActive(isActive);
                btnEdit.onClick.RemoveAllListeners();
                btnEdit.onClick.AddListener(() =>
                {
                    UIManager.Instance.OpenPopup(ProfileAssetPaths.GetPath(AssetIDs.PROFILE_POPUP));
                    AudioController.PlaySound(SoundKind.UIClickButton);
                });
            }

            rectBtnAnchorName.gameObject.SetActive(false);
            if (isActive)
            {
                StartCoroutine(IESetPositionBtnAnchorName());

                IEnumerator IESetPositionBtnAnchorName()
                {
                    yield return new WaitForEndOfFrame();
                    var posAnchorText = txtName.transform.GetChild(0).transform.position;
                    rectBtnAnchorName.position = posAnchorText;
                    rectBtnAnchorName.gameObject.SetActive(true);
                    rectBtnAnchorName.DOScale(1, 0.125f).From(0.25f);
                }
            }
        }
    }
}