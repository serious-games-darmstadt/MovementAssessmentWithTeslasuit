using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

/// <summary>
/// TsHapticSimplifiedChannel asset is used for targeting a haptic to the same place on the 
/// body between the devices.
/// </summary>
[CreateAssetMenu(menuName = "Teslasuit/Haptic/Simplified haptic channel")]
public class TsHapticSimplifiedChannel : ScriptableObject
{
    /// <summary>
    /// Bone index used to get device native channels.
    /// </summary>
    public TsHumanBoneIndex BoneIndex;

    /// <summary>
    /// Bone side used to get device native channels.
    /// </summary>
    public TsBone2dSide BoneSide;

    /// <summary>
    /// Returns native electric channels represented by given device and that covers <see cref="BoneIndex"/> and <see cref="BoneSide"/>.
    /// </summary>
    /// <param name="device">Device to get native channels from</param>
    public IMapping2dElectricChannel[] GetChannels(IDevice device)
    {
        if (device == null)
        {
            return null;
        }
        return device.Mapping2d.ElectricChannels.Where((item) =>
            item.BoneIndex == BoneIndex && item.BoneSide == BoneSide).ToArray();
    }

    /// <summary>
    /// Creates simplified channel asset instance and serializes it by given <paramref name="path"/>.
    /// </summary>
    /// <param name="boneIndex">Bone index that is used by simplified channel.</param>
    /// <param name="side">Bone side that is used by simplified channel.</param>
    /// <param name="path">Path where to save the asset, should start with "Assets/.."</param>
    /// <returns>TsHapticSimplifiedChannel instance of simplified channel</returns>
    #if UNITY_EDITOR
    public static TsHapticSimplifiedChannel Create(TsHumanBoneIndex boneIndex, TsBone2dSide side, string path)
    {
        var name = $"{boneIndex}{side}";
        var instance = CreateInstance<TsHapticSimplifiedChannel>();
        instance.BoneIndex = boneIndex;
        instance.BoneSide = side;
        instance.name = name;
        UnityEditor.AssetDatabase.CreateAsset(instance, Path.Combine(path, $"{name}.asset"));
        return instance;
    }
    #endif
}
