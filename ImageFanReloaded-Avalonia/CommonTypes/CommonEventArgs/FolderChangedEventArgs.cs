using System;

namespace ImageFanReloaded.CommonTypes.CommonEventArgs;

public class FolderChangedEventArgs
    : EventArgs
{
    public FolderChangedEventArgs(string path)
    {
        Path = path;
    }

    public string Path { get; }
}
