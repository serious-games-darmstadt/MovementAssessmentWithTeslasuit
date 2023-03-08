using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MyQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;
    public MyQuaternion(float X, float Y, float Z, float W)
    {
        x = X; y = Y; z = Z; w = W;
    }

    public static MyQuaternion ConverToMyQuat(Quaternion quat)
    {
        return new MyQuaternion(quat.x, quat.y, quat.z, quat.w);
    }
    public static Quaternion ConvertToQuat(MyQuaternion myQuat)
    {
        return new Quaternion(myQuat.x, myQuat.y, myQuat.z, myQuat.w);
    }
    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
    }

}