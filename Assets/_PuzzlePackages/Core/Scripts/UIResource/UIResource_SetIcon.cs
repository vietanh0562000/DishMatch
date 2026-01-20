using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResource_SetIcon : MonoBehaviour
{
    public ResourceType type;

    private Image image;

    private void OnEnable()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        // image.sprite = UISpriteController.Instance.resource.GetIcon(type);
    }
}
