using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	Task BuildSkipRecursionFolderPaths();

	Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders();
	Task<FileSystemEntryInfo> GetFileSystemEntryInfo(string folderPath);

	Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(
		string folderPath, ITabOptions tabOptions);

	Task<IReadOnlyList<IImageFile>> GetImageFiles(
		string folderPath, ITabOptions tabOptions);
	Task<IReadOnlyList<IImageFile>> GetImageFilesDefault(string folderPath);
}
