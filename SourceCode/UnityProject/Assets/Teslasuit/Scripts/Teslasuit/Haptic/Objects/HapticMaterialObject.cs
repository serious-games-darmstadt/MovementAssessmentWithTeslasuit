using UnityEngine;

namespace TeslasuitAPI
{
    [RequireComponent(typeof(Collider))]
    public abstract class HapticMaterialObject : HapticObject
    {
        public override HapticAsset HapticAsset
        {
            get
            {
                return material;
            }
        }

        protected abstract HapticMaterialAsset material { get; }


        protected virtual HapticHitInfo CreateHit(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent)
        {

            float _0_100_force = HapticConstants.Util.ForceCalculateFunction(poly, collision, contactPoint, hapticHitEvent);
            float force = Mathf.Clamp(forceMultiplier * _0_100_force, 0, HapticConstants.MaxForce);

            float forceNormalized = force / HapticConstants.MaxForce;

            return new HapticHitInfo(hapticHitEvent, forceNormalized,
                HapticConstants.DefaultHitDuration, material);
        }

        public abstract HapticCollision OnCollision(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent);
    }

}