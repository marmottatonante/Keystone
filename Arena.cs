using System.Runtime.InteropServices;

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

    public void Align(int alignment)
    {
        int remainder = _cursor % alignment;
        if (remainder != 0)
        {
            int padding = alignment - remainder;
            EnsureCapacity(padding);
            _cursor += padding;
        }
    }

    public Handle Write(T value)
    {
        EnsureCapacity(1);
        int offset = _cursor;
        _buffer[_cursor++] = value;
        return new Handle(offset, 1);
    }

    public Handle Write(ReadOnlySpan<T> values)
    {
        EnsureCapacity(values.Length);
        int offset = _cursor;
        values.CopyTo(_buffer.AsSpan(_cursor));
        _cursor += values.Length;
        return new Handle(offset, values.Length);
    }

    public Handle Write<S>(S value) where S : unmanaged
    {
        if (Marshal.SizeOf<S>() > Marshal.SizeOf<T>())
            Align(Marshal.SizeOf<S>());
        ReadOnlySpan<S> span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
        return Write(MemoryMarshal.Cast<S, T>(span));
    }

    public Handle Write<S>(ReadOnlySpan<S> values) where S : unmanaged
    {
        if (Marshal.SizeOf<S>() > Marshal.SizeOf<T>())
            Align(Marshal.SizeOf<S>());
        return Write(MemoryMarshal.Cast<S, T>(values));
    }

    public ReadOnlySpan<T> Read(Handle handle) =>
        _buffer.AsSpan(handle.Offset, handle.Length);

    public ReadOnlySpan<S> Read<S>(Handle handle) where S : unmanaged
    {
        ReadOnlySpan<T> span = Read(handle);
        return MemoryMarshal.Cast<T, S>(span);
    }

    public void Reset() => _cursor = 0;

    private void EnsureCapacity(int needed)
    {
        if (_cursor + needed <= _buffer.Length) return;
        int newSize = Math.Max(_buffer.Length * 2, _cursor + needed);
        Array.Resize(ref _buffer, newSize);
    }
}
