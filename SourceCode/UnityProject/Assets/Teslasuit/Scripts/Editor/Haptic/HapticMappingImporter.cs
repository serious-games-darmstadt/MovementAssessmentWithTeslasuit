using System.IO;
using UnityEditor;

using UnityEngine;

namespace TeslasuitAPI
{
    public enum MappingType
    {
        SourceMapping = 0,
        HitMapping,
        Unknown
    }

    //TODO bin file format?
    [UnityEditor.AssetImporters.ScriptedImporter(1, "bin")]
    public class HapticMappingImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            //HapticMappingAsset asset = ScriptableObject.CreateInstance<HapticMappingAsset>();

            MappingType type = DefineType(ctx.assetPath);

            Object assetObject;
            switch (type)
            {
                case MappingType.SourceMapping:
                    assetObject = ScriptableObject.CreateInstance<HapticSourceMappingAsset>().Init(ctx.assetPath);
                    break;
                case MappingType.HitMapping:
                    assetObject = ScriptableObject.CreateInstance<HapticHitMappingAsset>().Init(ctx.assetPath);
                    break;
                default:
                    Debug.Log(string.Format("Error importing {0}", ctx.assetPath));
                    return;
            }

            
            EditorUtility.SetDirty(assetObject);
            AssetDatabase.SaveAssets();

            ctx.AddObjectToAsset("mainObj", assetObject);
            ctx.SetMainObject(assetObject);

        }

        MappingType DefineType(string path)
        {
            //TODO need file parsing
            return MappingType.HitMapping;
        }
    } 
}
