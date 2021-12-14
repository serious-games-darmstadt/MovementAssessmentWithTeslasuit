using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticInteractorFactory
    {
        public static IHapticInteractor GetInteractor(MeshObjectInfo meshObjectInfo, IHapticMapping hapticMapping, HapticCollisionSolverType type)
        {
            return new HapticMeshCollisionSolver(meshObjectInfo, hapticMapping);
        }
    }
}