using System.Runtime.InteropServices;

namespace Keystone;

public sealed class Arena<T>(int initialCapacity = 256) where T : unmanaged
{
    [ThreadStatic]
    private static Arena<T>? _shared;
    public static Arena<T> Shared => _shared ??= new Arena<T>();

    public readonly record struct Handle(int Offset, int Length)
    {
        public Handle Trim(int length) =>
            length > Length
                ? throw new ArgumentOutOfRangeException(nameof(length))
                : new Handle(Offset, length);
    }

    private T[] _buffer = new T[initialCapacity];
    private int _cursor = 0;

    public int Count => _cursor;

    private void EnsureCapacity(int needed)
    {
        if (_cursor + needed <= _buffer.Length) return;
        int newSize = Math.Max(_buffer.Length * 2, _cursor + needed);
        Array.Resize(ref _buffer, newSize);
    }

    public void Reset() => _cursor = 0;

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

    public Handle Allocate(int count)
    {
        EnsureCapacity(count);
        int offset = _cursor;
        _cursor += count;
        return new Handle(offset, count);
    }

    public Handle Allocate<S>(int count) where S : unmanaged
    {
        int sSize = Marshal.SizeOf<S>();
        int tSize = Marshal.SizeOf<T>();
        if (sSize > tSize) Align(sSize);
        return Allocate(count * sSize / tSize);
    }

    public void Write(Handle handle, T value) => 
        _buffer[handle.Offset] = value;
    public void Write(Handle handle, ReadOnlySpan<T> values) =>
        values.CopyTo(_buffer.AsSpan(handle.Offset));
    public void Write<S>(Handle handle, ReadOnlySpan<S> values) where S : unmanaged =>
        Write(handle, MemoryMarshal.Cast<S, T>(values));
    public void Write<S>(Handle handle, S value) where S : unmanaged =>
        Write(handle, MemoryMarshal.CreateReadOnlySpan(ref value, 1));

    // Dangling Span risk because of Allocate. Document, eventually.
    public Span<T> GetSpan(Handle handle) =>
        _buffer.AsSpan(handle.Offset, handle.Length);
    public Span<S> GetSpan<S>(Handle handle) where S : unmanaged =>
        MemoryMarshal.Cast<T, S>(GetSpan(handle));

    public ReadOnlySpan<T> Read(Handle handle) => GetSpan(handle);
    public ReadOnlySpan<S> Read<S>(Handle handle) where S : unmanaged => GetSpan<S>(handle);
}