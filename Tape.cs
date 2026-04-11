using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Keystone;

using SizePrefix = ushort;
public sealed class Tape
{
    private readonly ArrayBufferWriter<byte> _buffer = new();

    public void Reset() => _buffer.Clear();

    public readonly ref struct Reservation(Span<byte> span)
    {
        public Span<byte> Span { get; } = span[sizeof(SizePrefix)..];
        internal Span<byte> Full { get; } = span;
    }

    public Reservation Reserve(int size)
    {
        var span = _buffer.GetSpan(sizeof(SizePrefix) + size);
        return new Reservation(span);
    }

    public void Commit(Reservation reservation, int size)
    {
        if(size > reservation.Span.Length)
            throw new InvalidOperationException("Size can't be higher than reserved.");
        MemoryMarshal.Write(reservation.Full, (SizePrefix)size);
        _buffer.Advance(sizeof(SizePrefix) + size);
    }

    public void Write<T>(T value) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        var reservation = Reserve(size);
        MemoryMarshal.Write(reservation.Span, value);
        Commit(reservation, size);
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        var reservation = Reserve(data.Length);
        data.CopyTo(reservation.Span);
        Commit(reservation, data.Length);
    }

    public Enumerator GetEnumerator() => new(_buffer.WrittenSpan);
    public ref struct Enumerator(ReadOnlySpan<byte> span)
    {
        private readonly ReadOnlySpan<byte> _span = span;
        private int _cursor = 0;

        public ReadOnlySpan<byte> Current { get; private set; }

        public bool MoveNext()
        {
            if (_cursor >= _span.Length) return false;
            SizePrefix size = MemoryMarshal.Read<SizePrefix>(_span[_cursor..]);
            _cursor += sizeof(SizePrefix);
            Current = _span.Slice(_cursor, size);
            _cursor += size;
            return true;
        }
    }
}
