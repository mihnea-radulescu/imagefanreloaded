using System;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
    public FolderChangedEventArgs(string name, string path, bool recursive)
    {
        Name = name;
        Path = path;
        Recursive = recursive;
    }

    public string Name { get; }
    public string Path { get; }
    public bool Recursive { get; }
}
