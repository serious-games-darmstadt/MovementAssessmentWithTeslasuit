using System.Linq;
using TsSDK;
using UnityEngine;

/// <summary>
/// Component that provides suit device interface by given SuitIndex.
/// </summary>
public class TsSuitBehaviour : TsDeviceBehaviour
{
    /// <summary>
    /// Suit index used by component. 
    /// </summary>
    public SuitIndex TargetSuitIndex
    {
        get { return m_suitIndex; }
        set
        {
            if (m_suitIndex != value)
            {
                if (Device != null)
                {
                    UpdateState(null, false);
                }
                m_suitIndex = value;
                ValidateSuitIndex();
            }
        }
    }

    /// <summary>
    /// Returns suit device interface if available. Otherwise returns null.
    /// </summary>
    public ISuit Suit { get { return (ISuit)Device; } }
    
    [SerializeField]
    private SuitIndex m_suitIndex = SuitIndex.Suit0;
    
    private void Start()
    {
        var suitManager = TsManager.Root.SuitManager;
        suitManager.OnSuitConnected += OnSuitConnected;
        suitManager.OnSuitDisconnected += OnSuitDisconnected;

        foreach (var suit in suitManager.Suits)
        {
            OnSuitConnected(suit);
        }
    }

    private void OnSuitConnected(ISuit obj)
    {
        if (obj.Index != TargetSuitIndex)
        {
            return;
        }
        UpdateState(obj, true);
    }

    private void OnSuitDisconnected(ISuit obj)
    {
        if (obj.Index != TargetSuitIndex)
        {
            return;
        }
        UpdateState(null, false);
    }

    private void ValidateSuitIndex()
    {
        var suitManager = TsManager.Root.SuitManager;
        var targetSuitExpr = suitManager.Suits.Where(item => item.Index == m_suitIndex);
        if (targetSuitExpr.Any())
        {
            OnSuitConnected(targetSuitExpr.First());
        }
    }
}
