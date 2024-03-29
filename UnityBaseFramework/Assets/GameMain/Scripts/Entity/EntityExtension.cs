using BaseFramework.DataTable;
using System;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        //public static Entity GetGameEntity(this EntityComponent entityComponent, int entityId)
        //{
        //    UnityBaseFramework.Runtime.Entity entity = entityComponent.GetEntity(entityId);
        //    if (entity == null)
        //    {
        //        return null;
        //    }

        //    return (Entity)entity.Logic;
        //}

        //public static void HideEntity(this EntityComponent entityComponent, Entity entity)
        //{
        //    entityComponent.HideEntity(entity);
        //}

        //public static void AttachEntity(this EntityComponent entityComponent, Entity entity, int ownerId, string parentTransformPath = null, object userData = null)
        //{
        //    entityComponent.AttachEntity(entity.Entity, ownerId, parentTransformPath, userData);
        //}

        //public static void ShowEffect(this EntityComponent entityComponent, EffectData data)
        //{
        //    entityComponent.ShowEntity(typeof(Effect), "Effect", Constant.AssetPriority.EffectAsset, data);
        //}

        //private static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, EntityData data)
        //{
        //    if (data == null)
        //    {
        //        Log.Warning("Data is invalid.");
        //        return;
        //    }

        //    IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
        //    DREntity drEntity = dtEntity.GetDataRow(data.TypeId);
        //    if (drEntity == null)
        //    {
        //        Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
        //        return;
        //    }

        //    entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, priority, data);
        //}

        public static void ShowEntity<T>(this EntityComponent entityComponent, int serialId, int entityId, object userData = null)
        {
            entityComponent.ShowEntity(serialId, entityId, typeof(T), userData);
        }

        public static void ShowEntity(this EntityComponent entityComponent, int serialId, int entityId, Type logicType, object userData = null)
        {
            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow(entityId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", entityId.ToString());
                return;
            }

            entityComponent.ShowEntity(serialId, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), drEntity.GroupName, Constant.AssetPriority.Player, userData);
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}
