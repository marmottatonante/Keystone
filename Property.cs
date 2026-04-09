namespace Keystone;

/// <summary>
/// Represents a read only Property.
/// </summary>
/// <typeparam name="T">Type of the encapsulated value.</typeparam>
public interface IReadOnlyProperty<T> : IWatchable
{
    /// <summary>
    /// Encapsulated gettable value.
    /// </summary>
    T Value { get; }
}

/// <summary>
/// Represents a Property.
/// </summary>
/// <typeparam name="T">Type of the encapsulated value.</typeparam>
public interface IProperty<T> : IReadOnlyProperty<T>, IBindable<T>
{
    /// <summary>
    /// Encapsulated settable value.
    /// </summary>
    new T Value { get; set; }
}

/// <summary>
/// Encapsulates a value that can be get and set, firing events on change.
/// </summary>
/// <typeparam name="T">Type of the encapsulated value.</typeparam>
public sealed partial class Property<T>(T initial) : IProperty<T>
{
    private T _value = initial;
    private bool _isBound = false;

    public event Action? Changing;
    public event Action? Changed;

    public bool IsBound => _isBound;

    public T Value
    {
        get => _value;
        set
        {
            if (_isBound) throw new InvalidOperationException("Property is bound.");
            SetAndFireEvents(value);
        }
    }

    private void SetAndFireEvents(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        Changing?.Invoke();
        _value = value;
        Changed?.Invoke();
    }

    public Cleanup Bind(Func<T> compute, params IWatchable[] sources)
    {
        if (_isBound) throw new InvalidOperationException("Property is already bound.");
        void handler() => SetAndFireEvents(compute());
        foreach (var source in sources) source.Changed += handler;
        _isBound = true;
        handler();

        return new Cleanup(() =>
        {
            foreach (var source in sources) source.Changed -= handler;
            _isBound = false;
        });
    }
}