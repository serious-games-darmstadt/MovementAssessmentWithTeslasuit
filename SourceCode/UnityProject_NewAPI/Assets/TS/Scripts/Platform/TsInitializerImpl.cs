using System;
using System.IO;

public abstract class TsInitializerImpl
{
    public abstract string GetAPILibraryPath();
    public abstract bool IsLibraryLoaded();
    public abstract IntPtr LoadLibrary(string path);
    public abstract bool SetLibrariesDirectory(string path);

    public static TsInitializerImpl Get()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                return new TsInitializerWindows();
            default:
                break;
        }
        return null;
    }

    public virtual string GetLibName(string libPath)
    {
        var libName = "teslasuit_api";
        if (string.IsNullOrEmpty(libPath))
        {
            libName = Path.GetFileNameWithoutExtension(libPath);
        }
        return libName;
    }

}
