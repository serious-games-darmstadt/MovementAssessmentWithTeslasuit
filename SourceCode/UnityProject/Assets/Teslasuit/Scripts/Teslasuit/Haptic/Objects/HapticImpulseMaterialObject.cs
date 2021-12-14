using System.Threading.Tasks;
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticImpulseMaterialObject : HapticMaterialObject
    {
        [SerializeField]
        private HapticMaterialAsset hapticMaterial;

        protected override HapticMaterialAsset material
        {
            get
            {
                return hapticMaterial;
            }
        }

        public override HapticCollision OnCollision(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent)
        {
            HapticHitInfo hit = CreateHit(poly, collision, contactPoint, hapticHitEvent);
            Task<HapticHitInfo> task = Task.FromResult<HapticHitInfo>(hit);
            return new HapticPolygonCollision(task, poly);
        }
    }
}