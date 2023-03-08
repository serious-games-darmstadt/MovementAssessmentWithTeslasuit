using System;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

/*! \page ts_unity_haptic Haptic
 *  Haptic subsystem provides aggregated access to the haptic playback.
 *  \section ts_unity_ts_haptic_player Haptic player component
 *  TsHapticPlayer component is used to provide device IHapticPlayer interface to other components when it's available.
 *  TsDeviceBehaviour component is required.
 *  
 *  \section ts_unity_ts_haptic_playback Haptic playback
 *  
 *  \subsection ts_unity_haptic_assets Haptic assets
 *  Teslasuit Unity plugin supports for import the following haptic assets (*.ts_asset extension):
 *  - HapticPresetAnimationAsset
 *  - HapticEffectAsset
 *  - TouchSequenceAsset
 *  
 *  All imported Teslasuit assets are derived from TsAssetBase. Once an asset has been imported, it can be accessed and loaded with TsAssetBase.Instance.
 *  An example of haptic animation asset playback can be found here: `TS/Examples/Scenes/Haptic/HapticAnimationPlayback.unity`
 *  
 *  \subsection ts_unity_ts_haptic_channels Haptic channels
 *  The atomic unit of the place where a haptic signal is played is a haptic channel.
 *  There are different versions of devices with different haptic channel mapping, so targeting a haptic to the same place on the body 
 *  between the devices can be implemented with Simplified haptic channels.
 *  TsHapticSimplifiedChannel asset can be created by choosing the asset menu: `Assets/Create/Teslasuit/Haptic/Simplified haptic channel`.
 *  There are 2 properties determining from what body part the channels will be used for a haptic playback:
 *  - BoneIndex (For example, RightUpperArm)
 *  - BoneSide (For example, Front)
 *  
 *  \image html haptic_simplified_channel.JPG
 *  
 *  To use characters within a haptic feature, haptic channels must be mapped on the 3d model.
 *  TsSimplifiedCollisionBuilder is a editor component that can build collision by given simplified haptic channels.
 *  It should be added to the same GameObject where SkinnedMeshRenderer component is placed.
 *  
 *  \image html haptic_simplified_collision_builder.JPG
 *  
 *  - <b>BoneWeightThreshold</b> parameter takes the values from the range [0:1] and used as a threshold parameter for bone vertices selection.
 *  The smaller the BoneWeightThreshold parameter, the more bordering vertices between the bones will be selected to build the collision.
 *  - <b>AvatarSettings</b> - TsAvatarSettings asset used to generate collision.
 *  - <b>Channels</b> - TsSHapticSimplifiedChannel assets used to generate collision.
 *  - <b>HapticPlayer</b> - target TsHapticPlayer component where the collision events will be converted to hatpic.
 *  - <b>Build</b> property should be checked to rebuild the collision.
 *  
 *  When collision building is done, Collider objects with TsHapticCollisionHandler component will be generated.
 *  It can look like this:
 *  
 *  Character | Hierarchy | Collision handler
 *  ----------|-----------| -----------------
 *  \image html haptic_simplified_collision_builder_channels.JPG | \image html haptic_simplified_collision_builder_hierarchy.JPG | \image html haptic_simplified_collision_builder_handler.JPG
 *  
 *  An example of collison builder can be found here: `TS/Examples/Scenes/Haptic/BoneChannels.unity`
 *  After this actions, the character is configured to receive collisions events and generate haptic.
 *  
 *  \subsection ts_unity_ts_haptic_material Haptic material
 *  Haptic material is a kind of haptic asset that can be created inside Unity Editor.
 *  To create Haptic mateiral asset, choose the asset menu: `Assets/Create/Teslasuit/Haptic/Haptic Material`.
 *  Haptic material asset have the following properties:
 *  - <b>Haptic effect</b> - haptic effect asset reference.
 *  - <b>Touch sequence</b> - touch sequence asset reference.
 *  If all properies is set correctly, it can be instantiated later to use as haptic material playable.
 *  
 *  Component that uses Haptic material to generate haptic depended on collision impulse called TsHapticImpulseMaterialObject.
 *  It should be added to the physical object that can generate collision events (have Rigidbody and Collider components).
 *  TsHapticImpulseMaterialObject have the following properties:
 *  - <b>Max Impulse</b> Max impulse value that used to generate normalized impact. 
 *  For example, if `Max Impulse = 100`, collsion with `impulse = 50` will generate `impact = 50 / 100 = 0.5`.
 *  - <b>Min Collision Duration Ms</b> The minimal haptic duration interval when material objects is colliding.
 *  - <b>Haptic Material</b> Haptic Material asset reference.
 *  
 *  \image html haptic_impulse_material_object.JPG
 *  
 *  An example of hatpic material setup can be found here: `TS/Examples/Scenes/Haptic/BoneChannels.unity`
 *  
 *  \subsection ts_unity_ts_touches Playing touches
 *  
 *  Touch is an atomic playable item with constant haptic parameters that is addressed to the channel group.
 *  Touch can be created with TsHapticPlayer.CreateTouch with the following parameters:
 *  - frequency - Touch Frequency parameter. Should be in the range [0:150]
 *  - amplitude - Touch Amplitude parameter. Should be in the range [0:100]
 *  - pulseWidth - Touch PulseWidth parameter. Should be in the range [0:320]
 *  - duration - Touch Duration in milliseconds
 *  
 *  An example of the touch playback can be found here:`TS/Examples/Scenes/Haptic/PlayingTouches.unity`
 *  
 *  \subsection ts_unity_ts_multiplier Playable and player multipliers
 *  
 *  Haptic multipliers are used to get or control haptic output parameters multiplication coefficient.
 *  Master multipliers affects all playables playing in IHapticPlayer.
 *  Local multipliers can be used in IHapticPlayable context.
 *  Example for IHapticPlayer:
 *  
 *  @code
 *      public void TestMultiplier(IDevice device, IHapticPlayer player)
 *      {
 *          List<TsHapticParamMultiplier> tempMultipliers = player.MasterMultipliers.ToList();
 *
 *          for(int i=0; i<tempMultipliers.Count; ++i)
 *          {
 *              if(tempMultipliers[i].type == TsHapticParamType.Amplitude)
 *              {
 *                  tempMultipliers[i] = new TsHapticParamMultiplier(TsHapticParamType.Amplitude, 0.5f);
 *              }
 *          }
 *          player.MasterMultipliers = tempMultipliers;
 *      }
 *  @endcode
 *  
 *  Example for IHapticPlayable:
 *  @code
 *      public void TestMultiplier(IDevice device, IHapticPlayable playable)
 *      {
 *          List<TsHapticParamMultiplier> tempMultipliers = playable.Multipliers.ToList();
 *
 *          for(int i=0; i<tempMultipliers.Count; ++i)
 *          {
 *              if(tempMultipliers[i].type == TsHapticParamType.Amplitude)
 *              {
 *                  tempMultipliers[i] = new TsHapticParamMultiplier(TsHapticParamType.Amplitude, 0.5f);
 *              }
 *          }
 *          playable.Multipliers = tempMultipliers;
 *      }
 *  @endcode
 */
[RequireComponent(typeof(TsDeviceBehaviour))]
public class TsHapticPlayer : MonoBehaviour
{
    /// <summary>
    /// Handle to IHapticPlayer device interface.
    /// </summary>
    public IHapticPlayer PlayerHandle { get => m_hapticPlayer; }

    private TsDeviceBehaviour m_deviceBehaviour;

    private IHapticPlayer m_hapticPlayer;

    /// <summary>
    /// Target device that providing IHapticPlayer interface.
    /// </summary>
    public IDevice Device
    {
        get { return m_deviceBehaviour.Device; }
    }

    private void Start()
    {
        m_deviceBehaviour = GetComponent<TsDeviceBehaviour>();
        m_deviceBehaviour.ConnectionStateChanged += DeviceBehaviour_ConnectionStateChanged;
        if (m_deviceBehaviour.IsConnected)
        {
            DeviceBehaviour_ConnectionStateChanged(m_deviceBehaviour, true);
        }
    }

    private void DeviceBehaviour_ConnectionStateChanged(TsDeviceBehaviour devicetBehaviour, bool connected)
    {
        if(connected)
        {
            m_hapticPlayer = devicetBehaviour.Device.HapticPlayer;
        }
        else
        {
            m_hapticPlayer = null;
        }
    }

    /// <summary>
    /// Returns IHapticPlayable by given haptic <paramref name="asset"/>. Should be used to access cross-device playable instance.
    /// </summary>
    /// <param name="asset">Asset used to get playable.</param>
    /// <returns>IHapticPlayable interface</returns>
    public IHapticPlayable GetPlayable(IHapticAsset asset)
    {
        if (m_hapticPlayer == null)
        { 
            return null;
        }
        return m_hapticPlayer.GetPlayable(asset);
    }

    /// <summary>
    /// Creates touch playable from touch parameters.
    /// </summary>
    /// <param name="frequency">Touch Frequency parameter. Should be in the range [0:150]</param>
    /// <param name="amplitude">Touch Amplitude parameter. Should be in the range [0:100]</param>
    /// <param name="pulseWidth">Touch PulseWidth parameter. Should be in the range [0:320]</param>
    /// <param name="duration">Touch Duration in milliseconds</param>
    public IHapticDynamicPlayable CreateTouch(int frequency, int amplitude, int pulseWidth, long duration)
    {
        TsHapticParam[] touchParams = new TsHapticParam[3];
        touchParams[0] = new TsHapticParam(TsHapticParamType.Period, (ulong) (1000000 / frequency));
        touchParams[1] = new TsHapticParam(TsHapticParamType.Amplitude, (ulong) amplitude);
        touchParams[2] = new TsHapticParam(TsHapticParamType.PulseWidth, (ulong) pulseWidth);
        return m_hapticPlayer.CreateTouch(touchParams, new IntPtr[0], (ulong)duration);
    }

}
