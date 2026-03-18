namespace Pith.Reactive;

public sealed class Property<T>(T initial) : IProperty<T>
{
    private T _value = initial;
    public event Action? Changed;

    public T Value
    {
        get => _value;
        set => Set(value);
    }
    
    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        _value = value;
        Changed?.Invoke();
    }

    public void Bind<B>(IProperty<B> source, Func<B, T> transform) => 
        source.Changed += () => Value = transform(source.Value);

    public void Bind(IProperty<T> source) =>
        Bind(source, value => value);

    public ReadOnlyProperty<T> AsReadOnly() => new(this);
}