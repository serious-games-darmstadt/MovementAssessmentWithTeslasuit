using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TeslasuitAPI
{
    public static class EditorGUIExtensions
    {

        private static GUIStyle SettingsBoxStyle = null;
        private static GUIStyle SettingsBoxHeaderStyle = null;

        private static void Initialize()
        {
            if (SettingsBoxHeaderStyle == null)
            {
                SettingsBoxHeaderStyle = GetStyle("IN TitleText");
                SettingsBoxHeaderStyle.alignment = TextAnchor.LowerLeft;
            }
            if (SettingsBoxStyle == null)
            {
                SettingsBoxStyle = new GUIStyle(EditorStyles.helpBox);
                SettingsBoxStyle.padding.top = 2;
                SettingsBoxStyle.margin.left = 20;
                SettingsBoxStyle.margin.top = 0;
                SettingsBoxStyle.margin.bottom = 2;
            }
        }

        public static bool BeginSettingsBox(string header, ref bool enabled, ref bool showContent)
        {
            return BeginSettingsBox(new GUIContent(header), ref enabled, ref showContent);
        }

        public static void WarningBox(string description)
        {
            EditorGUILayout.HelpBox(description, MessageType.Warning);
        }

        public static bool BeginSettingsBox(GUIContent header, ref bool enabled, ref bool showContent)
        {
            bool _enabled_prev = enabled;
            bool _show_prev = showContent;

            Initialize();

            EditorGUILayout.BeginVertical(SettingsBoxStyle);

            EditorGUILayout.BeginHorizontal();
            enabled = GUILayout.Toggle(enabled, "", GUILayout.MaxWidth(18f));

            Rect rect1 = GUILayoutUtility.GetRect(20f, 18f);

            showContent = GUI.Toggle(rect1, showContent, header, SettingsBoxHeaderStyle);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginFadeGroup(showContent ? 1.0f : 0.0f);
            EditorGUI.BeginDisabledGroup(!enabled);
            return _enabled_prev != enabled || _show_prev != showContent;
        }

        public static void EndSettingsBox()
        {
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();

        }



        private static GUIStyle GetStyle(string styleName)
        {
            GUIStyle guiStyle = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (guiStyle == null)
            {
                Debug.LogError((object)("Missing built-in guistyle " + styleName));
            }
            return guiStyle;
        }
    } 
}
