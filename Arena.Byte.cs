using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Keystone;

public static class ArenaByteExtensions
{
    private static void Align(this Arena<byte> arena, int alignment)
    {
        int remainder = arena.Count % alignment;
        if (remainder != 0)
            arena.Allocate(alignment - remainder);
    }

    public static Arena<byte>.Handle Allocate<S>(this Arena<byte> arena, int count) where S : unmanaged
    {
        int sSize = Unsafe.SizeOf<S>();
        if (sSize > 1) arena.Align(sSize);
        int totalBytes = count * sSize;
        return arena.Allocate(totalBytes);
    }

    public static Arena<byte>.Handle Reallocate<S>(this Arena<byte> arena, Arena<byte>.Handle handle, int newCount) where S : unmanaged
    {
        int sSize = Unsafe.SizeOf<S>();
        int totalBytes = newCount * sSize;
        return arena.Reallocate(handle, totalBytes);
    }

    public static void Write<S>(this Arena<byte> arena, Arena<byte>.Handle handle, ReadOnlySpan<S> values) where S : unmanaged =>
        arena.Write(handle, MemoryMarshal.Cast<S, byte>(values));

    public static void Write<S>(this Arena<byte> arena, Arena<byte>.Handle handle, S value) where S : unmanaged =>
        arena.Write(handle, MemoryMarshal.CreateReadOnlySpan(ref value, 1));

    public static Span<S> GetSpan<S>(this Arena<byte> arena, Arena<byte>.Handle handle) where S : unmanaged =>
        MemoryMarshal.Cast<byte, S>(arena.GetSpan(handle));

    public static ReadOnlySpan<S> Read<S>(this Arena<byte> arena, Arena<byte>.Handle handle) where S : unmanaged =>
        arena.GetSpan<S>(handle);
}