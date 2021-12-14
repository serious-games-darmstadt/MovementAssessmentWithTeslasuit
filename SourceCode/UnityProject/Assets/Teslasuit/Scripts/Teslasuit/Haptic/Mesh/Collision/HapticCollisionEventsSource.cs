using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticCollisionEventsSource : MonoBehaviour
    {
        public const int MaxCollisions = 1024;

        public event RaycastHandler RaycastHappened;
        public event CollisionHandler CollisionHappened;

        IHapticInteractor interactor;

        public void SetInteractor(IHapticInteractor interactor)
        {
            this.interactor = interactor;
        }

        private void FixedUpdate()
        {
            if (interactor != null)
            {
                var hapticCollisions = interactor.GetHapticCollisions(); // TODO Length??
                for (int i = 0; i < hapticCollisions.Count; i++)
                {
                    CollisionHappened?.Invoke(hapticCollisions[i]);
                }
                interactor.ClearHapticCollisions();
            }
        }

        public bool Raycast(ref HapticRaycastHit hit)
        {
            if (interactor == null) return false;
            var ret = interactor.Raycast(ref hit);
            RaycastHappened?.Invoke(hit);
            return ret;
        }

        private void OnCollisionEnter(Collision collision)
        {
            ProcessCollision(new CollisionWithType(collision, CollisionType.ENTER));
        }

        private void OnCollisionStay(Collision collision)
        {
            ProcessCollision(new CollisionWithType(collision, CollisionType.STAY));            
        }

        private void OnCollisionExit(Collision collision)
        {
            ProcessCollision(new CollisionWithType(collision, CollisionType.EXIT));
        }

        public void ProcessCollision(CollisionWithType collision)
        {
            interactor.AddHapticCollisions(collision);
        }

        private void OnDestroy()
        {
            
        }
    }


    public struct CollisionWithType
    {
        public Collision collision;
        public CollisionType type;

        public CollisionWithType(Collision collision, CollisionType type)
        {
            this.collision = collision;
            this.type = type;
        }
    }

    public enum CollisionType
    {
        ENTER,
        STAY,
        EXIT
    }
}