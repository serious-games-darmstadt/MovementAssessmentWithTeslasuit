using System;
using System.Collections.Generic;
using TeslasuitAPI.Utils;
using UnityEngine;

namespace TeslasuitAPI
{
    public class TeslasuitHumanSkeleton
    {
        private GameObject _gameObject;

        private Dictionary<HumanBodyBones, Transform> _allBones;
        private Dictionary<Transform, SkeletonBone> _tPoseDescription;
        private Dictionary<Transform, SkeletonBone> _backupPoseDescription;

        public List<SuitMocapSkeletonNode> MocapNodes { get; private set; }

        public float TorsoHeight { get { return Distance(HumanBodyBones.Hips, HumanBodyBones.Neck); } }

        public bool IsTPosed { get; private set; }

        public bool IsValid { get; private set; }

        public TeslasuitHumanSkeleton(GameObject gameObject)
        {
            IsTPosed = false;
            this._gameObject = gameObject;

            this._allBones = AvatarSetupReader.GetHumanBodyBones(this._gameObject);
            this._tPoseDescription = AvatarSetupReader.GetTPoseDescripton(this._gameObject);

            this.IsValid = Validate();

            if(IsValid)
                this.BackupCurrentState();
                
            Reset();
        }

        private void Reset()
        {
            if(MocapNodes == null)
                this.MocapNodes = new List<SuitMocapSkeletonNode>();
            MocapNodes.Clear();

            if (!IsValid) return;

            Quaternion _backupRot = _gameObject.transform.rotation;
            Transform _backupParent = _gameObject.transform;
            _gameObject.transform.parent = null;
            _gameObject.transform.rotation = Quaternion.identity;
            SetTPosedState(true);
            foreach (var bone_kv in MocapBones.TeslasuitToUnityBones)
            {
                Transform transform = GetTransform(bone_kv.Value);
                if (transform != null)
                {
                    var mocapNode = new SuitMocapSkeletonNode(transform, bone_kv.Key, _gameObject.transform);

                    mocapNode.defaultOffset = transform.rotation.Inversed() * (GetOrigin(bone_kv.Value) * NodeAlignmentOffset[bone_kv.Value]);
                    MocapNodes.Add(mocapNode);
                }
            }
            SetTPosedState(false);
            _gameObject.transform.parent = _backupParent;
            _gameObject.transform.rotation = _backupRot;
        }

        private bool Validate()
        {
            bool valid = _allBones != null && _allBones.Count > 0;
            valid &= _tPoseDescription != null && _tPoseDescription.Count > 0;

            return valid;
        }


        private Transform GetTransform(HumanBodyBones bone)
        {
            Transform transform;
            _allBones.TryGetValue(bone, out transform);
            return transform;
        }

        private Quaternion GetOrigin(HumanBodyBones sourceBone)
        {
            if (UpOrigins.ContainsKey(sourceBone) && _allBones.ContainsKey(sourceBone) && BoneUpwards.ContainsKey(sourceBone))
            {
                Vector3 forward = GetForward(sourceBone);
                Quaternion origin = Quaternion.LookRotation(forward, BoneUpwards[sourceBone]);
                return origin;
            }

            return Quaternion.identity;
        }

        private Vector3 GetForward(HumanBodyBones sourceBone)
        {
            Transform boneTransform = _allBones[sourceBone];

            Vector3 bonePos = boneTransform.position;

            Tuple<HumanBodyBones, Direction> originBone = UpOrigins[sourceBone];
            HumanBodyBones targetBone = originBone.Item1;
            int direcion = (int)originBone.Item2;
            
            if (_allBones.ContainsKey(targetBone))
            {
                Vector3 targetPos = _allBones[targetBone].position;
                return (targetPos - bonePos).normalized * direcion;
            }
            else if (boneTransform.childCount > 0)
            {
                Vector3 childPos = boneTransform.GetChild(0).position;
                return (childPos - bonePos).normalized * direcion;
            }
                
            return Vector3.zero;
        }

        private void BackupCurrentState()
        {
            _backupPoseDescription = new Dictionary<Transform, SkeletonBone>();

            foreach (var kv in _tPoseDescription)
            {
                var backupBone = new SkeletonBone()
                {
                    position = kv.Key.localPosition,
                    rotation = kv.Key.localRotation,
                    scale = kv.Key.localScale,
                    name = kv.Key.name
                };
                _backupPoseDescription.Add(kv.Key, backupBone);
            }
        }

        private void SetToState(Dictionary<Transform, SkeletonBone> stateDescription)
        {
            foreach (var kv in stateDescription)
            {
                kv.Key.localPosition = kv.Value.position;
                kv.Key.localRotation = kv.Value.rotation;
                kv.Key.localScale = kv.Value.scale;
            }
        }

        public void SetTPosedState(bool tPosed)
        {
            this.IsTPosed = tPosed;
            SetToState(IsTPosed ? _tPoseDescription : _backupPoseDescription);
        }

        private float Distance(HumanBodyBones a, HumanBodyBones b)
        {
            if(_allBones.ContainsKey(a) && _allBones.ContainsKey(b))
            {
                Transform aTr = _allBones[a];
                Transform bTr = _allBones[b];
                return (bTr.position - aTr.position).magnitude;
            }
            return 0.0f;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(_gameObject);
        }

        private enum Direction { Straight = 1, Reversed = -1 };

        private static readonly Dictionary<HumanBodyBones, Tuple<HumanBodyBones, Direction>> UpOrigins = new Dictionary<HumanBodyBones, Tuple<HumanBodyBones, Direction>>
        {
            { HumanBodyBones.Hips,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Spine,                  Direction.Straight)},
            { HumanBodyBones.Spine,                     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Chest,                  Direction.Straight)},
            { HumanBodyBones.Chest,                     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.UpperChest,             Direction.Straight)},
            { HumanBodyBones.UpperChest,                new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Neck,                   Direction.Straight)},
            { HumanBodyBones.Neck,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Head,                   Direction.Straight)},
            { HumanBodyBones.Head,                      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.Neck,                   Direction.Reversed)},

            { HumanBodyBones.RightShoulder,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightUpperArm,          Direction.Reversed)},
            { HumanBodyBones.RightUpperArm,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLowerArm,          Direction.Reversed)},
            { HumanBodyBones.RightLowerArm,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightHand,              Direction.Reversed)},
            { HumanBodyBones.RightHand,                 new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleProximal,    Direction.Reversed)},

            { HumanBodyBones.LeftShoulder,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftUpperArm,           Direction.Reversed)},
            { HumanBodyBones.LeftUpperArm,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLowerArm,           Direction.Reversed)},
            { HumanBodyBones.LeftLowerArm,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftHand,               Direction.Reversed)},
            { HumanBodyBones.LeftHand,                  new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleProximal,     Direction.Reversed)},

            { HumanBodyBones.RightUpperLeg,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLowerLeg,          Direction.Reversed)},
            { HumanBodyBones.RightLowerLeg,             new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightFoot,              Direction.Reversed)},
            { HumanBodyBones.RightFoot,                 new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightToes,              Direction.Reversed)},

            { HumanBodyBones.LeftUpperLeg,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLowerLeg,           Direction.Reversed)},
            { HumanBodyBones.LeftLowerLeg,              new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftFoot,               Direction.Reversed)},
            { HumanBodyBones.LeftFoot,                  new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftToes,               Direction.Reversed)},

            //RH

            { HumanBodyBones.RightThumbProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbIntermediate,  Direction.Reversed)},
            { HumanBodyBones.RightThumbIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbDistal,        Direction.Reversed)},
            { HumanBodyBones.RightThumbDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightThumbIntermediate,  Direction.Straight)},

            { HumanBodyBones.RightIndexProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexIntermediate,  Direction.Reversed)},
            { HumanBodyBones.RightIndexIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexDistal,        Direction.Reversed)},
            { HumanBodyBones.RightIndexDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightIndexIntermediate,  Direction.Straight)},

            { HumanBodyBones.RightMiddleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleIntermediate, Direction.Reversed)},
            { HumanBodyBones.RightMiddleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleDistal,       Direction.Reversed)},
            { HumanBodyBones.RightMiddleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightMiddleIntermediate, Direction.Straight)},

            { HumanBodyBones.RightRingProximal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingIntermediate,   Direction.Reversed)},
            { HumanBodyBones.RightRingIntermediate,      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingDistal,         Direction.Reversed)},
            { HumanBodyBones.RightRingDistal,            new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightRingIntermediate,   Direction.Straight)},

            { HumanBodyBones.RightLittleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleIntermediate, Direction.Reversed)},
            { HumanBodyBones.RightLittleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleDistal,       Direction.Reversed)},
            { HumanBodyBones.RightLittleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.RightLittleIntermediate, Direction.Straight)},

            //LH

            { HumanBodyBones.LeftThumbProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbIntermediate,  Direction.Reversed)},
            { HumanBodyBones.LeftThumbIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbDistal,        Direction.Reversed)},
            { HumanBodyBones.LeftThumbDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftThumbIntermediate,  Direction.Straight)},

            { HumanBodyBones.LeftIndexProximal,         new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexIntermediate,  Direction.Reversed)},
            { HumanBodyBones.LeftIndexIntermediate,     new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexDistal,        Direction.Reversed)},
            { HumanBodyBones.LeftIndexDistal,           new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftIndexIntermediate,  Direction.Straight)},

            { HumanBodyBones.LeftMiddleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleIntermediate, Direction.Reversed)},
            { HumanBodyBones.LeftMiddleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleDistal,       Direction.Reversed)},
            { HumanBodyBones.LeftMiddleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftMiddleIntermediate, Direction.Straight)},

            { HumanBodyBones.LeftRingProximal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingIntermediate,   Direction.Reversed)},
            { HumanBodyBones.LeftRingIntermediate,      new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingDistal,         Direction.Reversed)},
            { HumanBodyBones.LeftRingDistal,            new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftRingIntermediate,   Direction.Straight)},

            { HumanBodyBones.LeftLittleProximal,        new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleIntermediate, Direction.Reversed)},
            { HumanBodyBones.LeftLittleIntermediate,    new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleDistal,       Direction.Reversed)},
            { HumanBodyBones.LeftLittleDistal,          new Tuple<HumanBodyBones, Direction>(HumanBodyBones.LeftLittleIntermediate, Direction.Straight)}

        };

        private static readonly Dictionary<HumanBodyBones, Vector3> BoneUpwards = new Dictionary<HumanBodyBones, Vector3>
        {
            { HumanBodyBones.RightUpperArm,         Vector3.up },
            { HumanBodyBones.LeftUpperArm,          Vector3.up },
            { HumanBodyBones.Spine,                 Vector3.right },
            { HumanBodyBones.Chest,                 Vector3.right },
            { HumanBodyBones.RightLowerArm,         Vector3.up },
            { HumanBodyBones.LeftLowerArm,          Vector3.up },
            { HumanBodyBones.RightUpperLeg,         Vector3.right },
            { HumanBodyBones.LeftUpperLeg,          Vector3.right },
            { HumanBodyBones.RightLowerLeg,         Vector3.right },
            { HumanBodyBones.LeftLowerLeg,          Vector3.right },
            { HumanBodyBones.RightHand,             Vector3.up },
            { HumanBodyBones.LeftHand,              Vector3.up },
            { HumanBodyBones.RightThumbProximal,    Vector3.up },
            { HumanBodyBones.RightIndexProximal,    Vector3.up },
            { HumanBodyBones.RightMiddleProximal,   Vector3.up },
            { HumanBodyBones.RightRingProximal,     Vector3.up },
            { HumanBodyBones.RightLittleProximal,   Vector3.up },
            { HumanBodyBones.LeftThumbProximal,     Vector3.up },
            { HumanBodyBones.LeftIndexProximal,     Vector3.up },
            { HumanBodyBones.LeftMiddleProximal,    Vector3.up },
            { HumanBodyBones.LeftRingProximal,      Vector3.up },
            { HumanBodyBones.LeftLittleProximal,    Vector3.up },
        };

        private static readonly Dictionary<HumanBodyBones, Quaternion> NodeAlignmentOffset = new Dictionary<HumanBodyBones, Quaternion>
        {
            { HumanBodyBones.RightUpperArm,         new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftUpperArm, 			new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.Spine, 			    new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.Chest, 			    new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.RightLowerArm,         new Quaternion(0f, 0f, 0.707f, 0.707f) },
            { HumanBodyBones.LeftLowerArm, 			new Quaternion(0f, 0f, -0.707f, 0.707f) },
            { HumanBodyBones.RightUpperLeg, 		new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftUpperLeg, 			new Quaternion(0f, 0f, -1f, 0f) },
            { HumanBodyBones.RightLowerLeg, 		new Quaternion(0f, 0f, -0.924f, 0.383f) },
            { HumanBodyBones.LeftLowerLeg, 			new Quaternion(0f, 0f, -0.383f, 0.924f) },
            { HumanBodyBones.RightHand, 			new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftHand, 			    new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.RightThumbProximal, 	new Quaternion(0f, 0f, -0.609f, 0.793f) },
            { HumanBodyBones.RightIndexProximal, 	new Quaternion(0f, 0f, -0.131f, 0.991f) },
            { HumanBodyBones.RightMiddleProximal, 	new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.RightRingProximal, 	new Quaternion(0f, 0f, 0.131f, 0.991f) },
            { HumanBodyBones.RightLittleProximal, 	new Quaternion(0f, 0f, 0.259f, 0.966f) },
            { HumanBodyBones.LeftThumbProximal, 	new Quaternion(0f, 0f, 0.609f, 0.793f) },
            { HumanBodyBones.LeftIndexProximal, 	new Quaternion(0f, 0f, 0.131f, 0.991f) },
            { HumanBodyBones.LeftMiddleProximal, 	new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftRingProximal, 		new Quaternion(0f, 0f, -0.131f, 0.991f) },
            { HumanBodyBones.LeftLittleProximal, 	new Quaternion(0f, 0f, -0.259f, 0.966f) },
        };
    } 
}