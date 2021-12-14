using System.Collections;
using System.Collections.Generic;
using TeslasuitAPI;
using UnityEngine;

public class HapticAnimationAsset : HapticAsset, IHapticAnimation
{
    public bool isStatic;
    public bool isLooped;

    public byte[] GetBytes()
    {
        return Bytes;
    }

    bool IHapticAnimation.IsLooped()
    {
        return isLooped;
    }

    bool IHapticAnimation.IsStatic()
    {
        return isStatic;
    }
}
