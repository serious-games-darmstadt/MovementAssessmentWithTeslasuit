using System.Collections;
using System.Collections.Generic;
using TeslasuitAPI;
using UnityEngine;

//[CreateAssetMenu(fileName = "SampleAsset", menuName = "Haptic/Create Sample")]
public class HapticSampleAsset : HapticAsset, IHapticSample
{
    public bool isStatic;
    public bool isLooped;

    public byte[] GetBytes()
    {
        return Bytes;
    }

    public bool IsLooped()
    {
        return isLooped;
    }

    public bool IsStatic()
    {
        return isStatic;
    }
}
