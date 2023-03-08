using System.IO;
using UnityEditor;
using UnityEngine;
#if UNITY_2020_3_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

[ScriptedImporter(1, "ts_asset")]
public class TsAssetImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var asset = TsAssetBase.Create(File.ReadAllBytes(ctx.assetPath));
        ctx.AddObjectToAsset("main obj", asset);
        ctx.SetMainObject(asset);
        AssetDatabase.SaveAssets();
    }
}