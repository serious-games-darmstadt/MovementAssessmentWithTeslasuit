using UnityEngine;

using System;
using TeslasuitAPI.Utils;

namespace TeslasuitAPI
{
    [RequireComponent(typeof(MeshFilter))]
    public class TriangleRaycastCompute : MonoBehaviour
    {
        private const string ComputeShaderName = "TriangleRaycast";
        protected ComputeShader Compute { get { return _compute; } }
        [SerializeField]
        private ComputeShader _compute = null;

        protected Mesh Mesh;

        protected CGTriangle[] CgTriangles;
        protected CGRayCast[] CgRaycastsResults;


        protected int RaycastGroupsCount;
        protected int CollectGroupsCount;

        protected int RaycastToBufferKernel;
        protected int CollectKernel;
        protected int PrebuildKernel;


        protected uint GroupDimX
        {
            get { return _groupDimX; }
        }

        protected uint GroupDimSx
        {
            get { return _groupDimSx; }
        }

        private uint _groupDimX;
        private uint _groupDimSx;

        //KERNELS
        private const string RaycastsToBufferKernelName = "RaycastToBuffer";
        private const string CollectBlocksKernelName = "CollectBlocks";
        private const string PrebuildKernelName = "Prebuild";
        //BUFFER PROPERTIES
        protected ComputeBuffer TrianglesBuffer { get; private set; }
        protected ComputeBuffer PrebuiltTrianglesBuffer { get; private set; }
        protected ComputeBuffer RaycastsBuffer { get; private set; }
        protected ComputeBuffer RaycastsOutBuffer { get; private set; }

        private const string TrianglesPropName = "triangles";
        private const string RaycastsPropName = "raycasts";
        private const string RaycastsOutPropName = "raycastsOut";
        private const string PrebuiltTrianglesPropName = "prebuiltTriangles";
        private const string PrebuiltTrianglesConstPropName = "prebuiltTrianglesConst";

        //PROPERTIES
        private const string RaycastsCountPropName = "raycasts_length";
        private const string RayOriginPropName = "ray_origin";
        private const string RayDirectionPropName = "ray_direction";
        private const string RayIndexPropName = "ray_index";

        private int RaycastsCountPropId;
        private int RayOriginPropId;
        private int RayDirectionPropId;
        private int RayIndexPropId;

        public int MaxCastsCount { get { return maxCastsCount; } private set { maxCastsCount = value; } }
        [SerializeField]
        private int maxCastsCount = 128;

        private Ray[] rays;

        protected virtual void Awake()
        {
            rays = new Ray[MaxCastsCount];
            RaycastsCountPropId = Shader.PropertyToID(RaycastsCountPropName);
            RayOriginPropId = Shader.PropertyToID(RayOriginPropName);
            RayDirectionPropId = Shader.PropertyToID(RayDirectionPropName);
            RayIndexPropId = Shader.PropertyToID(RayIndexPropName);

            _compute = (ComputeShader)Instantiate(Resources.Load(ComputeShaderName));
            Mesh = GetComponent<MeshFilter>().sharedMesh;

            Vector3[] vertices = Mesh.vertices;
            Vector2[] uvs = Mesh.uv;
            int[] triangles = Mesh.triangles;

            RaycastToBufferKernel = _compute.FindKernel(RaycastsToBufferKernelName);
            CollectKernel = _compute.FindKernel(CollectBlocksKernelName);
            PrebuildKernel = _compute.FindKernel(PrebuildKernelName);

            uint y, z;
            _compute.GetKernelThreadGroupSizes(RaycastToBufferKernel, out _groupDimX, out y, out z);
            _compute.GetKernelThreadGroupSizes(CollectKernel, out _groupDimSx, out y, out z);

            RaycastGroupsCount = Mathf.CeilToInt((triangles.Length / 3) / (float)GroupDimX);
            CollectGroupsCount = Mathf.CeilToInt(RaycastGroupsCount / (float)GroupDimSx);

            CgTriangles = CGTriangle.GetBuffer(triangles, uvs, vertices);
            CgRaycastsResults = CGRayCast.GetBuffer(MaxCastsCount);


            TrianglesBuffer = new ComputeBuffer(CgTriangles.Length, CGTriangle.Size);
            PrebuiltTrianglesBuffer = new ComputeBuffer(CgTriangles.Length, 48);
            RaycastsBuffer = new ComputeBuffer(RaycastGroupsCount, CGRayCast.Size);
            RaycastsOutBuffer = new ComputeBuffer(MaxCastsCount, CGRayCast.Size);

            RaycastsOutBuffer.SetData(CgRaycastsResults);
            TrianglesBuffer.SetData(CgTriangles);

            _compute.SetBuffer(RaycastToBufferKernel, TrianglesPropName, TrianglesBuffer);
            _compute.SetBuffer(RaycastToBufferKernel, RaycastsPropName, RaycastsBuffer);
            _compute.SetBuffer(CollectKernel, RaycastsOutPropName, RaycastsOutBuffer);
            PrebuildTriangles();
        }

        private void PrebuildTriangles()
        {
            _compute.SetBuffer(PrebuildKernel, PrebuiltTrianglesPropName, PrebuiltTrianglesBuffer);
            _compute.SetBuffer(PrebuildKernel, TrianglesPropName, TrianglesBuffer);
            _compute.Dispatch(PrebuildKernel, RaycastGroupsCount, 1, 1);
            _compute.SetBuffer(RaycastToBufferKernel, PrebuiltTrianglesConstPropName, PrebuiltTrianglesBuffer);
        }

        public virtual void Raycast(Ray[] rays, int count, Action<CGRayCast[], int, object> callback = null, object opaque = null)
        {
            for (int i = 0; i < rays.Length && i < MaxCastsCount && i < count; i++)
                RaycastToBuffer(rays[i], i);

            RequestBufferData(count, CgRaycastsResults, RaycastsOutBuffer, callback, opaque);
            ShowRaycasts(count);
        }

        protected virtual void RequestBufferData(int count, CGRayCast[] results, ComputeBuffer computeBuffer, Action<CGRayCast[], int, object> callback, object opaque = null)
        {
            computeBuffer.GetData(results, 0, 0, count);
            callback?.Invoke(results, count, opaque);
        }

        public virtual void Raycast(Ray ray, Action<CGRayCast[], int, object> callback = null, object opaque = null)
        {
            RaycastToBuffer(ray);
            RequestBufferData(1, CgRaycastsResults, RaycastsOutBuffer, callback, opaque);
        }

        private void RaycastToBuffer(Ray ray, int index = 0)
        {
            UpdateInParameters(ray, RaycastGroupsCount, index);
            _compute.Dispatch(RaycastToBufferKernel, RaycastGroupsCount, 1, 1);

            _compute.SetBuffer(CollectKernel, RaycastsPropName, RaycastsBuffer);

            int currentGroupsCount = CollectGroupsCount;

            while (currentGroupsCount >= 1)
            {
                _compute.Dispatch(CollectKernel, currentGroupsCount, 1, 1);
                if (currentGroupsCount == 1) break;
                currentGroupsCount = Mathf.Clamp(currentGroupsCount / (int)GroupDimSx, 1, CollectGroupsCount);
            }
        }

        private void UpdateInParameters(Ray ray, int raycastGroupsCount, int rayIndex)
        {
            _compute.SetInt(RaycastsCountPropId, RaycastGroupsCount);
            _compute.SetVector(RayOriginPropId, ray.origin);
            _compute.SetVector(RayDirectionPropId, ray.direction);
            _compute.SetInt(RayIndexPropId, rayIndex);
        }

        protected void ShowRaycasts(int count)
        {
            for (int i = 0; i < CgRaycastsResults.Length && i < count; i++)
            {
                var raycast = CgRaycastsResults[i];
                if (raycast.triangleIndex != -1)
                {
                    CGTriangle tr = CgTriangles[raycast.triangleIndex];
                    Vector3 a = transform.TransformPoint(tr.v0);
                    Vector3 b = transform.TransformPoint(tr.v1);
                    Vector3 c = transform.TransformPoint(tr.v2);
                    Debug.DrawLine(a, b, Color.red);
                    Debug.DrawLine(b, c, Color.red);
                    Debug.DrawLine(a, c, Color.red);
                }

            }
        }



        private void OnDestroy()
        {
            RaycastsBuffer.Release();
            TrianglesBuffer.Release();
            RaycastsOutBuffer.Release();
            PrebuiltTrianglesBuffer.Release();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                int raysCount = 1;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 100f);
                ray = MeshUtil.InverseTransformRay(ray, transform);
                for (int i = 0; i < raysCount; i++)
                {
                    rays[i] = ray;
                }


                Raycast(rays, raysCount);
            }
        }

    }


    public struct CGTriangle
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;

        public static CGTriangle[] GetBuffer(int[] triangles, Vector2[] uvs, Vector3[] vertices)
        {
            var cgTriangles = new CGTriangle[triangles.Length / 3];
            for (int i = 0, t = 0; i < triangles.Length - 2; i += 3, t++)
            {
                cgTriangles[t] = new CGTriangle()
                {
                    v0 = vertices[triangles[i]],
                    v1 = vertices[triangles[i + 1]],
                    v2 = vertices[triangles[i + 2]],
                    uv0 = uvs[triangles[i]],
                    uv1 = uvs[triangles[i + 1]],
                    uv2 = uvs[triangles[i + 2]],
                };
            }

            return cgTriangles;
        }

        public const int Size = 60;
    };

    public struct CGRayCast
    {
        public int triangleIndex;
        public Vector2 uv;
        public float distance;

        public static CGRayCast[] GetBuffer(int count)
        {
            CGRayCast[] CGRaycasts = new CGRayCast[count];
            for (int i = 0; i < count; i++)
            {
                CGRaycasts[i] = new CGRayCast()
                {
                    triangleIndex = -1,
                    uv = Vector2.zero,
                    distance = 3.402823466e+37F

                };
            }
            return CGRaycasts;
        }

        /*public static void FillNativeBuffer(NativeArray<CGRayCast> CGRaycasts, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CGRaycasts[i] = new CGRayCast()
                {
                    triangleIndex = -1,
                    uv = Vector2.zero,
                    distance = 3.402823466e+37F

                };
            }
        }*/

        public const int Size = 16;
    }; 
}