using System.Runtime.CompilerServices;

namespace FixMath
{
    /// <summary>
    /// Contains common math operations.
    /// </summary>
    public class FixMath
    {
        /// <summary>
        /// Represents a smaller number to handle accuracy issues.
        /// Fix64.One / (Fix64)1000，0.001 三位小数精度。
        /// </summary>
        public static Fix64 Epsilon = Fix64.Epsilon;

        /// <summary>
        /// Pi。
        /// </summary>
        public static Fix64 Pi = Fix64.Pi;

        /// <summary>
        /// Pi/2。
        /// </summary>
        public static Fix64 PiOver2 = Fix64.PiOver2;

        /// <summary>
        /// Pi/180。角度转弧度公式：rad=deg*Pi/180。
        /// </summary>
        public static Fix64 Deg2Rad = Fix64.Deg2Rad;

        /// <summary>
        /// 180/Pi。弧度转角度公式：deg=rad*180/Pi。
        /// </summary>
        public static Fix64 Rad2Deg = Fix64.Rad2Deg;


        #region Base

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(Fix64 value)
        {
            return Fix64.Sign(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Abs(Fix64 value)
        {
            return Fix64.Abs(value);
        }

        /// <summary>
        /// 向下取整。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Floor(Fix64 value)
        {
            return Fix64.Floor(value);
        }

        /// <summary>
        /// 向上取整。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Ceiling(Fix64 value)
        {
            return Fix64.Ceiling(value);
        }

        /// <summary>
        /// 四舍五入。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Round(Fix64 value)
        {
            return Fix64.Round(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Max(Fix64 value1, Fix64 value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Min(Fix64 value1, Fix64 value2)
        {
            return (value1 < value2) ? value1 : value2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
        {
            value = value < min ? min : value;
            value = value > max ? max : value;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Clamp01(Fix64 value)
        {
            value = value < Fix64.Zero ? Fix64.Zero : value;
            value = value > Fix64.One ? Fix64.One : value;
            return value;
        }

        #endregion


        #region 常见的数学运算

        /// <summary>
        /// 幂运算，2^x。提供至少6位小数的精度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Pow2(Fix64 x)
        {
            return Fix64.Pow2(x);
        }

        /// <summary>
        /// log₂(x) ，以 2 为底的对数。提供至少9位小数的精度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Log2(Fix64 x)
        {
            return Fix64.Log2(x);
        }

        /// <summary>
        /// ln(x)，以 e 为底的对数。提供至少7位小数的精度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Ln(Fix64 x)
        {
            return Fix64.Ln(x);
        }

        /// <summary>
        /// 幂运算，b^exp。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Pow(Fix64 b, Fix64 exp)
        {
            return Fix64.Pow(b, exp);
        }

        /// <summary>
        /// Square root. √x.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Sqrt(Fix64 x)
        {
            return Fix64.Sqrt(x);
        }

        #endregion


        #region 三角函数

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Sin(Fix64 value)
        {
            return Fix64.Sin(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Cos(Fix64 value)
        {
            return Fix64.Cos(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Tan(Fix64 value)
        {
            return Fix64.Tan(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Asin(Fix64 value)
        {
            return Fix64.Asin(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Acos(Fix64 value)
        {
            return Fix64.Acos(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Atan(Fix64 value)
        {
            return Fix64.Atan(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Atan2(Fix64 y, Fix64 x)
        {
            return Fix64.Atan2(y, x);
        }

        #endregion


        #region Geometric Operation

        /// <summary>
        /// 两个固定点之间距离。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Distance(Fix64 value1, Fix64 value2)
        {
            return FixMath.Abs(value1 - value2);
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fix64 Lerp(Fix64 value1, Fix64 value2, Fix64 amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        #endregion
    }
}
