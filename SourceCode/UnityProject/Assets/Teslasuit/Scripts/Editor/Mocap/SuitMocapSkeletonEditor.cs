using TeslasuitAPI.Utils;
using UnityEditor;
using UnityEngine;

namespace TeslasuitAPI
{
    [CustomEditor(typeof(SuitMocapSkeleton))]
    public class SuitMocapSkeletonEditor : Editor
    {
        private static readonly string IncorrectTPoseWarning = "T-Pose setup for this model is incorrect. Please setup T-Pose in ModelImportSettings correctly for better teslasuit bones auto-mapping.";
        private static readonly string NoSkinnedMeshWarning = "SkinnedMesh component is not found on this GameObject's hierarchy.";
        private static readonly string InitializedString = "_initialized";
        private static readonly string SuitApiString = "suitApi";

        private static readonly string PantsString = "Pants";
        private static readonly string JacketString = "Jacket";
        private static readonly string GloveString = "Gloves";

        private SuitMocapSkeleton Skeleton
        {
            get { return target as SuitMocapSkeleton; }
        }

        private SerializedProperty suitApiProperty;

        private Renderer Renderer
        {
            get
            {
                Renderer renderer = Skeleton.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer == null)
                    return Skeleton.GetComponentInChildren<MeshRenderer>();
                return renderer;
            }
        }

        private SkinnedMeshRenderer SkinnedMesh
        {
            get
            {
                return Skeleton.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }
        private MocapNodeItem _currentSelected;

        private MocapNodeItemsGroup[] NodeItemsGroups = new MocapNodeItemsGroup[0];

        private bool _IsValid = true;

        private bool _SkinnedMeshAvailable = true;

        void OnEnable()
        {
            this.suitApiProperty = serializedObject.FindProperty(SuitApiString);

            Reset();
        }

        private void Reset()
        {
            if (!IsInitialized(this.serializedObject))
               _IsValid &= Initialize();

            _SkinnedMeshAvailable = SkinnedMesh != null;
            _IsValid &= _SkinnedMeshAvailable;
            _IsValid &= AvatarSetupReader.HaveRightTPoseSetup(Skeleton.gameObject);

            if (_IsValid)
                CreateNodeItems();
        }

        private bool Initialize()
        {
            TeslasuitHumanSkeleton humanSkeleton = new TeslasuitHumanSkeleton(Skeleton.gameObject);

            if(_IsValid)
            {
                Skeleton.mocapNodes = humanSkeleton.MocapNodes.ToArray();
                Apply(serializedObject);
                SetInitialized(serializedObject, true);
                Apply(serializedObject);
            }
            return humanSkeleton.IsValid;
        }

        private void CreateNodeItems()
        {
            MocapNodeItem[] mocapNodeItems = new MocapNodeItem[Skeleton.mocapNodes.Length];
            for(int i=0;i<Skeleton.mocapNodes.Length;i++)
            {
                mocapNodeItems[i] = new MocapNodeItem(Skeleton.mocapNodes[i], Height(GetBounds()));
                mocapNodeItems[i].Selected += MocapNodeItemSelected;
            }

            NodeItemsGroups = new MocapNodeItemsGroup[3];
            NodeItemsGroups[0] = new MocapNodeItemsGroup(MocapSkeleton.Bindings.JacketBonesMask, mocapNodeItems, JacketString);
            NodeItemsGroups[1] = new MocapNodeItemsGroup(MocapSkeleton.Bindings.PantsBonesMask, mocapNodeItems, PantsString);
            NodeItemsGroups[2] = new MocapNodeItemsGroup(MocapSkeleton.Bindings.HandBonesMask, mocapNodeItems, GloveString);
        }

        private Bounds GetBounds()
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
            if (Renderer != null)
                bounds = MeshUtil.GetRecalculatedBounds(Renderer);
            return bounds;
        }

        private float Height(Bounds bounds)
        {
           return Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        }

        private void MocapNodeItemSelected(MocapNodeItem obj)
        {
            foreach(var group in NodeItemsGroups)
            {
                group.UnselectExcept(obj);
            }
        }

        private bool IsInitialized(SerializedObject serializedObject)
        {
            return this.serializedObject.FindProperty(InitializedString).boolValue;
        }

        private void SetInitialized(SerializedObject serializedObject, bool value)
        {
            serializedObject.FindProperty(InitializedString).boolValue = value;
        }

        private void Apply(SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.UpdateIfRequiredOrScript();
        }

        void OnSceneGUI()
        {
            bool updated = false;

            foreach (var itemsGroup in NodeItemsGroups)
                updated |= itemsGroup.OnSceneGUI();

            if (updated)
                EditorWindow.GetWindow<SceneView>().Repaint();

            DrawGUI(Skeleton);
        }

        public override void OnInspectorGUI()
        {
            bool updated = false;

            if (!_IsValid)
            {
                if(!_SkinnedMeshAvailable)
                    EditorGUILayout.HelpBox(NoSkinnedMeshWarning, MessageType.Warning);

                else
                    EditorGUILayout.HelpBox(IncorrectTPoseWarning, MessageType.Warning);
            }
                

            updated |= DrawObjectPoperty(suitApiProperty);

            if (suitApiProperty.objectReferenceValue == null) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Streaming type: ");

            Skeleton.MocapStreamingType = (MocapStreamingType)EditorGUILayout.EnumPopup(Skeleton.MocapStreamingType);
            EditorGUILayout.EndHorizontal();

            foreach (var itemsGroup in NodeItemsGroups)
                updated |= itemsGroup.OnInspectorGUI();

            if (updated)
            {
                serializedObject.ApplyModifiedProperties();
                EditorWindow.GetWindow<SceneView>().Repaint();
            }
            EditorGUILayout.BeginHorizontal();
            if(Skeleton.MocapStreamingType.HavePoseSupport() && GUILayout.Button("TPose"))
            {
                Skeleton.PoseCapture();
            }
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Reset", EditorStyles.miniButtonRight))
            {
                Reset();
            }


            EditorGUILayout.EndHorizontal();

        }

        private bool DrawObjectPoperty(SerializedProperty property)
        {
            var suitApiObject = property.objectReferenceValue;

            EditorGUILayout.PropertyField(property);
            return suitApiObject != property.objectReferenceValue;
        }

        private void DrawGUI(SuitMocapSkeleton skeleton)
        {
            Handles.BeginGUI();

            GUILayout.FlexibleSpace();
            DrawPrintOffsets();
            GUILayout.Space(30f);

            Handles.EndGUI();
        }

        private void DrawPrintOffsets()
        {
            GUILayout.BeginHorizontal();
            GUILayout.MinHeight(20f);
            string str = "";

            /*
            if (GUILayout.Button("Print offsets"))
            {
                foreach (SuitMocapSkeletonNode node in Skeleton.mocapNodes)
                {
                    Quaternion nO = node.defaultOffset;
                    str += string.Format("{{ HumanBodyBones.{0}, new Quaternion({1}f, {2}f, {3}f, {4}f) }},\n", MocapBones.TeslasuitToUnityBones[node.MocapBoneIndex], Mathf.Round(nO.x * 1000f)/1000f, Mathf.Round(nO.y * 1000f) / 1000f, Mathf.Round(nO.z * 1000f) / 1000f, Mathf.Round(nO.w * 1000f) / 1000f);
                    //str += string.Format("{{ HumanBodyBones.{0}, new Vector3({1}f, {2}f, {3}f) }},\n", MocapBones.TeslasuitToUnityBones[node.boneIndex], nO.eulerAngles.x, nO.eulerAngles.y, nO.eulerAngles.z);
                }
                MonoBehaviour.print(str);
            }*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }

    }

}
