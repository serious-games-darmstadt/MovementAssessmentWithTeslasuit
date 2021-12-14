using TeslasuitAPI.Utils;
using UnityEngine;

namespace TeslasuitAPI
{
    [CreateAssetMenu(fileName = "HapticMaterial", menuName = "Teslasuit/Create/Haptic Material")]
    public class HapticMaterialAsset : HapticAsset, IHapticMaterial
    {
        public HapticSampleAsset haptic_sample;
        public HapticEffectAsset haptic_effect;

        public byte[] GetBytes()
        {
            return Bytes;
        }

        public IHapticEffect GetEffect()
        {
            return haptic_effect;
        }

        public IHapticSample GetSample()
        {
            return haptic_sample;
        }

    }

}