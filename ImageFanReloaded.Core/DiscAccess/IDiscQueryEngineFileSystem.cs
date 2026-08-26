using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngineFileSystem
{
	void BuildSkipRecursionFolderPaths();

	IReadOnlyList<FileSystemEntryInfo> GetRootFolders();

	IReadOnlyList<IImageFile> GetImageFilesDefault(string folderPath);

	IReadOnlyList<FileSystemEntryInfo> GetSubFolders(
		string folderPath, ITabOptions tabOptions);

	IReadOnlyList<IImageFile> GetImageFiles(
		string folderPath, ITabOptions tabOptions);
}
