using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TeslasuitAPI;


[CustomEditor(typeof(HapticMappingImporter))]
public class HapticMappingImporterEditor : UnityEditor.AssetImporters.AssetImporterEditor
{
#if UNITY_2018_2_OR_NEWER
    public override bool showImportedObject { get { return false; } }
#endif
    public override void OnInspectorGUI()
    {
#if !UNITY_2018_2_OR_NEWER
        DrawDefaultInspector();
        return;
#else
        serializedObject.Update();


        HapticHitMappingAsset mapping = assetTarget as HapticHitMappingAsset;

        mapping.Height = EditorGUILayout.IntField("Mapping Height: ", mapping.Height);
        mapping.Width = EditorGUILayout.IntField("Mapping Width: ", mapping.Width);



        serializedObject.ApplyModifiedProperties();
#endif
    }

}
