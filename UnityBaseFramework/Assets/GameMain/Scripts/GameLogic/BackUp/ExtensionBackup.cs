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
            writer.Write(HasReachTarget);
            writer.Write(NeedMove);
        }

        public void ReadBackup(Deserializer reader)
        {
            HasReachTarget = reader.ReadBoolean();
            NeedMove = reader.ReadBoolean();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += HasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += NeedMove.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "HasReachTarget" + ":" + HasReachTarget.ToString());
            sb.AppendLine(prefix + "NeedMove" + ":" + NeedMove.ToString());
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
            sb.AppendLine(prefix + "IsEnable" + ":" + isEnable.ToString());
            sb.AppendLine(prefix + "IsOnFloor" + ":" + isOnFloor.ToString());
            sb.AppendLine(prefix + "IsSleep" + ":" + isSleep.ToString());
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
            sb.AppendLine(prefix + "Deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "Pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "Y" + ":" + y.ToString());
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
            sb.AppendLine(prefix + "Deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "High" + ":" + high.ToString());
            sb.AppendLine(prefix + "Pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "Radius" + ":" + radius.ToString());
            sb.AppendLine(prefix + "Size" + ":" + size.ToString());
            sb.AppendLine(prefix + "Up" + ":" + up.ToString());
            sb.AppendLine(prefix + "Y" + ":" + y.ToString());
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
            writer.Write(LocalId);
            //writer.Write(curHealth);
            //writer.Write(damage);
            //writer.Write(isInvincible);
            //writer.Write(maxHealth);
            writer.Write(IsFire);
            writer.Write(MoveSpd);
            writer.Write(TurnSpd);
            ColliderData.WriteBackup(writer);
            Input.WriteBackup(writer);
            Mover.WriteBackup(writer);
            Rigidbody.WriteBackup(writer);
            CTransform.WriteBackup(writer);
        }

        public void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            ConfigId = reader.ReadInt32();
            LocalId = reader.ReadInt32();
            //curHealth = reader.ReadInt32();
            //damage = reader.ReadInt32();
            //isInvincible = reader.ReadBoolean();
            //maxHealth = reader.ReadInt32();
            IsFire = reader.ReadBoolean();
            MoveSpd = reader.ReadLFloat();
            TurnSpd = reader.ReadLFloat();
            ColliderData.ReadBackup(reader);
            Input.ReadBackup(reader);
            Mover.ReadBackup(reader);
            Rigidbody.ReadBackup(reader);
            CTransform.ReadBackup(reader);
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ConfigId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += LocalId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += IsFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MoveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += TurnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ColliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Input.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Mover.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CTransform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + ConfigId.ToString());
            sb.AppendLine(prefix + "LocalId" + ":" + LocalId.ToString());
            //sb.AppendLine(prefix + "curHealth" + ":" + curHealth.ToString());
            //sb.AppendLine(prefix + "damage" + ":" + damage.ToString());
            //sb.AppendLine(prefix + "isInvincible" + ":" + isInvincible.ToString());
            //sb.AppendLine(prefix + "maxHealth" + ":" + maxHealth.ToString());
            sb.AppendLine(prefix + "IsFire" + ":" + IsFire.ToString());
            sb.AppendLine(prefix + "MoveSpd" + ":" + MoveSpd.ToString());
            sb.AppendLine(prefix + "TurnSpd" + ":" + TurnSpd.ToString());
            sb.AppendLine(prefix + "ColliderData" + ":"); ColliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "Input" + ":"); Input.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "Mover" + ":"); Mover.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "Rigidbody" + ":"); Rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "CTransform" + ":"); CTransform.DumpStr(sb, "\t" + prefix);
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
            //writer.Write(GameObjectSerialId);
            writer.Write(Dir);
            writer.Write(CurrTime);
            ColliderData.WriteBackup(writer);
            Rigidbody.WriteBackup(writer);
            CTransform.WriteBackup(writer);
        }

        public void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            ConfigId = reader.ReadInt32();
            //GameObjectSerialId = reader.ReadInt32();
            Dir = reader.ReadLVector2();
            CurrTime = reader.ReadLFloat();
            ColliderData.ReadBackup(reader);
            Rigidbody.ReadBackup(reader);
            CTransform.ReadBackup(reader);
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ConfigId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += GameObjectSerialId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Dir.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CurrTime.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ColliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CTransform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + ConfigId.ToString());
            //sb.AppendLine(prefix + "GameObjectSerialId" + ":" + GameObjectSerialId.ToString());
            sb.AppendLine(prefix + "Dir" + ":" + Dir.ToString());
            sb.AppendLine(prefix + "CurrTime" + ":" + CurrTime.ToString());
            sb.AppendLine(prefix + "colliderData" + ":"); ColliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); Rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); CTransform.DumpStr(sb, "\t" + prefix);
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
