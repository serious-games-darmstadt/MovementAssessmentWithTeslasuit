using System.Collections;
using System.Collections.Generic;
using TeslasuitAPI;
using UnityEngine;

public class HapticEffectAsset : HapticAsset, IHapticEffect
{
    public bool isStatic;

    public byte[] GetBytes()
    {
        return Bytes;
    }

    public bool IsStatic()
    {
        return isStatic;
    }
}
