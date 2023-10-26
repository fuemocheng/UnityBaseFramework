using BaseFramework;
using Lockstep.Game;
using Lockstep.Math;
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

        public void BindView(BaseEntity baseEntity, BaseEntity oldEntity = null)
        {
            if (oldEntity != null)
            {
                if (oldEntity.PrefabId == baseEntity.PrefabId)
                {
                    baseEntity.EngineTransform = oldEntity.EngineTransform;
                    GameObject obj = (oldEntity.EngineTransform as Transform).gameObject;
                    EntityLogicBase entityLogicBase = obj.GetComponent<EntityLogicBase>();
                    entityLogicBase.BindLSEntity(baseEntity, oldEntity);
                }
                else
                {
                    UnbindView(oldEntity);
                }
            }
            else
            {

                //默认10001
                m_EntityLoader.ShowEntity(10001, typeof(EntityLogicPlayer), (entity) =>
                {
                    if (entity == null)
                    {
                        return;
                    }
                    entity.transform.position = baseEntity.transform.Pos3.ToVector3();
                    entity.transform.rotation = Quaternion.Euler(new Vector3(0, baseEntity.transform.deg, 0));
                    baseEntity.EngineTransform = entity.transform;


                    EntityLogicPlayer entityLogic = entity.gameObject.GetComponent<EntityLogicPlayer>();
                    if (entityLogic == null)
                    {
                        entityLogic = entity.gameObject.AddComponent<EntityLogicPlayer>();
                    }

                    entityLogic.BindLSEntity(baseEntity);
                });
            }
        }

        public void UnbindView(BaseEntity entity)
        {
            entity.OnRollbackDestroy();
        }
    }
}
