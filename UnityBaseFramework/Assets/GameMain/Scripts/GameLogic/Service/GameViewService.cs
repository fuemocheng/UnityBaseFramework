using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace XGame
{
    public class GameViewService : IService
    {
        public void BindView(BaseEntity entity, BaseEntity oldEntity = null)
        {
            if (oldEntity != null)
            {
                if (oldEntity.PrefabId == entity.PrefabId)
                {
                    entity.engineTransform = oldEntity.engineTransform;
                    var obj = (oldEntity.engineTransform as Transform).gameObject;
                    var views = obj.GetComponents<EntityView>();
                    foreach (var view in views)
                    {
                        view.BindEntity(entity, oldEntity);
                    }
                }
                else
                {
                    UnbindView(oldEntity);
                }
            }
            else
            {
                GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                if (prefab == null)
                    return;
                prefab.transform.position = entity.transform.Pos3.ToVector3();
                prefab.transform.rotation = Quaternion.Euler(new Vector3(0, entity.transform.deg, 0));
                entity.engineTransform = prefab.transform;


                var views = prefab.GetComponents<EntityView>();
                if (views.Length <= 0)
                {
                    var view = prefab.AddComponent<EntityView>();
                    view.BindEntity(entity);
                }
                else
                {
                    foreach (var view in views)
                    {
                        view.BindEntity(entity);
                    }
                }
            }
        }

        public void UnbindView(BaseEntity entity)
        {
            entity.OnRollbackDestroy();
        }
    }
}
