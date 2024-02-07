using System;

namespace FixMath
{
    /// <summary>
    /// Represents a fixed-point Quaternion.
    /// </summary>
    public struct FixQuaternion : IEquatable<FixQuaternion>
    {
        public static readonly FixQuaternion Identity = new FixQuaternion(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

        public Fix64 X;
        public Fix64 Y;
        public Fix64 Z;
        public Fix64 W;

        public FixQuaternion(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public FixQuaternion(FixVector3 vectorPart, Fix64 scalarPart)
        {
            X = vectorPart.X;
            Y = vectorPart.Y;
            Z = vectorPart.Z;
            W = scalarPart;
        }

        #region Base

        public bool IsIdentity
        {
            get
            {
                if (X == Fix64.Zero && Y == Fix64.Zero && Z == Fix64.Zero)
                {
                    return W == Fix64.One;
                }
                return false;
            }
        }

        /// <summary>
        /// ZXY顺规, 旋转 γ（绕惯性坐标系Z轴），β（绕惯性坐标系X轴），ɑ（绕惯性坐标系Y轴）角度。
        /// 通过 FixQuaternion 获取欧拉角，误差在0.1°左右。
        /// </summary>
        public FixVector3 EulerAngles
        {
            get
            {
                /// 四元数->旋转矩阵
                /// 旋转矩阵为：
                /// --                                             --
                /// |   1-2yy-2zz,      2xy-2zw,        2xz+2yw     |
                /// |   2xy+2zw,        1-2xx-2zz,      2yz-2xw     |
                /// |   2xz-2yw,        2yz+2xw,        1-2xx-2yy   |
                /// --                                             --
                /// 
                /// 按ZXY顺规旋转γ，β，ɑ，令:  
                /// c1=cos(ɑ)=cos(Y(yaw)), s1=sin(ɑ)=sin(Y(yaw))
                /// c2=cos(β)=cos(X(pitch)), s2=sin(β)=sin(X(pitch))
                /// c3=cos(γ)=cos(Z(roll)), s3=sin(γ)=sin(Z(roll))
                /// 旋转矩阵为：
                /// --                                             --
                /// |   c1c3+s1s2s3,    s1s2s3-c1s3,    s1c2        |
                /// |   c2s3,           c2c3,           -s2         |
                /// |   c1s2s3-s1c3,    s1s3+c1s2c3,    c1c2        |
                /// --                                             --
                /// 
                /// 旋转矩阵->Euler
                /// ɑ = arctan(m13/m33)
                /// β = arcsin(-m23)
                /// γ = arctan(m21/m22)

                Fix64 xSqr = X * X;
                Fix64 ySqr = Y * Y;
                Fix64 zSqr = Z * Z;

                Fix64 m11 = Fix64.One - (Fix64)2 * (ySqr + zSqr);
                Fix64 m12 = (Fix64)2 * (X * Y - Z * W);
                Fix64 m13 = (Fix64)2 * (X * Z + Y * W);
                Fix64 m21 = (Fix64)2 * (X * Y + Z * W);
                Fix64 m22 = Fix64.One - (Fix64)2 * (xSqr + zSqr);
                Fix64 m23 = (Fix64)2 * (Y * Z - X * W);
                //Fix64 m31 = (Fix64)2 * (X * Z - Y * W);
                //Fix64 m32 = (Fix64)2 * (Y * Z + X * W);
                Fix64 m33 = Fix64.One - (Fix64)2 * (xSqr + ySqr);

                /// 处理万向节死锁：
                /// 当第二旋转角（绕X轴） β 为 ±π/2 时，即 m23 = -sinβ = ±1 时，Y、Z轴的旋转将失去一个自由度，出现万向节死锁。
                /// 这时 Atan2(m13, m33)，Atan2(m21, m22) 分子分母都为0，公式则没有意义。
                /// 所以要处理 β 为 ±π/2 的情况。
                /// 
                /// 当 β 为 π/2 时，sin(β)=1，cos(β)=0，简化矩阵，最终得到矩阵:
                /// --                                     --
                /// |   cos(ɑ - γ),     sin(ɑ - γ),     0   |
                /// |   0,              0,              1   |
                /// |   -sin(ɑ - γ),    cos(ɑ + γ),     0   |
                /// --                                     --
                /// 由此得 ɑ - γ = atan2(m12, m11)，只要给ɑ或γ其中一个赋值，另一个就可以计算出来。
                /// 
                /// 当 β 为 -π/2 时，sin(β)=-1，cos(β)=0，简化矩阵，最终得到矩阵:
                /// --                                     --
                /// |   cos(ɑ + γ),     -sin(ɑ + γ),    0   |
                /// |   0,              0,              1   |
                /// |   -sin(ɑ + γ),    -cos(ɑ + γ),    0   |
                /// --                                     --
                /// 由此得 ɑ + γ = atan2(-m12, m11)，只要给ɑ或γ其中一个赋值，另一个就可以计算出来。
                ///
                /// 这里当出现万向节死锁的时候，令绕Z轴的旋转γ为0；

                FixVector3 result = default;

                if (FixMath.Abs(m23 - Fix64.One) < FixMath.EN8)
                {
                    // m23 = -sinβ 约为 1 时，β 为 -π/2；
                    // 令 γ 为 0；
                    result.X = -Fix64.PiOver2;
                    result.Y = FixMath.Atan2(-m12, m11);
                    result.Z = Fix64.Zero;
                }
                else if (FixMath.Abs(m23 + Fix64.One) < FixMath.EN8)
                {
                    // m23 = -sinβ 约为 -1 时，β 为 π/2；
                    // 令 γ 为 0；
                    result.X = Fix64.PiOver2;
                    result.Y = FixMath.Atan2(m12, m11);
                    result.Z = Fix64.Zero;
                }
                else
                {
                    // β 在 (-π/2, π/2) 之间，不会出现万向节死锁的情况；
                    result.X = FixMath.Asin(-m23);
                    result.Y = FixMath.Atan2(m13, m33);
                    result.Z = FixMath.Atan2(m21, m22);
                }

                // 每个弧度都限定在 [0, 2π]；
                result.X = result.X < Fix64.Zero ? (result.X + FixMath.PiTimes2) : (result.X > FixMath.PiTimes2 ? result.X - FixMath.PiTimes2 : result.X);
                result.Y = result.Y < Fix64.Zero ? (result.Y + FixMath.PiTimes2) : (result.Y > FixMath.PiTimes2 ? result.Y - FixMath.PiTimes2 : result.Y);
                result.Z = result.Z < Fix64.Zero ? (result.Z + FixMath.PiTimes2) : (result.Z > FixMath.PiTimes2 ? result.Z - FixMath.PiTimes2 : result.Z);

                // 转换成角度；
                result.X *= FixMath.Rad2Deg;
                result.Y *= FixMath.Rad2Deg;
                result.Z *= FixMath.Rad2Deg;

                return result;
            }
            set
            {
                this = FixQuaternion.Euler(value);
            }
        }

        public FixQuaternion Normalized
        {
            get
            {
                return FixQuaternion.Normalize(this);
            }
        }

        public Fix64 Length()
        {
            Fix64 num = X * X + Y * Y + Z * Z + W * W;
            return FixMath.Sqrt(num);
        }

        public Fix64 LengthSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        public UnityEngine.Quaternion ToQuaternion()
        {
            return new UnityEngine.Quaternion(X.AsFloat(), Y.AsFloat(), Z.AsFloat(), W.AsFloat());
        }

        #endregion

        #region Operators

        public static bool operator ==(FixQuaternion left, FixQuaternion right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Z == right.Z)
            {
                return left.W == right.W;
            }

            return false;
        }

        public static bool operator !=(FixQuaternion left, FixQuaternion right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Z == right.Z)
            {
                return left.W != right.W;
            }

            return true;
        }

        public static FixQuaternion operator -(FixQuaternion value)
        {
            FixQuaternion result = default;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
            return result;
        }

        public static FixQuaternion operator +(FixQuaternion left, FixQuaternion right)
        {
            FixQuaternion result = default;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
            return result;
        }

        public static FixQuaternion operator -(FixQuaternion left, FixQuaternion right)
        {
            FixQuaternion result = default;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
            return result;
        }

        public static FixQuaternion operator *(FixQuaternion left, FixQuaternion right)
        {
            Fix64 x = left.X;
            Fix64 y = left.Y;
            Fix64 z = left.Z;
            Fix64 w = left.W;
            Fix64 x2 = right.X;
            Fix64 y2 = right.Y;
            Fix64 z2 = right.Z;
            Fix64 w2 = right.W;
            Fix64 num = y * z2 - z * y2;
            Fix64 num2 = z * x2 - x * z2;
            Fix64 num3 = x * y2 - y * x2;
            Fix64 num4 = x * x2 + y * y2 + z * z2;
            FixQuaternion result = default;
            result.X = x * w2 + x2 * w + num;
            result.Y = y * w2 + y2 * w + num2;
            result.Z = z * w2 + z2 * w + num3;
            result.W = w * w2 - num4;
            return result;
        }

        public static FixQuaternion operator *(FixQuaternion value, Fix64 factor)
        {
            FixQuaternion result = default;
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
            result.W = value.W * factor;
            return result;
        }

        public static FixQuaternion operator /(FixQuaternion left, FixQuaternion right)
        {
            Fix64 x = left.X;
            Fix64 y = left.Y;
            Fix64 z = left.Z;
            Fix64 w = left.W;
            Fix64 num = right.X * right.X + right.Y * right.Y + right.Z * right.Z + right.W * right.W;
            Fix64 num2 = Fix64.One / num;
            Fix64 num3 = (-right.X) * num2;
            Fix64 num4 = (-right.Y) * num2;
            Fix64 num5 = (-right.Z) * num2;
            Fix64 num6 = right.W * num2;
            Fix64 num7 = y * num5 - z * num4;
            Fix64 num8 = z * num3 - x * num5;
            Fix64 num9 = x * num4 - y * num3;
            Fix64 num10 = x * num3 + y * num4 + z * num5;
            FixQuaternion result = default;
            result.X = x * num6 + num3 * w + num7;
            result.Y = y * num6 + num4 * w + num8;
            result.Z = z * num6 + num5 * w + num9;
            result.W = w * num6 - num10;
            return result;
        }

        /// <summary>
        /// 取负。
        /// </summary>
        public static FixQuaternion Negate(FixQuaternion value)
        {
            return -value;
        }

        public static FixQuaternion Add(FixQuaternion left, FixQuaternion right)
        {
            return left + right;
        }

        public static FixQuaternion Subtract(FixQuaternion left, FixQuaternion right)
        {
            return left - right;
        }

        public static FixQuaternion Multiply(FixQuaternion left, FixQuaternion right)
        {
            return left * right;
        }

        public static FixQuaternion Multiply(FixQuaternion value, Fix64 factor)
        {
            return value * factor;
        }

        public static FixQuaternion Divide(FixQuaternion left, FixQuaternion right)
        {
            return left / right;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 归一化（normalize）。
        /// 归一化四元数是指将四元数的分量调整，使得它的模（magnitude）等于1。
        /// </summary>
        public static FixQuaternion Normalize(FixQuaternion value)
        {
            Fix64 num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            Fix64 num2 = Fix64.One / FixMath.Sqrt(num);
            FixQuaternion result = default;
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            result.Z = value.Z * num2;
            result.W = value.W * num2;
            return result;
        }

        /// <summary>
        /// 共轭四元数（conjugate）。
        /// 共轭四元数是将原始四元数的虚部取相反数得到的四元数。
        /// </summary>
        public static FixQuaternion Conjugate(FixQuaternion value)
        {
            FixQuaternion result = default;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
            return result;
        }

        /// <summary>
        /// 逆四元数（inverse）。
        /// 逆四元数与原始四元数相乘后得到单位四元数。
        /// </summary>
        public static FixQuaternion Inverse(FixQuaternion value)
        {
            Fix64 num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            Fix64 num2 = Fix64.One / num;
            FixQuaternion result = default;
            result.X = -value.X * num2;
            result.Y = -value.Y * num2;
            result.Z = -value.Z * num2;
            result.W = value.W * num2;
            return result;
        }

        /// <summary>
        /// 通过给定轴向量和旋转角度生成相应的四元数。
        /// </summary>
        /// <param name="axis">轴向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static FixQuaternion CreateFromAxisAngle(FixVector3 axis, Fix64 angle)
        {
            Fix64 num = angle * Fix64.Half;
            Fix64 num2 = FixMath.Sin(num);
            Fix64 w = FixMath.Cos(num);
            FixQuaternion result = default;
            result.X = axis.X * num2;
            result.Y = axis.Y * num2;
            result.Z = axis.Z * num2;
            result.W = w;
            return result;
        }

        /// <summary>
        /// 通过欧拉角创建四元数。
        /// ZXY顺规，外旋。
        /// </summary>
        /// <param name="x">绕固定轴X轴旋转角度。</param>
        /// <param name="y">绕固定轴Y轴旋转角度。</param>
        /// <param name="z">绕固定轴Z轴旋转角度。</param>
        /// <returns></returns>
        public static FixQuaternion Euler(Fix64 x, Fix64 y, Fix64 z)
        {
            x *= FixMath.Deg2Rad;
            y *= FixMath.Deg2Rad;
            z *= FixMath.Deg2Rad;
            return CreateFromYawPitchRoll(y, x, z);
        }

        /// <summary>
        /// 通过欧拉角创建四元数。
        /// ZXY顺规，外旋。
        /// </summary>
        /// <param name="euler">欧拉角</param>
        /// <returns></returns>
        public static FixQuaternion Euler(FixVector3 euler)
        {
            return Euler(euler.X, euler.Y, euler.Z);
        }

        /// <summary>
        /// 根据给定的欧拉角（yaw、pitch、roll）生成相应的四元数。
        /// 绕固定轴 Z, X, Y 外旋 roll, pitch, yaw 角度。
        /// </summary>
        /// <param name="yaw">绕Y轴的偏航角（Yaw）</param>
        /// <param name="pitch">绕X轴的俯仰角（Pitch）</param>
        /// <param name="roll">绕Z轴的滚转角（Roll）</param>
        /// <returns></returns>
        public static FixQuaternion CreateFromYawPitchRoll(Fix64 yaw, Fix64 pitch, Fix64 roll)
        {
            Fix64 num = roll * Fix64.Half;
            Fix64 num2 = FixMath.Sin(num);
            Fix64 num3 = FixMath.Cos(num);
            Fix64 num4 = pitch * Fix64.Half;
            Fix64 num5 = FixMath.Sin(num4);
            Fix64 num6 = FixMath.Cos(num4);
            Fix64 num7 = yaw * Fix64.Half;
            Fix64 num8 = FixMath.Sin(num7);
            Fix64 num9 = FixMath.Cos(num7);
            FixQuaternion result = default;
            result.X = num9 * num5 * num3 + num8 * num6 * num2;
            result.Y = num8 * num6 * num3 - num9 * num5 * num2;
            result.Z = num9 * num6 * num2 - num8 * num5 * num3;
            result.W = num9 * num6 * num3 + num8 * num5 * num2;
            return result;
        }

        /// <summary>
        /// 通过旋转矩阵构建四元数。
        /// </summary>
        public static FixQuaternion CreateFromRotationMatrix(FixMatrix4x4 matrix)
        {
            //计算矩阵的迹（trace）
            Fix64 num = matrix.M11 + matrix.M22 + matrix.M33;
            FixQuaternion result = default;

            //如果迹值 num 大于 0，使用一种常见的情况来计算四元数的分量。
            if (num > Fix64.Zero)
            {
                Fix64 num2 = FixMath.Sqrt(num + Fix64.One);
                result.W = num2 * Fix64.Half;
                num2 = Fix64.Half / num2;
                result.X = (matrix.M23 - matrix.M32) * num2;
                result.Y = (matrix.M31 - matrix.M13) * num2;
                result.Z = (matrix.M12 - matrix.M21) * num2;
            }
            else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
            {
                //如果 matrix.M11 是最大的元素，旋转轴最接近X轴，通过计算一个辅助变量来构建四元数。
                Fix64 num3 = FixMath.Sqrt(Fix64.One + matrix.M11 - matrix.M22 - matrix.M33);
                Fix64 num4 = Fix64.Half / num3;
                result.X = Fix64.Half * num3;
                result.Y = (matrix.M12 + matrix.M21) * num4;
                result.Z = (matrix.M13 + matrix.M31) * num4;
                result.W = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                //如果 matrix.M22 是最大的元素，旋转轴最接近Y轴，同样通过计算辅助变量构建四元数。
                Fix64 num5 = FixMath.Sqrt(Fix64.One + matrix.M22 - matrix.M11 - matrix.M33);
                Fix64 num6 = Fix64.Half / num5;
                result.X = (matrix.M21 + matrix.M12) * num6;
                result.Y = Fix64.Half * num5;
                result.Z = (matrix.M32 + matrix.M23) * num6;
                result.W = (matrix.M31 - matrix.M13) * num6;
            }
            else
            {
                //如果 matrix.M33 是最大的元素，旋转轴最接近Z轴，同样通过计算辅助变量构建四元数。
                Fix64 num7 = FixMath.Sqrt(Fix64.One + matrix.M33 - matrix.M11 - matrix.M22);
                Fix64 num8 = Fix64.Half / num7;
                result.X = (matrix.M31 + matrix.M13) * num8;
                result.Y = (matrix.M32 + matrix.M23) * num8;
                result.Z = Fix64.Half * num7;
                result.W = (matrix.M12 - matrix.M21) * num8;
            }

            return result;
        }

        /// <summary>
        /// 点乘（dot product）。
        /// 方向表示：如果两个四元数的点积为正，则它们的旋转方向相同；如果点积为负，则它们的旋转方向相反。
        /// 旋转合成：两个旋转，每个都由一个四元数表示，通过计算这两个四元数的点积来获得一个新的四元数，该四元数表示这两个旋转的合成。
        /// </summary>
        public static Fix64 Dot(FixQuaternion quaternion1, FixQuaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        }

        /// <summary>
        /// 球形插值（Slerp）。
        /// 用于在两个给定四元数之间进行平滑的旋转插值。
        /// </summary>
        public static FixQuaternion Slerp(FixQuaternion quaternion1, FixQuaternion quaternion2, Fix64 amount)
        {
            Fix64 num = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            bool flag = false;
            if (num < Fix64.Zero)
            {
                flag = true;
                num = Fix64.Zero - num;
            }

            Fix64 num2;
            Fix64 num3;
            if (num > (Fix64.One - Fix64.EN6)) //(Fix64)0.999999f
            {
                num2 = Fix64.One - amount;
                num3 = flag ? (Fix64.Zero - amount) : amount;
            }
            else
            {
                Fix64 num4 = FixMath.Acos(num);
                Fix64 num5 = Fix64.One / FixMath.Sin(num4);
                num2 = FixMath.Sin((Fix64.One - amount) * num4) * num5;
                num3 = flag ? Fix64.Zero - FixMath.Sin(amount * num4) * num5 : FixMath.Sin(amount * num4) * num5;
            }

            FixQuaternion result = default;
            result.X = num2 * quaternion1.X + num3 * quaternion2.X;
            result.Y = num2 * quaternion1.Y + num3 * quaternion2.Y;
            result.Z = num2 * quaternion1.Z + num3 * quaternion2.Z;
            result.W = num2 * quaternion1.W + num3 * quaternion2.W;
            return result;
        }

        /// <summary>
        /// 线性插值（Lerp）。
        /// 用于在两个给定四元数之间进行线性插值。
        /// </summary>
        public static FixQuaternion Lerp(FixQuaternion quaternion1, FixQuaternion quaternion2, Fix64 amount)
        {
            Fix64 num = Fix64.One - amount;
            FixQuaternion result = default;
            Fix64 num2 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            if (num2 >= Fix64.Zero)
            {
                result.X = num * quaternion1.X + amount * quaternion2.X;
                result.Y = num * quaternion1.Y + amount * quaternion2.Y;
                result.Z = num * quaternion1.Z + amount * quaternion2.Z;
                result.W = num * quaternion1.W + amount * quaternion2.W;
            }
            else
            {
                result.X = num * quaternion1.X - amount * quaternion2.X;
                result.Y = num * quaternion1.Y - amount * quaternion2.Y;
                result.Z = num * quaternion1.Z - amount * quaternion2.Z;
                result.W = num * quaternion1.W - amount * quaternion2.W;
            }

            Fix64 num3 = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
            Fix64 num4 = Fix64.One / FixMath.Sqrt(num3);
            result.X *= num4;
            result.Y *= num4;
            result.Z *= num4;
            result.W *= num4;
            return result;
        }

        /// <summary>
        /// 将两个四元数连接。(相乘)
        /// 通常用于将一个物体的局部旋转与另一个物体的旋转相结合，得到它们的总体旋转。
        /// 使用四元数来表示和处理旋转可以避免万向锁等问题，提供更精确和稳定的旋转表示。
        /// </summary>
        public static FixQuaternion Concatenate(FixQuaternion value1, FixQuaternion value2)
        {
            Fix64 x = value2.X;
            Fix64 y = value2.Y;
            Fix64 z = value2.Z;
            Fix64 w = value2.W;
            Fix64 x2 = value1.X;
            Fix64 y2 = value1.Y;
            Fix64 z2 = value1.Z;
            Fix64 w2 = value1.W;
            Fix64 num = y * z2 - z * y2;
            Fix64 num2 = z * x2 - x * z2;
            Fix64 num3 = x * y2 - y * x2;
            Fix64 num4 = x * x2 + y * y2 + z * z2;
            FixQuaternion result = default;
            result.X = x * w2 + x2 * w + num;
            result.Y = y * w2 + y2 * w + num2;
            result.Z = z * w2 + z2 * w + num3;
            result.W = w * w2 - num4;
            return result;
        }

        #endregion

        #region Inherit/Override

        public bool Equals(FixQuaternion other)
        {
            if (X == other.X && Y == other.Y && Z == other.Z)
            {
                return W == other.W;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixQuaternion)
            {
                return Equals((FixQuaternion)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2) ^ (W.GetHashCode() >> 1);
        }

        public override string ToString()
        {
            return string.Format("({0:f2}, {1:f2}, {2:f2}, {3:f2})", X.AsFloat(), Y.AsFloat(), Z.AsFloat(), W.AsFloat());
        }

        #endregion
    }
}
