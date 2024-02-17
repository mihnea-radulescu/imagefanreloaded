using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.Info;

public record FileSystemEntryInfo
{
    public FileSystemEntryInfo(string name, string path, bool hasSubFolders, Bitmap icon)
    {
        Name = name;
        Path = path;
        HasSubFolders = hasSubFolders;
        
        Icon = icon;
    }
    
    public string Name { get; }
    public string Path { get; }
    public bool HasSubFolders { get; }

    public Bitmap Icon { get; }
}
