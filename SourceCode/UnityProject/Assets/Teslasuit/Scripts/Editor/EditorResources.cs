using UnityEditor;
using UnityEngine;

namespace TeslasuitAPI.Utils
{
    public class EditorResources : ScriptableSingleton<EditorResources>
    {
        public Mesh NodeMesh { get { return _nodeMesh; } }
        [SerializeField]
        private Mesh _nodeMesh;
        public Material NodeMaterial { get { return _nodeMaterial; } }
        [SerializeField]
        private Material _nodeMaterial;
    } 
}


