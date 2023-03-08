using TsSDK;
using UnityEngine;

[RequireComponent(typeof(TsDeviceBehaviour))]
public class TsPpgProvider : MonoBehaviour
{
    private TsDeviceBehaviour m_deviceBehaviour;
    private IPpg m_ppg;

    public bool IsRunning
    {
        get { return m_ppg != null && m_ppg.IsRunning; }
    }

    // Start is called before the first frame update
    void Start()
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
        if (isConnected)
        {
            m_ppg = m_deviceBehaviour.Device.Biometry.Ppg;
            StartInternal();
        }
        else
        {
            m_ppg = null;
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
        if (m_ppg != null && !m_ppg.IsRunning)
        {
            m_ppg.Start();
        }
    }

    private void StopInternal()
    {
        if (m_ppg != null && m_ppg.IsRunning)
        {
            m_ppg.Stop();
        }
    }

    public IProcessedPpgData GetData()
    {
        return m_ppg?.ProcessedData;
    }
}
