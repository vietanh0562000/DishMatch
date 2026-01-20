using UnityEngine;

public abstract class IntPrevNext : MonoBehaviour, IPrevNext<int>
{
    [SerializeField] private Vector2Int _range;
    [SerializeField] private int _value;

    [SerializeField, Tooltip("Tốc độ thay đổi giá trị")]
    private float _factor = 1.3f;

    [SerializeField, Tooltip("Có thể nhảy qua lại giữa giá trị đầu tiên và cuối cùng khi tới giới hạn")]
    private bool _loop;

    public int Value => _value;
    private float _delta;
    private bool _increasing;

    protected abstract void OnSetValue(int newValue);

    private void Awake()
    {
        OnSetValue(_value);
    }

    public void OnClickPrev()
    {
        if (_value == _range.x && _loop)
        {
            _value = _range.y;
            _delta = 0f;
            OnSetValue(_value);
            return;
        }

        Calculate(false);
    }

    public void OnClickNext()
    {
        if (_value == _range.y && _loop)
        {
            _value = _range.x;
            _delta = 0f;
            OnSetValue(_value);
            return;
        }

        Calculate(true);
    }

    public void SetValue(int value)
    {
        _value = value;
        _delta = 0;
        OnSetValue(value);
    }

    public void Calculate(bool increasing)
    {
        if (increasing != _increasing || _delta == 0f)
            _delta = 0.5f;
        else
            _delta *= _factor;

        _increasing = increasing;

        if (_delta > 100000000) _delta = 100000000;
        var intDelta = Mathf.CeilToInt(_delta);

        if (increasing)
        {
            _value = _value > _range.y - intDelta ? _range.y : _value + intDelta;
        }
        else
        {
            _value = _value < _range.x + intDelta ? _range.x : _value - intDelta;
        }

        OnSetValue(_value);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _value = Mathf.Clamp(_value, _range.x, _range.y);
        _factor = Mathf.Clamp(_factor, 1, 5);
    }
#endif
}