namespace Pith.Reactive;

public interface IReadOnlyProperty<T>
{
    T Value { get; }
    event Action? Changing;
    event Action? Changed;
}

public sealed class Property<T>(T initial) : IReadOnlyProperty<T>
{
    private T _value = initial;
    public event Action? Changing;
    public event Action? Changed;

    public T Value
    {
        get => _value;
        set => Set(value);
    }
    
    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        Changing?.Invoke();
        _value = value;
        Changed?.Invoke();
    }

    public void Bind<B>(IReadOnlyProperty<B> source, Func<B, T> transform) => 
        source.Changed += () => Value = transform(source.Value);

    public void Bind(IReadOnlyProperty<T> source) =>
        Bind(source, value => value);

    public IReadOnlyProperty<T> AsReadOnly() => this;
}