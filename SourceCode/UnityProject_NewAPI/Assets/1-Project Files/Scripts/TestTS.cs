using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsAPI;
using TsAPI.Types;
using TsSDK;
using System;

public class TestTS : MonoBehaviour
{
    [SerializeField]
    private TsMotionProvider m_motionProvider;
    ISkeleton skeleton;
    // Start is called before the first frame update
    void Start()
    {
       // TsAPI.Types.TsMocapSensor
       

    }

    // Update is called once per frame
    void Update()
    {
        skeleton = m_motionProvider.GetSkeleton(Time.time);
    }
    private void OnDrawGizmos()
    {
        Color a = Color.red; a.a = 128;
        Gizmos.color = a;
    
        foreach(TsHumanBoneIndex i in TsHumanBones.SuitBones)
        {
            TsTransform boneTransform;

        if (skeleton !=null && skeleton.GetBoneTransform(i,out boneTransform))
            {
                Gizmos.DrawSphere( Conversion.TsVector3ToUnityVector3( boneTransform.position), 0.05f);
            }
        }
    }
}
