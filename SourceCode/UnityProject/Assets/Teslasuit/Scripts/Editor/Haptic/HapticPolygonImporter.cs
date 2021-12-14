using System.Collections;
using System.IO;
using UnityEditor;

using UnityEngine;

namespace TeslasuitAPI
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "polygon")]
    public class HapticPolygonImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            //HapticPolygonAsset asset = ScriptableObject.CreateInstance<HapticPolygonAsset>();
            //asset.Initialize(ctx.assetPath);

            //string path = Path.GetDirectoryName(ctx.assetPath) + "/" + Path.GetFileNameWithoutExtension(ctx.assetPath);
            //AssetDatabase.CreateAsset(asset, path + ".asset");
            //EditorUtility.SetDirty(asset);
            //AssetDatabase.SaveAssets();

            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = asset;

        }


    }

}