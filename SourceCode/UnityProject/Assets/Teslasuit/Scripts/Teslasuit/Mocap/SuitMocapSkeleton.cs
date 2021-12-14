using System;
using System.Collections;
using System.Linq;
using System.Threading;
using TeslasuitAPI.Utils;
using UnityEngine;

namespace TeslasuitAPI
{
    /**@addtogroup UnityComponents */
    /*@{*/

    /// <summary>
    /// Component responsible for working with mocap module of suit
    /// </summary>
    public class SuitMocapSkeleton : MonoBehaviour
    {
        private Quaternion RootRotation
        {
            get
            {
                return transform.rotation;
            }
        }

        public MocapStreamingType MocapStreamingType {
            get
            {
                return mocapStreamingType;
            }
            set
            {
                this.mocapStreamingType = value;
                InitializeStreaming();
            }
        }
        
        [SerializeField]
        private MocapStreamingType mocapStreamingType;


        private MocapSkeleton skeleton;
        [SerializeField]
        private SuitAPIObject suitApi;

        public SuitMocapSkeletonNode[] mocapNodes;

        [HideInInspector]
        [SerializeField]
        private bool _initialized = false; // used by editor

        private void Start()
        {
            enabled = false;

            if(suitApi != null)
            {
                suitApi.BecameAvailable += SuitApi_BecameAvailable;
                suitApi.BecameUnavailable += SuitApi_BecameUnavailable;
            }

        }

        private void SuitApi_BecameUnavailable(SuitHandleObject obj)
        {
            if (!_initialized) return;
            this.enabled = false;
        }

        private void SuitApi_BecameAvailable(SuitHandleObject obj)
        {
            if (!_initialized) return;
            this.enabled = true;
        }

        private void OnEnable()
        {
            if (suitApi != null && suitApi.IsAvailable)
            {
                this.skeleton = suitApi.Suit.Mocap.Skeleton;
                UpdateOffsets();
                if (skeleton != null)
                {
                    foreach (SuitMocapSkeletonNode node in mocapNodes)
                    {
                        node.Initialize(skeleton.Nodes[node.MocapBoneIndex]);
                    }
                }

                InitializeStreaming();

                StartCoroutine(UpdateStreamingRoutine());
            }
        }

        private void UpdateOffsets()
        {
            if (!suitApi.IsAvailable) return;
            {
                SuitDetails details = suitApi.Handle.Suit.SuitInfo.SuitDetails;

                foreach (var bone in suitApi.Handle.Suit.Mocap.Skeleton.Nodes)
                {
                    var rotOffset = TryGetOffset(details.HardwareVersion, bone.Key);
                    SuitMocapSkeletonNode skelNode = mocapNodes.Where(item => item.MocapBoneIndex == bone.Key).First(); 

                    Quat4f offset = Quat4f.Identity;
                    Quaternion origOffset = Quaternion.identity;

                    if (rotOffset != null)
                    {
                        origOffset = Quaternion.Euler(rotOffset.originOffset);
                        offset = Quat4f.Euler(new Vector3f(rotOffset.offset.x, rotOffset.offset.y, rotOffset.offset.z));
                    }

                    if (skelNode != null)
                        skelNode.originOffset = origOffset;
                    bone.Value.UpdateBaseOffset(offset);
                }
            }
        }

        public BoneRotationOffset TryGetOffset(SuitHardwareVersion hardwareVersion, MocapBone mocapBone)
        {
            var verOffset = GetBoneVersionOffset(hardwareVersion);
            if (verOffset != null)
                return GetRotationOffset(verOffset, mocapBone);
            return null;
        }

        public BoneVersionOffset GetBoneVersionOffset(SuitHardwareVersion hardwareVersion)
        {
            return boneVersionOffsets.Where((item) => item.SuitHardwareVersion == hardwareVersion).FirstOrDefault();
        }

        public BoneRotationOffset GetRotationOffset(BoneVersionOffset boneVersionOffset, MocapBone mocapBone)
        {
            return boneVersionOffset.Offsets.Where(item => (1 << (int)item.MocapBone) == (int)mocapBone).FirstOrDefault();
        }

        private void OnDisable()
        {
            skeleton = null;
            if (suitApi != null && suitApi.IsAvailable)
            {
                foreach (SuitMocapSkeletonNode node in mocapNodes)
                {
                    node.Stop();
                }
                suitApi.Mocap.Stop();
            }
        }

        private void Update()
        {
            if (!_initialized) return;
            foreach(SuitMocapSkeletonNode mocapSkeletonNode in mocapNodes)
            {
                mocapSkeletonNode.Update();
            }
        }

        /// <summary>
        /// call method when person is standing for calibrating...
        /// </summary>
        public void PoseCapture()
        {
            if (suitApi != null && suitApi.IsAvailable && suitApi.Mocap.StreamingType.HavePoseSupport())
            {
                suitApi.Mocap.Skeleton.TPoseCapture(Quat4f.Identity);
            }
            else if(!suitApi.Mocap.StreamingType.HavePoseSupport())
            {
                Debug.LogError("Streaming type must have 6-Axis Quaternion included (FullData or Quat6x9x)");
            }
            UpdateOffsets();
        }

        private void InitializeStreaming()
        {
            if (suitApi != null && suitApi.IsAvailable)
            {
                suitApi.Mocap.StreamingType = mocapStreamingType;

            }
                
            foreach (SuitMocapSkeletonNode node in mocapNodes)
            {
                node.UpdateStreamingType(mocapStreamingType);
            }
        }

        private IEnumerator UpdateStreamingRoutine()
        {

            suitApi.Mocap.Stop();
            yield return new WaitForSeconds(0.1f);
            suitApi.Mocap.Start();
            yield return new WaitForSeconds(0.06f);

            yield return null;
        }

        public BoneVersionOffset[] boneVersionOffsets = new BoneVersionOffset[]
        {
            new BoneVersionOffset(
                SuitHardwareVersion.TS_HARDWARE_VERSION_4_5_0_0,
                new BoneRotationOffset[]
                {
                    new BoneRotationOffset(MocapBoneInt.LeftLowerLeg, new Vector3(0,20,0), new Vector3(-8, 135, -7)),
                    new BoneRotationOffset(MocapBoneInt.RightLowerLeg, new Vector3(0,-20,0), new Vector3(-8, -135, 7))
                }),
            new BoneVersionOffset(
                SuitHardwareVersion.TS_HARDWARE_VERSION_4_5_1_0,
                new BoneRotationOffset[]
                {
                    new BoneRotationOffset(MocapBoneInt.LeftLowerLeg, new Vector3(0,20,0), new Vector3(-8, 135, -7)),
                    new BoneRotationOffset(MocapBoneInt.RightLowerLeg, new Vector3(0,-20,0), new Vector3(-8, -135, 7)),
                    new BoneRotationOffset(MocapBoneInt.LeftUpperLeg, new Vector3(0,-90,0), new Vector3(0, -90, 0)),
                    new BoneRotationOffset(MocapBoneInt.RightUpperLeg, new Vector3(0, 90,0), new Vector3(0, 90, 0))
                }),
            new BoneVersionOffset(
                SuitHardwareVersion.TS_HARDWARE_VERSION_4_5_2_0,
                new BoneRotationOffset[]
                {
                    new BoneRotationOffset(MocapBoneInt.LeftLowerLeg, new Vector3(0,20,0), new Vector3(-8, 135, -7)),
                    new BoneRotationOffset(MocapBoneInt.RightLowerLeg, new Vector3(0,-20,0), new Vector3(-8, -135, 7)),
                    new BoneRotationOffset(MocapBoneInt.LeftUpperLeg, new Vector3(0,-90,0), new Vector3(0, -90, 0)),
                    new BoneRotationOffset(MocapBoneInt.RightUpperLeg, new Vector3(0, 90,0), new Vector3(0, 90, 0))
                }),
            new BoneVersionOffset(
                SuitHardwareVersion.TS_HARDWARE_VERSION_4_5_3_0,
                new BoneRotationOffset[]
                {
                    new BoneRotationOffset(MocapBoneInt.LeftLowerLeg, new Vector3(0,20,0), new Vector3(-8, 135, -7)),
                    new BoneRotationOffset(MocapBoneInt.RightLowerLeg, new Vector3(0,-20,0), new Vector3(-8, -135, 7)),
                    new BoneRotationOffset(MocapBoneInt.LeftUpperLeg, new Vector3(0,-90,0), new Vector3(0, -90, 0)),
                    new BoneRotationOffset(MocapBoneInt.RightUpperLeg, new Vector3(0, 90,0), new Vector3(0, 90, 0))
                }),
            new BoneVersionOffset(
                SuitHardwareVersion.TS_HARDWARE_VERSION_4_5_4_0,
                new BoneRotationOffset[]
                {
                    new BoneRotationOffset(MocapBoneInt.LeftLowerLeg, new Vector3(0,20,0), new Vector3(-8, 135, -7)),
                    new BoneRotationOffset(MocapBoneInt.RightLowerLeg, new Vector3(0,-20,0), new Vector3(-8, -135, 7)),
                    new BoneRotationOffset(MocapBoneInt.LeftUpperLeg, new Vector3(0,-90,0), new Vector3(0, -90, 0)),
                    new BoneRotationOffset(MocapBoneInt.RightUpperLeg, new Vector3(0, 90,0), new Vector3(0, 90, 0))
                }),
        };
        

        public class BoneVersionOffset
        {
            public SuitHardwareVersion SuitHardwareVersion;
            public BoneRotationOffset[] Offsets;

            public BoneVersionOffset(SuitHardwareVersion SuitHardwareVersion, BoneRotationOffset[] Offsets)
            {
                this.SuitHardwareVersion = SuitHardwareVersion;
                this.Offsets = Offsets;
            }
        }
        [Serializable]
        public class BoneRotationOffset
        {
            public MocapBoneInt MocapBone;

            public Vector3 offset;
            public Vector3 originOffset;

            public BoneRotationOffset(MocapBoneInt bone, Vector3 offset, Vector3 originOffset)
            {
                this.MocapBone = bone;
                this.offset = offset;
                this.originOffset = originOffset;
            }
        }
        public enum MocapBoneInt
        {
            Hips = 0,
            LeftUpperLeg = 1,
            RightUpperLeg = 2,
            LeftLowerLeg = 3,
            RightLowerLeg = 4,
            LeftFoot = 5,
            RightFoot = 6,
            Spine = 7,
            Chest = 8,
            Neck = 9,
            Head = 10,
            LeftShoulder = 11,
            RightShoulder = 12,
            LeftUpperArm = 13,
            RightUpperArm = 14,
            LeftLowerArm = 15,
            RightLowerArm = 16,
            LeftHand = 17,
            RightHand = 18,
            LeftToes = 19,
            RightToes = 20,
            LeftEye = 21,
            RightEye = 22,
            Jaw = 23,
            LeftThumbProximal = 24,
            LeftThumbIntermediate = 25,
            LeftThumbDistal = 26,
            LeftIndexProximal = 27,
            LeftIndexIntermediate = 28,
            LeftIndexDistal = 29,
            LeftMiddleProximal = 30,
            LeftMiddleIntermediate = 31,
            LeftMiddleDistal = 32,
            LeftRingProximal = 33,
            LeftRingIntermediate = 34,
            LeftRingDistal = 35,
            LeftLittleProximal = 36,
            LeftLittleIntermediate = 37,
            LeftLittleDistal = 38,
            RightThumbProximal = 39,
            RightThumbIntermediate = 40,
            RightThumbDistal = 41,
            RightIndexProximal = 42,
            RightIndexIntermediate = 43,
            RightIndexDistal = 44,
            RightMiddleProximal = 45,
            RightMiddleIntermediate = 46,
            RightMiddleDistal = 47,
            RightRingProximal = 48,
            RightRingIntermediate = 49,
            RightRingDistal = 50,
            RightLittleProximal = 51,
            RightLittleIntermediate = 52,
            RightLittleDistal = 53,
            UpperChest = 54,
            LastBone = 55
        }
    }
    /*@}*/
}