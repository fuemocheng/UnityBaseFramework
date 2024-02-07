using System;
using System.Runtime.CompilerServices;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Vector2.
    /// </summary>
    public struct FixVector2 : IEquatable<FixVector2>
    {
        public static readonly FixVector2 Zero = new FixVector2(Fix64.Zero, Fix64.Zero);
        public static readonly FixVector2 One = new FixVector2(Fix64.One, Fix64.One);

        public static readonly FixVector2 Left = new FixVector2(-Fix64.One, Fix64.Zero);
        public static readonly FixVector2 Right = new FixVector2(Fix64.One, Fix64.Zero);

        public static readonly FixVector2 Up = new FixVector2(Fix64.Zero, Fix64.One);
        public static readonly FixVector2 Down = new FixVector2(Fix64.Zero, -Fix64.One);

        public Fix64 X;
        public Fix64 Y;

        public FixVector2(Fix64 value) : this(value, value)
        {
        }

        public FixVector2(Fix64 x, Fix64 y)
        {
            X = x;
            Y = y;
        }

        public FixVector2(FixVector2 value)
        {
            X = value.X;
            Y = value.Y;
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
                return FixMath.Sqrt(X * X + Y * Y);
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
                return X * X + Y * Y;
            }
        }

        /// <summary>
        /// 归一化向量。
        /// </summary>
        public FixVector2 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return FixVector2.Normalize(this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 Length()
        {
            return FixMath.Sqrt(X * X + Y * Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fix64 LengthSquared()
        {
            return X * X + Y * Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixVector3 ToFixVector3()
        {
            return new FixVector3(X, Y, Fix64.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(X.AsFloat(), Y.AsFloat());
        }

        #endregion

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FixVector2 left, FixVector2 right)
        {
            if (left.X == right.X)
            {
                return left.Y == right.Y;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FixVector2 left, FixVector2 right)
        {
            if (left.X == right.X)
            {
                return left.Y != right.Y;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator -(FixVector2 value)
        {
            return new FixVector2(-value.X, -value.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator +(FixVector2 left, FixVector2 right)
        {
            return new FixVector2(left.X + right.X, left.Y + right.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator -(FixVector2 left, FixVector2 right)
        {
            return new FixVector2(left.X - right.X, left.Y - right.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator *(FixVector2 left, FixVector2 right)
        {
            return new FixVector2(left.X * right.X, left.Y * right.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator *(FixVector2 value, Fix64 scaleFactor)
        {
            return new FixVector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator *(Fix64 scaleFactor, FixVector2 value)
        {
            return new FixVector2(scaleFactor * value.X, scaleFactor * value.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator /(FixVector2 left, FixVector2 right)
        {
            return new FixVector2(left.X / right.X, left.Y / right.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 operator /(FixVector2 value, Fix64 divisor)
        {
            Fix64 factor = Fix64.One / divisor;
            return new FixVector2(value.X * factor, value.Y * factor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Negate(FixVector2 value)
        {
            return -value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Add(FixVector2 left, FixVector2 right)
        {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Subtract(FixVector2 left, FixVector2 right)
        {
            return left - right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Multiply(FixVector2 left, FixVector2 right)
        {
            return left * right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Multiply(FixVector2 value, Fix64 scaleFactor)
        {
            return value * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Multiply(Fix64 scaleFactor, FixVector2 value)
        {
            return scaleFactor * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Divide(FixVector2 left, FixVector2 right)
        {
            return left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Divide(FixVector2 value, Fix64 divisor)
        {
            return value / divisor;
        }

        #endregion

        #region Public Static Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Abs(FixVector2 value)
        {
            return new FixVector2(FixMath.Abs(value.X), FixMath.Abs(value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Min(FixVector2 value1, FixVector2 value2)
        {
            return new FixVector2((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Max(FixVector2 value1, FixVector2 value2)
        {
            return new FixVector2((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Clamp(FixVector2 value, FixVector2 min, FixVector2 max)
        {
            Fix64 x = value.X;
            x = ((x > max.X) ? max.X : x);
            x = ((x < min.X) ? min.X : x);
            Fix64 y = value.Y;
            y = ((y > max.Y) ? max.Y : y);
            y = ((y < min.Y) ? min.Y : y);
            return new FixVector2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Lerp(FixVector2 value1, FixVector2 value2, Fix64 amount)
        {
            return new FixVector2(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Distance(FixVector2 value1, FixVector2 value2)
        {
            Fix64 num1 = value1.X - value2.X;
            Fix64 num2 = value1.Y - value2.Y;
            return FixMath.Sqrt(num1 * num1 + num2 * num2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 DistanceSquared(FixVector2 value1, FixVector2 value2)
        {
            Fix64 num1 = value1.X - value2.X;
            Fix64 num2 = value1.Y - value2.Y;
            return num1 * num1 + num2 * num2;
        }

        /// <summary>
        /// 对每个分量进行平方根运算。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 SquareRoot(FixVector2 value)
        {
            return new FixVector2(FixMath.Sqrt(value.X), FixMath.Sqrt(value.Y));
        }

        /// <summary>
        /// 归一化（Normalization）操作，将向量缩放为单位长度（长度为1）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Normalize(FixVector2 value)
        {
            Fix64 disSquared = value.X * value.X + value.Y * value.Y;
            Fix64 factor = Fix64.One / FixMath.Sqrt(disSquared);
            return new FixVector2(value.X * factor, value.Y * factor);
        }

        /// <summary>
        /// 点乘（Dot Product）运算。
        /// 如果结果为 0，说明两个向量垂直（正交）。
        /// 如果结果大于 0，说明两个向量夹角小于 90°。
        /// 如果结果小于 0，说明两个向量夹角大于 90°。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Dot(FixVector2 value1, FixVector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        /// <summary>
        /// 叉乘（Cross Product）运算。
        /// Cross(v1,v2)=v1.x⋅v2.y−v1.y⋅v2.x
        /// 面积：二维向量的叉乘的绝对值表示由这两个向量构成的平行四边形的面积。方向（正负号）表示了叉乘结果的方向，垂直于所在平面。
        /// 方向：叉乘的正负号表示了两个向量之间的相对方向。如果叉乘结果为正，表示从第一个向量到第二个向量需要逆时针旋转，而如果为负，则需要顺时针旋转。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Cross(FixVector2 value1, FixVector2 value2)
        {
            return value1.X * value2.Y - value1.Y * value2.X;
        }

        /// <summary>
        /// 在给定法线方向上的反射操作。
        /// 反射公式： Reflect(vector,normal)=vector−2⋅dot⋅normal
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Reflect(FixVector2 vector, FixVector2 normal)
        {
            Fix64 dot = Dot(vector, normal);
            return vector - (Fix64)2f * dot * normal;
        }

        /// <summary>
        /// 计算 vector 在 onNormal 上的投影。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixVector2 Project(FixVector2 vector, FixVector2 onNormal)
        {
            Fix64 sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < FixMath.Epsilon)
            {
                return FixVector2.Zero;
            }
            else
            {
                Fix64 dot = Dot(vector, onNormal);
                return new FixVector2(onNormal.X * dot / sqrMag, onNormal.Y * dot / sqrMag);
            }
        }

        #endregion

        #region Inherit/Override

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FixVector2 other)
        {
            if (X == other.X)
            {
                return Y == other.Y;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is not FixVector2)
            {
                return false;
            }
            return Equals((FixVector2)obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("({0:f2}, {1:f2})", X.AsFloat(), Y.AsFloat());
        }

        #endregion
    }
}
