namespace Keystone.Reactivity;

public interface IObservable
{
    event Action? Changing;
    event Action? Changed;
}