using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI
{
    public static class MocapBones
    {
        public static Dictionary<MocapBone, HumanBodyBones> TeslasuitToUnityBones = new Dictionary<MocapBone, HumanBodyBones>
        {
            { MocapBone.RightUpperArm,          HumanBodyBones.RightUpperArm },
            { MocapBone.LeftUpperArm,           HumanBodyBones.LeftUpperArm },
            { MocapBone.Spine,                  HumanBodyBones.Spine},
            { MocapBone.Chest,                  HumanBodyBones.Chest},
            { MocapBone.RightLowerArm,          HumanBodyBones.RightLowerArm},
            { MocapBone.LeftLowerArm,           HumanBodyBones.LeftLowerArm},
            { MocapBone.RightUpperLeg,          HumanBodyBones.RightUpperLeg},
            { MocapBone.LeftUpperLeg,           HumanBodyBones.LeftUpperLeg},
            { MocapBone.RightLowerLeg,          HumanBodyBones.RightLowerLeg},
            { MocapBone.LeftLowerLeg,           HumanBodyBones.LeftLowerLeg},
            { MocapBone.RightHand,              HumanBodyBones.RightHand},
            { MocapBone.LeftHand,               HumanBodyBones.LeftHand},
            { MocapBone.RightThumbProximal,     HumanBodyBones.RightThumbProximal },
            { MocapBone.RightIndexProximal,     HumanBodyBones.RightIndexProximal },
            { MocapBone.RightMiddleProximal,    HumanBodyBones.RightMiddleProximal },
            { MocapBone.RightRingProximal,      HumanBodyBones.RightRingProximal },
            { MocapBone.RightLittleProximal,    HumanBodyBones.RightLittleProximal },
            { MocapBone.LeftThumbProximal,      HumanBodyBones.LeftThumbProximal },
            { MocapBone.LeftIndexProximal,      HumanBodyBones.LeftIndexProximal },
            { MocapBone.LeftMiddleProximal,     HumanBodyBones.LeftMiddleProximal },
            { MocapBone.LeftRingProximal,       HumanBodyBones.LeftRingProximal },
            { MocapBone.LeftLittleProximal,     HumanBodyBones.LeftLittleProximal }
        };
    }
}

