using System;
using UnityEngine;

public abstract class EnumPrevNext<T> : MonoBehaviour, IPrevNext<T> where T : Enum
{
    [SerializeField] private T _enum;
    public T Value => _enum;
    
    [SerializeField, Tooltip("Có thể nhảy qua lại giữa giá trị đầu tiên và cuối cùng khi tới giới hạn")] 
    private bool _loop;
    
    protected abstract void OnSetValue(T type);

    private void Awake()
    {
        OnSetValue(_enum);
    }

    public void OnClickPrev()
    {
        Array values = Enum.GetValues(typeof(T));

        int index = Array.IndexOf(values, _enum);
        int prevIndex = index - 1;

        if (prevIndex >= 0)
        {
            _enum = (T)values.GetValue(prevIndex);
            OnSetValue(_enum);
            return;
        }

        if (!_loop) return;
        _enum = (T)values.GetValue(values.Length - 1);
        OnSetValue(_enum);
    }

    public void OnClickNext()
    {
        Array values = Enum.GetValues(typeof(T));

        int index = Array.IndexOf(values, _enum);
        int nextIndex = index + 1;

        if (nextIndex < values.Length)
        {
            _enum = (T)values.GetValue(nextIndex);
            OnSetValue(_enum);
            return;
        }

        if (!_loop) return;
        _enum = (T)values.GetValue(0);
        OnSetValue(_enum);
    }

    public void SetValue(T value)
    {
        _enum = value;
        OnSetValue(value);
    }
}
