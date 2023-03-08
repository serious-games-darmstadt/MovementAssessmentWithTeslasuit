using TsSDK;
using UnityEngine;
using UnityEngine.UI;

public class TsProcessedPpgViewItem : MonoBehaviour
{
    [SerializeField]
    private Text m_nodeLabel;

    [SerializeField]
    private Text m_heartrateLabel;

    [SerializeField]
    private Text m_spo2Label;

    private Color m_normalColor;
    private Color m_warningColor;

    private void Start()
    {
        m_normalColor = Color.green;
        m_warningColor = Color.yellow;
    }

    public void UpdateView(ProcessedPpgNodeData data)
    {
        m_nodeLabel.text = data.nodeIndex.ToString();
        m_heartrateLabel.text = data.heartRate.ToString();
        m_heartrateLabel.color = data.isHeartrateValid ? m_normalColor : m_warningColor;
        m_spo2Label.text = data.oxygenPercent.ToString();
        m_spo2Label.color = data.isOxygenPercentValid ? m_normalColor : m_warningColor;
    }
}
