using UnityEngine;
using TeslasuitAPI;
using System.Collections.Generic;

namespace TeslasuitAPI
{
    public abstract class HapticCollisionSolverBase
    {
        protected MeshObjectInfo MeshObjectInfo { get; private set; }
        protected IHapticMapping HapticMapping { get; private set; }

        private List<HapticCollision> hapticCollisions = new List<HapticCollision>(HapticCollisionEventsSource.MaxCollisions);


        public HapticCollisionSolverBase(MeshObjectInfo meshObjectInfo, IHapticMapping mapping)
        {
            if (meshObjectInfo == null || mapping == null) throw new System.ArgumentException("Object reference is null");
            this.MeshObjectInfo = meshObjectInfo;
            this.HapticMapping = mapping;
           
        }

        public virtual HapticCollision[] CreateHapticCollisions(CollisionWithType[] typedCollisions)
        {
            if (typedCollisions == null || typedCollisions.Length == 0)
                return null;

            hapticCollisions.Clear();

            //TODO REWRITE WITH LINQ & TEST PERFORMANCE
            //PROCESS ALL COLLISIONS
            for (int i = 0; i < typedCollisions.Length; i++)
            {
                Collision collision = typedCollisions[i].collision;

                if(collision == null || collision.collider == null)
                {
                    continue;
                }
                //SKIP if no HapticObject Component on colliding object
                HapticMaterialObject hapticObject = collision.collider.gameObject.GetComponent<HapticMaterialObject>();
                if (hapticObject == null)
                    continue;

                CollisionType collisionType = typedCollisions[i].type;

                //PROCESS ALL CONTACT POINTS
                ContactPoint[] contacts = collision.contacts;
                for (int j = 0; j < contacts.Length; j++)
                {
                    HapticCollision hapticCollision = CreateCollision(collision, (HapticHitEvent)collisionType, contacts[j], hapticObject);
                    if(hapticCollision != null)
                        hapticCollisions.Add(hapticCollision);
                }
            }
            //TODO alloc
            HapticCollision[] result = hapticCollisions.ToArray();
            hapticCollisions.Clear();
            return result;
        }

        public virtual void AddHapticCollisions(CollisionWithType typedCollision)
        {
            Collision collision = typedCollision.collision;

            if (collision == null || collision.collider == null)
            {
                return;
            }

            //SKIP if no HapticObject Component on colliding object
            HapticMaterialObject hapticObject = collision.collider.gameObject.GetComponent<HapticMaterialObject>();
            if (hapticObject == null)
                return;

            CollisionType collisionType = typedCollision.type;

            //PROCESS ALL CONTACT POINTS
            ContactPoint[] contacts = collision.contacts;
            for (int j = 0; j < contacts.Length; j++)
            {
                HapticCollision hapticCollision = CreateCollision(collision, (HapticHitEvent)collisionType, contacts[j], hapticObject);
                if (hapticCollision != null)
                    hapticCollisions.Add(hapticCollision);
            }
        }

        public virtual void ClearHapticCollisions()
        {
            hapticCollisions.Clear();
        }

        public virtual List<HapticCollision> GetHapticCollisions()
        {
            return hapticCollisions;
        }

        protected abstract HapticCollision CreateCollision(Collision collision, HapticHitEvent hapticHitEvent, ContactPoint contactPoint, HapticMaterialObject hapticObject);


        public virtual void Destroy()
        {
            hapticCollisions.Clear();
            HapticMapping = null;
        }
    }

    public enum HapticCollisionSolverType
    {
        ChannelMeshCollision
    }
}


