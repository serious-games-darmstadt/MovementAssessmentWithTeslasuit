using TeslasuitAPI;
using System;
using UnityEngine;
using UnityEngine.Events;

/** @defgroup UnityComponents
 *
 * 
 *
 */

namespace TeslasuitAPI
{
    [Serializable]
    public class OnSuitConnectionEvent : UnityEvent<SuitHandleObject> { }

    /**@addtogroup UnityComponents */
    /*@{*/

    /// <summary>
    /// Main component responsible for suit management
    /// </summary>
    public class SuitAPIObject : SuitHandleObject
    {
        public OnSuitConnectionEvent OnSuitConnected;
        public OnSuitConnectionEvent OnSuitDisconnected;

        new public void Start()
        {
            base.Start();
            BecameAvailable += (suithandle)=> { OnSuitConnected?.Invoke(suithandle); };
            BecameUnavailable += (suithandle)=> { OnSuitDisconnected?.Invoke(suithandle); };
        }
        /// <summary>
        /// Mocap module of suit
        /// </summary>
        public IMocap Mocap
        {
            get
            {
                if (IsAvailable)
                    return Suit.Mocap;
                return null;
            }
        }

        /// <summary>
        /// haptic module
        /// </summary>
        public IHaptic Haptic
        {
            get
            {
                if (IsAvailable)
                    return Suit.Haptic;
                return null;
            }
        }

        public IGeneral General
        {
            get
            {
                if (IsAvailable)
                    return Suit.General;
                return null;
            }
        }

        /// <summary>
        /// biometry module
        /// </summary>
        public IBiometry Biometry
        {
            get
            {
                if (IsAvailable)
                    return Suit.Biometry;
                return null;
            }
        }

    }
    /*@}*/
}