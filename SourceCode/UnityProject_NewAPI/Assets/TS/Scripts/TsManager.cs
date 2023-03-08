using System;
using System.IO;
using System.Runtime.InteropServices;
using TsSDK;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Root component aggregator that is used by Teslasuit components.
/// It can be placed into the scene where the Teslasuit components are used, but it's not required at all.
/// TsManager sets DontDestroyOnLoad flag, so the object with this component will be alive between the scenes.
/// </summary>
public class TsManager : MonoBehaviour
{
    public static TsManager Instance { get; private set; }

    /// <summary>
    /// Returns TsRoot instance created in this session.
    /// </summary>
    public static TsRoot Root
    {
        get
        {
            if (Instance == null)
            {
                var go = new GameObject("TsManager");
                Instance = go.AddComponent<TsManager>();
                if (Instance.m_root == null)
                {
                    Instance.InitTsManager();
                }
            }
            return Instance.m_root;
        }
    }

    private TsRoot m_root;

    private void Awake()
    {
        if (m_root != null)
        {
            return;
        }
        InitTsManager();
    }

    private void InitTsManager()
    {
        // Only allow one instance at runtime.
        if (Instance != null)
        {
            enabled = false;
            DestroyImmediate(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        InitializeRoot();
    }

    private void InitializeRoot()
    {
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
#endif
        m_root = new TsRoot();
        Debug.Log("[TS] TsManager initialized.");
    }

    private void OnDestroy()
    {
        Destroy();
    }

    private void Destroy()
    {
        if (m_root != null)
        {
            Debug.Log("[TS] TsManager destroyed.");
            m_root.Dispose();
            m_root = null;
        }
        if (Instance == this)
        {
            Instance = null;
        }

#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload -= BeforeAssemblyReload;
#endif
    }

#if UNITY_EDITOR
    private void BeforeAssemblyReload()
    {
        Destroy();
    }
#endif
}
