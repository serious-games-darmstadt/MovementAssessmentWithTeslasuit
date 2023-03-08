using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshReader : MonoBehaviour
{
    Vector3[] _vertices;
   
    void Start()
    {
       
    }


    void Update()
    {
        
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] += normals[i] * Mathf.Sin(Time.time);
        }

        mesh.vertices = vertices;



        Debug.Log(mesh.triangles.Length);
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertices.Length);
        //foreach(int p in mesh.triangles)
        //{
        //    Debug.Log(p);
        //}
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(0,0,0), 1);
        Gizmos.color = Color.blue;
        if(_vertices.Length>0)
        {
         
            for (int i = 0; i < 10; i++)
            {
                Gizmos.DrawSphere(_vertices[i], 0.01f);
            }
            //foreach (Vector3 p in _vertices)
            //{
                
            //    Gizmos.DrawSphere(p, 0.01f);
            //}
        }
       
    }
}
