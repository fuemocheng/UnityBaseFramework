using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FixMath
{
    /// <summary>
    /// Represents a Q31.32 fixed-point number.
    /// </summary>
    public partial struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>
    {
        private readonly long m_rawValue;

        private const long MAX_VALUE = long.MaxValue;               //9223372036854775807L      二进制：01111111,11111111,11111111,11111111,11111111,11111111,11111111,11111111
        private const long MIN_VALUE = long.MinValue;               //-9223372036854775808L     二进制：10000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000 (补码)
        private const int NUM_BITS = 64;
        private const int FRACTIONAL_PLACES = 32;
        private const long ONE = 1L << FRACTIONAL_PLACES;           //1L << 32 = 2^32 = 4294967296L
        private const long PI_TIMES_2 = 0x6487ED511;                //0x6487ED511 = 26986075409L = Math.PI * 2 * (1L << 32)
        private const long PI = 0x3243F6A88;                        //0x3243F6A88 = 13493037704L = Math.PI * (1L << 32)
        private const long PI_OVER_2 = 0x1921FB544;                 //0x1921FB544 = 6746518852L = Math.PI / 2 * (1L << 32)
        private const long LN2 = 0xB17217F7;                        //0xB17217F7 = 2977044471L = Math.Log(2) * (1L << 32)；ln(2)，以 e 为底 2 的对数，即 e^x=2，x约为 0.6931
        private const long LOG2MAX = 0x1F00000000;
        private const long LOG2MIN = -0x2000000000;

        private const int LUT_SIZE = (int)(PI_OVER_2 >> 15);        //205887 查找表预存数量

        // Precision of this type is 2^-32, that is 2.3283064365386962890625E-10
        // Fix64 的精度为2^-32，即 2.3283064365386962890625E-10
        public static readonly decimal Precision = (decimal)(new Fix64(1L));                //0.00000000023283064365386962890625m;
        public static readonly Fix64 MaxValue = new Fix64(MAX_VALUE);                       //"2147483647.9999999998"    2147483647 = int.MaxValue =  2^32-1
        public static readonly Fix64 MinValue = new Fix64(MIN_VALUE);                       //"-2147483648"             -2147483648 = int.MinValue = -2^32
        public static readonly Fix64 One = new Fix64(ONE);                                  //"1"                       
        public static readonly Fix64 Zero = new Fix64();                                    //"0"
        public static readonly Fix64 Pi = new Fix64(PI);                                    //"3.1415926535"
        public static readonly Fix64 PiOver2 = new Fix64(PI_OVER_2);                        //"1.5707963267"
        public static readonly Fix64 PiTimes2 = new Fix64(PI_TIMES_2);                      //"6.2831853072"
        public static readonly Fix64 PiInv = (Fix64)0.3183098861837906715377675267M;
        public static readonly Fix64 PiOver2Inv = (Fix64)0.6366197723675813430755350535M;

        public static readonly Fix64 EN1 = Fix64.One / (Fix64)10;
        public static readonly Fix64 EN2 = Fix64.One / (Fix64)100;
        public static readonly Fix64 EN3 = Fix64.One / (Fix64)1000;
        public static readonly Fix64 EN4 = Fix64.One / (Fix64)10000;
        public static readonly Fix64 EN5 = Fix64.One / (Fix64)100000;
        public static readonly Fix64 EN6 = Fix64.One / (Fix64)1000000;
        public static readonly Fix64 EN7 = Fix64.One / (Fix64)10000000;
        public static readonly Fix64 EN8 = Fix64.One / (Fix64)100000000;
        public static readonly Fix64 Epsilon = Fix64.EN3;                                   //Epsilon Number 极小的数值。

        private static readonly Fix64 Log2Max = new Fix64(LOG2MAX);                         //"31"
        private static readonly Fix64 Log2Min = new Fix64(LOG2MIN);                         //"-32"
        private static readonly Fix64 Ln2 = new Fix64(LN2);                                 //"0.6931471804"

        private static readonly Fix64 LutInterval = (Fix64)(LUT_SIZE - 1) / PiOver2;        //"131071.0984587427"

        /// <summary>
        /// The underlying integer representation
        /// </summary>
        public long RawValue => m_rawValue;

        /// <summary>
        /// This is the constructor from raw value; it can only be used interally.
        /// </summary>
        /// <param name="rawValue"></param>
        private Fix64(long rawValue)
        {
            m_rawValue = rawValue;
        }

        public Fix64(int value)
        {
            m_rawValue = value * ONE;
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(Fix64 value)
        {
            return
                value.m_rawValue < 0 ? -1 :
                value.m_rawValue > 0 ? 1 :
                0;
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// 处理了最小值在计算后溢出的情况。
        /// </summary>
        public static Fix64 Abs(Fix64 value)
        {
            // 最小的定点数的绝对值，比最大的定点数大1，所以要处理下，避免在绝对值计算中出现溢出的情况。
            if (value.m_rawValue == MIN_VALUE)
            {
                return MaxValue;
            }

            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            // 通过对 value 的原始值进行右移 63 位，得到符号位的掩码（mask），即 mask 的值为 0 或 1，表示 value 的符号。
            // 然后，将 mask 加到 value.m_rawValue 上，再与 mask 异或，得到取绝对值后的结果。
            // 这个过程实现了绝对值的计算，而无需使用条件分支。
            var mask = value.m_rawValue >> 63;
            return new Fix64((value.m_rawValue + mask) ^ mask);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// 未处理最小值在计算后溢出的情况。
        /// </summary>
        public static Fix64 FastAbs(Fix64 value)
        {
            // branchless implementation, see http://www.strchr.com/optimized_abs_function
            var mask = value.m_rawValue >> 63;
            return new Fix64((value.m_rawValue + mask) ^ mask);
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fix64 Floor(Fix64 value)
        {
            // Just zero out the fractional part
            // 直接通过将小数部分截断为零来实现。
            // 这里使用 ulong 类型掩码 0xFFFFFFFF00000000 进行按位与运算，目的是保留 value.m_rawValue 的高 32 位，而将低 32 位清零。
            // 这里的掩码 0xFFFFFFFF00000000 是 ulong 类型，所以要把 value.m_rawValue 转换成 ulong 再进行 & 操作。
            return new Fix64((long)((ulong)value.m_rawValue & 0xFFFFFFFF00000000));
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static Fix64 Ceiling(Fix64 value)
        {
            var hasFractionalPart = (value.m_rawValue & 0x00000000FFFFFFFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// 如果该值刚好是偶数和奇数一半，则返回偶数值。
        /// </summary>
        public static Fix64 Round(Fix64 value)
        {
            var fractionalPart = value.m_rawValue & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);

            // 0x80000000 -> 2^31
            if (fractionalPart < 0x80000000)
            {
                return integralPart;
            }
            if (fractionalPart > 0x80000000)
            {
                return integralPart + One;
            }
            // if number is halfway between two values, round to the nearest even number
            // this is the method used by System.Math.Round().
            return (integralPart.m_rawValue & ONE) == 0
                       ? integralPart
                       : integralPart + One;
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// 进行溢出检查。将x和y相加。执行饱和加法，即在溢出的情况下，根据操作数的符号取到MinValue或MaxValue。
        /// </summary>
        public static Fix64 operator +(Fix64 x, Fix64 y)
        {
            var xl = x.m_rawValue;
            var yl = y.m_rawValue;
            var sum = xl + yl;
            // if signs of operands are equal and signs of sum and x are different
            // 如果 x，y 的符号相同，但是结果 sum 的符号和 x 的符号不同，则说明溢出，则截取到最大值和最小值之间。
            if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0)
            {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }
            return new Fix64(sum);
        }

        /// <summary>
        /// Adds x and y witout performing overflow checking. Should be inlined by the CLR.
        /// 不进行溢出检查。
        /// </summary>
        public static Fix64 FastAdd(Fix64 x, Fix64 y)
        {
            return new Fix64(x.m_rawValue + y.m_rawValue);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// 进行溢出检查。将x减去y。执行饱和减法，即在溢出的情况下，根据操作数的符号四舍五入到MinValue或MaxValue。
        /// </summary>
        public static Fix64 operator -(Fix64 x, Fix64 y)
        {
            var xl = x.m_rawValue;
            var yl = y.m_rawValue;
            var diff = xl - yl;
            // if signs of operands are different and signs of sum and x are different
            // 如果 x，y 的符号不同，并且结果 sum 的符号和 x 的符号不同，则说明溢出，则截取到最大值和最小值之间。
            if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0)
            {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }
            return new Fix64(diff);
        }

        /// <summary>
        /// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
        /// 不进行溢出检查。
        /// </summary>
        public static Fix64 FastSub(Fix64 x, Fix64 y)
        {
            return new Fix64(x.m_rawValue - y.m_rawValue);
        }

        /// <summary>
        /// 辅助函数, 处理加法并更新溢出标志。
        /// </summary>
        static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            var sum = x + y;
            // x + y overflows if sign(x) ^ sign(y) != sign(sum)
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        /// <summary>
        /// Performs multiplication with checking for overflow.
        /// 进行溢出检查，大量计算性能会受影响。
        /// </summary>
        public static Fix64 operator *(Fix64 x, Fix64 y)
        {

            var xl = x.m_rawValue;
            var yl = y.m_rawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);     // 存储 x 的低 32 位；
            var xhi = xl >> FRACTIONAL_PLACES;              // 存储 x 的高 32 位；
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);     // 存储 y 的低 32 位；
            var yhi = yl >> FRACTIONAL_PLACES;              // 存储 y 的高 32 位；

            var lolo = xlo * ylo;                           // 低位低32位乘法结果；
            var lohi = (long)xlo * yhi;                     // 低位高32位乘法结果；
            var hilo = xhi * (long)ylo;                     // 高位低32位乘法结果；
            var hihi = xhi * yhi;                           // 高位高32位乘法结果；

            var loResult = lolo >> FRACTIONAL_PLACES;       // 将低位结果右移 32 位，得到最终低32位的结果；
            var midResult1 = lohi;                          // 低位高32位乘法结果作为中间结果1；
            var midResult2 = hilo;                          // 高位低32位乘法结果作为中间结果2；
            var hiResult = hihi << FRACTIONAL_PLACES;       // 将高位结果左移 32 位，得到最终高32位的结果；

            bool overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // if signs of operands are equal and sign of result is negative,
            // then multiplication overflowed positively
            // the reverse is also true
            // 如果操作数的符号相等，结果的符号为负，则乘法溢出为正，反之亦然
            if (opSignsEqual)
            {
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
            // then this means the result overflowed.
            // 如果hihi的前32位（在结果中未使用）既不是全0也不是全1，则这意味着结果溢出。
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // If signs differ, both operands' magnitudes are greater than 1,
            // and the result is greater than the negative operand, then there was negative overflow.
            // 如果符号不同，两个操作数的大小都大于1，并且结果大于负操作数，则存在负溢出。
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }
                if (sum > negOp && negOp < -ONE && posOp > ONE)
                {
                    return MinValue;
                }
            }

            return new Fix64(sum);
        }

        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow.
        /// 执行乘法而不检查溢出。对于保证不会导致溢出的性能关键代码非常有用。
        /// </summary>
        public static Fix64 FastMul(Fix64 x, Fix64 y)
        {

            var xl = x.m_rawValue;
            var yl = y.m_rawValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            return new Fix64(sum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
            return result;
        }

        /// <summary>
        /// Performs division with checking for overflow.
        /// 执行除法并检查溢出。
        /// </summary>
        public static Fix64 operator /(Fix64 x, Fix64 y)
        {
            var xl = x.m_rawValue;
            var yl = y.m_rawValue;

            if (yl == 0)
            {
                throw new DivideByZeroException();
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);        // remainder 存储被除数的绝对值；
            var divider = (ulong)(yl >= 0 ? yl : -yl);          // divider 存储除数的绝对值；
            var quotient = 0UL;                                 // quotient 存储最终的商；
            var bitPos = NUM_BITS / 2 + 1;                      // bitPos 用于跟踪当前处理的位的位置，初始值为定点数的位数的一半加一；


            // If the divider is divisible by 2^n, take advantage of it.
            // 通过检查除数是否能被 2^n 整除来优化算法：在循环中，如果除数能被 2^n 整除，就右移divider和更新bitPos，以便加快计算过程。
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            // 执行除法的主循环，核心思想是通过二进制的除法方式，逐位计算商，处理余数。
            while (remainder != 0 && bitPos >= 0)
            {
                int shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // Detect overflow
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0)
            {
                result = -result;
            }

            return new Fix64(result);
        }

        /// <summary>
        /// Reliable but slower modulo.
        /// 可靠但较慢的模运算。
        /// </summary>
        public static Fix64 operator %(Fix64 x, Fix64 y)
        {
            return new Fix64(
                x.m_rawValue == MIN_VALUE & y.m_rawValue == -1 ?
                0 :
                x.m_rawValue % y.m_rawValue);
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// 以尽可能快的速度执行模运算；如果 x==MinValue 且 y==-1，则抛出。使用运算符（%）进行更可靠但较慢的模运算。
        /// </summary>
        public static Fix64 FastMod(Fix64 x, Fix64 y)
        {
            return new Fix64(x.m_rawValue % y.m_rawValue);
        }

        public static Fix64 operator -(Fix64 x)
        {
            return x.m_rawValue == MIN_VALUE ? MaxValue : new Fix64(-x.m_rawValue);
        }

        public static bool operator ==(Fix64 x, Fix64 y)
        {
            return x.m_rawValue == y.m_rawValue;
        }

        public static bool operator !=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue != y.m_rawValue;
        }

        public static bool operator >(Fix64 x, Fix64 y)
        {
            return x.m_rawValue > y.m_rawValue;
        }

        public static bool operator <(Fix64 x, Fix64 y)
        {
            return x.m_rawValue < y.m_rawValue;
        }

        public static bool operator >=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue >= y.m_rawValue;
        }

        public static bool operator <=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue <= y.m_rawValue;
        }

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// 计算 2^x 的值，其中 x 是一个 Fix64 类型的参数。提供至少6位小数的精度。
        /// 近似计算以 2 为底的指数函数的方法，通过级数展开来逼近该函数的值。
        /// </summary>
        public static Fix64 Pow2(Fix64 x)
        {
            if (x.m_rawValue == 0)
            {
                return One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.m_rawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == One)
            {
                return neg ? One / (Fix64)2 : (Fix64)2;
            }
            if (x >= Log2Max)
            {
                return neg ? One / MaxValue : MaxValue;
            }
            if (x <= Log2Min)
            {
                return neg ? MaxValue : Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)Floor(x);
            // Take fractional part of exponent
            x = new Fix64(x.m_rawValue & 0x00000000FFFFFFFF);

            var result = One;
            var term = One;
            int i = 1;
            while (term.m_rawValue != 0)
            {
                term = FastMul(FastMul(x, term), Ln2) / (Fix64)i;
                result += term;
                i++;
            }

            result = FromRaw(result.m_rawValue << integerPart);
            if (neg)
            {
                result = One / result;
            }

            return result;
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// log₂(x) ，以 2 为底的对数。提供至少9位小数的精度。
        /// 这个算法采用了迭代的方式，通过一系列近似计算来求解以 2 为底的对数。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Log2(Fix64 x)
        {
            if (x.m_rawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.m_rawValue;
            while (rawX < ONE)
            {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= (ONE << 1))
            {
                rawX >>= 1;
                y += ONE;
            }

            var z = new Fix64(rawX);

            for (int i = 0; i < FRACTIONAL_PLACES; i++)
            {
                z = FastMul(z, z);
                if (z.m_rawValue >= (ONE << 1))
                {
                    z = new Fix64(z.m_rawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return new Fix64(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// ln(x)，以 e 为底的对数。提供至少7位小数的精度。
        /// 这个实现的思路是通过性能更高的近似计算方法，结合以 2 为底的对数和常数 ln(2)，来得到自然对数的近似值。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Ln(Fix64 x)
        {
            // 换底公式：
            // log_a(b) = log_c(b) / log_c(a)，其中 c 是不为 1 的正数。
            // ln(x) = log2(x) * ln(2)
            return FastMul(Log2(x), Ln2);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// b^exp，自定义的次方函数，用于计算给定底数 b 和指数 exp 的幂运算。
        /// 提供大约5位数的准确性。
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero, with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static Fix64 Pow(Fix64 b, Fix64 exp)
        {
            if (b == One)
            {
                return One;
            }
            if (exp.m_rawValue == 0)
            {
                return One;
            }
            if (b.m_rawValue == 0)
            {
                if (exp.m_rawValue < 0)
                {
                    throw new DivideByZeroException();
                }
                return Zero;
            }

            // t = b^exp
            // log2(t) = log2(b^exp)
            // log2(t) = exp*log2(b)
            // t = 2^(exp*log2(b))
            Fix64 log2 = Log2(b);
            return Pow2(exp * log2);
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// 平方根。通过迭代逼近的方式，利用位操作和数学处理，计算了给定参数的平方根。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static Fix64 Sqrt(Fix64 x)
        {
            var xl = x.m_rawValue;
            if (xl < 0)
            {
                // We cannot represent infinities like Single and Double, and Sqrt is
                // mathematically undefined for x < 0. So we just throw an exception.
                throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // second-to-top bit
            var bit = 1UL << (NUM_BITS - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            // The main part is executed twice, in order to avoid
            // using 128 bit values in computations.
            for (var i = 0; i < 2; ++i)
            {
                // First we get the top 48 bits of the answer.
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result = result >> 1;
                    }
                    bit >>= 2;
                }

                if (i == 0)
                {
                    // Then process it again to get the lowest 16 bits.
                    if (num > (1UL << (NUM_BITS / 2)) - 1)
                    {
                        // The remainder 'num' is too large to be shifted left
                        // by 32, so we have to add 1 to result manually and
                        // adjust 'num' accordingly.
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= (NUM_BITS / 2);
                        result <<= (NUM_BITS / 2);
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }
            // Finally, if next bit would have been 1, round the result upwards.
            if (num > result)
            {
                ++result;
            }
            return new Fix64((long)result);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// The relative error is less than 1E-10 for x in [-2PI, 2PI], and less than 1E-7 in the worst case.
        /// It may lose accuracy as the value of x grows.
        /// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
        /// x的正弦值。
        /// 在[-2PI，2PI]中，相对误差小于1E-10，在最坏的情况下小于1E-7。
        /// 随着x值的增长，它可能会失去准确性。
        /// 性能：在 x64 中，比 Math.Sin() 慢 25% 左右，在 x86 中慢 200% 。
        /// </summary>
        public static Fix64 Sin(Fix64 x)
        {
            var clampedL = ClampSinValue(x.m_rawValue, out var flipHorizontal, out var flipVertical);
            var clamped = new Fix64(clampedL);

            // Find the two closest values in the LUT and perform linear interpolation
            // This is what kills the performance of this function on x86 - x64 is fine though
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = FastSub(rawIndex, roundedIndex);

            var nearestValue = new Fix64(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex :
                (int)roundedIndex]);
            var secondNearestValue = new Fix64(SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)roundedIndex - Sign(indexError) :
                (int)roundedIndex + Sign(indexError)]);

            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).m_rawValue;
            var interpolatedValue = nearestValue.m_rawValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;
            return new Fix64(finalValue);
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// x的正弦值的粗略近似值。
        /// 比x86上的Sin()快3倍，比Math.Sin()稍微快一点。对于足够小的x值，其精度被限制在4-5位小数。
        /// </summary>
        public static Fix64 FastSin(Fix64 x)
        {
            var clampedL = ClampSinValue(x.m_rawValue, out bool flipHorizontal, out bool flipVertical);

            // Here we use the fact that the SinLut table has a number of entries
            // equal to (PI_OVER_2 >> 15) to use the angle to index directly into it
            var rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE)
            {
                rawIndex = LUT_SIZE - 1;
            }
            var nearestValue = SinLut[flipHorizontal ?
                SinLut.Length - 1 - (int)rawIndex :
                (int)rawIndex];
            return new Fix64(flipVertical ? -nearestValue : nearestValue);
        }


        static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            var largePI = 7244019458077122842;
            // Obtained from ((Fix64)1686629713.065252369824872831112M).m_rawValue
            // This is (2^29)*PI, where 29 is the largest N such that (2^N)*PI < MaxValue.
            // The idea is that this number contains way more precision than PI_TIMES_2,
            // and (((x % (2^29*PI)) % (2^28*PI)) % ... (2^1*PI) = x % (2 * PI)
            // In practice this gives us an error of about 1,25e-9 in the worst case scenario (Sin(MaxValue))
            // Whereas simply doing x % PI_TIMES_2 is the 2e-3 range.

            var clamped2Pi = angle;
            for (int i = 0; i < 29; ++i)
            {
                clamped2Pi %= (largePI >> i);
            }
            if (angle < 0)
            {
                clamped2Pi += PI_TIMES_2;
            }

            // The LUT contains values for 0 - PiOver2; every other value must be obtained by
            // vertical or horizontal mirroring
            flipVertical = clamped2Pi >= PI;
            // obtain (angle % PI) from (angle % 2PI) - much faster than doing another modulo
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI)
            {
                clampedPi -= PI;
            }
            flipHorizontal = clampedPi >= PI_OVER_2;
            // obtain (angle % PI_OVER_2) from (angle % PI) - much faster than doing another modulo
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2)
            {
                clampedPiOver2 -= PI_OVER_2;
            }
            return clampedPiOver2;
        }

        /// <summary>
        /// Returns the cosine of x.
        /// The relative error is less than 1E-10 for x in [-2PI, 2PI], and less than 1E-7 in the worst case.
        /// x的余弦值。
        /// 在[-2PI，2PI]中，相对误差小于1E-10，在最坏的情况下小于1E-7。
        /// </summary>
        public static Fix64 Cos(Fix64 x)
        {
            var xl = x.m_rawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return Sin(new Fix64(rawAngle));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static Fix64 FastCos(Fix64 x)
        {
            var xl = x.m_rawValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new Fix64(rawAngle));
        }

        /// <summary>
        /// Returns the tangent of x.
        /// x 的正切值。
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// 这个功能没有经过很好的测试。可能不准确。
        /// </remarks>
        public static Fix64 Tan(Fix64 x)
        {
            var clampedPi = x.m_rawValue % PI;
            var flip = false;
            if (clampedPi < 0)
            {
                clampedPi = -clampedPi;
                flip = true;
            }
            if (clampedPi > PI_OVER_2)
            {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = new Fix64(clampedPi);

            // Find the two closest values in the LUT and perform linear interpolation
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = FastSub(rawIndex, roundedIndex);

            var nearestValue = new Fix64(TanLut[(int)roundedIndex]);
            var secondNearestValue = new Fix64(TanLut[(int)roundedIndex + Sign(indexError)]);

            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).m_rawValue;
            var interpolatedValue = nearestValue.m_rawValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;
            return new Fix64(finalValue);
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// This function has at least 7 decimals of accuracy.
        /// x的反余弦值。
        /// 通过Atan和Sqrt计算，精度至少为7位小数。
        /// </summary>
        public static Fix64 Acos(Fix64 x)
        {
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (x.m_rawValue == 0) return PiOver2;

            var result = Atan(Sqrt(One - x * x) / x);
            return x.m_rawValue < 0 ? result + Pi : result;
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// This function has at least 7 decimals of accuracy.
        /// 反正切函数，使用 Euler series 计算。
        /// 通过迭代计算逼近反正切值，精度至少为7位小数。
        /// </summary>
        public static Fix64 Atan(Fix64 z)
        {
            if (z.m_rawValue == 0) return Zero;

            // Force positive values for argument
            // Atan(-z) = -Atan(z).
            var neg = z.m_rawValue < 0;
            if (neg)
            {
                z = -z;
            }

            Fix64 result;
            var two = (Fix64)2;
            var three = (Fix64)3;

            bool invert = z > One;
            if (invert) z = One / z;

            result = One;
            var term = One;

            var zSq = z * z;
            var zSq2 = zSq * two;
            var zSqPlusOne = zSq + One;
            var zSq12 = zSqPlusOne * two;
            var dividend = zSq2;
            var divisor = zSqPlusOne * three;

            for (var i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += zSq2;
                divisor += zSq12;

                if (term.m_rawValue == 0) break;
            }

            result = result * z / zSqPlusOne;

            if (invert)
            {
                result = PiOver2 - result;
            }

            if (neg)
            {
                result = -result;
            }
            return result;
        }

        /// <summary>
        /// 通过 y/z 的比值计算反正切值。
        /// 通过迭代计算逼近反正切值。Atan2 更加灵活，能够处理更广泛的情况，包括分母为零的情况。
        /// </summary>
        public static Fix64 Atan2(Fix64 y, Fix64 x)
        {
            var yl = y.m_rawValue;
            var xl = x.m_rawValue;
            if (xl == 0)
            {
                if (yl > 0)
                {
                    return PiOver2;
                }
                if (yl == 0)
                {
                    return Zero;
                }
                return -PiOver2;
            }
            Fix64 atan;
            var z = y / x;

            // Deal with overflow
            if (One + (Fix64)0.28M * z * z == MaxValue)
            {
                return y < Zero ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One)
            {
                atan = z / (One + (Fix64)0.28M * z * z);
                if (xl < 0)
                {
                    if (yl < 0)
                    {
                        return atan - Pi;
                    }
                    return atan + Pi;
                }
            }
            else
            {
                atan = PiOver2 - z / (z * z + (Fix64)0.28M);
                if (yl < 0)
                {
                    return atan - Pi;
                }
            }
            return atan;
        }

        /// <summary>
        /// 反正弦函数。
        /// </summary>
        public static Fix64 Asin(Fix64 x)
        {
            // sin(x)=cos(2/π−x) => Asin(x)=2/π−Acos(x)
            return FastSub(PiOver2, Acos(x));
        }

        /// <summary>
        /// 反余弦函数。x属于[-1,1]
        /// 只提供4位小数左右精度。
        /// </summary>
        //public static Fix64 Acos(Fix64 x)
        //{
        //    if (x < -One || x > One)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(x));
        //    }
        //    if (x == (Fix64)0)
        //    {
        //        return Fix64.PiOver2;
        //    }

        //    bool flip = false;
        //    if (x < (Fix64)0)
        //    {
        //        x = -x;
        //        flip = true;
        //    }

        //    //Find the two closest values in the LUT and perform linear interpolation
        //    var rawIndex = FastMul(x, (Fix64)(LUT_SIZE - 1));
        //    var roundedIndex = Round(rawIndex);
        //    if (roundedIndex >= (Fix64)LUT_SIZE)
        //    {
        //        roundedIndex = (Fix64)(LUT_SIZE - 1);
        //    }
        //    var nearestValue = new Fix64(AcosLut[(int)roundedIndex]);

        //    var indexError = FastSub(rawIndex, roundedIndex);
        //    var nextIndex = (int)roundedIndex + Sign(indexError);
        //    if (nextIndex >= LUT_SIZE)
        //    {
        //        nextIndex = LUT_SIZE - 1;
        //    }
        //    var secondNearestValue = new Fix64(AcosLut[nextIndex]);

        //    var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue))).m_rawValue;
        //    Fix64 interpolatedValue = new Fix64(nearestValue.m_rawValue - delta);
        //    Fix64 finalValue = flip ? (Fix64.Pi - interpolatedValue) : interpolatedValue;

        //    return finalValue;
        //}

        #region Convert

        public static explicit operator Fix64(long value)
        {
            return new Fix64(value * ONE);
        }
        public static explicit operator long(Fix64 value)
        {
            return value.m_rawValue >> FRACTIONAL_PLACES;
        }
        public static explicit operator Fix64(float value)
        {
            return new Fix64((long)(value * ONE));
        }
        public static explicit operator float(Fix64 value)
        {
            return (float)value.m_rawValue / ONE;
        }
        public static explicit operator Fix64(double value)
        {
            return new Fix64((long)(value * ONE));
        }
        public static explicit operator double(Fix64 value)
        {
            return (double)value.m_rawValue / ONE;
        }
        public static explicit operator Fix64(decimal value)
        {
            return new Fix64((long)(value * ONE));
        }
        public static explicit operator decimal(Fix64 value)
        {
            return (decimal)value.m_rawValue / ONE;
        }

        public static explicit operator Fix64(int value)
        {
            return new Fix64(value * ONE);
        }

        public static explicit operator int(Fix64 value)
        {
            return (int)(value.m_rawValue / ONE);
        }

        public int AsInt()
        {
            return (int)this;
        }

        public long AsLong()
        {
            return (long)this;
        }

        public float AsFloat()
        {
            return (float)this;
        }

        public double AsDouble()
        {
            return (double)this;
        }

        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        public static int ToInt(Fix64 value)
        {
            return (int)value;
        }

        public static float ToFloat(Fix64 value)
        {
            return (float)value;
        }

        public static Fix64 FromInt(int value)
        {
            return (Fix64)value;
        }

        public static Fix64 FromFloat(float value)
        {
            return (Fix64)value;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return obj is Fix64 && ((Fix64)obj).m_rawValue == m_rawValue;
        }

        public override int GetHashCode()
        {
            return m_rawValue.GetHashCode();
        }

        public bool Equals(Fix64 other)
        {
            return m_rawValue == other.m_rawValue;
        }

        public int CompareTo(Fix64 other)
        {
            return m_rawValue.CompareTo(other.m_rawValue);
        }

        public override string ToString()
        {
            // Up to 10 decimal places
            return ((decimal)this).ToString("0.##########");
        }

        public static Fix64 FromRaw(long rawValue)
        {
            return new Fix64(rawValue);
        }

        #region GenerateLut

        public static void GenerateSinLut()
        {
            using (var writer = new StreamWriter("Fix64SinLut.cs"))
            {
                writer.Write(
@"namespace FixMath 
{
    partial struct Fix64 
    {
        public static readonly long[] SinLut = new[] 
        {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var sin = Math.Sin(angle);
                    var rawValue = ((Fix64)sin).m_rawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        public static void GenerateTanLut()
        {
            using (var writer = new StreamWriter("Fix64TanLut.cs"))
            {
                writer.Write(
@"namespace FixMath 
{
    partial struct Fix64 
    {
        public static readonly long[] TanLut = new[] 
        {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = i * Math.PI * 0.5 / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var tan = Math.Tan(angle);
                    if (tan > (double)MaxValue || tan < 0.0)
                    {
                        tan = (double)MaxValue;
                    }
                    var rawValue = (((decimal)tan > (decimal)MaxValue || tan < 0.0) ? MaxValue : (Fix64)tan).m_rawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        public static void GenerateAcosLut()
        {
            using (var writer = new StreamWriter("Fix64AcosLut.cs"))
            {
                writer.Write(
@"namespace FixMath
{
    partial struct Fix64 
    {
        public static readonly long[] AcosLut = new[] 
        {");
                int lineCounter = 0;
                for (int i = 0; i < LUT_SIZE; ++i)
                {
                    var angle = (double)i / (LUT_SIZE - 1);
                    if (lineCounter++ % 8 == 0)
                    {
                        writer.WriteLine();
                        writer.Write("            ");
                    }
                    var acos = Math.Acos(angle);
                    var rawValue = ((Fix64)acos).m_rawValue;
                    writer.Write(string.Format("0x{0:X}L, ", rawValue));
                }
                writer.Write(
@"
        };
    }
}");
            }
        }

        #endregion
    }
}
