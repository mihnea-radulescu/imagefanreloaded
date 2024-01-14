using Avalonia.Media.Imaging;

namespace ImageFanReloaded.CommonTypes.Info;

public class FileSystemEntryInfo
{
    public FileSystemEntryInfo(string name, string path, Bitmap icon)
    {
        Name = name;
        Path = path;

        Icon = icon;
    }
    
    public string Name { get; }
    public string Path { get; }

    public Bitmap Icon { get; }
}
