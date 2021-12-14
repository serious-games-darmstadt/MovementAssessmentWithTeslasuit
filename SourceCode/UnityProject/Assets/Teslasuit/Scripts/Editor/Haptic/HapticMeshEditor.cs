using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TeslasuitAPI
{
    [CustomEditor(typeof(HapticMesh), true)]
    public class HapticMeshEditor : Editor
    {
        private Material material;

        private void OnEnable()
        {
            material = new Material(Shader.Find("Hidden/Internal-Colored"));
        }
        public Vector2 scrollPosition = Vector2.zero;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            HapticMesh hapticMesh = (HapticMesh)target;
            if (hapticMesh == null || hapticMesh.HitMapping == null) return;

            List<Rect> rects = new List<Rect>();

            for (int sm = 0; sm < hapticMesh.SubMeshIndexes.Length; sm++)
            {
                Rect viewRect = new Rect(0, 0, 300, 300);
                Rect view = GUILayoutUtility.GetRect(viewRect.width, viewRect.width * 2, viewRect.height, viewRect.height * 2);
                rects.Add(view);
            }



            if (Event.current.type == EventType.Repaint)
            {

                
                for (int sm = 0; sm < hapticMesh.SubMeshIndexes.Length; sm++)
                {
                    var view = rects[sm];
                    view.position += new Vector2(10, 0);


                    GUI.DrawTexture(view, hapticMesh.GetComponent<SkinnedMeshRenderer>().materials[hapticMesh.SubMeshIndexes[sm]].mainTexture, ScaleMode.StretchToFill, true);

                    GUI.BeginClip(view);
                    UnityPolygon[] UVPolygons = HapticMappingConversion.ToUVPolygons(hapticMesh.HitMapping.Polygons, hapticMesh.HitMapping.Width, hapticMesh.HitMapping.Height);
                    for (int i = 0; i < UVPolygons.Length; i++)
                    {
                        DrawPolyGL(UVPolygons[i].vertices, Color.HSVToRGB(((float)i) / UVPolygons.Length, 1.0f, 1.0f),
                            material, new Vector2(view.width, view.height * -1.0f), new Vector2(0.0f, -1.0f));
                    }
                    GUI.EndClip();

                }


            }

        }

        private static void DrawPolyGL(Vector2[] poly, Color color, Material _glMaterial, Vector2 scale, Vector2 offset)
        {
            GL.PushMatrix();
            GL.Clear(true, false, color);
            _glMaterial.SetPass(0);
            offset.Scale(scale);
            for (int i = 0; i < poly.Length; i++)
            {
                Vector2 a = poly[i];
                Vector2 b = poly[(i + 1) % poly.Length];

                a.Scale(scale);
                b.Scale(scale);

                a += offset;
                b += offset;

                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex(a);
                GL.Vertex(b);
                GL.End();

            }
            GL.PopMatrix();
        }
    }

}