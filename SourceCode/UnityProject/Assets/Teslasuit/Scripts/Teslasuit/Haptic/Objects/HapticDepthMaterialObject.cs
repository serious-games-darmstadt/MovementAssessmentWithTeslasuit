using UnityEngine;

namespace TeslasuitAPI
{


    /**@addtogroup UnityComponents */
    /*@{*/

    ///<summary>
    ///HapticDepthMaterialObject.. the deeper interaction the greater force of haptic material
    ///</summary>
    public class HapticDepthMaterialObject : HapticImpulseMaterialObject
    {
        private Volume volume;
        
        /// <summary>
        /// to normalize depth or use unit length
        /// </summary>
        public bool NormalizedDepth
        {
            get { return _isNormalizedDepth; }
        }
        [SerializeField]
        private bool _isNormalizedDepth = false;

        /// <summary>
        /// newtonsbyunit
        /// </summary>
        public float NewtonsByUnit
        {
            get { return _newtonsByUnit; }
        }
        [SerializeField]
        private float _newtonsByUnit = 1.0f;

        private void Start()
        {
            volume = new Volume(GetComponent<Collider>());
        }

        protected override HapticHitInfo CreateHit(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent)
        {
            Vector3 hitPoint = contactPoint.thisCollider.ClosestPoint(volume.Center);

            if ((hitPoint - volume.Center).magnitude < Mathf.Epsilon)
                hitPoint = contactPoint.thisCollider.bounds.center;

            float depth = 0.0f;
            float impact = 0.0f;

            if (NormalizedDepth)
            {
                depth = volume.GetDepthNormalized(hitPoint);
                impact = depth * forceMultiplier;
            }
            else
            {
                depth = volume.GetDepth(hitPoint);
                impact = Mathf.Clamp01((depth * NewtonsByUnit) / HapticConstants.MaxForce);
            }
                
            return new HapticHitInfo(HapticHitEvent.HitEnter, impact,
                HapticConstants.DefaultHitDuration, material);
        }
    }
    /*@}*/
}


