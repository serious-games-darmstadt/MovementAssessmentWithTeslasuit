using System;
using System.Collections.Generic;
using System.Linq;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

[Serializable]
public struct TsHumanBone
{
    public TsHumanBoneIndex boneIndex;
    public string boneName;
    public Quaternion iPoseRotation;
    public Quaternion tPoseRotation;
    public Vector3 tPosePosition;
}

[CreateAssetMenu(menuName = "Teslasuit/Motion/AvatarSettings")]
public class TsAvatarSettings : ScriptableObject
{
    [SerializeField]
    private Avatar m_avatar;
    [SerializeField]
    private float m_armsTPoseToIPoseDegrees = 83;
    [SerializeField]
    [HideInInspector]
    private TsHumanBone[] m_bones;
    [SerializeField]
    [HideInInspector]
    private GameObject m_characterModel;

    public bool IsValid { get; private set; }

    private void OnValidate()
    {
        if (m_avatar == null)
        {
            IsValid = false;
            m_bones = null;
            return;
        }

        m_characterModel = GetCharacterModel(m_avatar);
        if (m_characterModel == null)
        {
            IsValid = false;
            m_bones = null;
            return;
        }
        IsValid = m_avatar.isHuman && m_avatar.isValid;
    }

    public void Setup()
    {
        var result = new List<TsHumanBone>();

        var desc = GetHumanDescription();
        var character = InstantiateWithAvatarSettings(m_characterModel, desc);

        var required = TsHumanBones.SuitBones;

        foreach (var reqBoneIndex in required)
        {
            //Debug.Log(reqBoneIndex.ToString());continue;
            try
            {
                var boneStr = reqBoneIndex.ToString();
                var bone = desc.human.Where((item) => item.humanName == boneStr);

                if (bone.Any())
                {
                    var foundBone = bone.First();

                    var targetTransform = TransformUtils.FindChildRecursive(character.transform, foundBone.boneName);
                    var rotation = targetTransform.rotation;
                    var position = targetTransform.position;
                    var adjustedPose = AdjustTPoseToIPose(reqBoneIndex, rotation);
                    result.Add(new TsHumanBone()
                    {
                        boneIndex = reqBoneIndex,
                        boneName = foundBone.boneName,
                        iPoseRotation = adjustedPose,
                        tPoseRotation = rotation,
                        tPosePosition = position
                    });
                }
                else
                {
                    continue;
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }

        DestroyImmediate(character);
        m_bones = result.ToArray();
    }

    public GameObject InstantiateWithAvatarSettings(GameObject obj, HumanDescription desc)
    {
        var character = Instantiate(obj);
        foreach (var bone in desc.skeleton)
        {
            
            try
            {
                var boneTransform = TransformUtils.FindChildRecursive(character.transform, bone.name);
                boneTransform.localRotation = bone.rotation;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message + " " + bone.name);
                // ignored
            }
        }

        return character;
    }

    public string GetTransformName(TsHumanBoneIndex boneIndex)
    {
        return m_bones.FirstOrDefault(item => item.boneIndex == boneIndex).boneName;
    }

    public Quaternion GetIPoseRotation(TsHumanBoneIndex boneIndex)
    {
        return m_bones.FirstOrDefault(item => item.boneIndex == boneIndex).iPoseRotation;
    }

    public Vector3 GetTPosePosition(TsHumanBoneIndex boneIndex)
    {
        return m_bones.FirstOrDefault(item => item.boneIndex == boneIndex).tPosePosition;
    }

    public Quaternion AdjustTPoseToIPose(TsHumanBoneIndex boneIndex, Quaternion quaternion)
    {
        var rightTPoseToIPose = Quaternion.Euler(0, 0, -m_armsTPoseToIPoseDegrees);
        var leftTPoseToIPose = Quaternion.Euler(0, 0, m_armsTPoseToIPoseDegrees);

        switch (boneIndex)
        {
            case TsHumanBoneIndex.RightUpperArm:
            case TsHumanBoneIndex.RightLowerArm:
            case TsHumanBoneIndex.RightHand:
                {
                    quaternion = rightTPoseToIPose * quaternion;
                    break;
                }

            case TsHumanBoneIndex.LeftUpperArm:
            case TsHumanBoneIndex.LeftLowerArm:
            case TsHumanBoneIndex.LeftHand:
                {
                    quaternion = leftTPoseToIPose * quaternion;
                    break;
                }
        }

        return quaternion;
    }

    private HumanDescription GetHumanDescription()
    {
#if UNITY_2019_1_OR_NEWER
        return m_avatar.humanDescription;
#elif UNITY_EDITOR
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(m_avatar);
        UnityEditor.ModelImporter modelImporter = UnityEditor.AssetImporter.GetAtPath(assetPath) as UnityEditor.ModelImporter;
        return modelImporter.humanDescription;
#endif

    }

    private GameObject GetCharacterModel(Avatar avatar)
    {
#if UNITY_EDITOR
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(m_avatar);
        return UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
#else
        return null;
#endif
    }
}
