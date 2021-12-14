using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

using UnityEngine;

namespace TeslasuitAPI
{
    public enum HapticAssetType
    {
        Animation = 1,
        Preset = 1,
        FX,
        Sample,
        Project,
        Unknown
    }

    [UnityEditor.AssetImporters.ScriptedImporter(1, "haptic_asset")]
    public class HapticAssetImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        private const int IconSize = 128;

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            //Debug.Log(string.Format("ctx.assetPath : {0}", ctx.assetPath)); 
            HapticAssetType type = GetHapticType(ctx.assetPath);


            Object assetObject;

            switch (type)
            {
                case HapticAssetType.Animation:
                    assetObject = ScriptableObject.CreateInstance<HapticAnimationAsset>().Init(ctx.assetPath);
                    break;
                case HapticAssetType.FX:
                    assetObject = ScriptableObject.CreateInstance<HapticEffectAsset>().Init(ctx.assetPath);
                    break;
                case HapticAssetType.Sample:
                    assetObject = ScriptableObject.CreateInstance<HapticSampleAsset>().Init(ctx.assetPath);
                    break;
                case HapticAssetType.Project:
                    Debug.Log(string.Format("Error importing {0} : Asset type is \"project\"", ctx.assetPath));
                    return;
                case HapticAssetType.Unknown:
                default:
                    Debug.Log(string.Format("Error importing {0}...", ctx.assetPath));
                    return;
                    //throw new System.Exception("Unknown Type Exception"); 
            }
            ctx.AddObjectToAsset("mainObj", assetObject, GetIconByType(type));
            ctx.SetMainObject(assetObject);

            //Debug.Log("bytes " + ((HapticAsset)assetObject).GetBytes().Length); 
            //AssetDatabase.ImportAsset(ctx.assetPath, ImportAssetOptions.Default); 
            //EditorUtility.SetDirty(assetObject); 
            //AssetDatabase.SaveAssets(); 
            //AssetDatabase.Refresh(); 


        }

        Texture2D GetIconByType(HapticAssetType type)
        {
            switch (type)
            {
                case HapticAssetType.Animation:
                    return createIcon(Color.yellow, IconSize);
                case HapticAssetType.FX:
                    return createIcon(Color.blue, IconSize);
                case HapticAssetType.Sample:
                    return createIcon(Color.green, IconSize);
                case HapticAssetType.Project:
                case HapticAssetType.Unknown:
                default:
                    return createIcon(Color.red, IconSize);
            }
        }

        Texture2D createIcon(Color color, int size)
        {
            Vector2 center = new Vector2(size / 2, size / 2);
            float radius = size / 2f;
            float circleEnd = 0.65f * radius;
            float circleBegin = 0.4f * radius;

            Texture2D icon = new Texture2D(size, size);
            foreach (var x in Enumerable.Range(0, size))
                foreach (var y in Enumerable.Range(0, size))
                {
                    Vector2 curr = new Vector2(x, y);
                    float dist = Vector2.Distance(curr, center);
                    float mul = (dist > circleBegin ? 1f : 0f) * (dist < circleEnd ? 1f : 0f);
                    Color current = color * mul * (1f - dist / radius);
                    icon.SetPixel(x, y, current);
                }
            return icon;
        }


        HapticAssetType GetHapticType(string path)
        {
            if (!File.Exists(path))
                return HapticAssetType.Unknown;

            using (var reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                reader.ReadInt16();
                short count = reader.ReadInt16();

                for (int i = 0; i < count; i++)
                {
                    string key = ReadNullTerminatedString(reader);
                    string val = ReadNullTerminatedString(reader);
                    if (key == "type")
                    {
                        switch (val)
                        {
                            case "preset":
                            case "animation":
                                return HapticAssetType.Preset;
                            case "fx":
                                return HapticAssetType.FX;
                            case "sample":
                                return HapticAssetType.Sample;
                            case "project":
                                return HapticAssetType.Project;
                            default:
                                return HapticAssetType.Unknown;
                        }
                    }
                }
            }
            return HapticAssetType.Unknown;
        }

        public static string ReadNullTerminatedString(System.IO.BinaryReader stream)
        {
            StringBuilder sb = new StringBuilder();
            char ch;
            while ((ch = stream.ReadChar()) != 0)
                sb.Append(ch);
            return sb.ToString();
        }

    }

    //public class TeslasuitAssetModificationProcessor : UnityEditor.AssetModificationProcessor 
    //{ 
    //    public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options) 
    //    { 
    //        System.Type asset_type = AssetDatabase.GetMainAssetTypeAtPath(path); 

    //        if (asset_type.IsSubclassOf(typeof(BaseHapticAsset))) 
    //        { 
    //            BaseHapticAsset asset = AssetDatabase.LoadAssetAtPath<BaseHapticAsset>(path); 
    //            Debug.Log(asset.name + " deleted"); 
    //            if (asset.Loaded) 
    //                asset.Unload(); 
    //        } 

    //        return AssetDeleteResult.DidNotDelete; 
    //    } 
    //} 
}