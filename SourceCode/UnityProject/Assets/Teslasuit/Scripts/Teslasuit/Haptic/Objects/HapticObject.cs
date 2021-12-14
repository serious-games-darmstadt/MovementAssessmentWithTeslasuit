using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI
{
    public abstract class HapticObject : MonoBehaviour
    {
        public abstract HapticAsset HapticAsset { get; }

        private void Reset()
        {
            this.gameObject.layer = HapticConstants.Util.HapticObjectLayer;
        }

        [SerializeField]
        [Range(0, 1)]
        protected float forceMultiplier = 1.0f;


    } 
}
