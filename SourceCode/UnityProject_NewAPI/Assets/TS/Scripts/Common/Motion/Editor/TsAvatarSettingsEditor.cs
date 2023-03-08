using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TsAvatarSettings))]
public class TsAvatarSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var avatarSettings = target as TsAvatarSettings;
        
        var canBuild = avatarSettings.IsValid;
        GUI.enabled = canBuild;
        if (GUILayout.Button("Automap"))
        {
            avatarSettings.Setup();
            EditorUtility.SetDirty(avatarSettings);
        }
        GUI.enabled = true;
       
    }
}