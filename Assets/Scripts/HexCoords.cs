using UnityEngine;

public struct HexCoords
{
    public int q;
    public int r;
    public int s;

    public HexCoords(int q_, int r_)
    {
        q = q_;
        r = r_;
        s = -q_ - r_;
    }

    public HexCoords(int q_, int r_, int s_)
    {
        q = q_;
        r = r_;
        s = s_;
    }

    public static HexCoords operator +(HexCoords a, HexCoords b)
    {
        return new HexCoords(a.q + b.q, a.r + b.r, a.s + b.s);
    }

    public static bool operator ==(HexCoords a, HexCoords b)
    {
        return a.q == b.q && a.r == b.r && a.s == b.s;
    }

    public static bool operator !=(HexCoords a, HexCoords b)
    {
        return !(a == b);
    }

    public static HexCoords operator -(HexCoords a, HexCoords b)
    {
        return new HexCoords(a.q - b.q, a.r - b.r, a.s - b.s);
    }
    public static HexCoords operator *(HexCoords a, int k)
    {
        return new HexCoords(a.q * k, a.r * k, a.s * k);
    }

    public static int Length(HexCoords hex)
    {
        return (Mathf.Abs(hex.q) + Mathf.Abs(hex.r) + Mathf.Abs(hex.s)) / 2;
    }

    public static int Distance(HexCoords a, HexCoords b)
    {
        return Length(a - b);
    }

    public static HexCoords[] directions =
    {
        new HexCoords(1, 0, -1), new HexCoords(1, -1, 0), new HexCoords(0, -1, 1),
        new HexCoords(-1, 0, 1), new HexCoords(-1, 1, 0), new HexCoords(0, 1, -1)
    };

    public static HexCoords Direction(int direction)
    {
        if (0 <= direction && direction < 6)
        {
            return directions[direction];
        }
        throw new System.Exception("Direction value invalid.");
    }

    public static int GetDirection(HexCoords h)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (h == directions[i])
                return i;
        }
        throw new System.Exception("Can not get direction! Check input.");
    }

    public static HexCoords Normalize(HexCoords hex)
    {
        return new HexCoords(
            hex.q / hex.q,
            hex.r / hex.r,
            hex.s / hex.s);
    }

    public static HexCoords RotateHexClockwise(HexCoords center, HexCoords point)
    {
        HexCoords vec = point - center;
        HexCoords rotatedVec = new HexCoords(-vec.r, -vec.s, -vec.q);
        return rotatedVec + center;
    }
    public static HexCoords RotateHexCounterClockwise(HexCoords center, HexCoords point)
    {
        HexCoords vec = point - center;
        HexCoords rotatedVec = new HexCoords(-vec.s, -vec.q, -vec.r);
        return rotatedVec + center;
    }

    public static HexCoords Neighbour(HexCoords hex, int direction)
    {
        return hex + Direction(direction);
    }

    public override string ToString()
    {
        return $"{this.q}, {this.r}, {this.s}";
    }
}

public struct FractionalHex
{
    public float q, r, s;
    public FractionalHex(float q, float r, float s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public FractionalHex(float q, float r) : this()
    {
        this.q = q;
        this.r = r;
    }
}
public struct Orientation
{
    public double f0, f1, f2, f3;
    public double b0, b1, b2, b3;
    public double start_angle; // in multiples of 60°
    public Orientation(double f0_, double f1_, double f2_, double f3_,
                double b0_, double b1_, double b2_, double b3_,
                double start_angle_)
    {
        f0 = f0_;
        f1 = f1_;
        f2 = f2_;
        f3 = f3_;
        b0 = b0_;
        b1 = b1_;
        b2 = b2_;
        b3 = b3_;
        start_angle = start_angle_;
    }

    public static Orientation layoutPointy
        = new Orientation(Mathf.Sqrt(3f), Mathf.Sqrt(3f) / 2f, 0, 3.0 / 2.0,
            Mathf.Sqrt(3f) / 3f, -1.0 / 3.0, 0.0, 2.0 / 3.0,
            0.5);
};

public struct Layout
{
    public Orientation orientation;
    public Vector2 size;
    public Vector2 origin;

    public Layout(Orientation orientation, Vector2 size, Vector2 origin)
    {
        this.orientation = orientation;
        this.size = size;
        this.origin = origin;
    }
};

public static class HexHelpers
{
    public static Vector2 HexToPixel(Layout layout, HexCoords h)
    {
        Orientation M = layout.orientation;
        float x = (float)((M.f0 * h.q + M.f1 * h.r) * layout.size.x);
        float y = (float)((M.f2 * h.q + M.f3 * h.r) * layout.size.y);
        return new Vector2(x + layout.origin.x, y + layout.origin.y);
    }
}