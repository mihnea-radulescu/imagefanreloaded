using System.Collections.Generic;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngineFileSystem
{
	void BuildSkipRecursionFolderPaths();

	IReadOnlyList<IFileSystemEntryInfo> GetRootFolders(ITabOptions tabOptions);

	IReadOnlyList<IImageFile> GetImageFilesDefault(string folderPath);

	IReadOnlyList<IFileSystemEntryInfo> GetSubFolders(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions);

	IReadOnlyList<IImageFile> GetImageFiles(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions);
}
