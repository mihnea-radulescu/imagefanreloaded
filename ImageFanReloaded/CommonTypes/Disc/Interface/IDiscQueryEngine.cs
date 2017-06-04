using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using System.Collections.Generic;

namespace ImageFanReloaded.CommonTypes.Disc.Interface
{
    public interface IDiscQueryEngine
    {
        IReadOnlyCollection<FileSystemEntryInfo> GetAllDrives();
        IReadOnlyCollection<FileSystemEntryInfo> GetSpecialFolders();
        IReadOnlyCollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

        IReadOnlyCollection<IImageFile> GetImageFiles(string folderPath);
    }
}
