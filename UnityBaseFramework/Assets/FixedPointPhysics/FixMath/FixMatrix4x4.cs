using System;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Matrix4x4.
    /// 按照 列主序（Column-Major Order）存储。
    /// 在列主序中，矩阵的元素按列顺序依次存储，即矩阵的第一列元素依次存储在内存中的连续位置，接着是第二列元素，以此类推。参考 System.Numerics.Matrix4x4 。
    /// Unity中的矩阵也是按照 列主序 存储。
    /// 列主序、行主序，只是是存储形式不同，在数学上都是指同一个矩阵。行主序，形式和数学上一致，可以直接拿来做计算。列主序，需要转换一下，再拿来做计算。
    /// </summary>
    public struct FixMatrix4x4 : IEquatable<FixMatrix4x4>
    {
        public static readonly FixMatrix4x4 ZeroMatrix = new FixMatrix4x4(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly FixMatrix4x4 Identity = new FixMatrix4x4(Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

        public Fix64 M11;       // 第一行第1个元素。
        public Fix64 M12;       // 第一行第2个元素。
        public Fix64 M13;       // 第一行第3个元素。
        public Fix64 M14;       // 第一行第4个元素。
        public Fix64 M21;       // 第二行第1个元素。
        public Fix64 M22;       // 第二行第2个元素。
        public Fix64 M23;       // 第二行第3个元素。
        public Fix64 M24;       // 第二行第4个元素。
        public Fix64 M31;       // 第三行第1个元素。
        public Fix64 M32;       // 第三行第2个元素。
        public Fix64 M33;       // 第三行第3个元素。
        public Fix64 M34;       // 第三行第4个元素。
        public Fix64 M41;       // 第四行第1个元素。
        public Fix64 M42;       // 第四行第2个元素。
        public Fix64 M43;       // 第四行第3个元素。
        public Fix64 M44;       // 第四行第4个元素。

        public FixMatrix4x4(Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m14, Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m24, Fix64 m31, Fix64 m32, Fix64 m33, Fix64 m34, Fix64 m41, Fix64 m42, Fix64 m43, Fix64 m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        #region Base

        /// <summary>
        /// 计算矩阵行列式（Determinant）。
        /// 行列式告诉我们矩阵的一些特性，这些特性对解线性方程组很有用，也可以帮我们找逆矩阵，并且在微积分及其他领域都很有用。
        /// </summary>
        public Fix64 GetDeterminant()
        {
            Fix64 m = M11;
            Fix64 m2 = M12;
            Fix64 m3 = M13;
            Fix64 m4 = M14;
            Fix64 m5 = M21;
            Fix64 m6 = M22;
            Fix64 m7 = M23;
            Fix64 m8 = M24;
            Fix64 m9 = M31;
            Fix64 m10 = M32;
            Fix64 m11 = M33;
            Fix64 m12 = M34;
            Fix64 m13 = M41;
            Fix64 m14 = M42;
            Fix64 m15 = M43;
            Fix64 m16 = M44;
            Fix64 num = m11 * m16 - m12 * m15;
            Fix64 num2 = m10 * m16 - m12 * m14;
            Fix64 num3 = m10 * m15 - m11 * m14;
            Fix64 num4 = m9 * m16 - m12 * m13;
            Fix64 num5 = m9 * m15 - m11 * m13;
            Fix64 num6 = m9 * m14 - m10 * m13;
            return m * (m6 * num - m7 * num2 + m8 * num3) - m2 * (m5 * num - m7 * num4 + m8 * num5) + m3 * (m5 * num2 - m6 * num4 + m8 * num6) - m4 * (m5 * num3 - m6 * num5 + m7 * num6);
        }

        #endregion

        #region Operators

        public static bool operator ==(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            if (left.M11 == right.M11 && left.M22 == right.M22 && left.M33 == right.M33 && left.M44 == right.M44 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42)
            {
                return left.M43 == right.M43;
            }

            return false;
        }

        public static bool operator !=(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            if (left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43)
            {
                return left.M44 != right.M44;
            }

            return true;
        }

        public static FixMatrix4x4 operator -(FixMatrix4x4 value)
        {
            FixMatrix4x4 result = default;
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
            return result;
        }

        public static FixMatrix4x4 operator +(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            FixMatrix4x4 result = default;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M14 = left.M14 + right.M14;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M24 = left.M24 + right.M24;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
            result.M34 = left.M34 + right.M34;
            result.M41 = left.M41 + right.M41;
            result.M42 = left.M42 + right.M42;
            result.M43 = left.M43 + right.M43;
            result.M44 = left.M44 + right.M44;
            return result;
        }

        public static FixMatrix4x4 operator -(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            FixMatrix4x4 result = default;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M14 = left.M14 - right.M14;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M24 = left.M24 - right.M24;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
            result.M34 = left.M34 - right.M34;
            result.M41 = left.M41 - right.M41;
            result.M42 = left.M42 - right.M42;
            result.M43 = left.M43 - right.M43;
            result.M44 = left.M44 - right.M44;
            return result;
        }

        public static FixMatrix4x4 operator *(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            FixMatrix4x4 result = default;
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;
            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;
            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;
            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
            return result;
        }

        public static FixMatrix4x4 operator *(FixMatrix4x4 value, Fix64 factor)
        {
            FixMatrix4x4 result = default;
            result.M11 = value.M11 * factor;
            result.M12 = value.M12 * factor;
            result.M13 = value.M13 * factor;
            result.M14 = value.M14 * factor;
            result.M21 = value.M21 * factor;
            result.M22 = value.M22 * factor;
            result.M23 = value.M23 * factor;
            result.M24 = value.M24 * factor;
            result.M31 = value.M31 * factor;
            result.M32 = value.M32 * factor;
            result.M33 = value.M33 * factor;
            result.M34 = value.M34 * factor;
            result.M41 = value.M41 * factor;
            result.M42 = value.M42 * factor;
            result.M43 = value.M43 * factor;
            result.M44 = value.M44 * factor;
            return result;
        }

        /// <summary>
        /// 取负。
        /// </summary>
        public static FixMatrix4x4 Negate(FixMatrix4x4 value)
        {
            return -value;
        }

        public static FixMatrix4x4 Add(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            return left + right;
        }

        public static FixMatrix4x4 Subtract(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            return left - right;
        }

        public static FixMatrix4x4 Multiply(FixMatrix4x4 left, FixMatrix4x4 right)
        {
            return left * right;
        }

        public static FixMatrix4x4 Multiply(FixMatrix4x4 value, Fix64 factor)
        {
            return value * factor;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 平移矩阵。
        /// 根据传入的位置参数 (position) 生成一个4x4的平移矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateTranslation(FixVector3 position)
        {
            FixMatrix4x4 result = default;
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            result.M34 = Fix64.Zero;
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 平移矩阵。
        ///  根据传入的位置参数（xPosition, yPosition, zPosition）生成一个4x4的平移矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateTranslation(Fix64 xPosition, Fix64 yPosition, Fix64 zPosition)
        {
            FixMatrix4x4 result = default;
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            result.M34 = Fix64.Zero;
            result.M41 = xPosition;
            result.M42 = yPosition;
            result.M43 = zPosition;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（scale）生成一个4x4的缩放矩阵。
        /// 该方法通过将缩放因子分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示等比例缩放的矩阵。
        /// <summary>
        public static FixMatrix4x4 CreateScale(Fix64 scale)
        {
            FixMatrix4x4 result = default;
            result.M11 = scale;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = scale;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = scale;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（scales）生成一个4x4的缩放矩阵。
        /// 该方法通过将缩放因子的各个分量分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示等比例缩放的矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateScale(FixVector3 scales)
        {
            FixMatrix4x4 result = default;
            result.M11 = scales.X;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = scales.Y;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = scales.Z;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 缩放矩阵。根据传入的缩放因子（xScale, yScale, zScale）生成一个4x4的缩放矩阵。
        /// 该方法通过将各个缩放因子分别赋值给矩阵的对角线元素（M11、M22、M33），并将其他元素设置为单位矩阵的形式，得到了一个表示缩放的矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateScale(Fix64 xScale, Fix64 yScale, Fix64 zScale)
        {
            FixMatrix4x4 result = default;
            result.M11 = xScale;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = yScale;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = zScale;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 绕X轴旋转的4x4旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕X轴旋转的矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateRotationX(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix4x4 result = default;
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = num;
            result.M23 = num2;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = -num2;
            result.M33 = num;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 绕Y轴旋转的4x4旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕Y轴旋转的矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateRotationY(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix4x4 result = default;
            result.M11 = num;
            result.M12 = Fix64.Zero;
            result.M13 = -num2;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = num2;
            result.M32 = Fix64.Zero;
            result.M33 = num;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 绕Z轴旋转的4x4旋转矩阵。
        /// 该方法通过欧拉角表示法，使用给定的弧度值（radians）创建一个绕Z轴旋转的矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateRotationZ(Fix64 radians)
        {
            Fix64 num = FixMath.Cos(radians);
            Fix64 num2 = FixMath.Sin(radians);
            FixMatrix4x4 result = default;
            result.M11 = num;
            result.M12 = num2;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = -num2;
            result.M22 = num;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 根据轴和角度创建旋转矩阵。
        /// 该方法使用给定的轴（Vector3 axis）和角度（angle），通过轴角（Axis-Angle）表示法生成一个4x4的旋转矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateFromAxisAngle(FixVector3 axis, Fix64 angle)
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
            FixMatrix4x4 result = default;
            result.M11 = num3 + num2 * (Fix64.One - num3);
            result.M12 = num6 - num2 * num6 + num * z;
            result.M13 = num7 - num2 * num7 - num * y;
            result.M14 = Fix64.Zero;
            result.M21 = num6 - num2 * num6 - num * z;
            result.M22 = num4 + num2 * (Fix64.One - num4);
            result.M23 = num8 - num2 * num8 + num * x;
            result.M24 = Fix64.Zero;
            result.M31 = num7 - num2 * num7 + num * y;
            result.M32 = num8 - num2 * num8 - num * x;
            result.M33 = num5 + num2 * (Fix64.One - num5);
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 创建观察矩阵（LookAt矩阵）。
        /// 观察矩阵用于定义相机在场景中的位置和方向，以及相机的上方向。
        /// </summary>
        /// <param name="cameraPosition">相机的位置。</param>
        /// <param name="cameraTarget">相机的目标位置，相机视线所指向的点。</param>
        /// <param name="cameraUpVector">相机的上方向向量。</param>
        /// <returns></returns>
        public static FixMatrix4x4 CreateLookAt(FixVector3 cameraPosition, FixVector3 cameraTarget, FixVector3 cameraUpVector)
        {
            FixVector3 vector = FixVector3.Normalize(cameraPosition - cameraTarget);
            FixVector3 vector2 = FixVector3.Normalize(FixVector3.Cross(cameraUpVector, vector));
            FixVector3 vector3 = FixVector3.Cross(vector, vector2);
            FixMatrix4x4 result = default;
            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = Fix64.Zero;
            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = Fix64.Zero;
            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = Fix64.Zero;
            result.M41 = -FixVector3.Dot(vector2, cameraPosition);
            result.M42 = -FixVector3.Dot(vector3, cameraPosition);
            result.M43 = -FixVector3.Dot(vector, cameraPosition);
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 根据四元数（Quaternion）创建旋转矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateFromQuaternion(FixQuaternion quaternion)
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
            FixMatrix4x4 result = default;
            result.M11 = Fix64.One - (Fix64)2 * (num2 + num3);
            result.M12 = (Fix64)2 * (num4 + num5);
            result.M13 = (Fix64)2 * (num6 - num7);
            result.M14 = Fix64.Zero;
            result.M21 = (Fix64)2 * (num4 - num5);
            result.M22 = Fix64.One - (Fix64)2 * (num3 + num);
            result.M23 = (Fix64)2 * (num8 + num9);
            result.M24 = Fix64.Zero;
            result.M31 = (Fix64)2 * (num6 + num7);
            result.M32 = (Fix64)2 * (num8 - num9);
            result.M33 = Fix64.One - (Fix64)2 * (num2 + num);
            result.M34 = Fix64.Zero;
            result.M41 = Fix64.Zero;
            result.M42 = Fix64.Zero;
            result.M43 = Fix64.Zero;
            result.M44 = Fix64.One;
            return result;
        }

        /// <summary>
        /// 创建旋转矩阵。
        /// 该方法接受欧拉角（Yaw、Pitch、Roll）作为输入，然后通过创建对应的四元数（Quaternion）来构建旋转矩阵。
        /// </summary>
        public static FixMatrix4x4 CreateFromYawPitchRoll(Fix64 yaw, Fix64 pitch, Fix64 roll)
        {
            FixQuaternion quaternion = FixQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return CreateFromQuaternion(quaternion);
        }

        /// <summary>
        /// 矩阵求逆。
        /// A 的逆矩阵是 A⁻¹ ,仅当 A×A⁻¹=A⁻¹×A = I(单位矩阵)
        /// </summary>
        public static bool Invert(FixMatrix4x4 matrix, out FixMatrix4x4 result)
        {
            Fix64 m = matrix.M11;
            Fix64 m2 = matrix.M12;
            Fix64 m3 = matrix.M13;
            Fix64 m4 = matrix.M14;
            Fix64 m5 = matrix.M21;
            Fix64 m6 = matrix.M22;
            Fix64 m7 = matrix.M23;
            Fix64 m8 = matrix.M24;
            Fix64 m9 = matrix.M31;
            Fix64 m10 = matrix.M32;
            Fix64 m11 = matrix.M33;
            Fix64 m12 = matrix.M34;
            Fix64 m13 = matrix.M41;
            Fix64 m14 = matrix.M42;
            Fix64 m15 = matrix.M43;
            Fix64 m16 = matrix.M44;
            Fix64 num = m11 * m16 - m12 * m15;
            Fix64 num2 = m10 * m16 - m12 * m14;
            Fix64 num3 = m10 * m15 - m11 * m14;
            Fix64 num4 = m9 * m16 - m12 * m13;
            Fix64 num5 = m9 * m15 - m11 * m13;
            Fix64 num6 = m9 * m14 - m10 * m13;
            Fix64 num7 = m6 * num - m7 * num2 + m8 * num3;
            Fix64 num8 = -(m5 * num - m7 * num4 + m8 * num5);
            Fix64 num9 = m5 * num2 - m6 * num4 + m8 * num6;
            Fix64 num10 = -(m5 * num3 - m6 * num5 + m7 * num6);
            Fix64 num11 = m * num7 + m2 * num8 + m3 * num9 + m4 * num10;    //矩阵的行列式
            // 如果行列式的绝对值小于一个极小的浮点数（FixMath.Epsilon），则说明矩阵不可逆。
            if (FixMath.Abs(num11) < FixMath.Epsilon)
            {
                result = new FixMatrix4x4(Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN, Fix64.NaN);
                return false;
            }

            result = default;

            Fix64 num12 = Fix64.One / num11;
            result.M11 = num7 * num12;
            result.M21 = num8 * num12;
            result.M31 = num9 * num12;
            result.M41 = num10 * num12;
            result.M12 = (-(m2 * num - m3 * num2 + m4 * num3)) * num12;
            result.M22 = (m * num - m3 * num4 + m4 * num5) * num12;
            result.M32 = (-(m * num2 - m2 * num4 + m4 * num6)) * num12;
            result.M42 = (m * num3 - m2 * num5 + m3 * num6) * num12;
            Fix64 num13 = m7 * m16 - m8 * m15;
            Fix64 num14 = m6 * m16 - m8 * m14;
            Fix64 num15 = m6 * m15 - m7 * m14;
            Fix64 num16 = m5 * m16 - m8 * m13;
            Fix64 num17 = m5 * m15 - m7 * m13;
            Fix64 num18 = m5 * m14 - m6 * m13;
            result.M13 = (m2 * num13 - m3 * num14 + m4 * num15) * num12;
            result.M23 = (-(m * num13 - m3 * num16 + m4 * num17)) * num12;
            result.M33 = (m * num14 - m2 * num16 + m4 * num18) * num12;
            result.M43 = (-(m * num15 - m2 * num17 + m3 * num18)) * num12;
            Fix64 num19 = m7 * m12 - m8 * m11;
            Fix64 num20 = m6 * m12 - m8 * m10;
            Fix64 num21 = m6 * m11 - m7 * m10;
            Fix64 num22 = m5 * m12 - m8 * m9;
            Fix64 num23 = m5 * m11 - m7 * m9;
            Fix64 num24 = m5 * m10 - m6 * m9;
            result.M14 = (-(m2 * num19 - m3 * num20 + m4 * num21)) * num12;
            result.M24 = (m * num19 - m3 * num22 + m4 * num23) * num12;
            result.M34 = (-(m * num20 - m2 * num22 + m4 * num24)) * num12;
            result.M44 = (m * num21 - m2 * num23 + m3 * num24) * num12;
            return true;
        }

        /// <summary>
        /// 转置矩阵。
        /// 通过交换矩阵的行和列得到的新矩阵。
        /// </summary>
        public static FixMatrix4x4 Transpose(FixMatrix4x4 matrix)
        {
            FixMatrix4x4 result = default;
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;
            return result;
        }

        /// <summary>
        /// 线性插值（Lerp）。
        /// 这种插值操作在图形学和动画领域经常用于平滑过渡和动画效果的计算。
        /// </summary>
        public static FixMatrix4x4 Lerp(FixMatrix4x4 matrix1, FixMatrix4x4 matrix2, Fix64 amount)
        {
            FixMatrix4x4 result = default;
            result.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
            result.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
            result.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
            result.M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount;
            result.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
            result.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
            result.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
            result.M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount;
            result.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
            result.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
            result.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
            result.M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount;
            result.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
            result.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
            result.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
            result.M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount;
            return result;
        }

        #endregion

        #region Inherit/Override

        public bool Equals(FixMatrix4x4 other)
        {
            if (M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 && M21 == other.M21 && M23 == other.M23 && M24 == other.M24 && M31 == other.M31 && M32 == other.M32 && M34 == other.M34 && M41 == other.M41 && M42 == other.M42)
            {
                return M43 == other.M43;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixMatrix4x4)
            {
                return Equals((FixMatrix4x4)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int column1HashCode = M11.GetHashCode() ^ (M21.GetHashCode() << 2) ^ (M31.GetHashCode() >> 2) ^ (M41.GetHashCode() >> 1);
            int column2HashCode = M12.GetHashCode() ^ (M22.GetHashCode() << 2) ^ (M32.GetHashCode() >> 2) ^ (M42.GetHashCode() >> 1);
            int column3HashCode = M13.GetHashCode() ^ (M23.GetHashCode() << 2) ^ (M33.GetHashCode() >> 2) ^ (M43.GetHashCode() >> 1);
            int column4HashCode = M14.GetHashCode() ^ (M24.GetHashCode() << 2) ^ (M34.GetHashCode() >> 2) ^ (M44.GetHashCode() >> 1);
            return column1HashCode.GetHashCode() ^ (column2HashCode.GetHashCode() << 2) ^ (column3HashCode.GetHashCode() >> 2) ^ (column4HashCode.GetHashCode() >> 1);
        }

        public override string ToString()
        {
            return string.Format("[ ({0:f2}, {1:f2}, {2:f2}, {3:f2}) ({4:f2}, {5:f2}, {6:f2}, {7:f2}) ({8:f2}, {9:f2}, {10:f2}, {11:f2}) ({12:f2}, {13:f2}, {14:f2}, {15:f2}) ]",
                M11.AsFloat(), M12.AsFloat(), M13.AsFloat(), M14.AsFloat(),
                M21.AsFloat(), M22.AsFloat(), M23.AsFloat(), M24.AsFloat(),
                M31.AsFloat(), M32.AsFloat(), M33.AsFloat(), M34.AsFloat(),
                M41.AsFloat(), M42.AsFloat(), M43.AsFloat(), M44.AsFloat());
        }

        #endregion
    }
}
