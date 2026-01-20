using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UITapButton : UISelectableExtension
{
    [Sirenix.OdinInspector.PropertyOrder(-100)]
    public bool IsPlaySoundPress = true;

    [Sirenix.OdinInspector.PropertyOrder(-100)]
    public float ratioOffsetX = 0.95f;

    [Sirenix.OdinInspector.PropertyOrder(-100)]
    public float ratioOffsetY = 0.95f;

    [Sirenix.OdinInspector.PropertyOrder(-100)]
    [Sirenix.OdinInspector.Button]
    private void Scale()
    {
        transform.localScale = new Vector3(ratioOffsetX, ratioOffsetY, 1);
    }

    [Sirenix.OdinInspector.PropertyOrder(-100)]
    [Sirenix.OdinInspector.Button]
    private void Restore()
    {
        transform.localScale = Vector3.one;
    }

    private Button button;

    private Tween _tween;
    private void Awake()
    {
        button = GetComponent<Button>();

        OnButtonPress.AddListener(x =>
        {
            if (button != null && button.interactable)
            {
                if (IsPlaySoundPress)
                {
                    AudioController.PlaySound(SoundKind.UIClickButton);
                }
                if (_tween != null && _tween.IsActive())
                {
                    _tween.Complete();
                }
                var scale = transform.localScale;
                _tween = transform.DOScale(new Vector3(scale.x * ratioOffsetX, scale.y * ratioOffsetY, 1), 0.1f).From(scale).SetUpdate(true);
            }
        });

        OnButtonRelease.AddListener(x =>
        {
            if (button != null && button.interactable)
            {
                if (_tween != null && _tween.IsActive())
                {
                    _tween.Complete();
                }
                var scale = transform.localScale;
                _tween = transform.DOScale(new Vector3(scale.x / ratioOffsetX, scale.y / ratioOffsetY, 1), 0.1f).From(scale).SetUpdate(true);
            } 
        });
    }

    private void OnEnable()
    {
        transform.DOKill(true);
        //transform.localScale = Vector3.one;
    }

    

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
