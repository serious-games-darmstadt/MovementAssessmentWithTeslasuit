using UnityEngine;
using UnityEditor;
using System;

namespace TeslasuitAPI
{
    [RequireComponent(typeof(Renderer))]
    public class HapticMesh : HapticReceiver
    {
        [SerializeField]
        public HapticHitMappingAsset _hitMappingAsset;
        public HapticHitMappingAsset MappingAsset { get { return _hitMappingAsset; } }

        public event Action<IHapticMapping> HitMappingUpdated;

        public int[] SubMeshIndexes { get { return subMeshIndexes; } }
        [SerializeField]
        private int[] subMeshIndexes;

        private IHapticMapping _hitMapping;
        public IHapticMapping HitMapping
        {
            get { return _hitMapping; }
            private set
            {
                _hitMapping = value;
                HitMappingUpdated?.Invoke(_hitMapping);
            }
        }

        public MeshObjectInfo MeshObjectInfo
        {
            get
            {
                return _meshObjectInfo;
            }
        }
        private MeshObjectInfo _meshObjectInfo;
        protected override void Awake()
        {
            UpdateMeshInfo();
            base.Awake();
        }

        private void UpdateMeshInfo()
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();

            if (skinnedMesh != null)
                _meshObjectInfo = new MeshObjectInfo(this.gameObject, skinnedMesh, SubMeshIndexes);
            else
            {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null)
                    _meshObjectInfo = new MeshObjectInfo(this.gameObject, meshFilter, subMeshIndexes);
            }

            if (_meshObjectInfo == null)
                Debug.LogWarning("SkinnedMeshRenderer or MeshFilter component is not found. HapticMesh only works with one of these components.");
        }


        protected override void OnSuitApiBecameAvailable(SuitHandleObject obj)
        {
            HitMapping = SuitAPI.Haptic.CreateHitMapping(MappingAsset);
            base.OnSuitApiBecameAvailable(obj);
        }

        protected override void OnSuitApiBecameUnavailable(SuitHandleObject obj)
        {
            HitMapping = null;
            base.OnSuitApiBecameUnavailable(obj);
        }

        public override void Hit(HapticCollision collision)
        {
            _hitMapping.Hit(collision);
        }

        public override void Hit(HapticCollision[] collisionBuffer, int count)
        {
            _hitMapping.Hit(collisionBuffer, count);
        }

        public override void PointHit(HapticPointHit point_hit)
        {
            _hitMapping.PointHit(point_hit);
        }

        public override void CircleHit(HapticCircleHit circle_hit)
        {
            _hitMapping.CircleHit(circle_hit);
        }

        public override void PolyHit(HapticPolyHit poly_hit)
        {
            _hitMapping.PolyHit(poly_hit);
        }
    }
}

