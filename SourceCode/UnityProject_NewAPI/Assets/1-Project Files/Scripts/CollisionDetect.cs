using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    Vector3? colP;
    Collider col;
    MeshFilter mesh;
    List<Vector3> points;
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>();
        col = GetComponent<Collider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] points = collision.contacts;

        foreach(ContactPoint p in points)
        {
            Debug.Log(p.point);//world
            colP = transform.InverseTransformPoint(p.point);//local
            Debug.Log(colP.Value.ToString("F4"));
            DetectVertice(p.point);

        }
        
    }

    private Vector3 DetectVertice(Vector3 point)
    {
        foreach (Vector3 vertice in mesh.mesh.vertices)
        {
            Vector3 globalVertice = transform.TransformPoint(vertice);
            float distance = Vector3.Distance(globalVertice, point);
            if (distance < 0.1f)
                points.Add(globalVertice);
            //Debug.Log(Vector3.Distance(vertice, point));
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (colP.HasValue) Gizmos.DrawSphere(colP.Value, 0.05f);
        Gizmos.color = Color.blue;
        if (points!=null && points.Count > 0)
        {
            foreach(Vector3 p in points)
            {
                Gizmos.DrawSphere(p, 0.05f);
            }
        }
    }
}
