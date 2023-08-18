using System;
using System.Diagnostics;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.Info
{
    [DebuggerDisplay("{Path}")]
    public class FileSystemEntryInfo
    {
        public FileSystemEntryInfo(string name, string path, ImageSource icon)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be empty.", nameof(path));
            }

            Name = name;
            Path = path;

            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        }
        
        public string Name { get; private set; }
        public string Path { get; private set; }

        public ImageSource Icon { get; private set; }
    }
}
