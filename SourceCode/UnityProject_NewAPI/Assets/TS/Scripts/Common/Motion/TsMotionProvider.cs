using TsSDK;
using UnityEngine;

/*! \page ts_unity_motion Mocap
 *  \section ts_unity_mocap_data_provider Mocap data provider
 *  The mocap module covers the job of manipulating mocap data, calibrating 
 *  and applying mocap data to a character or hand model.
 *  TsMotionProvider component is used as a source of motion data in an abstract level and is required by TsHumanAnimator. 
 *  TsLiveMotionProvider component is an implementation of TsMotionProvider that starts mocap data streaming from a device
 *  when that device is available and when the component is in the "enabled" state.
 *  
 *  \image html motion_provider.JPG 
 *  
 *  \section ts_unity_suit_mocap Suit mocap
 *  TsHumanAnimator component applies mocap data given by Suit devices to animate the humanoid skeleton. 
 *  
 *  \image html human_animator.JPG
 *  
 *  TsAvatarSettings asset contains the configurable data that is used to transform the incoming mocap data 
 *  so that it applies to the target character model. This component should be attached to the model's root transform component.
 *  
 *  \image html avatar_settings.JPG
 *  
 *  An example of suit mocap can be found here: `TS/Examples/Scenes/Mocap/SuitMocap.unity`
 *  
 *  \section ts_unity_glove_mocap Glove mocap
 *  TsHandAnimator component applies mocap data given by Glove devices to animate a hand skeleton. 
 *  This component should be attached to the model's root transform component.
 *  
 *  \image html hand_animator.JPG
 *  
 *  TsHandAvatarSettings asset contains the configurable data that is used to transform the incoming mocap data so that it applies to the target hand model.
 *  
 *  \image html hand_avatar_settings.JPG
 *  
 *  An example of glove mocap can be found here: `TS/Examples/Scenes/Mocap/GloveMocap.unity`
 */

/// <summary>
/// TsMotionProvider component is used as a source of motion data in an abstract level.
/// </summary>
public abstract class TsMotionProvider : MonoBehaviour
{
    /// <summary>
    /// Gets Mocap running state.
    /// </summary>
    public abstract bool Running { get; }

    /// <summary>
    /// Gets Mocap skeleton model interface. The set of bones may be different, depending on the device that provides the data.
    /// </summary>
    /// <param name="time">For non-live mocap data providers time parameter can be used to get skeleton data by given timeframe</param>
    /// <returns><see cref="ISkeleton"/>Mocap skeleton interface</returns>
    public abstract ISkeleton GetSkeleton(float time = 0.0f);

    /// <summary>
    /// Calibrates skeleton model by known pose, depending on the device that provides the data.
    /// </summary>
    public abstract void Calibrate();

}
