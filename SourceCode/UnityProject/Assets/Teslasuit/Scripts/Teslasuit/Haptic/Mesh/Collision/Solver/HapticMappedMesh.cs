using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using TeslasuitAPI.Utils;
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticMappedMesh
    {
        public bool TryGetChannel(int channelIndex, out Polygon poly)
        {
            return HapticChannels.TryGetValue(channelIndex, out poly);
        }

        public ReadOnlyDictionary<int, Polygon> Channels { get; }
        public ReadOnlyDictionary<int, MeshCollider> Colliders { get; }

        private Dictionary<int, Polygon> HapticChannels = new Dictionary<int, Polygon>();
        private Dictionary<int, MeshCollider> HapticColliders = new Dictionary<int, MeshCollider>();

        protected MeshObjectInfo MeshObjectInfo { get; private set; }
        protected IHapticMapping HapticMapping { get; private set; }

        protected UnityPolygon[] UVPolygons { get; private set; }
        protected Polygon[] Polygons { get; private set; }

        public HapticMappedMesh(MeshObjectInfo meshObjectInfo, IHapticMapping hapticMapping)
        {
            Channels = new ReadOnlyDictionary<int, Polygon>(HapticChannels);
            Colliders = new ReadOnlyDictionary<int, MeshCollider>(HapticColliders);
            
            this.MeshObjectInfo = meshObjectInfo;
            this.HapticMapping = hapticMapping;

            Polygons = HapticMapping.Polygons;
            UVPolygons = HapticMappingConversion.ToUVPolygons(Polygons, HapticMapping.Width, HapticMapping.Height);

            Build();
        }

        public void Build()
        {
            Vector3[] sharedVertices = MeshObjectInfo.Vertices;
            MeshUtil.MeshEntity[] meshEntities = new MeshUtil.MeshEntity[MeshObjectInfo.SubMeshIndexes.Length];

            for (int i = 0; i < meshEntities.Length; i++)
            {
                meshEntities[i] = new MeshUtil.MeshEntity(MeshObjectInfo.Mesh, sharedVertices, MeshObjectInfo.SubMeshIndexes[i]);
            }

            if (UVPolygons != null)
            {
                foreach (MeshUtil.MeshEntity submesh in meshEntities)
                {
                    for (int i = 0; i < UVPolygons.Length; i++)
                    {
                        CreateMeshColliders(submesh, UVPolygons[i], Polygons[i]);
                    }
                }
            }
        }


        protected virtual void CreateMeshColliders(MeshUtil.MeshEntity entity, UnityPolygon uv_poly, Polygon polygon)
        {
            int boneIndex = -1;
            Transform boneParent = MeshObjectInfo.Root.transform;
            Transform[] meshBones = MeshObjectInfo.Bones;
            Matrix4x4[] bindPoses = MeshObjectInfo.Mesh.bindposes;
            Matrix4x4 bindPose = Matrix4x4.identity;
            new Thread(() =>
            {
                MeshUtil.MeshEntity[] builtMeshes = MeshUtil.CreateEntitiesFromUV(entity, uv_poly, out boneIndex);

                if (boneIndex != -1 && meshBones != null)
                {
                    boneParent = MeshObjectInfo.Bones[boneIndex];
                    bindPose = bindPoses[boneIndex];
                }
                else
                    return;
                MainThreadDispatcher.Execute(() =>
                {
                    GenerateMeshColliders(builtMeshes, boneParent, bindPose, polygon);
                });
            }).Start();

        }

        private void GenerateMeshColliders(MeshUtil.MeshEntity[] builtMeshes, Transform parent, Matrix4x4 bindPose, Polygon poly)
        {
            Transform root = MeshObjectInfo.Root.root;
            var rotationMatrix = bindPose;

            foreach (MeshUtil.MeshEntity mesh in builtMeshes)
            {
                MeshCollider meshCollider = CreateMeshCollider(MeshUtil.CreateMesh(mesh, rotationMatrix), parent);
                HapticChannels.Add(meshCollider.GetInstanceID(), poly);
                HapticColliders.Add(meshCollider.GetInstanceID(), meshCollider);
            }
        }

        public void SetConvex(bool is_convex)
        {
            foreach (var colliderKV in HapticColliders)
                colliderKV.Value.convex = is_convex;
        }

        private MeshCollider CreateMeshCollider(Mesh mesh, Transform parent)
        {
            GameObject hapticChannel = new GameObject("HapticChannel");
            hapticChannel.layer = HapticConstants.Util.HapticReceiverLayer;
            hapticChannel.transform.SetParent(parent);
            hapticChannel.transform.localRotation = Quaternion.identity;
            hapticChannel.transform.localPosition = Vector3.zero;
            hapticChannel.transform.localScale = Vector3.one;
            MeshCollider channelCollider = hapticChannel.AddComponent<MeshCollider>();

            channelCollider.cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.CookForFasterSimulation;// | MeshColliderCookingOptions.InflateConvexMesh;
            channelCollider.sharedMesh = mesh;
            channelCollider.convex = true;

            return channelCollider;
        }



        public void Destroy()
        {
            foreach (var colliderKV in HapticColliders)
            {
                Object.Destroy(colliderKV.Value.gameObject);
            }
        }
    }
}
