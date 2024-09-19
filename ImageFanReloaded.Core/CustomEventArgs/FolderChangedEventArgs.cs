using System;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
    public FolderChangedEventArgs(
        string name,
        string path,
        FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering,
        int thumbnailSize,
        bool recursive)
    {
        Name = name;
        Path = path;
        FileSystemEntryInfoOrdering = fileSystemEntryInfoOrdering;
        ThumbnailSize = thumbnailSize;
        Recursive = recursive;
    }

    public string Name { get; }
    public string Path { get; }
    public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; }
    public int ThumbnailSize { get; }
    public bool Recursive { get; }
}
