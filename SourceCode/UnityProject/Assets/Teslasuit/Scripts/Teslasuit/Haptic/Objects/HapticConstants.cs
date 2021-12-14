using System;
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticConstants
    {
        public const string HapticReceiverLayer = "HapticReceiver";
        public const string HapticObjectLayer = "HapticObject";

        public const uint DefaultHitDuration = 100;
        public const float MaxForce = 100.0f;
        public const float MinImpact = 0.001f;


        public static class Util
        {
            /// <summary>
            /// Haptic force calculation function - by default returns <see cref="HapticForceCalculateDefault"/>.
            /// Override this function to customize collision force solving.
            /// </summary>
            public static Func<Polygon, Collision, ContactPoint, HapticHitEvent, float> ForceCalculateFunction = HapticForceCalculateDefault;


            /// <summary>
            /// Default haptic force calculation function.
            /// </summary>
            /// <returns>
            /// Force calculated by collision impulse in newtons range 0..100
            /// </returns>
            private static float HapticForceCalculateDefault(Polygon poly, Collision collision, ContactPoint contactPoint, HapticHitEvent hapticHitEvent)
            {
                return collision.ForceMagnitude();
            }

            public static readonly int HapticReceiverLayer = GetOrCreateLayer(HapticConstants.HapticReceiverLayer);
            public static readonly int HapticObjectLayer = GetOrCreateLayer(HapticConstants.HapticObjectLayer);

            static Util()
            {
                
                UpdateCollisionSettings();
            }

            private static void UpdateCollisionSettings()
            {
                Physics.IgnoreLayerCollision(HapticReceiverLayer, HapticObjectLayer, false);
            }

            public static int GetOrCreateLayer(string layerName)
            {
#if UNITY_EDITOR
                int currentLayer = LayerMask.NameToLayer(layerName);
                if (currentLayer == -1)
                    CreateLayer(layerName);
                else return currentLayer;
#endif
                return LayerMask.NameToLayer(layerName);
            }
#if UNITY_EDITOR
            private static void CreateLayer(string name)
            {
                UnityEditor.SerializedObject tagManager = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                UnityEditor.SerializedProperty tagsProp = tagManager.FindProperty("layers");

                for (int i = 8; i <= 31; i++)
                {
                    UnityEditor.SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
                    if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                    {
                        sp.stringValue = name;
                        tagManager.ApplyModifiedPropertiesWithoutUndo();
                        return;
                    }

                }
                
                Debug.LogWarning("Error creating layer with name \"" + name + "\". There are no free layers.");
            }

#endif
        }


    }



    public static class CollisionExtensions
    {
        public static Vector3 Force(this Collision collision)
        {
            return collision.impulse / Time.fixedDeltaTime;
        }

        public static float ForceMagnitude(this Collision collision)
        {
            return collision.impulse.magnitude / Time.fixedDeltaTime;
        }
    } 
}