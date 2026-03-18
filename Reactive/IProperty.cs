namespace Pith.Reactive;

public interface IProperty<T>
{
    T Value { get; }
    event Action? Changed;
}