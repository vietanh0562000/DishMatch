using PuzzleGames;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteController : Singleton<UISpriteController>
{
    public ListDataSo<Sprite> Avatar => _profileSo.Avatars;
    public ListDataSo<Sprite> Frame => _profileSo.Frames;

    [SerializeField] private ProfileSO _profileSo;
    
    public void SetImageAvatar(int id, Image image)
    {
        image.sprite = Avatar.GetData(id);
    }

    public void SetMyImageAvatar(UserProfile userProfile, Image image)
    {
        if (userProfile.avatar_id < 0 && !string.IsNullOrEmpty(userProfile.avatar_url))
        {
            image.sprite = Avatar.GetData(0);
            LoadTextureUtils.LoadImageFromEncodeFileOrUrl(image, userProfile.avatar_url);
            return;
        }

        image.sprite = Avatar.GetData(userProfile.avatar_id);
    }

    public void SetMyFacebookAvatar(UserProfile userProfile, Image image)
    {
        if (!string.IsNullOrEmpty(userProfile.avatar_url))
        {
            image.sprite = Avatar.GetData(0);
            LoadTextureUtils.LoadImageFromEncodeFileOrUrl(image, userProfile.avatar_url);
            return;
        }

        image.sprite = Avatar.GetData(userProfile.avatar_id);
    }

    public void SetImageAvatar(UserProfile userProfile, Image image)
    {
        if (image.TryGetComponent<LoadTextureCoroutine>(out var p))
        {
            p.StopPreLoading();
        }

        if (userProfile.avatar_id < 0 && !string.IsNullOrEmpty(userProfile.avatar_url))
        {
            image.sprite = Avatar.GetData(0);
            LoadTextureUtils.LoadAvatarFBFromDicOrWebRequest(image, userProfile.avatar_url, image.gameObject);
            return;
        }

        image.sprite = Avatar.GetData(userProfile.avatar_id);
    }

    // public void SetTeamLogo(TeamInfoSimple teamInfo, Image image)
    // {
    //     //image.sprite = Avatar.GetTeamLogo(teamInfo.avatar_id);
    // }

    public Sprite[] TeamLogos => null;
    
    
    public void SetRawImageAvatar(UserProfile userProfile, RawImage rawImage)
    {
        if (rawImage.TryGetComponent<LoadTextureCoroutine>(out var p))
        {
            p.StopPreLoading();
        }
        if (userProfile.avatar_id  < 0 && !string.IsNullOrEmpty(userProfile.avatar_url))
        {
            rawImage.texture = Avatar.GetData(0).texture;
            LoadTextureUtils.LoadRawAvatarFBFromDicOrWebRequest(rawImage, userProfile.avatar_url, rawImage.gameObject);
            return;
        }
        rawImage.texture = Avatar.GetData(userProfile.avatar_id).texture;
    }
}