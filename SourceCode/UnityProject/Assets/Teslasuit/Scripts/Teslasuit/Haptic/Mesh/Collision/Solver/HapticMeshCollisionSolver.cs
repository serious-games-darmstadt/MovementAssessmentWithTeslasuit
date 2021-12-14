using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticMeshCollisionSolver : HapticCollisionSolverBase, IHapticInteractor
    {
        private bool _haveRigidbodyOnStart;
        private Rigidbody _rigidbody;

        public HapticMappedMesh HapticMappedMesh { get { return _hapticMappedMesh; } }
        private HapticMappedMesh _hapticMappedMesh;

        public HapticMeshCollisionSolver(MeshObjectInfo meshObjectInfo, IHapticMapping hapticMapping) : base(meshObjectInfo, hapticMapping)
        {
            _hapticMappedMesh = new HapticMappedMesh(MeshObjectInfo, hapticMapping);
            CreateRigidbody();
        }

        protected void CreateRigidbody()
        {
            _rigidbody = MeshObjectInfo.Root.GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = MeshObjectInfo.Root.gameObject.AddComponent<Rigidbody>();
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
                _haveRigidbodyOnStart = false;
            }
            else
                _haveRigidbodyOnStart = true;
        }

        public bool Raycast(ref HapticRaycastHit hapticRaycast)
        {
            if (hapticRaycast.raycastHit.collider == null) return false;
            Polygon poly;
            if (!_hapticMappedMesh.TryGetChannel(hapticRaycast.raycastHit.collider.GetInstanceID(), out poly))
                return false;

            hapticRaycast.hapticReceiver = MeshObjectInfo.Container.GetComponent<HapticReceiver>();
            hapticRaycast.channelPoly = poly;
            return true;
        }


        protected override HapticCollision CreateCollision(Collision collision, HapticHitEvent hapticHitEvent, ContactPoint contactPoint, HapticMaterialObject hapticObject)
        {
            Polygon poly;
            if (!_hapticMappedMesh.TryGetChannel(contactPoint.thisCollider.GetInstanceID(), out poly))
                return null;

            return hapticObject.OnCollision(poly, collision, contactPoint, hapticHitEvent);
        }

        public override void Destroy()
        {
            if (_hapticMappedMesh != null)
                _hapticMappedMesh.Destroy();
            if (_rigidbody && !_haveRigidbodyOnStart)
                Object.Destroy(_rigidbody);

            base.Destroy();
        }
    }

}