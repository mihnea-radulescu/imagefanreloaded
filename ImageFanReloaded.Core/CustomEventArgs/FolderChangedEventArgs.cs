using System;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
    public FolderChangedEventArgs(string name, string path, int thumbnailSize, bool recursive)
    {
        Name = name;
        Path = path;
        ThumbnailSize = thumbnailSize;
        Recursive = recursive;
    }

    public string Name { get; }
    public string Path { get; }
    public int ThumbnailSize { get; }
    public bool Recursive { get; }
}
