using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TeslasuitAPI.Tutorials
{
    [CustomEditor(typeof(TutorialList))]
    public class TutorialListEditor : Editor
    {
        bool showElements = true;
        int elemsSize;
        SerializedProperty elementsArray;
        private void OnEnable()
        {
            elementsArray = serializedObject.FindProperty("elements");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //base.OnInspectorGUI();
            showElements = EditorGUILayout.Foldout(showElements, "Elements");
            if (showElements)
            {
                elemsSize = elementsArray.arraySize;
                elemsSize = EditorGUILayout.DelayedIntField("Size:", elemsSize);

                if (elemsSize != elementsArray.arraySize)
                {
                    while (elemsSize > elementsArray.arraySize)
                    {
                        elementsArray.InsertArrayElementAtIndex(elementsArray.arraySize);
                    }
                    while (elemsSize < elementsArray.arraySize)
                    {
                        elementsArray.DeleteArrayElementAtIndex(elementsArray.arraySize - 1);
                    }
                }


                for (int i = 0; i < elementsArray.arraySize; i++)
                {
                    SerializedProperty element = elementsArray.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element);
                    //EditorGUILayout.LabelField("zip");

                    TutorialElement e = (TutorialElement)element.objectReferenceValue;
                    e.title = EditorGUILayout.TextField(e.title);
                    e.popupPrefab = (PopupMessage)EditorGUILayout.ObjectField(e.popupPrefab, typeof(PopupMessage), false);

                }
            }



            serializedObject.ApplyModifiedProperties();
            //DrawDefaultInspector();
        }
    }
}