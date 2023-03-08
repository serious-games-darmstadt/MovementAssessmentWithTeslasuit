using System.Linq;
using TsSDK;
using UnityEngine;

/// <summary>
/// Component is used to generate haptic output from collision events.
/// </summary>
public class TsHapticCollisionHandler : MonoBehaviour
{
    /// <summary>
    /// Haptic player component
    /// </summary>
    [SerializeField]
    public TsHapticPlayer HapticPlayer;

    /// <summary>
    /// Simplified haptic channel used to get native electric channels to play haptic.
    /// </summary>
    [SerializeField]
    public TsHapticSimplifiedChannel Channel;


    /// <summary>
    /// Returns native electric channels represented by this handler.
    /// </summary>
    public IMapping2dElectricChannel[] GetChannels()
    {
        var device = HapticPlayer.Device;
        if (device == null)
        {
            return null;
        }
        return Channel.GetChannels(device);
    }

    /// <summary>
    /// Adds channels to the material playable if not exist and sets or updates its impact for a given lifetime in milliseconds.
    /// </summary>
    /// <remarks>
    /// The channels will be deleted right after the time specified in the <paramref name="duration"/> parameter.
    /// </remarks>
    /// <param name="material">Material used to set impact</param>
    /// <param name="impact">Channel impact parameter that takes values in the range [0:1]</param>
    /// <param name="duration">Duration of impact in milliseconds</param>
    public void AddImpact(IHapticMaterialPlayable material, float impact, int duration)
    {
        if (HapticPlayer.PlayerHandle == null)
        {
            return;
        }

        var channels = GetChannels();
        if (channels == null)
        {
            return;
        }
        material.Play();
        foreach (var channel in channels)
        {
            HapticPlayer.PlayerHandle.AddImpact(material, channel, impact, duration);
        }
    }

    /// <summary>
    /// Removes impact for all channels from the material playable.
    /// </summary>
    /// <param name="material">Material used to remove impact</param>
    public void RemoveImpact(IHapticMaterialPlayable material)
    {
        if (HapticPlayer.PlayerHandle  == null)
        {
            return;
        }
        var channels = GetChannels();
        if (channels == null)
        {
            return;
        }

        foreach (var channel in channels)
        {
            HapticPlayer.PlayerHandle.RemoveImpact(material, channel);
        }
    }
}
