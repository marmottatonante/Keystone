namespace Pith.Reactive;

public sealed class ReadOnlyProperty<T>(Property<T> source) : IProperty<T>
{
    private readonly Property<T> _source = source;
    public T Value => _source.Value;
    public event Action? Changed
    {
        add => _source.Changed += value;
        remove => _source.Changed -= value;
    }
}