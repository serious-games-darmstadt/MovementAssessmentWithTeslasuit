using System;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;
//* marked places are where i have added something.

public class TsHumanAnimator : MonoBehaviour
{
    [SerializeField]
    private TsMotionProvider m_motionProvider;

    [SerializeField]
    private TsAvatarSettings m_avatarSettings;
    public bool IPose = false;

    private TsHumanBoneIndex m_rootBone = TsHumanBoneIndex.Hips;
    private Dictionary<TsHumanBoneIndex, Transform> m_bonesTransforms = new Dictionary<TsHumanBoneIndex, Transform>();
    //*
    public Dictionary<TsHumanBoneIndex, Transform> BoneTransforms { get { return m_bonesTransforms; } }
    //*
    public bool Replay;
    //*
    float IPoseTimer;
    //*
    float? firstHipPosition;
    //*
    private SkinnedMeshRenderer _meshRenderer;
    //* 
    private DataGateway dataGateway;



    private void Start()
    {
        int i = FindObjectsOfType<TsHumanAnimator>().Length;

        dataGateway = FindObjectOfType<DataGateway>();

        if (m_avatarSettings == null)
        {
            Debug.LogError("Missing avatar settings for this character.");
            enabled = false;
            return;
        }

        if(!m_avatarSettings.IsValid)
        {
            Debug.LogError("Invalid avatar settings for this character. Check that all required bones is configured correctly.");
            enabled = false;
            return;
        }

        SetupAvatarBones();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void SetupAvatarBones()
    {
        foreach (var reqBoneIndex in TsHumanBones.SuitBones)
        {            
            var transformName = m_avatarSettings.GetTransformName(reqBoneIndex);
            //*
            //There is a problem with EthanSpine2. Recursive algorithm has failed to find it.
            //This part of the code is a temporary quick fix.
            Transform boneTransform;
            if (reqBoneIndex == TsHumanBoneIndex.UpperSpine)
            {
                // original ->
                boneTransform = transform.Find("EthanSkeleton/EthanHips/EthanSpine/EthanSpine1/EthanSpine2");
              //  boneTransform = TransformUtils.FindChildRecursive(transform, transformName);

            }
            else
            {
                boneTransform = TransformUtils.FindChildRecursive(transform, transformName);
               
            }
            
            if (boneTransform != null && !m_bonesTransforms.ContainsKey(reqBoneIndex))
            {
                m_bonesTransforms.Add(reqBoneIndex, boneTransform);
            }
        }

        ////*
        //foreach (KeyValuePair<TsHumanBoneIndex, Transform> kvp in m_bonesTransforms)
        //{
        //    //Debug.Log("index: " + (int)kvp.Key + " pos: " + kvp.Value.position + " pos: " + kvp.Value.rotation);
        //    Debug.Log("index: " + kvp.Key);
        //}
    }

    // Update is called once per frame
    private void Update()
    {
        var skeleton = m_motionProvider.GetSkeleton(Time.time);

        //*

        ReplayObject rO = new ReplayObject { };

        //*
        //if replay is true, character will be replayed by TsReplaySaver script
        if(!Replay) Update(skeleton);
        
    }

   
    private void Update(ISkeleton skeleton)
    {
        

        if (skeleton == null)
        {
            return;
        }
        #region Quaternion
        //Original- Quaternion
        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
            var targetRotation = Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform(boneIndex).rotation);

           // if (boneIndex != TsHumanBoneIndex.Hips) //dont understand why this is here-> breaks the hips rotation of imported models.
           //proly because hips is not being found in recursive search. we had placd it for ethan ourselves.
                TryDoWithBone(boneIndex, (boneTransform) =>
                {
                    boneTransform.rotation = targetRotation * poseRotation;
                    //*
                   // boneTransform.position = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform(boneIndex).position);
                });

        }
        //Set hips(rootbone) as main position of model
        TryDoWithBone(m_rootBone, (boneTransform) =>
        {
            var hipsPos = skeleton.GetBoneTransform(TsHumanBoneIndex.Hips).position;
            boneTransform.transform.position = Conversion.TsVector3ToUnityVector3(hipsPos);
        });

        #endregion
        #region Position+Rotation
        ////Pos+rot
        //foreach (var boneIndex in TsHumanBones.SuitBones)
        //{
        //    var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //    var targetPosition = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform(boneIndex).position);
        //    var targetRotation = Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform(boneIndex).rotation).eulerAngles;
        //    ////var targetRotation = Quaternion.identity; 
        //    //if (ri.replayRotation.ContainsKey(boneIndex))
        //    //{
        //    //    poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //    //    targetRotation = ri.replayRotation[boneIndex];
        //    //}
        //    //else
        //    //{
        //    //    //Debug.Log("bulunamadi: " + boneIndex.ToString());
        //    //    continue;
        //    //}

        //    TryDoWithBone(boneIndex, (boneTransform) =>
        //    {
        //        boneTransform.rotation = Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z);
        //        boneTransform.position = targetPosition;
        //    });
        //}

        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //    // var hipsPos = m_motionProvider.GetSkeleton(Time.time).GetBoneTransform(TsHumanBoneIndex.Hips).position;
        //    // boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];
        //});
        #endregion
        if (IPose)
        {
            //*
            IPoseTimer += Time.deltaTime;
            if (IPoseTimer > 3)
            {
                Calibrate();
                IPose = false;
                IPoseTimer = 0;
            }
            
        }
        //*
        // Calibrate();
    }
    public void SetIPose()
    {
        IPose = true;
    }
    public void Calibrate()
    {
        m_motionProvider?.Calibrate();
    }
    //*
    public void ReplayUpdate(ReplayInfo ri)
    {
        ////pos + euler rot(Uncomment this section if you want to replay using both positions(vec3) and rotations(vec3) )
        //foreach (var boneIndex in TsHumanBones.SuitBones)
        //{
        //    var poseRotation = Quaternion.identity;
        //    var targetRotation = Vector3.zero;
        //    //var targetRotation = Quaternion.identity; 
        //   if (ri.replayRotation.ContainsKey(boneIndex))
        //    {
        //        poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //        targetRotation = ri.replayRotation[boneIndex];
        //    }
        //    else
        //    {
        //        //Debug.Log("bulunamadi: " + boneIndex.ToString());
        //        continue;
        //    }

        //    TryDoWithBone(boneIndex, (boneTransform) =>
        //    {
        //        boneTransform.rotation = Quaternion.Euler( targetRotation.x, targetRotation.y, targetRotation.z);
        //        boneTransform.position = ri.replayPosition[boneIndex];
        //    });
        //}

        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //   // var hipsPos = m_motionProvider.GetSkeleton(Time.time).GetBoneTransform(TsHumanBoneIndex.Hips).position;
        //   // boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];
        //});

        //quaternion
        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            //var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
            MyQuaternion my;
            Quaternion targetRotation;
            if (ri.replayRotationQuaternion.TryGetValue(boneIndex, out my))
            {
                if (boneIndex == TsHumanBoneIndex.Hips)
                {
                    targetRotation = Inverse(MyQuaternion.ConvertToQuat(my),true,true,true);
                }else
                targetRotation = MyQuaternion.ConvertToQuat(my);
            }
            else continue;


            TryDoWithBone(boneIndex, (boneTransform) =>
            {
                boneTransform.rotation = targetRotation;
            });
        }
        TryDoWithBone(m_rootBone, (boneTransform) =>
        {
            boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];
        });

        //Debug.Log(ri.replayPosition[TsHumanBoneIndex.Hips].y);
        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //    if (ri.replayPosition[TsHumanBoneIndex.Hips].y < 1)
        //    {
        //        boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];

        //    }
        //    else
        //    {
        //        boneTransform.transform.position = new Vector3(ri.replayPosition[TsHumanBoneIndex.Hips].x, 1, ri.replayPosition[TsHumanBoneIndex.Hips].z);
        //    }

        //});

        //if (firstHipPosition.HasValue)
        //{
        //    TryDoWithBone(m_rootBone, (boneTransform) =>
        //    {
        //        // if(ri.replayPosition[TsHumanBoneIndex.Hips].y<1)
        //        boneTransform.transform.position = new Vector3(ri.replayPosition[TsHumanBoneIndex.Hips].x,
        //            Mathf.Lerp(firstHipPosition.Value, ri.replayPosition[TsHumanBoneIndex.Hips].y,Time.deltaTime*0.1f),
        //            ri.replayPosition[TsHumanBoneIndex.Hips].z);
        //        firstHipPosition = boneTransform.position.y;

        //    });

        //}
        //else
        //{
        //    firstHipPosition = ri.replayPosition[TsHumanBoneIndex.Hips].y;
        //}       


    }

    private void TryDoWithBone(TsHumanBoneIndex boneIndex, Action<Transform> action)
    {
        if (!m_bonesTransforms.TryGetValue(boneIndex, out var boneTransform))
        {
            return;
        }

        action(boneTransform);
    }

    //*
    private void OnDrawGizmos()
    {
        Color blue = new Color(0, 0, 1, 125);
        Gizmos.color = blue;
        foreach (KeyValuePair<TsHumanBoneIndex, Transform> kvp in m_bonesTransforms)
        {
           // if( kvp.Key == (TsHumanBoneIndex)12 || kvp.Key == (TsHumanBoneIndex)13)
            Gizmos.DrawSphere(kvp.Value.position, 0.05f);
        }
    }
    public static Quaternion HeadingOffset( Quaternion b)
    {
        Quaternion offset = Inverse(b,true,true,true);
        offset.x = 0f;
        offset.z = 0f;

        float mag = offset.w * offset.w + offset.y * offset.y;

        offset.w /= Mathf.Sqrt(mag);
        offset.y /= Mathf.Sqrt(mag);

        return offset;
    }
    private static Quaternion Inverse(Quaternion vector, bool X, bool Y, bool Z)
    {
        vector.x *= X ? -1f : 1f;
        vector.y *= Y ? -1f : 1f;
        vector.z *= Z ? -1f : 1f;
        return vector;
    }
}
