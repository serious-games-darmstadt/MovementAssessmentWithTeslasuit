using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TsAPI.Types;
using UnityEngine;

public class HandAvatarMapper
{
    private static Dictionary<string, string[]> PhalanxKeyWords = new Dictionary<string, string[]>()
    {
        { "thumb", new[] {"thumb"}},
        { "index", new[] {"index"}},
        { "middle", new[] {"middle"}},
        { "ring", new[] {"ring"}},
        { "little", new[] {"little", "pinky"}}
    };

    public static Dictionary<string, string[]> PhalanxPartKeywords = new Dictionary<string, string[]>()
    {
        { "proximal", new[] {"prox", "1"}},
        { "intermediate", new[] {"inter","2"}},
        { "distal", new[] {"dist","3"}},
    };

    public static List<TsHumanBoneIndex> RightHandPhalanxes = new List<TsHumanBoneIndex>()
    {
        TsHumanBoneIndex.RightThumbProximal,
        TsHumanBoneIndex.RightThumbIntermediate,
        TsHumanBoneIndex.RightThumbDistal,
        TsHumanBoneIndex.RightIndexProximal,
        TsHumanBoneIndex.RightIndexIntermediate,
        TsHumanBoneIndex.RightIndexDistal,
        TsHumanBoneIndex.RightMiddleProximal,
        TsHumanBoneIndex.RightMiddleIntermediate,
        TsHumanBoneIndex.RightMiddleDistal,
        TsHumanBoneIndex.RightRingProximal,
        TsHumanBoneIndex.RightRingIntermediate,
        TsHumanBoneIndex.RightRingDistal,
        TsHumanBoneIndex.RightLittleProximal,
        TsHumanBoneIndex.RightLittleIntermediate,
        TsHumanBoneIndex.RightLittleDistal
    };

    public static List<TsHumanBoneIndex> LeftHandPhalanxes = new List<TsHumanBoneIndex>()
    {
        TsHumanBoneIndex.LeftThumbProximal,
        TsHumanBoneIndex.LeftThumbIntermediate,
        TsHumanBoneIndex.LeftThumbDistal,
        TsHumanBoneIndex.LeftIndexProximal,
        TsHumanBoneIndex.LeftIndexIntermediate,
        TsHumanBoneIndex.LeftIndexDistal,
        TsHumanBoneIndex.LeftMiddleProximal,
        TsHumanBoneIndex.LeftMiddleIntermediate,
        TsHumanBoneIndex.LeftMiddleDistal,
        TsHumanBoneIndex.LeftRingProximal,
        TsHumanBoneIndex.LeftRingIntermediate,
        TsHumanBoneIndex.LeftRingDistal,
        TsHumanBoneIndex.LeftLittleProximal,
        TsHumanBoneIndex.LeftLittleIntermediate,
        TsHumanBoneIndex.LeftLittleDistal
    };

    private static bool IsDistal(string name)
    {
        return PhalanxPartKeywords["distal"].Any((item) => name.Contains(item));
    }

    public static Transform FindPhalanxTransform(TsHumanBoneIndex key, Transform root)
    {
        Transform result = null;
        var keyFormatted = key.ToString().ToLowerInvariant();
        var phalanxKeywords = PhalanxKeyWords.First((item) => keyFormatted.Contains(item.Key)).Value;
        var phalanxPartKeywords = PhalanxPartKeywords.First((item) => keyFormatted.Contains(item.Key)).Value;
        var distal = IsDistal(keyFormatted);
        TransformUtils.IterateChildsRecursive(root, (child) =>
        {
            var name = child.name.ToLowerInvariant();
            if (!distal && child.childCount == 0) return false;
            if (!MatchKeywords(name, phalanxKeywords)) return false;
            if (!MatchKeywords(name, phalanxPartKeywords)) return false;

            result = child;

            return true;

        });
        return result;
    }

    private static bool MatchKeywords(string name, string[] keywords)
    {
        return keywords.Any(kw => name.Contains(kw));
    }
}
