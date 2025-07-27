using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	Task BuildSkipRecursionFolderPaths();

	Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders();

	Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(
		string folderPath,
		FileSystemEntryInfoOrdering folderOrdering,
		FileSystemEntryInfoOrderingDirection folderOrderingDirection);

	Task<FileSystemEntryInfo> GetFileSystemEntryInfo(string folderPath);

	Task<IReadOnlyList<IImageFile>> GetImageFiles(
		string folderPath,
		FileSystemEntryInfoOrdering imageFileOrdering,
		FileSystemEntryInfoOrderingDirection imageFileOrderingDirection,
		bool recursive);
}
