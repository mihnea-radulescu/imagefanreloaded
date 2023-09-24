using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.CommonTypes.Disc
{
    public interface IDiscQueryEngine
    {
		ICollection<FileSystemEntryInfo> GetSpecialFoldersWithPaths();
		ICollection<FileSystemEntryInfo> GetDrives();

        ICollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

        ICollection<IImageFile> GetImageFiles(string folderPath);
    }
}
