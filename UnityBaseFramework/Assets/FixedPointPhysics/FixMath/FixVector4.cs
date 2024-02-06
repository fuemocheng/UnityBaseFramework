using System;
using System.Runtime.CompilerServices;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Vector4.
    /// </summary>
    public struct FixVector4 : IEquatable<FixVector4>
    {
        public static readonly FixVector4 Zero = new FixVector4(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly FixVector4 One = new FixVector4(Fix64.One, Fix64.One, Fix64.One, Fix64.One);
        public static readonly FixVector4 UnitX = new FixVector4(Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly FixVector4 UnitY = new FixVector4(Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero);
        public static readonly FixVector4 UnitZ = new FixVector4(Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero);
        public static readonly FixVector4 UnitW = new FixVector4(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

        public Fix64 X;
        public Fix64 Y;
        public Fix64 Z;
        public Fix64 W;

        public FixVector4(Fix64 value) : this(value, value, value, value)
        {
        }

        public FixVector4(FixVector2 value, Fix64 z, Fix64 w) : this(value.X, value.Y, z, w)
        {
        }

        public FixVector4(FixVector3 value, Fix64 w) : this(value.X, value.Y, value.Z, w)
        {
        }

        public FixVector4(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public FixVector4(FixVector4 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        /// <summary>
        /// 向量的模。
        /// </summary>
        public Fix64 Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FixMath.Sqrt(X * X + Y * Y + Z * Z + W * W);
            }
        }

        /// <summary>
        /// 向量的模的平方。
        /// </summary>
        public Fix64 SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return X * X + Y * Y + Z * Z + W * W;
            }
        }

        /// <summary>
        /// 归一化向量。
        /// </summary>
        public FixVector4 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FixVector4.Normalize(this);
            }
        }

        #region Base

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FixVector4 other)
        {
            if (X == other.X && Y == other.Y && Z == other.Z)
            {
                return W == other.W;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is FixVector4))
            {
                return false;
            }
            return Equals((FixVector4)obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2) ^ (W.GetHashCode() >> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("({0:f2}, {1:f2}, {2:f2}, {3:f2})", X.AsFloat(), Y.AsFloat(), Z.AsFloat(), W.AsFloat());
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityEngine.Vector4 ToVector4()
        {
            return new UnityEngine.Vector4(X.AsFloat(), Y.AsFloat(), Z.AsFloat(), W.AsFloat());
        }
        #endregion


        #region operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FixVector4 left, FixVector4 right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FixVector4 left, FixVector4 right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator -(FixVector4 value)
        {
            return new FixVector4(-value.X, -value.Y, -value.Z, -value.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator +(FixVector4 left, FixVector4 right)
        {
            return new FixVector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator -(FixVector4 left, FixVector4 right)
        {
            return new FixVector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator *(FixVector4 left, FixVector4 right)
        {
            return new FixVector4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator *(FixVector4 value, Fix64 scaleFactor)
        {
            return new FixVector4(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor, value.W * scaleFactor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator *(Fix64 scaleFactor, FixVector4 value)
        {
            return new FixVector4(scaleFactor * value.X, scaleFactor * value.Y, scaleFactor * value.Z, scaleFactor * value.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator /(FixVector4 left, FixVector4 right)
        {
            return new FixVector4(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 operator /(FixVector4 value, Fix64 divisor)
        {
            Fix64 factor = Fix64.One / divisor;
            return new FixVector4(value.X * factor, value.Y * factor, value.Z * factor, value.W * factor);
        }

        /// <summary>
        /// 取负。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Negate(FixVector4 value)
        {
            return -value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Add(FixVector4 left, FixVector4 right)
        {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Subtract(FixVector4 left, FixVector4 right)
        {
            return left - right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Multiply(FixVector4 left, FixVector4 right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Multiply(FixVector4 value, Fix64 scaleFactor)
        {
            return value * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Multiply(Fix64 scaleFactor, FixVector4 value)
        {
            return scaleFactor * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Divide(FixVector4 left, FixVector4 right)
        {
            return left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Divide(FixVector4 value, Fix64 divisor)
        {
            return value / divisor;
        }

        #endregion


        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 Length()
        {
            Fix64 num = X * X + Y * Y + Z * Z + W * W;
            return FixMath.Sqrt(num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Abs(FixVector4 value)
        {
            return new FixVector4(FixMath.Abs(value.X), FixMath.Abs(value.Y), FixMath.Abs(value.Z), FixMath.Abs(value.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Min(FixVector4 value1, FixVector4 value2)
        {
            return new FixVector4((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y, (value1.Z < value2.Z) ? value1.Z : value2.Z, (value1.W < value2.W) ? value1.W : value2.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Max(FixVector4 value1, FixVector4 value2)
        {
            return new FixVector4((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y, (value1.Z > value2.Z) ? value1.Z : value2.Z, (value1.W > value2.W) ? value1.W : value2.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Clamp(FixVector4 value1, FixVector4 min, FixVector4 max)
        {
            Fix64 x = value1.X;
            x = ((x > max.X) ? max.X : x);
            x = ((x < min.X) ? min.X : x);
            Fix64 y = value1.Y;
            y = ((y > max.Y) ? max.Y : y);
            y = ((y < min.Y) ? min.Y : y);
            Fix64 z = value1.Z;
            z = ((z > max.Z) ? max.Z : z);
            z = ((z < min.Z) ? min.Z : z);
            Fix64 w = value1.W;
            w = ((w > max.W) ? max.W : w);
            w = ((w < min.W) ? min.W : w);
            return new FixVector4(x, y, z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Lerp(FixVector4 value1, FixVector4 value2, Fix64 amount)
        {
            return new FixVector4(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount, value1.Z + (value2.Z - value1.Z) * amount, value1.W + (value2.W - value1.W) * amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Distance(FixVector4 value1, FixVector4 value2)
        {
            Fix64 num2 = value1.X - value2.X;
            Fix64 num3 = value1.Y - value2.Y;
            Fix64 num4 = value1.Z - value2.Z;
            Fix64 num5 = value1.W - value2.W;
            Fix64 num6 = num2 * num2 + num3 * num3 + num4 * num4 + num5 * num5;
            return FixMath.Sqrt(num6);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 DistanceSquared(FixVector4 value1, FixVector4 value2)
        {
            Fix64 num = value1.X - value2.X;
            Fix64 num2 = value1.Y - value2.Y;
            Fix64 num3 = value1.Z - value2.Z;
            Fix64 num4 = value1.W - value2.W;
            return num * num + num2 * num2 + num3 * num3 + num4 * num4;
        }

        /// <summary>
        /// 对每个分量进行平方根运算。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 SquareRoot(FixVector4 value)
        {
            return new FixVector4(FixMath.Sqrt(value.X), FixMath.Sqrt(value.Y), FixMath.Sqrt(value.Z), FixMath.Sqrt(value.W));
        }

        /// <summary>
        /// 归一化（Normalization）操作，将向量缩放为单位长度（长度为1）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector4 Normalize(FixVector4 vector)
        {
            Fix64 num2 = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
            Fix64 num3 = Fix64.One / FixMath.Sqrt(num2);
            return new FixVector4(vector.X * num3, vector.Y * num3, vector.Z * num3, vector.W * num3);
        }

        /// <summary>
        /// 点乘（Dot Product）运算。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Dot(FixVector4 vector1, FixVector4 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
        }

        #endregion
    }
}
