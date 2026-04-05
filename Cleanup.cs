namespace Keystone;

public sealed class Cleanup(Action onDispose) : IDisposable
{
    private readonly Action _onDispose = onDispose;
    private bool _disposed = false;
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _onDispose();
    }

    public static Cleanup Merge(params Cleanup[] cleanups) =>
        new(() => { foreach (var c in cleanups) c.Dispose(); });
}