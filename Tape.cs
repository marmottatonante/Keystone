using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Keystone;

public sealed class Tape
{
    private readonly ArrayBufferWriter<byte> _buffer = new();

    public void Write<T>(T value) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        var span = _buffer.GetSpan(sizeof(ushort) + size);
        MemoryMarshal.Write(span, (ushort)size);
        MemoryMarshal.Write(span[sizeof(ushort)..], value);
        _buffer.Advance(sizeof(ushort) + size);
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        var span = _buffer.GetSpan(sizeof(ushort) + data.Length);
        MemoryMarshal.Write(span, (ushort)data.Length);
        data.CopyTo(span[sizeof(ushort)..]);
        _buffer.Advance(sizeof(ushort) + data.Length);
    }

    public void Reset() => _buffer.Clear();

    public Enumerator GetEnumerator() => new(_buffer.WrittenSpan);
    public ref struct Enumerator(ReadOnlySpan<byte> span)
    {
        private readonly ReadOnlySpan<byte> _span = span;
        private int _cursor = 0;

        public ReadOnlySpan<byte> Current { get; private set; }

        public bool MoveNext()
        {
            if (_cursor >= _span.Length) return false;
            ushort size = MemoryMarshal.Read<ushort>(_span[_cursor..]);
            _cursor += sizeof(ushort);
            Current = _span.Slice(_cursor, size);
            _cursor += size;
            return true;
        }
    }
}
