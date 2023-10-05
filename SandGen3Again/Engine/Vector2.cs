using SFML.System;

namespace SFML_Game_Engine
{
    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Magnitude()
        {
            return MathF.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Rotates a point <paramref name="degrees"/> around (0,0)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 point, float degrees)
        {
            float deg = degrees * (MathF.PI / 180); // Convert degrees to radians

            Vector2 newVec = new Vector2(
                (point.x * MathF.Cos(deg)) - (point.y * MathF.Sin(deg)),
                (point.y * MathF.Cos(deg)) + (point.x * MathF.Sin(deg))
                );

            return newVec;
        }

        /// <summary>
        /// Rotates a point <paramref name="degrees"/> around <paramref name="origin"/>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <returns>the rotated point</returns>
        public static Vector2 RotateAroundPoint(Vector2 point, Vector2 origin, float degrees)
        {
            return (Rotate(point - origin, degrees) + origin);
        }

        public static Vector2 Flip(Vector2 vec)
        {
            return new Vector2(vec.y, vec.x);
        }

        /// <summary>
        /// Normalizes a Vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 v)
        {
            float mag = MathF.Sqrt(v.x * v.x + v.y * v.y);
            if (mag == 0 || mag == float.NaN) { return v; }
            return v / mag;
        }

        public static Vector2 Lerp(Vector2 A, Vector2 B, float T)
        {
            //a + (b - a) * x
            return A + (B - A) * T;
        }

        public static float Distance(Vector2 A, Vector2 B)
        {
            return MathF.Sqrt(MathF.Pow(B.x - A.x, 2) + MathF.Pow(B.y - A.y, 2));
        }

        public Vector2 Clamp(float min, float max)
        {
            return new Vector2(MathF.Min(MathF.Max(min, x), max), MathF.Min(MathF.Max(min, y), max));
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return (a.x * b.y) - (a.y * b.x);
        }

        public static Vector2 Floor(Vector2 vec)
        {
            return new Vector2(MathF.Floor(vec.x), MathF.Floor(vec.y));
        }

        public override string ToString()
        {
            return $"X[{x}] Y:[{y}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) { return false; }
            if (obj is Vector2)
            {
                Vector2 ob = (Vector2)obj;
                return x == ob.x && y == ob.y;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

        public static Vector2 operator *(float a, Vector2 b) => new Vector2(a * b.x, a * b.y);

        public static Vector2 operator -(Vector2 a, float b) => new Vector2(a.x - b, a.y - b);

        public static Vector2 operator +(Vector2 a, float b) => new Vector2(a.x + b, a.y + b);

        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);

        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

        // Below is vector conversion hell, i still dont know why we all dont use Vec<> but whatever

        public static implicit operator Vector2f(Vector2 vec) => new Vector2f(vec.x, vec.y);

        public static implicit operator Vector2(Vector2f vec) => new Vector2(vec.X, vec.Y);

        public static explicit operator Vector2i(Vector2 vec) => new Vector2i((int)Math.Floor(vec.x), (int)Math.Floor(vec.y));

        public static explicit operator Vector2(Vector2i vec) => new Vector2(vec.X, vec.Y);

        public static explicit operator Vector2(Vector2u v) => new Vector2(v.X, v.Y);
    }
}
