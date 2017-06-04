using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using System.Collections.Generic;

namespace ImageFanReloaded.CommonTypes.Disc.Interface
{
    public interface IDiscQueryEngine
    {
        IEnumerable<FileSystemEntryInfo> GetAllDrives();
        IEnumerable<FileSystemEntryInfo> GetSpecialFolders();
        IEnumerable<FileSystemEntryInfo> GetSubFolders(string folderPath);

        IEnumerable<IImageFile> GetImageFiles(string folderPath);
    }
}
