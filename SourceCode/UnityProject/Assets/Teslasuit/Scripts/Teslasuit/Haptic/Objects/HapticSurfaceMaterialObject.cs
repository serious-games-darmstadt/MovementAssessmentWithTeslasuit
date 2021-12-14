using System;
using UnityEngine;
using System.Threading.Tasks;

namespace TeslasuitAPI
{
    public class HapticSurfaceMaterialObject : HapticImpulseMaterialObject
    {
        public Texture2D heightMap;
        public Material bumpedMaterial;

        [SerializeField]
        private float _minHeight = 0.0f;
        [SerializeField]
        private float _maxHeight = 1.0f;

        public TriangleRaycastCompute Solver { get; private set; }
        private BufferedQueueDispatcher<Ray, InterHitInfo> BufferedRayDispatcher { get; set; }

        struct InterHitInfo
        {
            public HapticHitInfo hitInfo;
            public TaskCompletionSource<HapticHitInfo> tcs;

            public InterHitInfo(HapticHitInfo hitInfo, TaskCompletionSource<HapticHitInfo> tcs)
            {
                this.hitInfo = hitInfo;
                this.tcs = tcs;
            }
        }


        private void Awake()
        {
            NullReferenceCheck();
            Solver = GetComponent<TriangleRaycastCompute>();
            if (Solver == null)
                Solver = gameObject.AddComponent<TriangleRaycastCompute>();
            if(heightMap == null)
                heightMap = (Texture2D)TryGetBumpedTexture();
            NullReferenceCheck();
            BufferedRayDispatcher = new BufferedQueueDispatcher<Ray, InterHitInfo>(Solver.MaxCastsCount);
        }

        private void NullReferenceCheck()
        {
            if (heightMap == null)
                throw new NullReferenceException("No height map found.");
            if (bumpedMaterial == null)
                throw new NullReferenceException("No material found.");

        }

        private void OnDispatch(Ray[] rays, int count, object opaque)
        {
            Solver.Raycast(rays, count, OnRaycastResult, opaque);
        }

        public override HapticCollision OnCollision(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent)
        {
            TaskCompletionSource<HapticHitInfo> tcs = new TaskCompletionSource<HapticHitInfo>();
            Task<HapticHitInfo> task = tcs.Task;

            HapticHitInfo hitInfo = base.CreateHit(poly, collision, contactPoint, hapticHitEvent);

            InterHitInfo interHitInfo = new InterHitInfo(hitInfo, tcs);

            Ray ray = new Ray(contactPoint.point + contactPoint.normal * 0.1f, -contactPoint.normal);

            BufferedRayDispatcher.Add(ray, interHitInfo);
            return new HapticPolygonCollision(task, poly);
        }

        private void OnRaycastResult(CGRayCast[] raycasts, int count, object opaque)
        {
            Tuple<InterHitInfo[], int> hits = (Tuple<InterHitInfo[], int>)opaque;
            for (int i = 0; i < raycasts.Length && i < count; i++)
            {
                if (raycasts[i].triangleIndex != -1)
                {
                    TaskCompletionSource<HapticHitInfo> tcs = hits.Item1[i].tcs;

                    HapticHitInfo hitInfo = hits.Item1[i].hitInfo;
                    float impact = GetImpact(raycasts[i].uv);
                    hitInfo.impact *= impact;

                    tcs.SetResult(hitInfo);
                }
            }

        }

        private float GetImpact(Vector2 uv)
        {
            float gs = Grayscale(uv);
            return Mathf.Clamp(gs, _minHeight, _maxHeight);
        }

        protected float Grayscale(Vector2 textureCoordinates)
        {
            return heightMap.GetPixelBilinear(textureCoordinates.x, textureCoordinates.y).grayscale;
        }

        private void FixedUpdate()
        {
            BufferedRayDispatcher.Dispatch(OnDispatch);
        }

        [SerializeField]
        private string[] BaseHeightMapTextureNames = { "_ParallaxMap", "_Parallax", "_DispTex", "_Displacement", "_HeightMap", "_BumpMap" };

        private Texture TryGetBumpedTexture()
        {
            foreach (string texName in BaseHeightMapTextureNames)
            {
                if (bumpedMaterial.HasProperty(texName))
                    return bumpedMaterial.GetTexture(texName);
            }

            throw new NullReferenceException("No bump(height) map texture found. Set { Texture Property Name } to get a texture.");
        }
    } 
}
