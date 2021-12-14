using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticReceiver : MonoBehaviour
    {
        [SerializeField]
        private SuitAPIObject _suitApi;
        public SuitAPIObject SuitAPI { get { return _suitApi; } }

        private IHapticMapping _mapping;

        protected virtual void Awake()
        {
            _suitApi.BecameAvailable += OnSuitApiBecameAvailable;
            _suitApi.BecameUnavailable += OnSuitApiBecameUnavailable;
        }

#region SuitAPI object
        protected virtual void OnSuitApiBecameAvailable(SuitHandleObject obj)
        {
            _mapping = SuitAPI.Haptic.SourceMapping;
        }

        protected virtual void OnSuitApiBecameUnavailable(SuitHandleObject obj)
        {
            _mapping = null;
        }
#endregion

        protected virtual void OnDestroy()
        {
            if (_suitApi != null)
            {
                _suitApi.BecameAvailable -= OnSuitApiBecameAvailable;
                _suitApi.BecameUnavailable -= OnSuitApiBecameUnavailable;
            }
        }

        #region Haptic Playable

        public virtual void Hit(HapticCollision collision)
        {
            _mapping.Hit(collision);
        }

        public virtual void Hit(HapticCollision[] collisionBuffer, int count)
        {
            _mapping.Hit(collisionBuffer, count);
        }

        public virtual void PointHit(HapticPointHit point_hit)
        {
            _mapping.PointHit(point_hit);
        }

        public virtual void CircleHit(HapticCircleHit circle_hit)
        {
            _mapping.CircleHit(circle_hit);
        }

        public virtual void PolyHit(HapticPolyHit poly_hit)
        {
            _mapping.PolyHit(poly_hit);
        }

        public virtual void PlayAnimation(IHapticAnimation animation)
        {
            _mapping.Play(animation);
        }

        public virtual void UpdateAnimation(IHapticAnimation animation)
        {
            bool is_playing = _mapping.IsPlaying(animation);
            if (!is_playing)
                PlayAnimation(animation);
        }

        public virtual bool IsPlaying(IHapticAnimation animation)
        {
            return _mapping.IsPlaying(animation);
        }

        public virtual void StopAnimation(IHapticAnimation animation)
        {
            StopAnimation(animation);
        }
#endregion
    }

}