using System;
using System.Collections;
using PuzzleGames;
using TMPro;
using UnityEngine;

public class ScreenShotMapController : MonoBehaviour
{
    private int resWidth = 240;
    private int resHeight = 360;

    private Camera camcamcam;

    private LevelLoader levelLoader;

    private void SetMapSize(int x, int y)
    {
        var v = camcamcam.transform.localPosition;
        v.x = (x - 6) / 2f;
        if (Screen.safeArea.y == 0)
        {
            v.y = (y - 6) * 0.4f + 1.6f;
            if (GetAspectRatio() == "9:22")
            {
                v.z = (6 - x) * 2;
                v.z = Mathf.Min((10 - y) * 1.25f + 2, v.z);
            }
            else if (GetAspectRatio() == "9:21")
            {
                v.z = (6 - x) * 2 + 1;
                v.z = Mathf.Min((10 - y) * 1.25f + 2, v.z);
            }
            else if (GetAspectRatio() == "9:20")
            {
                v.z = (6 - x) * 2 + 2;
                v.z = Mathf.Min((10 - y) * 1.25f + 2, v.z);
            }
            else if (GetAspectRatio() == "9:18")
            {
                v.z = (6 - x) * 2 + 3;
                v.z = Mathf.Min((10 - y) * 1.25f + 1.5f, v.z);
            }
            else
            {
                //9:16
                v.z = (9 - x) * 1.5f;
                v.z = Mathf.Min((10 - y) * 1.5f, v.z);
            }
        }
        else
        {
            v.y = (y - 3) / 2f + 0.62f;
            if (GetAspectRatio() == "9:22")
            {
                v.z = (6 - x) * 2;
                v.z = Mathf.Min((10 - y) * 1.25f + 2, v.z);
            }
            else if (GetAspectRatio() == "9:21")
            {
                v.z = (6 - x) * 2 + 1;
                v.z = Mathf.Min((10 - y) * 1.25f + 1.5f, v.z);
            }
            else if (GetAspectRatio() == "9:20")
            {
                v.z = (6 - x) * 2 + 2;
                v.z = Mathf.Min((10 - y) * 1.25f + 1, v.z);
            }
            else if (GetAspectRatio() == "9:18")
            {
                v.z = (6 - x) * 2 + 3;
                v.z = Mathf.Min((10 - y) * 1.25f, v.z);
            }
            else
            {
                //9:16
                v.z = (9 - x) * 1.5f;
                v.z = Mathf.Min((10 - y) * 1.5f - 1f, v.z);
            }
        }

        camcamcam.transform.localPosition = v;
    }

    private string GetAspectRatio()
    {
        var r = camcamcam.aspect;
        var ratio = Mathf.FloorToInt(r * 100);
        if (ratio >= 80)
        {
            return "4:5";
        }

        if (ratio >= 75)
        {
            return "3:4";
        }

        if (ratio >= 67)
        {
            return "2:3";
        }

        if (ratio >= 62)
        {
            return "10:16";
        }

        if (ratio >= 56)
        {
            return "9:16";
        }

        if (ratio >= 50)
        {
            return "9:18";
        }

        if (ratio >= 44)
        {
            return "9:20";
        }

        if (ratio >= 42)
        {
            return "9:21";
        }

        if (ratio >= 40)
        {
            return "9:22";
        }

        return string.Empty;
    }

    public byte[] ScreenShot(string levelTxt)
    {
        try
        {
            if (camcamcam == null)
                camcamcam = GetComponentInChildren<Camera>();
            if (levelLoader == null)
                levelLoader = GetComponentInChildren<LevelLoader>();

            if (camcamcam == null || levelLoader == null)
            {
                return null;
            }

            levelLoader.CreateMapFromTxt(levelTxt);
            var a = ScreenShot();
            gameObject.SetActive(false);
            Destroy(gameObject);
            return a;
        }
        catch
        {
            Debug.Log("ERR AT SCREEN SHOT (SPAWN MAP)");
            return null;
        }
    }


    private byte[] ScreenShot()
    {
        try
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camcamcam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

            camcamcam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camcamcam.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);

            Texture2D croppedTexture = new Texture2D(160, 200);
            Color[] pixels = screenShot.GetPixels(20, 50, 160, 200);

            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            byte[] bytes = screenShot.EncodeToJPG();
            System.IO.File.WriteAllBytes("Assets/AAA.png", bytes);
            return bytes;
        }
        catch
        {
            Debug.LogError("ERR AT SCREEN SHOT");
            return null;
        }
    }
}