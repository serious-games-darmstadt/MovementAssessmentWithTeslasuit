using System.IO;
using TsAPI.Types;
using UnityEditor;
using UnityEngine;

public class SimplifiedChannelsGenerator : MonoBehaviour
{
    [MenuItem("Teslasuit/Generate Simplified Haptic Channels")]
    static void Generate()
    {
        var path = $"Assets/TS/Assets/SimplifiedHapticChannels/";
        Directory.CreateDirectory(path);

        foreach (var bone in TsHapticBones.RequiredBones)
        {
            TsHapticSimplifiedChannel.Create(bone, TsBone2dSide.Front, path);
            TsHapticSimplifiedChannel.Create(bone, TsBone2dSide.Back, path);
        }
        AssetDatabase.SaveAssets();
    }

}
