using TsSDK;
using UnityEngine;

[RequireComponent(typeof(TsDeviceBehaviour))]
public class TsLiveMotionProvider : TsMotionProvider
{
    private TsDeviceBehaviour m_deviceBehaviour;
    private IMocap m_mocap;

    public override bool Running
    {
        get
        {
            return m_mocap != null && m_mocap.IsRunning;
        }
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

    private void DeviceBehaviour_ConnectionStateChanged(TsDeviceBehaviour deviceBehaviour, bool isConnected)
    {
        if(isConnected)
        {
            m_mocap = m_deviceBehaviour.Device.Mocap;
            StartInternal();
        }
        else
        {
            m_mocap = null;
        }
        
    }

    void OnEnable()
    {
        StartInternal();
    }

    void OnDisable()
    {
        StopInternal();
    }

    private void StartInternal()
    {
        if (m_mocap != null && !m_mocap.IsRunning)
        {
            m_mocap.Start();
        }
    }

    private void StopInternal()
    {
        if (m_mocap != null && m_mocap.IsRunning)
        {
            m_mocap.Stop();
        }
    }

    public override ISkeleton GetSkeleton(float time = 0)
    {
        return m_mocap?.Skeleton;
    }

    public override void Calibrate()
    {
        m_mocap?.Calibrate();
    }

}
