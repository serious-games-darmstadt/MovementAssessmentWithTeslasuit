using System.Collections.Generic;
using TsAPI.Types;


/// <summary>
/// Contains bones that are covered by haptic channels.
/// </summary>
public static class TsHapticBones
{
    public static IReadOnlyCollection<TsHumanBoneIndex> RequiredBones = new List<TsHumanBoneIndex>()
    {
        TsHumanBoneIndex.Chest,
        TsHumanBoneIndex.Spine,
        TsHumanBoneIndex.RightUpperArm,
        TsHumanBoneIndex.RightLowerArm,
        TsHumanBoneIndex.LeftUpperArm,
        TsHumanBoneIndex.LeftLowerArm,
        TsHumanBoneIndex.RightUpperLeg,
        TsHumanBoneIndex.RightLowerLeg,
        TsHumanBoneIndex.LeftUpperLeg,
        TsHumanBoneIndex.LeftLowerLeg,
    };
}
