using BaseFramework;
using BaseFramework.DataTable;
using Lockstep.Game;
using Lockstep.Math;
using System;
using UnityEngine;

namespace XGame
{
    public class GameViewService : IService
    {
        private EntityLoader m_EntityLoader;

        public GameViewService()
        {
            m_EntityLoader = EntityLoader.Create(this);
        }

        public void Clear()
        {
            if (m_EntityLoader != null)
            {
                ReferencePool.Release(m_EntityLoader);
            }
        }

        public void BindView(BaseEntity baseEntity, Action<BaseEntity> onBindViewFinished)
        {
            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow(baseEntity.ConfigId);
            Type logicType = Utility.Assembly.GetType(drEntity.LogicType);

            baseEntity.GameObjectSerialId = m_EntityLoader.ShowEntity(
                baseEntity.ConfigId,
                logicType,
                (entity) =>
                {
                    if (entity == null)
                    {
                        return;
                    }
                    entity.transform.position = baseEntity.CTransform.Pos3.ToVector3();
                    entity.transform.rotation = Quaternion.Euler(new Vector3(0, baseEntity.CTransform.deg, 0));
                    baseEntity.EngineTransform = entity.transform;

                    EntityLogicBase entityLogicBase = (EntityLogicBase)entity.gameObject.GetComponent(logicType);
                    if (entityLogicBase == null)
                    {
                        entityLogicBase = (EntityLogicBase)entity.gameObject.AddComponent(logicType);
                    }

                    entityLogicBase.BindLSEntity(baseEntity);
                    onBindViewFinished?.Invoke(baseEntity);
                });
        }

        public void UnbindView(BaseEntity entity)
        {
            entity.OnRollbackDestroy();
            m_EntityLoader.HideEntity(entity.GameObjectSerialId);
            //if (entity != null && entity.EntityLogicBase != null && entity.EntityLogicBase.Entity != null)
            //{
            //    m_EntityLoader.HideEntity(entity.EntityLogicBase.Entity.Id);
            //}
        }
    }
}
