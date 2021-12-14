using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using TeslasuitAPI;
using System;

[CustomEditor(typeof(HapticAssetImporter))]
public class HapticAssetImporterEditor : UnityEditor.AssetImporters.AssetImporterEditor
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
        //TODO if smth is needed

        serializedObject.Update();

        //GET ASSET TYPE

        HapticAsset Asset = assetTarget as HapticAsset;

        Type type = Asset.GetType();

        if(type == typeof(HapticSampleAsset))
        {
            //SAMPLE
            DrawSample((HapticSampleAsset)Asset);
        }else if (type == typeof(HapticEffectAsset))
        {
            //EFFECT
            DrawEfect((HapticEffectAsset)Asset);
        }else if(type == typeof(HapticAnimationAsset))
        {
            //ANIMATION
            DrawAnimation((HapticAnimationAsset)Asset);
        }

        serializedObject.ApplyModifiedProperties();
#endif
    }

    void DrawSample(HapticSampleAsset sample)
    {
        EditorGUILayout.LabelField("HapticSampleAsset");

        EditorGUILayout.LabelField("ByteArray size : " + sample.GetBytes().Length);
        sample.isLooped = EditorGUILayout.Toggle("Is Looped", sample.isLooped);
        sample.isStatic = EditorGUILayout.Toggle("Is Static", sample.isStatic);
    }

    void DrawEfect(HapticEffectAsset effect)
    {
        EditorGUILayout.LabelField("HapticSampleAsset");

        EditorGUILayout.LabelField("ByteArray size : " + effect.GetBytes().Length);
        effect.isStatic = EditorGUILayout.Toggle("Is Static", effect.isStatic);
    }

    void DrawAnimation(HapticAnimationAsset animation)
    {
        EditorGUILayout.LabelField("HapticSampleAsset");

        EditorGUILayout.LabelField("ByteArray size : " + animation.GetBytes().Length);
        animation.isLooped = EditorGUILayout.Toggle("Is Looped", animation.isLooped);
        animation.isStatic = EditorGUILayout.Toggle("Is Static", animation.isStatic);
    }
}
