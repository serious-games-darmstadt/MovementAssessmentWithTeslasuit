using System;
using System.IO;
using UnityEngine;

/*! \page ts_getting_started Getting started
 *  Teslasuit plugin initialization takes place in several stages, including the search and loading of libraries,
 *  and the initialization call itself in the API.
 *  
 *  \section Initialization
 *  \subsection TsInitializer
 *  TsInitializer class uses RuntimeInitializeOnLoadMethod to load the teslasuit native libraries before the editor or built scene will be loaded.
 *  As the teslasuit native libraries directory are not located in Unity project, they are need to be loaded manually from TsInitializer.
 *  So, TsInitializer will load Teslasuit native libraries from this point.
 *  If the Teslasuit SDK is not installed properly, DllNotFoundException will be thrown with a message 
 *  "Teslasuit api library not found! Please sure that Teslasuit SDK is installed.".
 *  
 *  
 *  \subsection TsManager
 *  TsManager component is a root component that is used by Teslasuit components.
 *  It can be placed into the scene where the Teslasuit components are used, but it's not required at all (it can be added to the scene in 
 *  runtime as used by package components or user scripts).
 *  TsManager sets DontDestroyOnLoad flag, so the object with this component will be alive between the scenes.
 *  
 *  @code
 *  using TsSDK;
 *  
 *  ...
 *  
 *  var root = TsManager.TsRoot;
 *  var suitManager = root.SuitManager;
 *  var suit = suitManager.Suits[0];
 *  @endcode
 *  
 *  \section device_behavour Device behaviour
 *  TsDeviceBehaviour is an abstract component containing <see cref="TsDeviceBehaviour.Device"/> interface that can be used by other components.
 *  TsSuitBehaviour and TsGloveBehaviour components are derived from TsDeviceBehaviour and can be added to GameObject.
 *  Note that <see cref="TsDeviceBehaviour.ConnectionStateChanged"/> event is called from thread that is non-rendering thread.
 *  @code
 *  using TsSDK;
 *  
 *  ...
 *  
 *  private TsDeviceBehaviour m_deviceBehaviour;
 *  
 *   private void Start()
 *   {
 *       m_deviceBehaviour = GetComponent<TsDeviceBehaviour>();
 *       m_deviceBehaviour.ConnectionStateChanged += DeviceBehaviour_ConnectionStateChanged;
 *       if (m_deviceBehaviour.IsConnected)
 *       {
 *           DeviceBehaviour_ConnectionStateChanged(m_deviceBehaviour, true);
 *       }
 *   }
 *   
 *   private void DeviceBehaviour_ConnectionStateChanged(TsDeviceBehaviour deviceBehaviour, bool isConnected)
 *   {
 *       if(isConnected)
 *       {
 *           Debug.Log("device connected:" + m_deviceBehaviour.Device.Ssid);
 *       }
 *       else
 *       {
 *           Debug.Log("device disconnected:" + m_deviceBehaviour.Device.Ssid);
 *       }
 *   }
 *  @endcode
 *  
 *  <see cref="TsSuitBehaviour"/> provides device with a ProductType=TsProductType.Suit only. SuitIndex parameter means that device 
 *  to be provided is a suit device with choosen index.
 *  For example, to access the first suit attached to the system, SuitIndex.Suit0 should be set:
 *  
 *  \image html device_behaviour_suit.JPG   
 *  
 *  <see cref="TsGloveBehaviour"/> provides device with a ProductType=TsProductType.Glove only. GloveIndex parameter means that device 
 *  to be provided is a glove device with choosen index.
 *  Glove devices also are separated by TsDeviceSide parameter. It is used to determine, which glove (Left | Right) is provided by this component.
 *  For example, to access the first right glove attached to the system, GloveIndex.Glove0 and TsDeviceSide.Right should be set:
 *  
 *  \image html device_behaviour_glove.JPG 
 */

/// <summary>
/// Uses RuntimeInitializeOnLoadMethod to load the teslasuit native libraries before the editor or built scene will be loaded.
/// As the teslasuit native libraries directory are not located in Unity project, they are need to be loaded manually.
/// So, TsInitializer will load Teslasuit native libraries.
/// If the Teslasuit SDK is not installed properly, DllNotFoundException will be thrown with a message 
/// "Teslasuit api library not found! Please sure that Teslasuit SDK is installed.".
/// </summary>
public class TsInitializer
{
    public static IntPtr Handle {get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        var impl = TsInitializerImpl.Get();
        if(impl == null)
        {
            Debug.LogError("[TS] Failed to initialize! Current platform is not supported.");
            return;
        }
        if(impl.IsLibraryLoaded())
        {
            return;
        }
        Debug.Log("[TS] Locating libraries...");
        var sdkInstallation = InitSDKInstallation(impl);
        Debug.Log($"[TS] Libraries loaded from path {sdkInstallation}.");
    }

    private static string InitSDKInstallation(TsInitializerImpl impl)
    {
        var libPath = impl.GetAPILibraryPath();
        var libName = impl.GetLibName(libPath);
        Handle = IntPtr.Zero;
        if (!File.Exists(libPath))
        {
            Handle = impl.LoadLibrary(libName);

            if (Handle == IntPtr.Zero)
            {
                throw new DllNotFoundException("Teslasuit api library not found! Please sure that Teslasuit SDK is installed.");
            }
            return "";
        }
        
        var pathFormatted = Path.GetDirectoryName(libPath);
        impl.SetLibrariesDirectory(pathFormatted);
        Handle = impl.LoadLibrary(Path.Combine(pathFormatted, libName));
        if (Handle == IntPtr.Zero)
        {
            throw new DllNotFoundException("Teslasuit api library not found! Please sure that Teslasuit SDK is installed.");
        }
        return pathFormatted;
    }

}
