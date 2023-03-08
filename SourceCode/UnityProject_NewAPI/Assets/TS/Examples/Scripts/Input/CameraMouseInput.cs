using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseInput : MonoBehaviour
{
    [SerializeField]
    private OrbitalCamera _cam;

    private Vector3 _prevMousePos;

    void Update()
    {
        const int LeftButton = 0;
        if (Input.GetMouseButton(LeftButton))
        {
            // mouse movement in pixels this frame
            Vector3 mouseDelta = Input.mousePosition - _prevMousePos;

            // adjust to screen size
            Vector3 moveDelta = mouseDelta * (360f / Screen.height);

            _cam.Move(moveDelta.x, -moveDelta.y);
        }
        _prevMousePos = Input.mousePosition;
    }
}
