using System;
using TsSDK;
using UnityEngine;

/// <summary>
/// Runtime Asset representation. Once instantiated, TsAssetInstance lifetime is managed by unity.
/// </summary>
public class TsAssetInstance : ScriptableObject
{
    /// <summary>
    /// Asset reference represented by this instance
    /// </summary>
    public IAsset Asset
    {
        get { return m_asset; }
    }

    private IAsset m_asset = null;

    /// <summary>
    /// Creates Asset instance
    /// </summary>
    /// <param name="assetCreator">Functor, used to create IAsset.</param>
    /// <returns>TsAssetInstance</returns>
    public static TsAssetInstance Create(Func<IAsset> assetCreator)
    {
        var instance = CreateInstance<TsAssetInstance>();
        instance.m_asset = assetCreator();
        
        return instance;
    }

    private void OnDestroy()
    {
        if (m_asset != null && TsManager.Instance != null)
        {
            TsManager.Root.AssetManager.Unload(m_asset);
        }
    }
}
