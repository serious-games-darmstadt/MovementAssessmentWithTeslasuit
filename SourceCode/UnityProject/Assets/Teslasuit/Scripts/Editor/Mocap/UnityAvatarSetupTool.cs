using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class UnityAvatarSetupTool
{
    private static readonly MethodInfo isPoseValidMethod = typeof(Editor).Assembly.GetType("UnityEditor.AvatarSetupTool").GetMethod("IsPoseValidOnInstance",
            new Type[] { typeof(GameObject), typeof(SerializedObject) });

    public static bool IsPoseValidOnInstance(GameObject modelPrefab, SerializedObject modelImporterSerializedObject)
    {
        return (bool)isPoseValidMethod.Invoke(null, new object[] { modelPrefab, modelImporterSerializedObject });
    }
    
}
