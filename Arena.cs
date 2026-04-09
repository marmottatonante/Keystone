namespace Keystone;

public sealed class Arena<T>(int initialCapacity = 256) where T : unmanaged
{
    [ThreadStatic]
    private static Arena<T>? _shared;
    public static Arena<T> Shared => _shared ??= new Arena<T>();

    public readonly record struct Handle(int Offset, int Length);

    private T[] _buffer = new T[initialCapacity];
    private int _cursor = 0;

    public int Count => _cursor;

    private void Resize(int needed)
    {
        if (_cursor + needed <= _buffer.Length) return;
        int newSize = Math.Max(_buffer.Length * 2, _cursor + needed);
        Array.Resize(ref _buffer, newSize);
    }

    public void Reset() => _cursor = 0;

    public Handle Allocate(int count)
    {
        Resize(count);
        int offset = _cursor;
        _cursor += count;
        return new Handle(offset, count);
    }

    public Handle Reallocate(Handle handle, int newCount)
    {
        if (handle.Offset + handle.Length != _cursor)
            throw new InvalidOperationException("Can only reallocate the most recently allocated block.");
        _cursor = handle.Offset;
        return Allocate(newCount);
    }

    public void Write(Handle handle, T value) => 
        _buffer[handle.Offset] = value;
    public void Write(Handle handle, ReadOnlySpan<T> values) =>
        values.CopyTo(_buffer.AsSpan(handle.Offset));

    // Dangling Span risk because of Allocate. Document, eventually.
    public Span<T> GetSpan(Handle handle) =>
        _buffer.AsSpan(handle.Offset, handle.Length);

    public ReadOnlySpan<T> Read(Handle handle) => GetSpan(handle);
}