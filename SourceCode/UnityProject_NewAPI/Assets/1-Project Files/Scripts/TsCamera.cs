using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsCamera : MonoBehaviour
{
    public GameObject Ethan;
    int zoomLevel = 0;
    const float zoomStepSize = 0.25f;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(Ethan.transform);
        Vector3 pos = Ethan.transform.position;
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
    }

    public void RotateAround180()
    {
        transform.RotateAround(Ethan.transform.position, Vector3.up, 180);
    }
    public void RotateAround45Right()
    {
        transform.RotateAround(Ethan.transform.position, Vector3.up, 45);
    }
    public void RotateAround45Left()
    {
        transform.RotateAround(Ethan.transform.position, Vector3.up, -45);
    }
    public void ZoomIn()
    {
        if (zoomLevel >= 2) return;
        cam.orthographicSize -= zoomStepSize;
        zoomLevel++;
    }

    public void ZoomOut()
    {
        if (zoomLevel <= -2) return;
        cam.orthographicSize += zoomStepSize;
        zoomLevel--;
    }
}
