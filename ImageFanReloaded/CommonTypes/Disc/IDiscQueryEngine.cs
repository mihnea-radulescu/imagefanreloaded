using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.CommonTypes.Disc;

public interface IDiscQueryEngine
{
	IReadOnlyCollection<FileSystemEntryInfo> GetUserFolders();
	IReadOnlyCollection<FileSystemEntryInfo> GetDrives();

	IReadOnlyCollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

	IReadOnlyCollection<IImageFile> GetImageFiles(string folderPath);
}
