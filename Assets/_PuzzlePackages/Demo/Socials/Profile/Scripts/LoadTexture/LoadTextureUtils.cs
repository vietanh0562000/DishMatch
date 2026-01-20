using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class LoadTextureUtils
{
    private static Dictionary<string, Sprite> _dicAvatarUrl = new Dictionary<string, Sprite>();
    private static Dictionary<string, Texture2D> _dicRawAvatarUrl = new Dictionary<string, Texture2D>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void RegisterEvent()
    {
        SceneController.instance.onChangeSceneState += (p) =>
        {
            _dicAvatarUrl.Clear();
        };
    }

    private static Sprite myAvatarFB;
    private const string key_my_avatar = "avatar_stored";

    public static void ReleasCacheAvatar()
    {
        myAvatarFB = null;
        PlayerPrefs.DeleteKey(key_my_avatar);
    }
    public static void LoadImageFromEncodeFileOrUrl(Image avatar, string urlAvatar)
    {
        if (myAvatarFB != null)
        {
            avatar.sprite = myAvatarFB;
            return;
        }
        if (string.IsNullOrEmpty(urlAvatar))
        {
            LogUtils.LogError("UrlAvatar is null!");

            return;
        }
        var dataPath = Application.persistentDataPath + "/avatar";
        Action<Texture2D, Sprite> callbackSuccess = (t, s) =>
        {
            myAvatarFB = s;

            if (!PlayerPrefs.HasKey(key_my_avatar))
            {
                SaveTextureToFile(t, dataPath);
                PlayerPrefs.SetInt(key_my_avatar, 1);
            }

        };
        Action callbackFailed = () =>
        {
            LogUtils.LogError("Load avatar FAILED!");
            if (PlayerPrefs.HasKey(key_my_avatar))
            {
                LoadAvatarFromEncodeFile(avatar);
            }
        };

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlAvatar);
        GameController.Instance.StartCoroutine(DownloadAvatarTexture(www, avatar, callbackSuccess, callbackFailed));

    }
    private static void SaveTextureToFile(Texture2D texture, string path)
    {
        System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
    }

    private static void LoadAvatarFromEncodeFile(Image avatar)
    {
        try
        {
            var dataPath = Application.persistentDataPath + "/avatar";
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(dataPath);
            var texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
            texture.LoadImage(bytes);
            myAvatarFB = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            avatar.sprite = myAvatarFB;
        }
        catch
        {

        }
    }

    public static void LoadAvatarFBFromDicOrWebRequest(Image image, string urlAvatar, GameObject target)
    {
        if (string.IsNullOrEmpty(urlAvatar))
        {
            LogUtils.LogError("UrlAvatar is null!!!!");
            return;
        }
        if (_dicAvatarUrl.ContainsKey(urlAvatar))
        {
            image.sprite = _dicAvatarUrl[urlAvatar];
        }
        else
        {
            //image.sprite = blankAvatar;
            Action<Texture2D, Sprite> callback = (t, s) =>
            {
                if (!_dicAvatarUrl.ContainsKey(urlAvatar))
                {
                    _dicAvatarUrl.Add(urlAvatar, s);
                }
            };
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlAvatar);
            if (target == null)
            {
                GameController.Instance.StartCoroutine(DownloadImageFromUrl(www, image, callback));
                return;
            }

            LoadTextureCoroutine coroutine;
            if ((coroutine = target.GetComponent<LoadTextureCoroutine>()) == null) coroutine = target.AddComponent<LoadTextureCoroutine>();
            coroutine.ResetAndRunCoroutine(DownloadImageFromUrl(www, image, callback), www);
        }
    }
    
    public static void LoadRawAvatarFBFromDicOrWebRequest(RawImage rawImage, string urlAvatar, GameObject target)
    {
        if (string.IsNullOrEmpty(urlAvatar))
        {
            LogUtils.LogError("UrlAvatar is null!!!!");
            return;
        }
        if (_dicRawAvatarUrl.ContainsKey(urlAvatar))
        {
            rawImage.texture = _dicRawAvatarUrl[urlAvatar];
        }
        else
        {
            //image.sprite = blankAvatar;
            Action<Texture2D> callback = (t) =>
            {
                if (!_dicRawAvatarUrl.ContainsKey(urlAvatar))
                {
                    _dicRawAvatarUrl.Add(urlAvatar, t);
                }
            };
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlAvatar);
            if (target == null)
            {
                GameController.Instance.StartCoroutine(DownloadRawImageFromUrl(www, rawImage, callback));
                return;
            }

            LoadTextureCoroutine coroutine;
            if ((coroutine = target.GetComponent<LoadTextureCoroutine>()) == null) coroutine = target.AddComponent<LoadTextureCoroutine>();
            coroutine.ResetAndRunCoroutine(DownloadRawImageFromUrl(www, rawImage, callback), www);
        }
    }

    private static IEnumerator DownloadImageFromUrl(UnityWebRequest www, Image avatar, Action<Texture2D, Sprite> callback)
    {
        yield return DownloadAvatarTexture(www, avatar, callback);

    }
    
    private static IEnumerator DownloadRawImageFromUrl(UnityWebRequest www, RawImage avatar, Action<Texture2D> callback)
    {
        yield return DownloadRawAvatarTexture(www, avatar, callback);
    }

    public static IEnumerator DownloadAvatarTexture(UnityWebRequest www, Image avatar, Action<Texture2D, Sprite> actionSuccess = null, System.Action actionFailed = null)
    {
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            LogUtils.LogError(www.error);
            actionFailed?.Invoke();
        }
        else
        {
            if (avatar == null)
            {
                yield break;
            }
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (myTexture != null)
            {
                Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
                avatar.sprite = sprite;
                actionSuccess?.Invoke(myTexture, sprite);
            }
        }

    }
    
    public static IEnumerator DownloadRawAvatarTexture(UnityWebRequest www, RawImage avatar, Action<Texture2D> actionSuccess = null, System.Action actionFailed = null)
    {
        yield return www.SendWebRequest();
        if (www.error != null)
        {
            LogUtils.LogError(www.error);
            actionFailed?.Invoke();
        }
        else
        {
            if (avatar == null) yield break;
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (myTexture != null) 
            {
                avatar.texture = myTexture;
                actionSuccess?.Invoke(myTexture);
            }
        }
    }
}
