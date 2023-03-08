using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedSuitViewer : MonoBehaviour
{
    [SerializeField]
    private TsSuitBehaviour m_suitBehaviour;

    [SerializeField]
    private Text m_suitNameLabel;


    // Update is called once per frame
    void Update()
    {
        if (m_suitBehaviour != null)
        {
            if (m_suitBehaviour.IsConnected)
            {
                m_suitNameLabel.text = m_suitBehaviour.Suit.Ssid;
            }
            else
            {
                m_suitNameLabel.text = "Disconnected";
            }
        }
    }
}
