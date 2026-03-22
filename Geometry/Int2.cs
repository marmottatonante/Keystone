namespace Keystone.Geometry;

public readonly record struct Int2(int X, int Y)
{
    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);

    public static Int2 operator +(Int2 a, Int2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Int2 operator -(Int2 a, Int2 b) => new(a.X - b.X, a.Y - b.Y);
    
    public static Int2 operator *(Int2 a, int s) => new(a.X * s, a.Y * s);

    public static implicit operator Int2((int X, int Y) t) => new(t.X, t.Y);
    public static implicit operator (int, int)(Int2 int2) => new(int2.X, int2.Y);

    public static bool operator >(Int2 a, Int2 b) => a.X > b.X && a.Y > b.Y;
    public static bool operator <(Int2 a, Int2 b) => a.X < b.X && a.Y < b.Y;
}