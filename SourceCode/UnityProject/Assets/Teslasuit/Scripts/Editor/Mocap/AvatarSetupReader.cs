using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace TeslasuitAPI
{
    public static class AvatarSetupReader
    {

        public static Dictionary<HumanBodyBones, Transform> GetHumanBodyBones(GameObject modelGameObject)
        {
            ModelImporter modelImporter = GetModelImporter(modelGameObject);
            if (modelImporter == null) return null;
            return GetHumanBodyBones(modelImporter, modelGameObject);
        }

        public static bool HaveRightTPoseSetup(GameObject modelGameObject)
        {
            ModelImporter modelImporter = GetModelImporter(modelGameObject);
            if (modelImporter == null) return false;
            SerializedObject modelImporterSerializedObject = new SerializedObject(modelImporter);
            return UnityAvatarSetupTool.IsPoseValidOnInstance(modelGameObject, modelImporterSerializedObject);
        }

        private static Dictionary<HumanBodyBones, Transform> GetHumanBodyBones(ModelImporter modelImporter, GameObject modelGameObject)
        {
            Dictionary<HumanBodyBones, Transform> humanBodyBones = new Dictionary<HumanBodyBones, Transform>();
            HumanBone[] humanBones = modelImporter.humanDescription.human;

            if (humanBones == null) return humanBodyBones;

            Transform[] allBones = modelGameObject.GetComponentsInChildren<Transform>();
            for(int i=0; i< allBones.Length; i++)
            {
                var boneWrapper = humanBones.FirstOrDefault((item) => item.boneName == allBones[i].name);
                if(!string.IsNullOrEmpty(boneWrapper.boneName))
                {
                    try
                    {
                        object parsedBoneType = Enum.Parse(typeof(HumanBodyBones), RemoveWhitespace(boneWrapper.humanName));
                        if (parsedBoneType != null)
                            humanBodyBones.Add((HumanBodyBones)parsedBoneType, allBones[i]);
                    }
                    catch { Debug.LogWarning("Failed to convert human body bone value from string: " + boneWrapper.humanName); }
                }
            }

            return humanBodyBones;
        }

        public static Dictionary<Transform, SkeletonBone> GetTPoseDescripton(GameObject modelGameObject)
        {
            Dictionary<Transform, SkeletonBone> poseDescription = new Dictionary<Transform, SkeletonBone>();

            ModelImporter modelImporter = GetModelImporter(modelGameObject);
            if (modelImporter == null) return null;
            
            Transform[] allBones = modelGameObject.GetComponentsInChildren<Transform>();

            foreach(SkeletonBone bonePoseDescription in modelImporter.humanDescription.skeleton)
            {
                var bone = allBones.FirstOrDefault((item) => item.name == bonePoseDescription.name);
                if(bone != null)
                    poseDescription.Add(bone, bonePoseDescription);
            }

            return poseDescription;
        }

        private static bool CheckForHumanModel(ModelImporter modelImporter)
        {
            if (modelImporter == null) return false;

            if (modelImporter.animationType != ModelImporterAnimationType.Human)
            {
                Debug.LogError("AnimationType of this model rig is not Human. Please set Human AnimationType and configure avatar of this model.", modelImporter);
                return false;
            }

            return true;
        }

        public static ModelImporter GetModelImporter(GameObject modelGameObject)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = modelGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null) return null;
            string assetPath = AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh);
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (!CheckForHumanModel(modelImporter))
                return null;
            
            return modelImporter;
        }


        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
       
    }

        
}           
            
            
            
            
            
            
            
            
            
            