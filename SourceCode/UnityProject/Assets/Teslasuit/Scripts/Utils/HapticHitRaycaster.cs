using UnityEngine;

namespace TeslasuitAPI
{
    /**@addtogroup UnityComponents */
    /*@{*/

    /// <summary>
    /// Static class for haptic raycasting 
    /// </summary>
    public static class HapticHitRaycaster
    {
        /// <summary>
        /// Raycast HapticObjects
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="hapticRaycastHit"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool Raycast(Ray ray, out HapticRaycastHit hapticRaycastHit, float range = float.MaxValue)
        {
            hapticRaycastHit = new HapticRaycastHit();

            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hapticRaycastHit.raycastHit, range, 1 << HapticConstants.Util.HapticReceiverLayer))
                return TryHapticRaycast(ref hapticRaycastHit);

            return false;
        }

        private static bool TryHapticRaycast(ref HapticRaycastHit hapticRaycastHit)
        {
            if (hapticRaycastHit.raycastHit.collider == null) return false;
            var collisionEventsSources = hapticRaycastHit.raycastHit.collider.GetComponentsInParent<HapticCollisionEventsSource>();

            if (collisionEventsSources.Length == 0) return false;

            foreach(var src in collisionEventsSources)
            {
                bool succeeded = src.Raycast(ref hapticRaycastHit);

                if (succeeded)
                    return true;
            }

            return false;
        }

        public static int Raycast(Ray ray, HapticRaycastHit[] hapticRaycastHits, float range = float.MaxValue)
        {
            lock (RaycastAllLock)
            {
                int count = Physics.RaycastNonAlloc(ray, NonAllocBuffer, range);
                int hits = 0;
                for (int i = 0; i < count && i < hapticRaycastHits.Length; i++, hits++)
                {
                    hapticRaycastHits[hits].raycastHit = NonAllocBuffer[i];
                    if (!TryHapticRaycast(ref hapticRaycastHits[hits]))
                    {
                        hits--;
                        continue;
                    }
                }
                return hits;
            }
        }

        private static object RaycastAllLock = new object();
        private const int RaycastAllocBufferLength = 64;
        private static RaycastHit[] NonAllocBuffer = new RaycastHit[RaycastAllocBufferLength];
    }

    /*@}*/

    public struct HapticRaycastHit
    {
        public RaycastHit raycastHit;
        public HapticReceiver hapticReceiver;
        public Polygon channelPoly;

    }
}

