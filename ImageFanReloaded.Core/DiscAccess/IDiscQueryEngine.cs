using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	Task BuildSkipRecursionFolderPaths();

	Task<IReadOnlyList<IFileSystemEntryInfo>> GetRootFolders(
		ITabOptions tabOptions);

	Task<IReadOnlyList<IImageFile>> GetImageFilesDefault(string folderPath);

	Task<IReadOnlyList<IFileSystemEntryInfo>> GetSubFolders(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions);

	Task<IReadOnlyList<IImageFile>> GetImageFiles(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions);
}
