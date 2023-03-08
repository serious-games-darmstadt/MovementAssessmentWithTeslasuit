using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using UnityEngine;

public static class Conversion
{
    public static Quaternion TsRotationToUnityRotation(TsQuat quat)
    {
        return new Quaternion(quat.x, -quat.y, -quat.z, quat.w);
    }

    public static Vector3 TsVector3ToUnityVector3(TsVec3f vec)
    {
        return new Vector3(-vec.x, vec.y, vec.z);
    }
}
