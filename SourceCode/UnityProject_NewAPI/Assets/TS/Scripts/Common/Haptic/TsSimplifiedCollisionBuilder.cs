using System;
using System.Collections.Generic;
using System.IO;
using TsAPI.Types;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class TsSimplifiedCollisionBuilder : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 0.99f)]
    private float m_boneWeightThreshold = 0.45f;

    private SkinnedMeshRenderer meshRenderer;

    [SerializeField]
    private TsAvatarSettings m_avatarSettings;

    [SerializeField]
    private TsHapticSimplifiedChannel[] channels;

    [SerializeField]
    private TsHapticPlayer m_hapticPlayer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();

    }

    private bool HasActiveBoneWeight(BoneWeight bw, int transformIndex)
    {
        float threshold = m_boneWeightThreshold;
        if (bw.boneIndex0 == transformIndex && bw.weight0 > threshold)
        {
            return true;
        }

        if (bw.boneIndex1 == transformIndex && bw.weight1 > threshold)
        {
            return true;
        }

        if (bw.boneIndex2 == transformIndex && bw.weight2 > threshold)
        {
            return true;
        }

        if (bw.boneIndex3 == transformIndex && bw.weight3 > threshold)
        {
            return true;
        }
        return false;
    }

    

    void Build()
    {
        var boneWeights = meshRenderer.sharedMesh.boneWeights;
        var vertices = meshRenderer.sharedMesh.vertices;

        var existing = meshRenderer.rootBone.GetComponentsInChildren<TsHapticCollisionHandler>();

        foreach (var channel in existing)
        {
            DestroyImmediate(channel.gameObject);
        }

        foreach(var channel in channels)
        {
            var handler = BuildChannel(channel, boneWeights, vertices);
        }

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }

    private TsHapticCollisionHandler BuildChannel(TsHapticSimplifiedChannel channel, BoneWeight[] boneWeights, Vector3[] vertices)
    {
        var transformName = m_avatarSettings.GetTransformName(channel.BoneIndex);
        var transformIndex = GetTransformIndex(transformName);
        var boneTransform = meshRenderer.bones[transformIndex];
        var channeObj = new GameObject(channel.name);
        channeObj.transform.position = boneTransform.position;
        channeObj.transform.rotation = boneTransform.rotation;
        channeObj.transform.localScale = boneTransform.lossyScale;
        channeObj.transform.SetParent(boneTransform);

        var channelCollider = channeObj.AddComponent<MeshCollider>();

        var mesh = GetMesh(channel, boneWeights, vertices);
        if (mesh == null)
        {
            DestroyImmediate(channeObj);
            return null;
        }
#if UNITY_EDITOR
        SerializeMesh(mesh);
        mesh = LoadMesh(channel.name);
#endif
        channelCollider.sharedMesh = mesh;
        channelCollider.convex = true;
        var handler = channeObj.AddComponent<TsHapticCollisionHandler>();
        handler.Channel = channel;
        handler.HapticPlayer = m_hapticPlayer;
        return handler;
    }

    private Mesh GetMesh(TsHapticSimplifiedChannel channel, BoneWeight[] boneWeights, Vector3[] vertices)
    {
        var transformName = m_avatarSettings.GetTransformName(channel.BoneIndex);
        var transformIndex = GetTransformIndex(transformName);
        if (transformIndex == -1)
        {
            Debug.LogWarning($"Failed to get transform index with name: {transformName}");
            return null;
        }
        var boneTransform = meshRenderer.bones[transformIndex];
        var child = boneTransform.GetChild(0);

        var poseRotation = m_avatarSettings.GetIPoseRotation(channel.BoneIndex);

        var delta = Quaternion.Inverse(poseRotation * Quaternion.Inverse(boneTransform.rotation));

        Vector3 pointA = boneTransform.position;
        Vector3 pointB = child.position;

        Vector3 pointC = (pointA + pointB) / 2 + delta * GetSidePlanePoint(channel.BoneIndex);

        Plane plane = new Plane(pointA, pointB, pointC);
        
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < boneWeights.Length; ++i)
        {
            var bw = boneWeights[i];
            var worldV = meshRenderer.rootBone.parent.TransformPoint(vertices[i]);

            var localV = boneTransform.InverseTransformPoint(worldV);
            if (HasActiveBoneWeight(bw, transformIndex))
            {
                if (plane.GetSide(worldV) == (channel.BoneSide == TsBone2dSide.Front))
                {
                    points.Add(localV);
                }
            }
        }

        if (points.Count < 4)
        {
            return null;
        }
        var calculator = new GK.ConvexHullCalculator();
        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        calculator.GenerateHull(points, true, ref newVerts, ref newTriangles, ref newNormals);

        var newMesh = new Mesh();
        newMesh.name = channel.name;
        newMesh.SetVertices(newVerts);
        newMesh.SetTriangles(newTriangles, 0);
        newMesh.SetNormals(newNormals);

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    private int GetTransformIndex(string transformName)
    {
        for (int i = 0; i < meshRenderer.bones.Length; ++i)
        {
            if (meshRenderer.bones[i].name == transformName)
            {
                return i;
            }
        }
        return -1;
    }

    private static Vector3 GetSidePlanePoint(TsHumanBoneIndex index)
    {
        switch (index)
        {
            case TsHumanBoneIndex.RightUpperArm:
            case TsHumanBoneIndex.RightLowerArm:
            case TsHumanBoneIndex.RightHand:
            {
                return Vector3.back;
            }
            case TsHumanBoneIndex.LeftUpperArm:
            case TsHumanBoneIndex.LeftLowerArm:
            case TsHumanBoneIndex.LeftHand:
            {
                return Vector3.forward;
            }
            case TsHumanBoneIndex.Spine:
            case TsHumanBoneIndex.Chest:
            {
                return Vector3.left;
            }
            default:
            {
                return Vector3.right;
            }
        }
    }

    public bool build = false;

    // Update is called once per frame
    void Update()
    {
        if(build)
        {
            try
            {
                Build();
            }
            catch (Exception)
            {
            }
            
            build = false;
        }
    }

#if UNITY_EDITOR
    private void SerializeMesh(Mesh mesh)
    {
        var assetPath = UnityEditor.AssetDatabase.GetAssetPath(meshRenderer.sharedMesh);
        var savePath = Path.Combine(Path.GetDirectoryName(assetPath), meshRenderer.sharedMesh.name);
        if (!UnityEditor.AssetDatabase.IsValidFolder(savePath))
        {
            UnityEditor.AssetDatabase.CreateFolder(Path.GetDirectoryName(assetPath), meshRenderer.sharedMesh.name);
        }

        UnityEditor.AssetDatabase.CreateAsset(mesh, Path.Combine(savePath, mesh.name + ".asset"));
    }

    private Mesh LoadMesh(string meshName)
    {
        var assetPath = UnityEditor.AssetDatabase.GetAssetPath(meshRenderer.sharedMesh);
        var savePath = Path.Combine(Path.GetDirectoryName(assetPath), meshRenderer.sharedMesh.name);
        if (!UnityEditor.AssetDatabase.IsValidFolder(savePath))
        {
            UnityEditor.AssetDatabase.CreateFolder(Path.GetDirectoryName(assetPath), meshRenderer.sharedMesh.name);
        }

        return UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(Path.Combine(savePath, meshName + ".asset"));
    }
#endif
}
