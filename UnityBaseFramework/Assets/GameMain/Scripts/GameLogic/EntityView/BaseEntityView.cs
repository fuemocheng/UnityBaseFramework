using Lockstep.Math;
using UnityEngine;

namespace XGame
{
    public class BaseEntityView : MonoBehaviour, IEntityView
    {
        public const float LerpPercent = 0.3f;

        public BaseEntity baseEntity;

        void Start()
        {

        }

        void Update()
        {

        }

        public virtual void BindEntity(BaseEntity e, BaseEntity oldEntity = null)
        {
            e.EntityView = this;
            baseEntity = e;
            var updateEntity = oldEntity ?? e;
            transform.position = updateEntity.transform.Pos3.ToVector3();
            transform.rotation = Quaternion.Euler(0, updateEntity.transform.deg.ToFloat(), 0);
        }

        public virtual void OnDead()
        {
            GameObject.Destroy(gameObject);
        }

        public virtual void OnRollbackDestroy()
        {
            GameObject.Destroy(gameObject);
        }

        public virtual void OnTakeDamage(int amount, LVector3 hitPoint)
        {

        }
    }
}
