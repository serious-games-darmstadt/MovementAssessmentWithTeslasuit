using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsAPI.Types;
using TsSDK;

public class ReplayInfo : ISkeleton
{
    public Dictionary<TsHumanBoneIndex, Vector3> replayPosition = new Dictionary<TsHumanBoneIndex, Vector3>();
    public Dictionary<TsHumanBoneIndex, Vector3> replayRotation = new Dictionary<TsHumanBoneIndex, Vector3>();
    //Test
    public Dictionary<TsHumanBoneIndex, MyQuaternion> replayRotationQuaternion = new Dictionary<TsHumanBoneIndex, MyQuaternion>();
    public TsTransform GetBoneTransform(TsHumanBoneIndex index)
    {
        //will be worked further
        TsTransform t = new TsTransform(new TsVec3f(), new TsQuat());
        TsVec3f tt = new TsVec3f(); tt.x = replayPosition[index].x;
        return t;
    }
    /// <summary>
    /// Needs a better interpolation technique
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    [System.Obsolete]
    public static ReplayInfo Interpolate(ReplayInfo r1, ReplayInfo r2)
    {
        if (r1 == null || r2 == null) return r1;

        ReplayInfo rReturn = new ReplayInfo();
        for (int i = 0; i < r1.replayPosition.Count; i++)
        {
            if (r1.replayPosition.ContainsKey((TsHumanBoneIndex)i))
            {
                rReturn.replayPosition.Add((TsHumanBoneIndex)i, (r1.replayPosition[(TsHumanBoneIndex)i] + r2.replayPosition[(TsHumanBoneIndex)i]) / 2);
                rReturn.replayRotation.Add((TsHumanBoneIndex)i, (r1.replayRotation[(TsHumanBoneIndex)i] + r2.replayRotation[(TsHumanBoneIndex)i]) / 2);
            }
        }
        return rReturn;

    }
    public bool GetBoneTransform(TsHumanBoneIndex boneIndex, out TsTransform boneTransform) { boneTransform = new TsTransform(); return false; }

    public override string ToString()
    {
        string row = "\n";
        string space = " : ";
        string s = string.Empty;

        //Just positions
        foreach (KeyValuePair<TsHumanBoneIndex, Vector3> kvp in replayPosition)
        {
            s = string.Concat(s, kvp.Key, space, kvp.Value, row);
        }

        return s;
    }


}