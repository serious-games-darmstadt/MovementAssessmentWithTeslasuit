using System;
using TsSDK;
using UnityEngine;

/// <summary>
/// An abstract component containing TsDeviceBehaviour.Device interface that can be used by other components.
/// </summary>
public abstract class TsDeviceBehaviour : MonoBehaviour
{
    /// <summary>
    /// Returns device interface if available. Otherwise returns null.
    /// </summary>
    public IDevice Device { get; protected set; }

    /// <summary>
    /// Connection state changed event. Note that this event is called in a thread that is different from the rendering thread.
    /// </summary>
    public event Action<TsDeviceBehaviour, bool> ConnectionStateChanged = delegate{ };

    /// <summary>
    /// Returns device connected state.
    /// </summary>
    public bool IsConnected { get; private set; }

    protected void UpdateState(IDevice device, bool isConnected)
    {
        if (isConnected == IsConnected)
        {
            return;
        }
        this.IsConnected = isConnected;
        this.Device = device;
        ConnectionStateChanged(this, isConnected);
        if (!isConnected)
        {
            this.Device = null;
        }
    }
}
