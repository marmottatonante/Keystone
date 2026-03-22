namespace Keystone.Reactivity;

public interface IBindable<T>
{
    void Bind(Func<T> compute, params IObservable[] sources);
    void Unbind();
}