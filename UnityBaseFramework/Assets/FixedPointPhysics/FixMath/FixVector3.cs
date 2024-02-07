using System;
using System.Runtime.CompilerServices;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Vector3.
    /// </summary>
    public struct FixVector3 : IEquatable<FixVector3>
    {
        public static readonly FixVector3 Zero = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly FixVector3 One = new FixVector3(Fix64.One, Fix64.One, Fix64.One);

        public static readonly FixVector3 Left = new FixVector3(-Fix64.One, Fix64.Zero, Fix64.Zero);
        public static readonly FixVector3 Right = new FixVector3(Fix64.One, Fix64.Zero, Fix64.Zero);

        public static readonly FixVector3 Up = new FixVector3(Fix64.Zero, Fix64.One, Fix64.Zero);
        public static readonly FixVector3 Down = new FixVector3(Fix64.Zero, -Fix64.One, Fix64.Zero);

        public static readonly FixVector3 Forward = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.One);
        public static readonly FixVector3 Back = new FixVector3(Fix64.Zero, Fix64.Zero, -Fix64.One);

        public Fix64 X;
        public Fix64 Y;
        public Fix64 Z;

        public FixVector3(Fix64 value) : this(value, value, value)
        {
        }

        public FixVector3(FixVector2 value, Fix64 z) : this(value.X, value.Y, z)
        {
        }

        public FixVector3(Fix64 x, Fix64 y, Fix64 z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public FixVector3(FixVector3 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }

        #region Base

        /// <summary>
        /// 向量的模。
        /// </summary>
        public Fix64 Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FixMath.Sqrt(X * X + Y * Y + Z * Z);
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
                return X * X + Y * Y + Z * Z;
            }
        }

        /// <summary>
        /// XZ平面向量的模的平方。
        /// </summary>
        public Fix64 SqrMagnitudeXZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return X * X + Z * Z;
            }
        }

        /// <summary>
        /// 归一化向量。
        /// </summary>
        public FixVector3 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FixVector3.Normalize(this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 Length()
        {
            return FixMath.Sqrt(X * X + Y * Y + Z * Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVector2 ToFixVector2XY()
        {
            return new FixVector2(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVector2 ToFixVector2XZ()
        {
            return new FixVector2(X, Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVector2 ToFixVector2YZ()
        {
            return new FixVector2(Y, Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(X.AsFloat(), Y.AsFloat(), Z.AsFloat());
        }

        #endregion

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FixVector3 left, FixVector3 right)
        {
            if (left.X == right.X && left.Y == right.Y)
            {
                return left.Z == right.Z;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FixVector3 left, FixVector3 right)
        {
            if (left.X == right.X && left.Y == right.Y)
            {
                return left.Z != right.Z;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator -(FixVector3 value)
        {
            return new FixVector3(-value.X, -value.Y, -value.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator +(FixVector3 left, FixVector3 right)
        {
            return new FixVector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator -(FixVector3 left, FixVector3 right)
        {
            return new FixVector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator *(FixVector3 left, FixVector3 right)
        {
            return new FixVector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator *(FixVector3 value, Fix64 scaleFactor)
        {
            return new FixVector3(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator *(Fix64 scaleFactor, FixVector3 value)
        {
            return new FixVector3(scaleFactor * value.X, scaleFactor * value.Y, scaleFactor * value.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator /(FixVector3 left, FixVector3 right)
        {
            return new FixVector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 operator /(FixVector3 value, Fix64 divisor)
        {
            Fix64 factor = Fix64.One / divisor;
            return new FixVector3(value.X * factor, value.Y * factor, value.Z * factor);
        }

        /// <summary>
        /// 取负。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Negate(FixVector3 value)
        {
            return -value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Add(FixVector3 left, FixVector3 right)
        {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Subtract(FixVector3 left, FixVector3 right)
        {
            return left - right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Multiply(FixVector3 left, FixVector3 right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Multiply(FixVector3 value, Fix64 scaleFactor)
        {
            return value * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Multiply(Fix64 scaleFactor, FixVector3 value)
        {
            return scaleFactor * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Divide(FixVector3 left, FixVector3 right)
        {
            return left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Divide(FixVector3 value, Fix64 divisor)
        {
            return value / divisor;
        }

        #endregion

        #region Public Static Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Abs(FixVector3 value)
        {
            return new FixVector3(FixMath.Abs(value.X), FixMath.Abs(value.Y), FixMath.Abs(value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Min(FixVector3 value1, FixVector3 value2)
        {
            return new FixVector3((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y, (value1.Z < value2.Z) ? value1.Z : value2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Max(FixVector3 value1, FixVector3 value2)
        {
            return new FixVector3((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y, (value1.Z > value2.Z) ? value1.Z : value2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Clamp(FixVector3 value, FixVector3 min, FixVector3 max)
        {
            Fix64 x = value.X;
            x = ((x > max.X) ? max.X : x);
            x = ((x < min.X) ? min.X : x);
            Fix64 y = value.Y;
            y = ((y > max.Y) ? max.Y : y);
            y = ((y < min.Y) ? min.Y : y);
            Fix64 z = value.Z;
            y = ((z > max.Z) ? max.Z : y);
            y = ((z < min.Z) ? min.Z : y);
            return new FixVector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Lerp(FixVector3 value1, FixVector3 value2, Fix64 amount)
        {
            return new FixVector3(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount, value1.Z + (value2.Z - value1.Z) * amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Distance(FixVector3 value1, FixVector3 value2)
        {
            Fix64 num1 = value1.X - value2.X;
            Fix64 num2 = value1.Y - value2.Y;
            Fix64 num3 = value1.Z - value2.Z;
            return FixMath.Sqrt(num1 * num1 + num2 * num2 + num3 * num3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 DistanceXZ(FixVector3 value1, FixVector3 value2)
        {
            Fix64 num1 = value1.X - value2.X;
            Fix64 num2 = value1.Z - value2.Z;
            return FixMath.Sqrt(num1 * num1 + num2 * num2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 DistanceSquared(FixVector3 value1, FixVector3 value2)
        {
            Fix64 num1 = value1.X - value2.X;
            Fix64 num2 = value1.Y - value2.Y;
            Fix64 num3 = value1.Z - value2.Z;
            return num1 * num1 + num2 * num2 + num3 * num3;
        }

        /// <summary>
        /// 对每个分量进行平方根运算。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 SquareRoot(FixVector3 value)
        {
            return new FixVector3(FixMath.Sqrt(value.X), FixMath.Sqrt(value.Y), FixMath.Sqrt(value.Z));
        }

        /// <summary>
        /// 归一化（Normalization）操作，将向量缩放为单位长度（长度为1）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Normalize(FixVector3 value)
        {
            Fix64 disSquared = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            Fix64 factor = Fix64.One / FixMath.Sqrt(disSquared);
            return new FixVector3(value.X * factor, value.Y * factor, value.Z * factor);
        }

        /// <summary>
        /// 点乘（Dot Product）运算。
        /// 如果结果为 0，说明两个向量是垂直的（正交的）。
        /// 如果结果大于 0，说明两个向量夹角小于 90°。
        /// 如果结果小于 0，说明两个向量夹角大于 90°。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Dot(FixVector3 vector1, FixVector3 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        /// <summary>
        /// 叉乘（Cross Product）运算。
        /// 结果向量垂直于原始两个向量所在的平面。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Cross(FixVector3 vector1, FixVector3 vector2)
        {
            return new FixVector3(
                vector1.Y * vector2.Z - vector1.Z * vector2.Y,
                vector1.Z * vector2.X - vector1.X * vector2.Z,
                vector1.X * vector2.Y - vector1.Y * vector2.X
            );
        }

        /// <summary>
        /// 在给定法线方向上的反射操作。
        /// 反射公式： Reflect(vector,normal)=vector−2⋅dot⋅normal
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Reflect(FixVector3 vector, FixVector3 normal)
        {
            Fix64 dot = Dot(vector, normal);
            return vector - (Fix64)2f * dot * normal;
        }

        /// <summary>
        /// 计算 vector 在 onNormal 上的投影。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector3 Project(FixVector3 vector, FixVector3 onNormal)
        {
            Fix64 sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < FixMath.Epsilon)
            {
                return FixVector3.Zero;
            }
            else
            {
                Fix64 dot = Dot(vector, onNormal) / sqrMag;
                return new FixVector3(onNormal.X * dot, onNormal.Y * dot, onNormal.Z * dot);
            }
        }

        #endregion

        #region Inherit/Override

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FixVector3 other)
        {
            if (X == other.X && Y == other.Y)
            {
                return Z == other.Z;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is not FixVector3)
            {
                return false;
            }
            return Equals((FixVector3)obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("({0:f2}, {1:f2}, {2:f2})", X.AsFloat(), Y.AsFloat(), Z.AsFloat());
        }

        #endregion
    }
}
