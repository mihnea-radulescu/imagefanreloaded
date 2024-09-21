using System;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class FolderChangedEventArgs : EventArgs
{
    public FolderChangedEventArgs(
        IContentTabItem contentTabItem,
        string name,
        string path,
        bool recursive)
    {
        ContentTabItem = contentTabItem;
        
        Name = name;
        Path = path;
        Recursive = recursive;
    }

    public IContentTabItem ContentTabItem { get; }
    
    public string Name { get; }
    public string Path { get; }
    public bool Recursive { get; }
}
