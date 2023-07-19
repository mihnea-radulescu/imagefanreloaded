using System.Collections.Generic;

using ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface;
using ImageFanReloadedWPF.CommonTypes.Info;

namespace ImageFanReloadedWPF.CommonTypes.Disc.Interface
{
    public interface IDiscQueryEngine
    {
        ICollection<FileSystemEntryInfo> GetAllDrives();
        ICollection<FileSystemEntryInfo> GetSpecialFolders();
        ICollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

        ICollection<IImageFile> GetImageFiles(string folderPath);
    }
}
