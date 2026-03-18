namespace Pith.Geometry;

public readonly record struct Rect(Int2 Position, Int2 Size)
{
    public int Left => Position.X;
    public int Top => Position.Y;
    public int Right => Position.X + Size.X;
    public int Bottom => Position.Y + Size.Y;

    public Int2 Max => Position + Size;
    public static Rect FromMinMax(Int2 min, Int2 max) => new(min, max - min);
    public static Rect Empty => default;
    public bool IsEmpty => Size.X == 0 && Size.Y == 0;

    public bool Contains(Int2 point) =>
        point.X >= Left && point.X < Right &&
        point.Y >= Top  && point.Y < Bottom;
    
    public bool Contains(Rect other) =>
        other.Left >= Left && other.Right <= Right &&
        other.Top >= Top && other.Bottom <= Bottom;

    public Rect? Intersect(Rect other)
    {
        var min = new Int2(Math.Max(Left, other.Left), Math.Max(Top, other.Top));
        var max = new Int2(Math.Min(Right, other.Right), Math.Min(Bottom, other.Bottom));
        return min.X < max.X && min.Y < max.Y ? FromMinMax(min, max) : null;
    }

    public Rect Deflate(int amount) =>
        Deflate(amount, amount);

    public Rect Deflate(int horizontal, int vertical) =>
        new(Position + new Int2(horizontal, vertical), 
            Size - new Int2(horizontal * 2, vertical * 2));

    public Rect Center(Int2 size)
    {
        var offset = new Int2((Size.X - size.X) / 2, (Size.Y - size.Y) / 2);
        return new Rect(Position + offset, size);
    }

    public (Rect top, Rect bottom) SplitHorizontal(int y) => (
        FromMinMax(Position, new Int2(Right, Top + y)),
        FromMinMax(new Int2(Left, Top + y), Max)
    );

    public (Rect left, Rect right) SplitVertical(int x) => (
        FromMinMax(Position, new Int2(Left + x, Bottom)),
        FromMinMax(new Int2(Left + x, Top), Max)
    );
}