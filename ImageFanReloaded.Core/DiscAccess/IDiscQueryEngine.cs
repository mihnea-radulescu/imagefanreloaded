using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	IReadOnlyCollection<FileSystemEntryInfo> GetUserFolders();
	IReadOnlyCollection<FileSystemEntryInfo> GetDrives();

	IReadOnlyCollection<FileSystemEntryInfo> GetSubFolders(string folderPath);

	IReadOnlyCollection<IImageFile> GetImageFiles(string folderPath);
}
