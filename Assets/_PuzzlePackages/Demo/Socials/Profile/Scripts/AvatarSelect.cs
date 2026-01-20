using UnityEngine;
using UnityEngine.UI;

public class AvatarSelect : MonoBehaviour
{
    [SerializeField] private Image _ava;
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _rectSelected;

    private int c_ID;

    public void Setup(int id)
    {
        //Cache
        c_ID = id;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(()=>
        {
            UIPopup_Profile.SelectIdAvatar(id);
            AudioController.PlaySound(SoundKind.UIClickButton);
        });
    }

    public void SetAvatar(int id)
    {
        UISpriteController.Instance.SetImageAvatar(id, _ava);
    }

    public void SetAvatar(UserProfile data)
    {
        if (!string.IsNullOrEmpty(data.avatar_url))
        {
            UISpriteController.Instance.SetMyFacebookAvatar(data, _ava);
            return;
        }

        //Nếu không có url thì tự tắt lựa chọn này đi
        gameObject.SetActive(false);
    }

    public void SetSelected(int idSelected)
    {
        _rectSelected.gameObject.SetActive(c_ID == idSelected);
    }
}
