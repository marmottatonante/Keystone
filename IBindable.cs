namespace Keystone;

/// <summary>
/// Represents an object capable of being bound.
/// </summary>
/// <typeparam name="T">Type of the binded value.</typeparam>
public interface IBindable<T>
{
    /// <summary>
    /// Tells whether the object is currently bound.
    /// </summary>
    bool IsBound { get; }

    /// <summary>
    /// Binds a function to the specified sources.
    /// </summary>
    /// <param name="compute">Function to execute on source changed.</param>
    /// <param name="sources">Sources to watch for changes.</param>
    /// <returns>Cleanup object to unbind.</returns>
    Cleanup Bind(Func<T> compute, params IWatchable[] sources);
}