using System.Collections.Generic;

using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.CommonTypes.Disc.Interface
{
    public interface IDiscQueryEngine
    {
        ICollection<FileSystemEntryInfo> GetAllDrives();
        ICollection<FileSystemEntryInfo> GetSpecialFolders();
        ICollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

        ICollection<IImageFile> GetImageFiles(string folderPath);
    }
}
