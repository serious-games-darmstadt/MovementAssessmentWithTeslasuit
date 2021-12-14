using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volume {

    public Vector3 Center { get { return _collider.transform.position; } }

    private Collider _collider;

	public Volume(Collider collider)
    {
        this._collider = collider;
    }

    public float GetDepth(Vector3 inVolumePoint)
    {
        Vector3? fullDepthVector = GetDepthVector(inVolumePoint);
        if (fullDepthVector == null)
            return 0.0f;
        return (Center + fullDepthVector - inVolumePoint).Value.magnitude;
    }

    public float GetDepthNormalized(Vector3 inVolumePoint)
    {
        Vector3? fullDepthVector = GetDepthVector(inVolumePoint);
        
        if (fullDepthVector == null)
            return 0.0f;
        Debug.DrawLine(Center + fullDepthVector.Value, inVolumePoint, Color.yellow);
        return (Center + fullDepthVector - inVolumePoint).Value.magnitude / fullDepthVector.Value.magnitude;
    }

    private Vector3? GetSurfacePoint(Vector3 pointInVolume)
    {
        const float MaxDistance = 1000000.0f;
        Vector3 fromCenterVector = (pointInVolume - Center).normalized;
        Ray ray = new Ray(Center + fromCenterVector * MaxDistance, -fromCenterVector);
        RaycastHit hitInfo;

        if (_collider.Raycast(ray, out hitInfo, MaxDistance * 2))
            return hitInfo.point;


        return null;
    }

    private Vector3? GetDepthVector(Vector3 directionPoint)
    {
        Vector3? surfacePoint = GetSurfacePoint(directionPoint);
        if (surfacePoint == null)
            return null;
        return surfacePoint.Value - Center;
    }


}
