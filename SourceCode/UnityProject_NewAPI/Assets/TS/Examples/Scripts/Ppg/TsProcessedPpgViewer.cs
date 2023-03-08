using System.Collections.Generic;
using System.Linq;
using TsSDK;
using UnityEngine;

public class TsProcessedPpgViewer : MonoBehaviour
{
    [SerializeField]
    private TsPpgProvider m_ppgProvider;

    [SerializeField] 
    private TsProcessedPpgViewItem m_viewItemPrefab;

    private Dictionary<int, TsProcessedPpgViewItem> m_viewItems = new Dictionary<int, TsProcessedPpgViewItem>();

    private void Start()
    {
        if (m_ppgProvider == null)
        {
            Debug.LogError("Processed ppg provider not found!");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ppgProvider.IsRunning)
        {
            var data = m_ppgProvider.GetData();
            if (data == null)
            {
                return;
            }
            var disconnectedNodes = m_viewItems.Where((item) => data.NodesData.All(node => node.nodeIndex != item.Key));
            foreach (var disconnected in disconnectedNodes)
            {
                m_viewItems.Remove(disconnected.Key);
            }

            foreach (var nodeData in data.NodesData)
            {
                UpdateNodeData(nodeData);
            }
        }
    }

    private void UpdateNodeData(ProcessedPpgNodeData nodeData)
    {
        TsProcessedPpgViewItem item = null;
        if (m_viewItems.ContainsKey(nodeData.nodeIndex))
        {
            item = m_viewItems[nodeData.nodeIndex];
        }
        else
        {
            item = Instantiate(m_viewItemPrefab);
            item.transform.SetParent(transform);
            m_viewItems.Add(nodeData.nodeIndex, item);
        }
        item.UpdateView(nodeData);
    }
}
