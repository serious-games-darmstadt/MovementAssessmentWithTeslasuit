using System;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

public class TsHandAnimator : MonoBehaviour
{
    [SerializeField]
    private TsMotionProvider m_motionProvider;

    [SerializeField]
    private TsHandAvatarSettings m_avatarSettings;

    private Dictionary<TsHumanBoneIndex, Transform> m_bonesTransforms = new Dictionary<TsHumanBoneIndex, Transform>();
    private Dictionary<TsHumanBoneIndex, Quaternion> m_initialPose = new Dictionary<TsHumanBoneIndex, Quaternion>();

    private void Start()
    {
        if (m_avatarSettings == null)
        {
            Debug.LogError("Missing avatar settings for this character.");
            enabled = false;
            return;
        }


        SetupAvatarBones();
    }

    private void SetupAvatarBones()
    {
        foreach (var fingerPart in m_avatarSettings.HandFingers)
        {
            var transformName = fingerPart.transform.name;
            var boneTransform = TransformUtils.FindChildRecursive(transform, transformName);
            if (boneTransform != null && !m_bonesTransforms.ContainsKey(fingerPart.boneIndex))
            {
                m_bonesTransforms.Add(fingerPart.boneIndex, boneTransform);
                m_initialPose.Add(fingerPart.boneIndex, boneTransform.localRotation);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        var skeleton = m_motionProvider.GetSkeleton(Time.time);
        Update(skeleton);
    }

    public bool calibrate = false;
    private void Update(ISkeleton skeleton)
    {
        if (skeleton == null)
        {
            return;
        }
        foreach (var finger in m_avatarSettings.HandFingers)
        {
            var poseRotation = m_initialPose[finger.boneIndex];
            var targetRotation = Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform(finger.boneIndex).rotation);

            TryDoWithBone(finger.boneIndex, (boneTransform) =>
            {
                boneTransform.localRotation = targetRotation * poseRotation;
            });
        }

        if (calibrate)
        {
            m_motionProvider.Calibrate();
            calibrate = false;
        }
    }

    public void Calibrate()
    {
        m_motionProvider?.Calibrate();
    }

    private void TryDoWithBone(TsHumanBoneIndex boneIndex, Action<Transform> action)
    {
        if (!m_bonesTransforms.TryGetValue(boneIndex, out var boneTransform))
        {
            return;
        }

        action(boneTransform);
    }
    
}
