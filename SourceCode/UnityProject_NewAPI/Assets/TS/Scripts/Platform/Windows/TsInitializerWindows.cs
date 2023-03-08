using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TsInitializerWindows : TsInitializerImpl
{
    private const string APIPathKey = "TESLASUIT_API_LIB_PATH";

    public override string GetAPILibraryPath()
    { 
        var pathKey = Environment.GetEnvironmentVariable(APIPathKey);
        if (pathKey != null)
        {
            return pathKey;
        }

        return "";
    }

    public override bool IsLibraryLoaded()
    {
        var libName = GetLibName(GetAPILibraryPath());
        var libraryHandle = GetModuleHandleWin(libName);
        return libraryHandle != IntPtr.Zero;
    }

    public override IntPtr LoadLibrary(string path)
    {
        var handle = LoadLibraryWin(path);
        var errloaddll = Marshal.GetLastWin32Error();
        if(errloaddll != 0)
        {
            Debug.LogWarning($"[TS] LoadLibrary({path}) resulted with code: {errloaddll}");
        }
        return handle;
    }

    public override bool SetLibrariesDirectory(string path)
    {
        var result = SetDllDirectoryWin(path);
        var errsetdir = Marshal.GetLastWin32Error();
        if(errsetdir != 0)
        {
            Debug.LogWarning($"[TS] SetLibrariesDirectory({path}) resulted with code: {errsetdir}");
        }
        return result;
    }


    [DllImport("kernel32", EntryPoint = "SetDllDirectory", SetLastError = true)]
    private static extern bool SetDllDirectoryWin(string lpPathName);

    [DllImport("kernel32", EntryPoint = "LoadLibrary", SetLastError = true)]
    private static extern IntPtr LoadLibraryWin(string lpFileName);

    [DllImport("kernel32", EntryPoint = "GetModuleHandle", SetLastError = true)]
    private static extern IntPtr GetModuleHandleWin(string lpFileName);


}
