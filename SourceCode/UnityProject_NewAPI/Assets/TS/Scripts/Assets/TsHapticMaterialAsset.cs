using TsSDK;
using UnityEngine;

/// <summary>
/// Haptic material used to create IHapticMaterial playable.
/// </summary>
[CreateAssetMenu(menuName = "Teslasuit/Haptic/Haptic Material")]
public class TsHapticMaterialAsset : TsAssetBase
{
    [SerializeField]
    private TsHapticEffectAsset m_hapticEffect;

    [SerializeField]
    private TsTouchSequenceAsset m_touchSequence;


    protected override IAsset Load()
    {
        return TsManager.Root.AssetManager.CreateMaterialAsset((IHapticAsset)m_touchSequence.Instance, (IHapticAsset)m_hapticEffect.Instance);
    }
}
