using TsAPI.Types;
using TsSDK;
using UnityEngine;

/// <summary>
/// Base class for Teslasuit asset (*.ts_asset) file
/// </summary>
public abstract class TsAssetBase : ScriptableObject
{
    /// <summary>
    /// Asset file bytes.
    /// </summary>
    public byte[] Bytes { get => m_bytes; }
    
    [SerializeField]
    [HideInInspector]
    protected byte[] m_bytes = null;

    /// <summary>
    /// Teslasuit asset type
    /// </summary>
    public TsAssetType AssetType
    {
        get { return m_assetType; }
    }

    [SerializeField]
    [HideInInspector]
    private TsAssetType m_assetType = TsAssetType.Undefined;

    /// <summary>
    /// Returns an asset instance. Loads asset and returns native instance when used.
    /// </summary>
    public IAsset Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = TsAssetInstance.Create(Load);
            }
            return m_instance.Asset;
        }
    }

    private TsAssetInstance m_instance = null;

    /// <summary>
    /// Creates asset instance from byte array.
    /// </summary>
    /// <param name="bytes">Asset bytes</param>
    /// <returns>TsAssetBase Scriptable object instance</returns>
    public static TsAssetBase Create(byte[] bytes)
    {
        var type = GetAssetType(bytes);
        TsAssetBase result = null;
        switch (type)
        {
            case TsAssetType.SceneAnimation:
            case TsAssetType.PresetAnimation:
                result = CreateInstance<TsHapticAnimationAsset>();
                break;
            case TsAssetType.TouchSequence:
                result = CreateInstance<TsTouchSequenceAsset>();
                break;
            case TsAssetType.HapticEffect:
                result = CreateInstance<TsHapticEffectAsset>();
                break;
        }

        if (result != null)
        {
            result.m_bytes = bytes;
            result.m_assetType = type;
        }

        return result;
    }

    /// <summary>
    /// Loads an asset from bytes and creates IAsset in memory.
    /// </summary>
    /// <returns>IAsset asset handle</returns>
    protected virtual IAsset Load()
    {
       
        var asset = TsManager.Root.AssetManager.Load(m_bytes);
        return asset;
    }


    /// <summary>
    /// Returns Asset type by given asset bytes.
    /// </summary>
    /// <param name="bytes">Asset bytes</param>
    protected static TsAssetType GetAssetType(byte[] bytes)
    {
        TsInitializer.Initialize();
        var root = new TsRoot();
        var assetRaw = root.AssetManager.Load(bytes);
        var type = assetRaw.AssetType;
        root.Dispose();
        root = null;
        return type;
    }
}
