using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.Info
{
    public class FileSystemEntryInfo
    {
        public FileSystemEntryInfo(string name, string path, ImageSource icon)
        {
            Name = name;
            Path = path;

            Icon = icon;
        }
        
        public string Name { get; }
        public string Path { get; }

        public ImageSource Icon { get; }
    }
}
