using System;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Matrix3x3.
    /// 按照 列主序（Column-Major Order）存储。
    /// 在列主序中，矩阵的元素按列顺序依次存储，即矩阵的第一列元素依次存储在内存中的连续位置，接着是第二列元素，以此类推。
    /// Unity中的矩阵也是按照 列主序 存储。
    /// 列主序、行主序，只是是存储形式不同，在数学上都是指同一个矩阵。行主序，形式和数学上一致，可以直接拿来做计算。列主序，需要转换一下，再拿来做计算。
    /// </summary>
    public struct FixMatrix3x3 : IEquatable<FixMatrix3x3>
    {
        public static readonly FixMatrix3x3 ZeroMatrix = new FixMatrix3x3(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly FixMatrix3x3 Identity = new FixMatrix3x3(Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

        public Fix64 M11;       // 第一行第1个元素。
        public Fix64 M12;       // 第一行第2个元素。
        public Fix64 M13;       // 第一行第3个元素。
        public Fix64 M21;       // 第二行第1个元素。
        public Fix64 M22;       // 第二行第2个元素。
        public Fix64 M23;       // 第二行第3个元素。
        public Fix64 M31;       // 第三行第1个元素。
        public Fix64 M32;       // 第三行第2个元素。
        public Fix64 M33;       // 第三行第3个元素。

        public FixMatrix3x3(Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m31, Fix64 m32, Fix64 m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        #region Base

        /// <summary>
        /// Gets or sets the forward vector of the matrix.
        /// </summary>
        public FixVector3 Forward
        {
            get
            {
                FixVector3 result = default;
                result.X = -M31;
                result.Y = -M32;
                result.Z = -M33;
                return result;
            }
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the backward vector of the matrix.
        /// </summary>
        public FixVector3 Backward
        {
            get
            {
                FixVector3 result = default;
                result.X = M31;
                result.Y = M32;
                result.Z = M33;
                return result;
            }
            set
            {
                M31 = value.X;
                M32 = value.Y;
                M33 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the up vector of the matrix.
        /// </summary>
        public FixVector3 Up
        {
            get
            {
                FixVector3 result = default;
                result.X = M21;
                result.Y = M22;
                result.Z = M23;
                return result;
            }
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the down vector of the matrix.
        /// </summary>
        public FixVector3 Down
        {
            get
            {
                FixVector3 result = default;
                result.X = -M21;
                result.Y = -M22;
                result.Z = -M23;
                return result;
            }
            set
            {
                M21 = -value.X;
                M22 = -value.Y;
                M23 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the left vector of the matrix.
        /// </summary>
        public FixVector3 Left
        {
            get
            {
                FixVector3 result = default;
                result.X = -M11;
                result.Y = -M12;
                result.Z = -M13;
                return result;
            }
            set
            {
                M11 = -value.X;
                M12 = -value.Y;
                M13 = -value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the right vector of the matrix.
        /// </summary>
        public FixVector3 Right
        {
            get
            {
                FixVector3 result = default;
                result.X = M11;
                result.Y = M12;
                result.Z = M13;
                return result;
            }
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        /// <summary>
        /// 计算矩阵行列式（Determinant）。
        /// 行列式告诉我们矩阵的一些特性，这些特性对解线性方程组很有用，也可以帮我们找逆矩阵，并且在微积分及其他领域都很有用。
        /// </summary>
        public Fix64 GetDeterminant()
        {
            Fix64 m1 = M11;
            Fix64 m2 = M12;
            Fix64 m3 = M13;
            Fix64 m4 = M21;
            Fix64 m5 = M22;
            Fix64 m6 = M23;
            Fix64 m7 = M31;
            Fix64 m8 = M32;
            Fix64 m9 = M33;
            Fix64 num1 = m5 * m9 - m6 * m8;
            Fix64 num2 = m4 * m9 - m6 * m7;
            Fix64 num3 = m4 * m8 - m5 * m7;
            return m1 * num1 - m2 * num2 + m3 * num3;
        }

        #endregion

        #region Operators

        public static bool operator ==(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            if (left.M11 == right.M11 && left.M22 == right.M22 && left.M33 == right.M33 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M23 == right.M23 && left.M31 == right.M31)
            {
                return left.M32 == right.M32;
            }
            return false;
        }

        public static bool operator !=(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            if (left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M31 == right.M31 && left.M32 == right.M32)
            {
                return left.M33 != right.M33;
            }
            return true;
        }

        public static FixMatrix3x3 operator -(FixMatrix3x3 value)
        {
            FixMatrix3x3 result = default;
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            return result;
        }

        public static FixMatrix3x3 operator +(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            FixMatrix3x3 result = default;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
            return result;
        }

        public static FixMatrix3x3 operator -(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            FixMatrix3x3 result = default;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
            return result;
        }

        public static FixMatrix3x3 operator *(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            FixMatrix3x3 result = default;
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;
            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;
            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;
            return result;
        }

        public static FixMatrix3x3 operator *(FixMatrix3x3 value, Fix64 factor)
        {
            FixMatrix3x3 result = default;
            result.M11 = value.M11 * factor;
            result.M12 = value.M12 * factor;
            result.M13 = value.M13 * factor;
            result.M21 = value.M21 * factor;
            result.M22 = value.M22 * factor;
            result.M23 = value.M23 * factor;
            result.M31 = value.M31 * factor;
            result.M32 = value.M32 * factor;
            result.M33 = value.M33 * factor;
            return result;
        }

        /// <summary>
        /// 取负。
        /// </summary>
        public static FixMatrix3x3 Negate(FixMatrix3x3 value)
        {
            return -value;
        }

        public static FixMatrix3x3 Add(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            return left + right;
        }

        public static FixMatrix3x3 Subtract(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            return left - right;
        }

        public static FixMatrix3x3 Multiply(FixMatrix3x3 left, FixMatrix3x3 right)
        {
            return left * right;
        }

        public static FixMatrix3x3 Multiply(FixMatrix3x3 value, Fix64 factor)
        {
            return value * factor;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（scale）生成一个3x3的缩放矩阵。
        /// 该方法通过将缩放因子分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示等比例缩放的矩阵。
        /// <summary>
        public static FixMatrix3x3 CreateScale(Fix64 scale)
        {
            FixMatrix3x3 result = default;
            result.M11 = scale;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = scale;
            result.M23 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = scale;
            return result;
        }

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（scales）生成一个3x3的缩放矩阵。
        /// 该方法通过将缩放因子的各个分量分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示等比例缩放的矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateScale(FixVector3 scales)
        {
            FixMatrix3x3 result = default;
            result.M11 = scales.X;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = scales.Y;
            result.M23 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = scales.Z;
            return result;
        }

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（xScale, yScale, zScale）生成一个3x3的缩放矩阵。
        /// 该方法通过将各个缩放因子分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示缩放的矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateScale(Fix64 xScale, Fix64 yScale, Fix64 zScale)
        {
            FixMatrix3x3 result = default;
            result.M11 = xScale;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = yScale;
            result.M23 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = zScale;
            return result;
        }

        /// <summary>
        /// 绕X轴旋转的3x3旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕X轴旋转的矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateRotationX(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix3x3 result = default;
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = num;
            result.M23 = num2;
            result.M31 = Fix64.Zero;
            result.M32 = -num2;
            result.M33 = num;
            return result;
        }

        /// <summary>
        /// 绕Y轴旋转的3x3旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕Y轴旋转的矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateRotationY(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix3x3 result = default;
            result.M11 = num;
            result.M12 = Fix64.Zero;
            result.M13 = -num2;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M31 = num2;
            result.M32 = Fix64.Zero;
            result.M33 = num;
            return result;
        }

        /// <summary>
        /// 绕Z轴旋转的3x3旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕Z轴旋转的矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateRotationZ(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix3x3 result = default;
            result.M11 = num;
            result.M12 = num2;
            result.M13 = Fix64.Zero;
            result.M21 = -num2;
            result.M22 = num;
            result.M23 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 根据轴和角度创建旋转矩阵。
        /// 该方法使用给定的轴（Vector3 axis）和角度（angle），通过轴角（Axis-Angle）表示法生成一个3x3的旋转矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateFromAxisAngle(FixVector3 axis, Fix64 angle)
        {
            Fix64 x = axis.X;
            Fix64 y = axis.Y;
            Fix64 z = axis.Z;
            Fix64 num = FixMath.Sin(angle);
            Fix64 num2 = FixMath.Cos(angle);
            Fix64 num3 = x * x;
            Fix64 num4 = y * y;
            Fix64 num5 = z * z;
            Fix64 num6 = x * y;
            Fix64 num7 = x * z;
            Fix64 num8 = y * z;
            FixMatrix3x3 result = default;
            result.M11 = num3 + num2 * (Fix64.One - num3);
            result.M12 = num6 - num2 * num6 + num * z;
            result.M13 = num7 - num2 * num7 - num * y;
            result.M21 = num6 - num2 * num6 - num * z;
            result.M22 = num4 + num2 * (Fix64.One - num4);
            result.M23 = num8 - num2 * num8 + num * x;
            result.M31 = num7 - num2 * num7 + num * y;
            result.M32 = num8 - num2 * num8 - num * x;
            result.M33 = num5 + num2 * (Fix64.One - num5);
            return result;
        }

        /// <summary>
        /// 根据四元数（Quaternion）创建旋转矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateFromQuaternion(FixQuaternion quaternion)
        {
            Fix64 num = quaternion.X * quaternion.X;
            Fix64 num2 = quaternion.Y * quaternion.Y;
            Fix64 num3 = quaternion.Z * quaternion.Z;
            Fix64 num4 = quaternion.X * quaternion.Y;
            Fix64 num5 = quaternion.Z * quaternion.W;
            Fix64 num6 = quaternion.Z * quaternion.X;
            Fix64 num7 = quaternion.Y * quaternion.W;
            Fix64 num8 = quaternion.Y * quaternion.Z;
            Fix64 num9 = quaternion.X * quaternion.W;
            FixMatrix3x3 result = default;
            result.M11 = Fix64.One - (Fix64)2 * (num2 + num3);
            result.M12 = (Fix64)2 * (num4 + num5);
            result.M13 = (Fix64)2 * (num6 - num7);
            result.M21 = (Fix64)2 * (num4 - num5);
            result.M22 = Fix64.One - (Fix64)2 * (num3 + num);
            result.M23 = (Fix64)2 * (num8 + num9);
            result.M31 = (Fix64)2 * (num6 + num7);
            result.M32 = (Fix64)2 * (num8 - num9);
            result.M33 = Fix64.One - (Fix64)2 * (num2 + num);
            return result;
        }

        /// <summary>
        /// 创建旋转矩阵。
        /// 该方法接受欧拉角（Yaw、Pitch、Roll）作为输入，然后通过创建对应的四元数（Quaternion）来构建旋转矩阵。
        /// </summary>
        public static FixMatrix3x3 CreateFromYawPitchRoll(Fix64 yaw, Fix64 pitch, Fix64 roll)
        {
            FixQuaternion quaternion = FixQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return CreateFromQuaternion(quaternion);
        }

        /// <summary>
        /// 矩阵求逆。
        /// A 的逆矩阵是 A⁻¹ ,仅当 A×A⁻¹=A⁻¹×A = I(单位矩阵)
        /// </summary>
        public static bool Invert(FixMatrix3x3 matrix, out FixMatrix3x3 result)
        {
            Fix64 m1 = matrix.M11;
            Fix64 m2 = matrix.M12;
            Fix64 m3 = matrix.M13;
            Fix64 m4 = matrix.M21;
            Fix64 m5 = matrix.M22;
            Fix64 m6 = matrix.M23;
            Fix64 m7 = matrix.M31;
            Fix64 m8 = matrix.M32;
            Fix64 m9 = matrix.M33;

            Fix64 num1 = m5 * m9 - m6 * m8;
            Fix64 num2 = m4 * m9 - m6 * m7;
            Fix64 num3 = m4 * m8 - m5 * m7;

            // 矩阵的行列式；
            Fix64 determinant = m1 * num1 + m2 * num2 + m3 * num3;
            // 如果行列式的绝对值小于一个极小的浮点数（FixMath.Epsilon），则说明矩阵不可逆；
            if (FixMath.Abs(determinant) < FixMath.Epsilon)
            {
                result = new FixMatrix3x3(Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN);
                return false;
            }

            result = default;

            // 每一项对应的行列式；
            Fix64 num4 = m2 * m9 - m3 * m8;
            Fix64 num5 = m1 * m9 - m3 * m7;
            Fix64 num6 = m1 * m8 - m2 * m7;
            Fix64 num7 = m2 * m6 - m3 * m5;
            Fix64 num8 = m1 * m6 - m4 * m4;
            Fix64 num9 = m1 * m5 - m2 * m4;
            // 1/行列式；
            Fix64 determinantInverse = Fix64.One / determinant;

            // 余子式矩阵 -> 代数余子式矩阵 -> 伴随（转置）-> 乘以 1/行列式；
            result.M11 = num1 * determinantInverse;
            result.M12 = -num4 * determinantInverse;
            result.M13 = num7 * determinantInverse;
            result.M21 = -num2 * determinantInverse;
            result.M22 = num5 * determinantInverse;
            result.M23 = -num8 * determinantInverse;
            result.M31 = num3 * determinantInverse;
            result.M32 = -num6 * determinantInverse;
            result.M33 = num9 * determinantInverse;
            return true;
        }

        /// <summary>
        /// 转置矩阵。
        /// 通过交换矩阵的行和列得到的新矩阵。
        /// </summary>
        public static FixMatrix3x3 Transpose(FixMatrix3x3 matrix)
        {
            FixMatrix3x3 result = default;
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            return result;
        }

        /// <summary>
        /// 线性插值（Lerp）。
        /// 这种插值操作在图形学和动画领域经常用于平滑过渡和动画效果的计算。
        /// </summary>
        public static FixMatrix3x3 Lerp(FixMatrix3x3 matrix1, FixMatrix3x3 matrix2, Fix64 amount)
        {
            FixMatrix3x3 result = default;
            result.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
            result.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
            result.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
            result.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
            result.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
            result.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
            result.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
            result.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
            result.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
            return result;
        }

        #endregion

        #region Inherit/Override

        public bool Equals(FixMatrix3x3 other)
        {
            if (M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M12 == other.M12 && M13 == other.M13 && M21 == other.M21 && M23 == other.M23 && M31 == other.M31)
            {
                return M32 == other.M32;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixMatrix3x3)
            {
                return Equals((FixMatrix3x3)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int column1HashCode = M11.GetHashCode() ^ (M21.GetHashCode() << 2) ^ (M31.GetHashCode() >> 2);
            int column2HashCode = M12.GetHashCode() ^ (M22.GetHashCode() << 2) ^ (M32.GetHashCode() >> 2);
            int column3HashCode = M13.GetHashCode() ^ (M23.GetHashCode() << 2) ^ (M33.GetHashCode() >> 2);
            return column1HashCode.GetHashCode() ^ (column2HashCode.GetHashCode() << 2) ^ (column3HashCode.GetHashCode() >> 2);
        }

        public override string ToString()
        {
            return string.Format("[ ({0:f2}, {1:f2}, {2:f2}) ({3:f2}, {4:f2}, {5:f2}) ({6:f2}, {7:f2}, {8:f2}) ]",
                M11.AsFloat(), M12.AsFloat(), M13.AsFloat(), M21.AsFloat(), M22.AsFloat(), M23.AsFloat(), M31.AsFloat(), M32.AsFloat(), M33.AsFloat());
        }

        #endregion
    }
}
