using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI
{
    [RequireComponent(typeof(HapticMesh))]
    public class HapticCollider : MonoBehaviour
    {
        [SerializeField]
        private HapticCollisionSolverType collisionSolverType; // TODO rename HapticCollisionSolverType
        private HapticMesh HapticMesh { get; set; }

        public event CollisionHandler CollisionHappened;
        public event RaycastHandler RaycastHappened;

        private HapticCollisionEventsSource CollisionEventsSource;


        public HapticCollisionSolverBase CollisionSolver { get; private set; }


        public event Action<HapticCollisionSolverBase> SolverCreated = delegate { };

        private void Awake()
        {
            HapticMesh = GetComponent<HapticMesh>();
            CollisionEventsSource = HapticMesh.MeshObjectInfo.Root.gameObject.AddComponent<HapticCollisionEventsSource>();
            HapticMesh.HitMappingUpdated += HapticMesh_HitMappingUpdated;

            CollisionHappened += HapticCollider_CollisionHappened;
        }

        private void HapticCollider_CollisionHappened(HapticCollision collision)
        {
            HapticMesh.Hit(collision);
        }

        private void HapticMesh_HitMappingUpdated(IHapticMapping mapping)
        {
            if(mapping != null)
            {
                IHapticInteractor interactor = HapticInteractorFactory.GetInteractor(HapticMesh.MeshObjectInfo, mapping, collisionSolverType);
                CollisionEventsSource.SetInteractor(interactor);

                CollisionEventsSource.CollisionHappened += CollisionHappened;
                CollisionEventsSource.RaycastHappened += RaycastHappened;

                CollisionSolver = (HapticCollisionSolverBase)interactor;
                SolverCreated(CollisionSolver);
            }
            else if (CollisionSolver != null)
            {
                CollisionSolver.Destroy();
                CollisionEventsSource.SetInteractor(null);
                CollisionSolver = null;
            }
        }

        private void OnDestroy()
        {
            Destroy(CollisionEventsSource);
        }
    } 

    public interface IHapticInteractor
    {
        HapticCollision[] CreateHapticCollisions(CollisionWithType[] collisions);
        void AddHapticCollisions(CollisionWithType typedCollision);
        List<HapticCollision> GetHapticCollisions();
        void ClearHapticCollisions();
        bool Raycast(ref HapticRaycastHit hit);
    }

    public delegate void CollisionHandler(HapticCollision collision);
    public delegate void RaycastHandler(HapticRaycastHit hit);
}
