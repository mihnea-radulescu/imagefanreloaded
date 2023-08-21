using Avalonia.Media;

namespace ImageFanReloaded.CommonTypes.Info;

public class FileSystemEntryInfo
{
    public FileSystemEntryInfo(string name, string path, IImage icon)
    {
        Name = name;
        Path = path;

        Icon = icon;
    }
    
    public string Name { get; }
    public string Path { get; }

    public IImage Icon { get; }
}
