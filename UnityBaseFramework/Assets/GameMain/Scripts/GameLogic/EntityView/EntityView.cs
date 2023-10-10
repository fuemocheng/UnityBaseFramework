using Lockstep.Math;
using UnityEngine;

namespace XGame
{
    public class EntityView : BaseEntityView, IEntityView
    {
        public CEntity cEntity;


        void Start()
        {

        }

        void Update()
        {
            var pos = cEntity.transform.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = cEntity.transform.deg.ToFloat();
            //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }

        public override void BindEntity(BaseEntity e, BaseEntity oldEntity = null)
        {
            base.BindEntity(e, oldEntity);
            e.EntityView = this;
            this.cEntity = e as CEntity;
        }

        public override void OnTakeDamage(int amount, LVector3 hitPoint)
        {

        }

        public override void OnDead()
        {
            GameObject.Destroy(gameObject);
        }

        public override void OnRollbackDestroy()
        {
            GameObject.Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            
        }
#endif
    }
}
