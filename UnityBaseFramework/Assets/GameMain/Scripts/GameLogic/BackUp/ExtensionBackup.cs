using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using Lockstep.Util;
using System.Text;

namespace XGame
{
    public partial class CMover : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(hasReachTarget);
            writer.Write(needMove);
        }

        public void ReadBackup(Deserializer reader)
        {
            hasReachTarget = reader.ReadBoolean();
            needMove = reader.ReadBoolean();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += hasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += needMove.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "hasReachTarget" + ":" + hasReachTarget.ToString());
            sb.AppendLine(prefix + "needMove" + ":" + needMove.ToString());
        }
    }
}

namespace Lockstep.Game
{
    public partial class CRigidbody : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(Mass);
            writer.Write(Speed);
            writer.Write(isEnable);
            writer.Write(isOnFloor);
            writer.Write(isSleep);
        }

        public void ReadBackup(Deserializer reader)
        {
            Mass = reader.ReadLFloat();
            Speed = reader.ReadLVector3();
            isEnable = reader.ReadBoolean();
            isOnFloor = reader.ReadBoolean();
            isSleep = reader.ReadBoolean();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += Mass.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Speed.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isEnable.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isOnFloor.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isSleep.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "Mass" + ":" + Mass.ToString());
            sb.AppendLine(prefix + "Speed" + ":" + Speed.ToString());
            sb.AppendLine(prefix + "isEnable" + ":" + isEnable.ToString());
            sb.AppendLine(prefix + "isOnFloor" + ":" + isOnFloor.ToString());
            sb.AppendLine(prefix + "isSleep" + ":" + isSleep.ToString());
        }
    }
}

namespace Lockstep.Collision2D
{
    public partial class CTransform2D : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(deg);
            writer.Write(pos);
            writer.Write(y);
        }

        public void ReadBackup(Deserializer reader)
        {
            deg = reader.ReadLFloat();
            pos = reader.ReadLVector2();
            y = reader.ReadLFloat();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += deg.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += pos.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += y.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "y" + ":" + y.ToString());
        }
    }
}

namespace Lockstep.Collision2D
{
    public partial class ColliderData : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(deg);
            writer.Write(high);
            writer.Write(pos);
            writer.Write(radius);
            writer.Write(size);
            writer.Write(up);
            writer.Write(y);
        }

        public void ReadBackup(Deserializer reader)
        {
            deg = reader.ReadLFloat();
            high = reader.ReadLFloat();
            pos = reader.ReadLVector2();
            radius = reader.ReadLFloat();
            size = reader.ReadLVector2();
            up = reader.ReadLVector2();
            y = reader.ReadLFloat();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += deg.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += high.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += pos.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += radius.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += size.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += up.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += y.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "high" + ":" + high.ToString());
            sb.AppendLine(prefix + "pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "radius" + ":" + radius.ToString());
            sb.AppendLine(prefix + "size" + ":" + size.ToString());
            sb.AppendLine(prefix + "up" + ":" + up.ToString());
            sb.AppendLine(prefix + "y" + ":" + y.ToString());
        }
    }
}


namespace XGame
{
    public partial class Player : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(ConfigId);
            //writer.Write(curHealth);
            //writer.Write(damage);
            //writer.Write(isFire);
            //writer.Write(isInvincible);
            writer.Write(localId);
            //writer.Write(maxHealth);
            writer.Write(moveSpd);
            writer.Write(turnSpd);
            colliderData.WriteBackup(writer);
            input.WriteBackup(writer);
            mover.WriteBackup(writer);
            rigidbody.WriteBackup(writer);
            transform.WriteBackup(writer);
        }

        public void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            ConfigId = reader.ReadInt32();
            //curHealth = reader.ReadInt32();
            //damage = reader.ReadInt32();
            //isFire = reader.ReadBoolean();
            //isInvincible = reader.ReadBoolean();
            localId = reader.ReadInt32();
            //maxHealth = reader.ReadInt32();
            moveSpd = reader.ReadLFloat();
            turnSpd = reader.ReadLFloat();
            colliderData.ReadBackup(reader);
            input.ReadBackup(reader);
            mover.ReadBackup(reader);
            rigidbody.ReadBackup(reader);
            transform.ReadBackup(reader);
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ConfigId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += isFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += localId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += input.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += mover.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + ConfigId.ToString());
            //sb.AppendLine(prefix + "curHealth" + ":" + curHealth.ToString());
            //sb.AppendLine(prefix + "damage" + ":" + damage.ToString());
            //sb.AppendLine(prefix + "isFire" + ":" + isFire.ToString());
            //sb.AppendLine(prefix + "isInvincible" + ":" + isInvincible.ToString());
            sb.AppendLine(prefix + "localId" + ":" + localId.ToString());
            //sb.AppendLine(prefix + "maxHealth" + ":" + maxHealth.ToString());
            sb.AppendLine(prefix + "moveSpd" + ":" + moveSpd.ToString());
            sb.AppendLine(prefix + "turnSpd" + ":" + turnSpd.ToString());
            sb.AppendLine(prefix + "colliderData" + ":"); colliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "input" + ":"); input.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "mover" + ":"); mover.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); transform.DumpStr(sb, "\t" + prefix);
        }
    }
}

namespace XGame
{
    public partial class Bullet : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(ConfigId);
            writer.Write(Dir);
            writer.Write(CurrTime);
            colliderData.WriteBackup(writer);
            rigidbody.WriteBackup(writer);
            transform.WriteBackup(writer);
        }

        public void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            ConfigId = reader.ReadInt32();
            Dir = reader.ReadLVector2();
            CurrTime = reader.ReadLFloat();
            colliderData.ReadBackup(reader);
            rigidbody.ReadBackup(reader);
            transform.ReadBackup(reader);
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ConfigId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Dir.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CurrTime.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + ConfigId.ToString());
            sb.AppendLine(prefix + "Dir" + ":" + Dir.ToString());
            sb.AppendLine(prefix + "CurrTime" + ":" + CurrTime.ToString());
            sb.AppendLine(prefix + "colliderData" + ":"); colliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); transform.DumpStr(sb, "\t" + prefix);
        }
    }
}

namespace GameProto
{
    public partial class Input : IBackup
    {
        public void WriteBackup(Serializer writer)
        {
            writer.Write(InputH);
            writer.Write(InputV);
            writer.Write(MousePosX);
            writer.Write(MousePosY);
            writer.Write(IsFire);
            writer.Write(IsSpeedUp);
            writer.Write(SkillId);
        }

        public void ReadBackup(Deserializer reader)
        {
            InputH = reader.ReadInt32();
            InputV = reader.ReadInt32();
            MousePosX = reader.ReadInt32();
            MousePosY = reader.ReadInt32();
            IsFire = reader.ReadBoolean();
            IsSpeedUp = reader.ReadBoolean();
            SkillId = reader.ReadInt32();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += InputH.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += InputV.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MousePosX.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MousePosY.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += IsFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += IsSpeedUp.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += SkillId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "InputH" + ":" + InputH.ToString());
            sb.AppendLine(prefix + "InputV" + ":" + InputV.ToString());
            sb.AppendLine(prefix + "MousePosX" + ":" + MousePosX.ToString());
            sb.AppendLine(prefix + "MousePosY" + ":" + MousePosY.ToString());
            sb.AppendLine(prefix + "IsFire" + ":" + IsFire.ToString());
            sb.AppendLine(prefix + "IsSpeedUp" + ":" + IsSpeedUp.ToString());
            sb.AppendLine(prefix + "SkillId" + ":" + SkillId.ToString());
        }
    }
}
