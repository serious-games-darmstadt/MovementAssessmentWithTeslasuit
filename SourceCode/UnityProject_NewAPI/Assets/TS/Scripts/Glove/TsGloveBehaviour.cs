using TsAPI.Types;
using TsSDK;
using UnityEngine;

/// <summary>
/// Component that provides glove device interface by given GloveIndex and TsDeviceSide.
/// </summary>
public class TsGloveBehaviour : TsDeviceBehaviour
{
    /// <summary>
    /// Glove index used by component. 
    /// </summary>
    public GloveIndex TargetGloveIndex { get { return m_gloveIndex; } }

    /// <summary>
    /// Glove side used by component. 
    /// </summary>
    public TsDeviceSide TargetGloveSide { get { return m_gloveSide; } }


    /// <summary>
    /// Returns glove device interface if available. Otherwise returns null.
    /// </summary>
    public IGlove Glove { get { return (IGlove)Device; } }

    [SerializeField]
    private GloveIndex m_gloveIndex = GloveIndex.Glove0;
    [SerializeField] 
    private TsDeviceSide m_gloveSide = TsDeviceSide.Right;

    void Start()
    {
        var gloveManager = TsManager.Root.GloveManager;
        gloveManager.OnGloveConnected += OnGloveConnected; ;
        gloveManager.OnGloveDisconnected += OnGloveDisconnected;

        switch (m_gloveSide)
        {
            case TsDeviceSide.Left:
            {
                foreach (var glove in gloveManager.LeftGloves)
                {
                    OnGloveConnected(glove);
                }
                break;
            }
            case TsDeviceSide.Right:
            {
                foreach (var glove in gloveManager.RightGloves)
                {
                    OnGloveConnected(glove);
                }
                break;
            }
        }
    }

    private void OnGloveConnected(IGlove obj)
    {
        if(obj.Index == TargetGloveIndex && obj.Side == m_gloveSide)
        {
            UpdateState(obj, true);
        }
    }

    private void OnGloveDisconnected(IGlove obj)
    {
        if(obj.Index == TargetGloveIndex && obj.Side == m_gloveSide)
        {
            UpdateState(obj, false);
        }
    }
}
