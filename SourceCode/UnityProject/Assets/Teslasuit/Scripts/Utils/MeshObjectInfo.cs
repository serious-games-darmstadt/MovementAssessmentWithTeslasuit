using System;
using UnityEngine;

public class MeshObjectInfo
{
    public Mesh Mesh { get; private set; }

    public int[] SubMeshIndexes { get; private set; }

    public Transform[] Bones { get; private set; }

    public Transform Root { get; private set; }

    public Transform Container { get; private set; }

    public Vector3[] Vertices { get; private set; }

    public MeshObjectInfo(GameObject gameObject, MeshFilter meshFilter, int[] subMeshIndexes)
    {
        if (gameObject == null || meshFilter == null) throw new ArgumentException();

        this.Mesh = meshFilter.mesh;
        this.Bones = new Transform[] { gameObject.transform };
        this.Root = gameObject.transform;
        this.Container = gameObject.transform;
        this.SubMeshIndexes = subMeshIndexes;
        this.Vertices = Mesh.vertices;
    }

    public MeshObjectInfo(GameObject gameObject, SkinnedMeshRenderer skinnedMesh, int[] subMeshIndexes)
    {
        if (gameObject == null || skinnedMesh == null) throw new ArgumentException();

        this.Mesh = skinnedMesh.sharedMesh;
        this.Bones = skinnedMesh.bones;
        this.Root = skinnedMesh.rootBone;
        this.Container = gameObject.transform;
        this.SubMeshIndexes = subMeshIndexes;
        this.Vertices = skinnedMesh.sharedMesh.vertices;

    }
}
