using System;
using UnityEngine;

namespace TeslasuitAPI
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class TeslasuitEnvironment
    {

        public static bool Initialized { get; private set; }

        public static event Action BeingDestroyed = delegate { };


        static TeslasuitEnvironment()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                OnInitialize();
#endif
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnInitialize_Runtime()
        {
#if !UNITY_EDITOR
            OnInitialize();
#endif
        }


        static void OnInitialize()
        {
            Initialized = true;
#if ENABLE_IL2CPP
            Debug.Log("il2cpp enabled");
            Teslasuit.Load(false);
#else
            Debug.Log("il2cpp disabled");
            Teslasuit.Load();
#endif
            //Logging
            Teslasuit.PluginError += Teslasuit_PluginError;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#else
            Application.quitting += OnExitedPlayMode;
#endif
        }

        private static void Teslasuit_PluginError(object sender, Exception ex)
        {
            Debug.Log(string.Format("WARNING from {0} : {1}", sender.ToString(), ex.Message));
        }
#if UNITY_EDITOR
        private static void EditorApplication_playModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    OnExitedPlayMode();
                    break;
            }
        }
#endif

        private static void OnExitedPlayMode()
        {
            BeingDestroyed();
            Initialized = false;
            Teslasuit.Unload();
        }
    }

}