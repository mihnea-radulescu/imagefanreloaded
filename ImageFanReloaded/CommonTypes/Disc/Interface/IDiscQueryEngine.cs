using System.Collections.Generic;

using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;

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
