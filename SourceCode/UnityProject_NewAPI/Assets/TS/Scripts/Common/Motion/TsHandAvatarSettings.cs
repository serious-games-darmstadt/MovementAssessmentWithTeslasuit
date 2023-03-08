using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

[CreateAssetMenu(menuName = "Teslasuit/Motion/Hand avatar settings")]
public class TsHandAvatarSettings : ScriptableObject
{
    public Transform HandModel;
    public Avatar HandAvatar;
    [SerializeField]
    public HandFinger[] HandFingers;
    public TsDeviceSide Side;
    [Serializable]
    public struct HandFinger
    {
        public Transform transform;
        public TsHumanBoneIndex boneIndex;
    }

    private void OnValidate()
    {
        if (HandModel == null)
        {
            return;
        }

        var phalanxes = TsHumanBones.RightHandBones;
        if (Side == TsDeviceSide.Left)
        {
            phalanxes = TsHumanBones.LeftHandBones;
        }

        List<HandFinger> fingers = new List<HandFinger>();
        foreach (var p in phalanxes)
        {
            var transform = HandAvatarMapper.FindPhalanxTransform(p, HandModel);
            fingers.Add( new HandFinger(){boneIndex = p, transform = transform});
        }

        HandFingers = fingers.ToArray();
    }

   
}
