using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HapticSampleAsset))]
public class HapticSampleAssetEditor : Editor
{
    SerializedProperty isLooped;
    SerializedProperty isStatic;
    SerializedProperty bytes;

    private void OnEnable()
    {
        isLooped = serializedObject.FindProperty("isLooped");
        isStatic = serializedObject.FindProperty("isStatic");
        bytes = serializedObject.FindProperty("_bytes");

    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        
        
        serializedObject.Update();

        //hideFlags = HideFlags.None;
        

        EditorGUILayout.LabelField("Haptic Sample hideFlags : " + hideFlags);
        //EditorGUILayout.LabelField("serializedObject.context.hideFlags : " + serializedObject.context.hideFlags);

        EditorGUILayout.LabelField("ByteLength : " + bytes.arraySize);
        EditorGUILayout.PropertyField(isLooped);
        EditorGUILayout.PropertyField(isStatic);

        serializedObject.ApplyModifiedProperties();


    }
}
