using System;
using TeslasuitAPI.Utils;
using UnityEditor;
using UnityEngine;

namespace TeslasuitAPI
{
    public class MocapNodeItem
    {
        private const string ShowContentKeyPrefix = "TSMN_ShowContent_";

        bool isHover = false;

        private Vector3 _meshSize = Vector3.one * 0.1f;
        private float _skeletonHeight = 1.0f;

        [SerializeField]
        private Material _material;
        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private Color _colorEnabled = Color.grey - new Color(0, 0, 0, 0.5f);
        [SerializeField]
        private Color _colorHover = new Color32(140, 136, 87, 255) - new Color(0, 0, 0, 0.5f);


        public event Action<MocapNodeItem> Selected = delegate { };

        public MocapBone MocapBoneIndex { get { return Node.MocapBoneIndex; } }
        public Quaternion Offset { get { return Node.defaultOffset; } private set { Node.defaultOffset = value; } }

        public Quaternion UserDefinedOffset { get { return Node.userDefinedOffset; } private set { Node.userDefinedOffset = value; } }

        public bool Enabled { get { return Node.Enabled; }  set { Node.Enabled = value; }  }

        private bool ShowContent { get { return _showContent; } set { _showContent = value; EditorPrefs.SetBool(ShowContentKeyPrefix + MocapBoneIndex.ToString(), _showContent); } }
        private bool _showContent = false;

        private bool _selected = false;

        public SuitMocapSkeletonNode Node { get; }
        public Transform transform { get { return Node.mocapNodeTransform; } }

        private bool mouseWasDown = false;

        private GUIContent nodeLabelInspectorText;

        private SuitMocapSkeleton Container
        {
            get
            {
                return Node.mocapNodeTransform.GetComponentInParent<SuitMocapSkeleton>();
            }
        }

        private int InstanceID
        {
            get
            {
                return Node.mocapNodeTransform.GetInstanceID();
            }
        }


        public MocapNodeItem(SuitMocapSkeletonNode mocapSkeletonNode, float skeletonHeight)
        {
            
            this._skeletonHeight = skeletonHeight;
            this._meshSize = Vector3.one * _skeletonHeight / 3.0f;
            this._mesh = EditorResources.instance.NodeMesh;
            this._material = EditorResources.instance.NodeMaterial;
            this._material.color = _colorEnabled;
            this.Node = mocapSkeletonNode;

            this.nodeLabelInspectorText = new GUIContent(Node.MocapBoneIndex.ToString());
            this._showContent = EditorPrefs.GetBool(ShowContentKeyPrefix + Node.MocapBoneIndex.ToString(), false);
        }

        public bool OnSceneGUI()
        {
            bool changed = DrawMesh();
            return changed;
        }

        private bool DrawMesh()
        {
            if (_mesh == null) return false;


            _material.color = isHover ? _colorHover : _colorEnabled;
            if (Node.IsRunning)
                return false;

            if (Enabled)
            {
                for (int i = 0; i < _material.passCount; i++)
                    _material.SetPass(i);


               
                Quaternion rotation = (transform.rotation * Offset).Normalized();//normalized;

                Quaternion udo = UserDefinedOffset.Normalized();//normalized

                Matrix4x4 _drawMatrix = Matrix4x4.TRS(transform.position, rotation * udo, _meshSize);
                  

                Graphics.DrawMeshNow(_mesh, _drawMatrix);

                if (_selected)
                {
                    EditorGUI.BeginChangeCheck();
                    Quaternion udRotation = Handles.RotationHandle(rotation * udo, transform.position);
                    
                    if(EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(Container, InstanceID + MocapBoneIndex.ToString());
                        UserDefinedOffset = Offset.Inversed() * transform.rotation.Inversed() * udRotation;
                    }
                    
                    
                }

                return HoverLabel();
            }
                
            
            return false;
        }

        private bool HoverLabel()
        {
            float distance = Vector3.Distance(transform.position, SceneView.currentDrawingSceneView.camera.transform.position);
            float toItemDistance = Vector3.Distance(HandleUtility.WorldToGUIPoint(transform.position), Event.current.mousePosition);
   
            Handles.BeginGUI();

            bool updated = false;


            if (isHover && toItemDistance > _meshSize.x * 10.0f / distance)
            {
                isHover = false;
                updated = true;
            }
            else if (!isHover && toItemDistance < _meshSize.x * 10.0f / distance)
            {
                isHover = true;
                updated = true;
            }
            if(isHover && LabelClicked())
            {
                ShowContent = true;
                SelectInternal();
            }

            if (isHover)
            {
                BoxedLabelLinked(HandleUtility.WorldToGUIPoint(transform.position), nodeLabelInspectorText, Color.white);
            }
                

            Handles.EndGUI();
            return updated;
        }


        private void BoxedLabelLinked(Vector2 pos2D, GUIContent text, Color color)
        {
            Color cached = Handles.color;
            Handles.color = color;

            Vector2 labelSize = GUI.skin.label.CalcSize(text);

            Rect labelRect = new Rect(pos2D.x - labelSize.x / 2, pos2D.y - labelSize.y / 2 - 20.0f, labelSize.x, labelSize.y);
            RectOffset rectOffset = new RectOffset(5, 5, 5, 0);

            GUI.Box(rectOffset.Add(labelRect), text);
            Handles.color = cached;
        }


        private bool LabelClicked()
        {
            bool result = false;
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            var eventType = e.GetTypeForControl(controlID);

            if (e.isMouse && isHover && eventType == EventType.MouseDown)
            {
                GUIUtility.hotControl = controlID;
                mouseWasDown = true;
                Event.current.Use();
            }
            else if(mouseWasDown && eventType == EventType.MouseUp)
            {
                GUIUtility.hotControl = 0;
                mouseWasDown = false;
                e.Use();
                result = true;
            }
            else if(mouseWasDown && eventType == EventType.MouseMove)
            {
                mouseWasDown = false;
            }
            return result;
        }

        public bool OnInspectorGUI()
        {
            bool updated = false;

            EditorGUILayout.BeginHorizontal();

            bool showContentChanged = _showContent;
            bool enabled = Node.Enabled;
            EditorGUI.BeginChangeCheck();
            updated |= EditorGUIExtensions.BeginSettingsBox(nodeLabelInspectorText, ref enabled, ref _showContent);
            ShowContent = _showContent;
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Container, InstanceID + MocapBoneIndex.ToString());
                Node.Enabled = enabled;
            }
                

            showContentChanged = _showContent != showContentChanged;

            if (showContentChanged && _showContent)
                SelectInternal();
                
            if (_showContent)
            {
                EditorGUI.BeginChangeCheck();
                
                Vector3 offset = EditorGUILayout.Vector3Field("User-defined offset ", Node.userDefinedOffset.eulerAngles);

                bool changed = EditorGUI.EndChangeCheck();
                if(changed)
                {
                    Undo.RecordObject(Container, InstanceID + MocapBoneIndex.ToString());
                    Node.userDefinedOffset = Quaternion.Euler(offset);
                }
                    
                updated |= changed;
                
            }
            EditorGUIExtensions.EndSettingsBox();

            EditorGUILayout.EndHorizontal();
            return updated;
        }

        private void SelectInternal()
        {
            EditorGUIUtility.PingObject(transform.GetInstanceID());
            var currentlActive = Selection.activeGameObject;
            Selection.activeGameObject = transform.gameObject;
            SceneView.lastActiveSceneView.LookAt(transform.position);
            Selection.activeGameObject = currentlActive;

            Select();
        }

        public void Select()
        {
            SetSelected(true);
            Selected(this);
        }

        public void SetSelected(bool isSelected)
        {
            _selected = isSelected;
        }

    } 
}
