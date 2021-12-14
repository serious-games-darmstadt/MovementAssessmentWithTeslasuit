using System;
using System.Collections.Generic;
using UnityEngine;
using TeslasuitAPI.Utils;

namespace TeslasuitAPI
{
    [Serializable]
    public class SuitMocapSkeletonNode
    {
        public Transform mocapNodeTransform;

        public MocapBone MocapBoneIndex
        {
            get { return (MocapBone) _mocapBoneIndex; }
            set { _mocapBoneIndex = (ulong) value; }
        }

        private MocapStreamingType currentStreamingType = MocapStreamingType.Quat9x;

        [SerializeField] private ulong _mocapBoneIndex = 0;


        [SerializeField] [HideInInspector] public Quaternion defaultOffset = Quaternion.identity;

        [SerializeField] [HideInInspector] public Quaternion userDefinedOffset = Quaternion.identity;

        [SerializeField] [HideInInspector] public Quaternion originOffset = Quaternion.identity;
        private Quaternion RootRelatedRotation { get; set; }

        [SerializeField] [HideInInspector] public bool Enabled = true;

        //Rigidbody needed for haptic collision 
        private Rigidbody rigidbody;

        private Quaternion rawRotation;

        private Quaternion BoneRotation
        {
            get { return ConvertToBoneRotation(rawRotation); }
        }

        private Quaternion Heading
        {
            get { return TransformExtensions.HeadingOffset(Quaternion.identity, root.transform.rotation); }
        }

        [SerializeField] [HideInInspector] private Transform root;

        private MocapNode mocapNode;

        public bool IsRunning
        {
            get { return mocapNode != null; }
        }
        
        private MocapReplay mocapPlayer;
        private MotionCapture _motionCapture;

        public SuitMocapSkeletonNode(Transform transform, MocapBone boneIndex, Transform root)
        {
            this.userDefinedOffset = Quaternion.identity;
            this.root = root;
            this.mocapNodeTransform = transform;
            this.MocapBoneIndex = boneIndex;

            this.rigidbody = mocapNodeTransform.GetComponentInParent<Rigidbody>();
        }

        public void Initialize(MocapNode mocapNode)
        {
            this.mocapNode = mocapNode;
            _wasEnabledOnStart = Enabled;
            originOffset = Quaternion.identity;
            this.RootRelatedRotation = root.rotation.Inversed() * mocapNodeTransform.rotation;

            mocapPlayer = GameObject.Find("Teslasuit_Man").GetComponent<MocapReplay>();
            _motionCapture = GameObject.Find("DataGateway").GetComponent<MotionCapture>();
        }

        public void UpdateStreamingType(MocapStreamingType streamingType)
        {
            this.currentStreamingType = streamingType;
        }

        public void Stop()
        {
            this.mocapNode = null;
        }


        private bool _wasEnabledOnStart = false;

        private bool IsValid = true;

        public void Update()
        {
            bool is_valid = true;
            if (mocapNode != null)
                is_valid = mocapNode.IsValid;
            else return;

            if (IsValid)
            {
                if (!is_valid)
                {
                    OnBecameInvalid();
                }
            }

            if (!IsValid)
            {
                if (is_valid)
                    OnBecameValid();
            }

            if (!is_valid && _wasEnabledOnStart)
                Enabled = false;
            else if (is_valid && !IsValid && _wasEnabledOnStart && !Enabled)
                Enabled = true;

            IsValid = is_valid;

            Quaternion currentRotation = Quaternion.identity;

            if (!mocapPlayer.DoReplay)
            {
                switch (currentStreamingType)
                {
                    case MocapStreamingType.Quat9x:
                        currentRotation = mocapNode.Rotation.Quaternion();
                        break;
                    case MocapStreamingType.FullData:
                    case MocapStreamingType.Quat9x6x:
                        currentRotation = mocapNode.PosedRotation6x.Quaternion();
                        break;
                }
            }
            else
            {
                currentRotation = mocapPlayer.GetCurrentReplayRotation(_mocapBoneIndex).Quaternion();
            }

            switch (mocapNode.mocapBone)
            {
                case MocapBone.Spine:
                    _motionCapture.tsSpine = ConvertToBoneRotation(currentRotation);
                    break;
                default:
                    UpdateRotation(currentRotation);
                    break;
            }
        }

        private void OnBecameInvalid()
        {
            mocapNodeTransform.rotation = RootRelatedRotation * root.rotation;
        }

        private void OnBecameValid()
        {
        }

        private void UpdateRotation(Quaternion rotation)
        {
            if (Enabled)
            {
                this.rawRotation = rotation;

                if (rigidbody)
                    rigidbody.MoveRotation(BoneRotation);
                else
                    mocapNodeTransform.rotation = BoneRotation;
            }
        }

        private Quaternion ConvertToBoneRotation(Quaternion rotation)
        {
            var res = rotation * userDefinedOffset.Inversed() * (defaultOffset).Inversed();

            return Heading * res;
        }
    }
}