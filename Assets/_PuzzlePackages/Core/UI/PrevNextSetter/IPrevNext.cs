
public interface IPrevNext<T>
{
    T Value { get; }
    void OnClickPrev();
    void OnClickNext();
    void SetValue(T value);
}
