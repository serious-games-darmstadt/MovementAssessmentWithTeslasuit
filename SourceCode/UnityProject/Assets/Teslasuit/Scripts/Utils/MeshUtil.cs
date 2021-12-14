using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeslasuitAPI.Utils
{
    public static class MeshUtil
    {
        public static Bounds GetRecalculatedBounds(Renderer renderer)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = renderer.gameObject.GetComponent<SkinnedMeshRenderer>();
            if(skinnedMeshRenderer != null)
                skinnedMeshRenderer.sharedMesh.RecalculateBounds();
            else
            {
                MeshFilter meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
                meshFilter.sharedMesh.RecalculateBounds();
            }
            return renderer.bounds;
        }

        public static Vector3 GetBarycentric(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 p)
        {
            Vector3 B = new Vector3();
            B.x = ((v2.y - v3.y) * (p.x - v3.x) + (v3.x - v2.x) * (p.y - v3.y)) /
                ((v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y));
            B.y = ((v3.y - v1.y) * (p.x - v3.x) + (v1.x - v3.x) * (p.y - v3.y)) /
                ((v3.y - v1.y) * (v2.x - v3.x) + (v1.x - v3.x) * (v2.y - v3.y));
            B.z = 1 - B.x - B.y;
            return B;
        }

        public static Vector3 UVTo3D(Vector2[] uvs, Vector3[] vertices, Triangle triangle, Vector2 uv)
        {
            Vector3 pt = GetBarycentric(uvs[triangle.a], uvs[triangle.b], uvs[triangle.c], uv);
            pt = pt.x * vertices[triangle.a] + pt.y * vertices[triangle.b] + pt.z * vertices[triangle.c];
            return pt;
        }

        public static MeshEntity[] CreateEntitiesFromUV(MeshEntity source, UnityPolygon polygon, out int boneIndex)
        {
            return SelectFromUV(source, polygon, false, out boneIndex).ToArray();
        }

        public static Mesh CreateMeshFromUV(Mesh source, UnityPolygon polygon, int sub_mesh_index, out int boneIndex)
        {
            MeshEntity meshEntity = SelectFromUV(source, polygon, sub_mesh_index, false, out boneIndex);
            return CreateMesh(meshEntity);
        }

        private static MeshEntity SelectFromUV(MeshEntity entity, UnityPolygon polygon, bool subdivide, out int boneIndex, int polygons_count = 60)
        {
            BoneWeight[] boneWeights = entity.boneWeights;
            boneIndex = -1;
            Vector2[] uvs = entity.uvs;
            Vector3[] vertices = new Vector3[entity.vertices.Length];
            Array.Copy(entity.vertices, vertices, vertices.Length);

            int[] triangles = entity.triangles;

            List<int> containedTriangles = new List<int>(triangles.Length / polygons_count);

            Dictionary<int, int> boneIndexes = new Dictionary<int, int>(polygons_count);

            List<Vector3> newVertices = new List<Vector3>();
            
            List<int> newTrinagles = new List<int>();

            SelectTriangles(polygon.vertices, uvs, vertices, triangles, boneWeights, ref containedTriangles, ref boneIndexes, ref newVertices, ref newTrinagles);

            int[] newLocalIndices = newTrinagles.ToArray();

            for(int i=0; i<newLocalIndices.Length; i++)
            {
                newLocalIndices[i] += vertices.Length;
            }
            Array.Resize(ref vertices, vertices.Length + newVertices.Count);
            containedTriangles.AddRange(newLocalIndices);

            Array.Copy(newVertices.ToArray(), 0, vertices, vertices.Length - newVertices.Count, newVertices.Count);

            int max = -1;

            foreach (var kv in boneIndexes)
            {
                if (kv.Value > 0 && kv.Value > max)
                {
                    max = kv.Value;
                    boneIndex = kv.Key;
                }
            }

            return new MeshEntity(containedTriangles.ToArray(), vertices, subdivide, true);
        }

        private static MeshEntity SelectFromUV(Mesh source, UnityPolygon polygon, int sub_mesh_index, bool subdivide, out int boneIndex)
        {
            return SelectFromUV(new MeshEntity(source, sub_mesh_index), polygon, subdivide, out boneIndex);
        }


        private static void TryAddIndex(Dictionary<int, int> boneIndexes, BoneWeight weight, float minWeight = 0.5f)
        {
            if (!boneIndexes.ContainsKey(weight.boneIndex0))
                boneIndexes.Add(weight.boneIndex0, 0);
            if (weight.weight0 > minWeight)
                boneIndexes[weight.boneIndex0] = ++boneIndexes[weight.boneIndex0];
        }

        public static Mesh CreateMesh(MeshEntity entity)
        {
            Mesh mesh = new Mesh();

            mesh.vertices = entity.vertices;
            mesh.triangles = entity.triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh CreateMesh(MeshEntity entity, Matrix4x4 rotationMatrix)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[entity.vertices.Length];
            
            for(int i=0; i< vertices.Length; ++i)
            {
                vertices[i] = rotationMatrix.MultiplyPoint(entity.vertices[i]);
            }

            mesh.vertices = vertices;
            mesh.triangles = entity.triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void TrimMesh(Vector3[] oldVertices, int[] oldTriangles, out Vector3[] newVertices, out int[] newTrianlges)
        {
            newVertices = new Vector3[oldTriangles.Length];
            newTrianlges = new int[oldTriangles.Length];

            int count = 0;
            for (int i = 0; i < oldTriangles.Length; i++)
            {
                int currIndex = oldTriangles[i];

                if (!ContainsIndex(oldTriangles, currIndex, 0, i))
                {
                    for (int t = i; t < oldTriangles.Length; t++)
                    {
                        if (oldTriangles[t] == currIndex)
                            newTrianlges[t] = count;
                    }

                    newVertices[count] = oldVertices[currIndex];
                    count++;
                }

            }

            Array.Resize(ref newVertices, count);
        }


        private static bool ContainsIndex(int[] triangles, int index, int offset, int length)
        {
            for (int i = offset; i < length && i < triangles.Length; i++)
            {
                if (triangles[i] == index)
                    return true;
            }
            return false;
        }


        private static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        private static bool PolyContainsPoint(Vector2[] polyPoints, Vector2 point)
        {
            var j = polyPoints.Length - 1;
            var inside = false;
            for (int i = 0; i < polyPoints.Length; j = i++)
            {
                if (((polyPoints[i].y <= point.y && point.y < polyPoints[j].y) || (polyPoints[j].y <= point.y && point.y < polyPoints[i].y)) &&
                   (point.x < (polyPoints[j].x - polyPoints[i].x) * (point.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
                    inside = !inside;
            }
            return inside;
        }

        private static bool PolyContainsTriangle(Vector2[] polyPoints, Vector2[] uvs, Triangle triangle, int contains_count_needed = 2)
        {
            contains_count_needed = Mathf.Clamp(contains_count_needed, 1, 3);
            int contains_count = 0;

            if (PolyContainsPoint(polyPoints, uvs[triangle.a]) && ++contains_count >= contains_count_needed)
                return true;
            if (PolyContainsPoint(polyPoints, uvs[triangle.b]) && ++contains_count >= contains_count_needed)
                return true;
            if (PolyContainsPoint(polyPoints, uvs[triangle.c]) && ++contains_count >= contains_count_needed)
                return true;

            return false;
        }

        


        private static Vector2[] CreateIntersectedTrinagles(Vector2[] poly, Vector2[] triangle, ref List<int> newTriangles)
        {
            
            const float clipperScale = 10e+5f;
            float maximum = 1024;
            float maximumScale = (float)Int32.MaxValue / maximum;
            float scale = Mathf.Min(clipperScale, maximumScale);

            List<List<ClipperLib.IntPoint>> subjectPaths = new List<List<ClipperLib.IntPoint>>();
            List<List<ClipperLib.IntPoint>> clipPaths = new List<List<ClipperLib.IntPoint>>();

            {
                List<ClipperLib.IntPoint> subjectPath = new List<ClipperLib.IntPoint>();

                EnumerateArray(poly, (point) =>
                {
                    subjectPath.Add(new ClipperLib.IntPoint(point.x * scale, point.y * scale));
                });
                subjectPaths.Add(subjectPath);
            }

            List<ClipperLib.IntPoint> clipPath = new List<ClipperLib.IntPoint>();
            EnumerateArray(triangle, (point) =>
            {
                clipPath.Add(new ClipperLib.IntPoint(point.x * scale, point.y * scale));
            });

            clipPaths.Add(clipPath);

            List<List<ClipperLib.IntPoint>> result = new List<List<ClipperLib.IntPoint>>();

            ClipperLib.Clipper clipper = new ClipperLib.Clipper();
            clipper.AddPaths(subjectPaths, ClipperLib.PolyType.ptClip, true);
            clipper.AddPaths(clipPaths, ClipperLib.PolyType.ptSubject, true);

            clipper.Execute(ClipperLib.ClipType.ctIntersection, result);

            List<List<ClipperLib.IntPoint>> simplified = ClipperLib.Clipper.SimplifyPolygons(result);


            List<Vector2[]> polygons = new List<Vector2[]>();

            for (int index = 0; index < simplified.Count; index++)
            {
                List<ClipperLib.IntPoint> eachSolutionPath = simplified[index];
                List<Vector2> eachSolutionPolygon = PolygonFromClipperPath(eachSolutionPath, scale);

                polygons.Add(eachSolutionPolygon.ToArray());
            }
            List<Vector2> newVertices = null;
            if(newVertices == null)
                newVertices = new List<Vector2>();
            if(newTriangles == null)
                newTriangles = new List<int>();

            
            int freeIndex = newTriangles.Count > 0 ? Mathf.Max(newTriangles.ToArray()) + 1 : 0;
            for(int i=0; i<polygons.Count; i++)
            {
                newVertices.AddRange(polygons[i]);
                int[] triangulated = Triangulate(polygons[i]);
                
                for(int t=0; t<triangulated.Length; t++)
                {
                    triangulated[t] += freeIndex;
                }

                freeIndex += polygons[i].Length;
                newTriangles.AddRange(triangulated);
            }
            return newVertices.ToArray();
        }

        private static List<Vector2> PolygonFromClipperPath(List<ClipperLib.IntPoint> path, float scale)
        {
            List<Vector2> points = new List<Vector2>();
            for (int index = path.Count - 1; index >= 0; index--) // Reverse enumeration (to flip normals)
            {
                ClipperLib.IntPoint eachPoint = path[index];
                points.Add(new Vector2(eachPoint.X / scale, eachPoint.Y / scale));
            }
            return points;
        }

        private static int[] Triangulate(Vector2[] polygon)
        {
            return new Triangulator(polygon).Triangulate();
        }


        private static void EnumerateArray<T>(T[] items, Action<T> action)
        {
            for (int i = 0; i < items.Length; i++)
            {
                action(items[i]);
            }
        }

        private static void SelectTriangles(Vector2[] polyPoints, Vector2[] uvs, Vector3[] vertices, int[] triangles, BoneWeight[] boneWeights, ref List<int> outTriangles, ref Dictionary<int, int> boneIndexes, ref List<Vector3> newVertices, ref List<int> newTriangles)
        {
            outTriangles.Clear();
            boneIndexes.Clear();
            for (int i = 0; i < triangles.Length - 2; i += 3)
            {
                int a = triangles[i], b = triangles[i + 1], c = triangles[i + 2];

                int containmentsCount = 0;

                if (PolyContainsPoint(polyPoints, uvs[a]))
                    containmentsCount++;
                if (PolyContainsPoint(polyPoints, uvs[b]))
                    containmentsCount++;
                if (PolyContainsPoint(polyPoints, uvs[c]))
                    containmentsCount++;

                if(containmentsCount > 0)
                {
                    TryAddIndex(boneIndexes, boneWeights[a]);
                    TryAddIndex(boneIndexes, boneWeights[b]);
                    TryAddIndex(boneIndexes, boneWeights[c]);

                    if (containmentsCount == 3)
                    {
                        outTriangles.Add(a);
                        outTriangles.Add(b);
                        outTriangles.Add(c);
                    }
                    else
                    {
                        var intersected = CreateIntersectedTrinagles(polyPoints, new Vector2[] { uvs[a], uvs[b], uvs[c], uvs[a]}, ref newTriangles);

                        for(int p=0; p<intersected.Length; p++)
                        {
                            newVertices.Add(UVTo3D(uvs, vertices, new Triangle() { a = a, b = b, c = c }, intersected[p]));
                        }
                    }
                }
            }
        }

        public static Ray InverseTransformRay(Ray ray, Transform transform)
        {
            ray.origin = transform.InverseTransformPoint(ray.origin);
            ray.direction = transform.InverseTransformDirection(ray.direction);
            return ray;
        }

        public class MeshEntity : IEnumerator<MeshEntity>, IEnumerable<MeshEntity>
        {

            public int[] triangles { get; private set; }
            public Vector3[] vertices { get; private set; }
            public Vector2[] uvs { get; private set; }
            public BoneWeight[] boneWeights { get; private set; }
            public MeshEntity Next { get; private set; }

            public MeshEntity Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current { get { return current; } }

            public MeshEntity(Mesh source, int submesh_index)
            {
                this.triangles = source.GetTriangles(submesh_index);
                this.vertices = source.vertices;
                this.uvs = source.uv;
                this.boneWeights = source.boneWeights;
            }

            public MeshEntity(Mesh source, Vector3[] vertices, int submesh_index)
            {
                this.triangles = source.GetTriangles(submesh_index);
                this.vertices = vertices;
                this.uvs = source.uv;
                this.boneWeights = source.boneWeights;
            }

            public MeshEntity(int[] triangles, Vector3[] vertices, bool subdivide, bool trim)
            {
                this.triangles = triangles;
                this.vertices = vertices;
                this.uvs = uvs;
                this.boneWeights = boneWeights;

                if (subdivide)
                    Subdivide();
                else if (trim)
                    Trim();

                Reset();
            }


            public void Subdivide()
            {
                if (triangles.Length < 3) return;
                List<int> chainSelected = new List<int>();
                List<int> trianglesList = new List<int>(triangles);

                CollectNeighborhoods(trianglesList, 0, chainSelected);

                if (triangles.Length - chainSelected.Count > 3)
                {
                    int[] nextTriangles = new int[triangles.Length - chainSelected.Count];
                    Array.Copy(triangles, chainSelected.Count, nextTriangles, 0, nextTriangles.Length);
                    Next = new MeshEntity(nextTriangles, vertices, true, false);
                }
                this.triangles = chainSelected.ToArray();

                Trim();
            }

            public void Trim()
            {
                int[] trimmedIndices;
                Vector3[] trimmedVertices;
                TrimMesh(vertices, triangles, out trimmedVertices, out trimmedIndices);
                this.vertices = trimmedVertices;
                this.triangles = trimmedIndices;
            }

            private int[] GetTriangle(List<int> trianglesIn, int triangleIndex)
            {
                if (trianglesIn.Count > triangleIndex + 2)
                    return new int[] { trianglesIn[triangleIndex], trianglesIn[triangleIndex + 1], trianglesIn[triangleIndex + 2] };

                return null;
            }

            private void CollectNeighborhoods(List<int> trianglesIn, int triangleIndex, List<int> trianglesOut)
            {
                if (triangleIndex > trianglesIn.Count - 2) return;
                int[] current = GetTriangle(trianglesIn, triangleIndex);
                trianglesOut.AddRange(current);
                trianglesIn.RemoveRange(triangleIndex, 3);

                for (int i = 0; i < trianglesIn.Count; i += 3)
                {
                    if (IsTriangleConnectedWithTriangle(trianglesIn, i, current))
                    {
                        CollectNeighborhoods(trianglesIn, i, trianglesOut);

                    }
                }
            }

            private bool IsTriangleConnectedWithTriangle(List<int> triangles, int triangleAIndex, int[] triangleB)
            {
                for (int i = triangleAIndex; i < triangleAIndex + 3; i++)
                {
                    for (int j = 0; j < triangleB.Length; j++)
                    {
                        if (triangles[i] == triangleB[j])
                            return true;
                    }
                }
                return false;
            }


            //IEnumerator
            public bool MoveNext()
            {
                if (current == this && first)
                {
                    first = false;
                    return true;
                }

                current = current.Next;
                return current != null;
            }

            //IEnumerable
            public void Reset()
            { current = this; first = true; }

            private bool first = true;

            public void Dispose()
            {

            }

            public IEnumerator<MeshEntity> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            private MeshEntity current;
        }


        public struct Triangle
        {
            public int a;
            public int b;
            public int c;

            public Vector2 uv;

            public bool IsEmpty()
            {
                return this == Zero;
            }

            public static readonly Triangle Zero = new Triangle() { a = -1, b = -1, c = -1 };

            public static bool operator ==(Triangle a, Triangle b)
            {
                return a.a == b.a && a.b == b.b && a.c == b.c;
            }
            public static bool operator !=(Triangle a, Triangle b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                Triangle other = (Triangle)obj;

                return a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.b);
            }


            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + a.GetHashCode();
                hash = hash * 23 + b.GetHashCode();
                hash = hash * 23 + c.GetHashCode();
                return hash;
            }
        }
    }

}