using TsSDK;
using UnityEngine;

/// <summary>
/// Material object component where the haptic impact is depended from physical impulse value.
/// </summary>
public class TsHapticImpulseMaterialObject : MonoBehaviour
{
    /// <summary>
    /// Max impulse value that used to generate normalized impact. 
    /// </summary>
    public float maxImpulse = 100.0f;
    
    /// <summary>
    /// The minimal haptic duration interval when material objects is colliding.
    /// </summary>
    public int minCollisionDurationMs = 50;

    /// <summary>
    /// Haptic material asset reference
    /// </summary>
    [SerializeField]
    private TsHapticMaterialAsset m_hapticMaterial;

    //Note to self:
    //OnEnter we are playing whatever playable is. But it doesnt have anything to play yet
    //OnStay we are adding the actual things to play
    //OnExit things to be played are removed


    private void OnCollisionEnter(Collision collision)
    {
        var collisionHandler = collision.gameObject.GetComponent<TsHapticCollisionHandler>();

        if(collisionHandler != null && collisionHandler.HapticPlayer.Device != null)
        {
            var device = collisionHandler.HapticPlayer.Device;
            var playable = device.HapticPlayer.GetPlayable(m_hapticMaterial.Instance as IHapticAsset) as IHapticMaterialPlayable;
            
           playable.Play();//dont know how this functions
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var collisionHandler = collision.gameObject.GetComponent<TsHapticCollisionHandler>();

        if(collisionHandler != null && collisionHandler.HapticPlayer.Device != null)
        {
            var playable = collisionHandler.HapticPlayer.PlayerHandle.GetPlayable(m_hapticMaterial.Instance as IHapticAsset) as IHapticMaterialPlayable;
            //here is where the asset being added to the playable list
            collisionHandler.AddImpact(playable, collision.impulse.magnitude / maxImpulse, minCollisionDurationMs);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var collisionHandler = collision.gameObject.GetComponent<TsHapticCollisionHandler>();

        if (collisionHandler != null && collisionHandler.HapticPlayer.Device != null)
        {
            var playable = collisionHandler.HapticPlayer.PlayerHandle.GetPlayable(m_hapticMaterial.Instance as IHapticAsset) as IHapticMaterialPlayable;
            collisionHandler.RemoveImpact(playable);
        }
    }

}
