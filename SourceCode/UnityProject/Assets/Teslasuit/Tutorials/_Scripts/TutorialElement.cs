using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TeslasuitAPI.Tutorials
{
    [SerializeField]
    [CreateAssetMenu(fileName = "tutorialMessage", menuName = "Tutorial/CreateMessage")]
    public class TutorialElement : ScriptableObject
    {
        public string title;
        [SerializeField]
        public PopupMessage popupPrefab;


#if UNITY_EDITOR
        public void OnValidate()
        {
            EditorUtility.SetDirty(this);
            //        AssetDatabase.SaveAssets();
            //        AssetDatabase.Refresh();
        }
#endif
    }
}